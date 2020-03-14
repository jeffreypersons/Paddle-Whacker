using System.Collections;
using UnityEngine;


public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public float responseTime;
    public float randomSlowDownVariance;
    public float minVerticalDistanceBeforeMoving;

    private Rigidbody2D ballBody;
    private BoxCollider2D ballCollider;

    private Rigidbody2D paddleBody;
    private BoxCollider2D paddleCollider;
    private Vector2 initialPaddlePosition;

    private float targetPaddleY;
    private BallTrajectoryPredictor ballPredictor;
    private Coroutine updateTargetCoroutine;

    private float BallHalfHeight   { get { return ballCollider.bounds.extents.y;   } }
    private float PaddleHalfHeight { get { return paddleCollider.bounds.extents.y; } }
    private bool IsBallWithinPaddleRange(float paddleY, float ballY)
    {
        return MathUtils.IsOverlappingRange(
            paddleY - PaddleHalfHeight - minVerticalDistanceBeforeMoving,
            paddleY + PaddleHalfHeight + minVerticalDistanceBeforeMoving,
            ballY - BallHalfHeight,
            ballY + BallHalfHeight
        );
    }
    private void StartTargetUpdateRoutine(IEnumerator task)
    {
        // override target to no movement to avoid strange synchronizing behavior
        if (updateTargetCoroutine != null)
        {
            StopCoroutine(updateTargetCoroutine);
        }
        targetPaddleY = paddleBody.position.y;
        updateTargetCoroutine = StartCoroutine(task);
    }

    public void Reset()
    {
        paddleBody.position = initialPaddlePosition;
        targetPaddleY = initialPaddlePosition.y;
        StartTargetUpdateRoutine(CoroutineUtils.RunRepeatedly(Time.fixedDeltaTime, TrackBall));
    }

    void Start()
    {
        ballBody     = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        ballCollider = GameObject.Find("Ball").GetComponent<BoxCollider2D>();

        paddleBody = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        paddleCollider = GameObject.Find(paddleName).GetComponent<BoxCollider2D>();
        initialPaddlePosition = paddleBody.position;
        ballPredictor = new BallTrajectoryPredictor();

        Reset();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(targetPaddleY - paddleBody.position.y) >= minVerticalDistanceBeforeMoving)
        {
            paddleBody.position = Vector2.MoveTowards(
                paddleBody.position,
                new Vector2(paddleBody.position.x, targetPaddleY),
                paddleSpeed * Time.fixedDeltaTime
            );
        }
    }

    void OnEnable()
    {
        GameEvents.onZoneIntersection.AddListener(UpdateTargetTask);
    }
    void OnDisable()
    {
        GameEvents.onZoneIntersection.RemoveListener(UpdateTargetTask);
    }
    public void UpdateTargetTask(ZoneIntersectInfo hitZoneInfo)
    {
        bool isOnAiSide         =  hitZoneInfo.ContainsPaddle(paddleName);
        bool isBallIncoming     = !isOnAiSide && hitZoneInfo.IsNearingMidline();
        bool isBallBehindPaddle =  isOnAiSide && hitZoneInfo.IsNearingGoalWall();

        IEnumerator task;
        if (isBallIncoming)
        {
            task = CoroutineUtils.RunAfter(responseTime, PredictBallPosition);
        }
        else if (isBallBehindPaddle)
        {
            task = CoroutineUtils.RunNow(TryToHitBallFromHorizontalEdge);
        }
        else
        {
            task = CoroutineUtils.RunRepeatedly(Time.fixedDeltaTime, TrackBall);
        }
        StartTargetUpdateRoutine(task);
    }
    private void PredictBallPosition()
    {
        Debug.Log("incoming");
        ballPredictor.Compute(ballBody.position, ballBody.velocity.normalized, paddleBody.position.x);
        ballPredictor.DrawInEditor(Color.green, 1.5f);
        targetPaddleY = ballPredictor.EndPoint.y;
    }
    private void TryToHitBallFromHorizontalEdge()
    {
        Debug.Log("behind");
        ballPredictor.Compute(ballBody.position, ballBody.velocity.normalized, paddleBody.position.x);
        ballPredictor.DrawInEditor(Color.red, 1.5f);

        float paddleY    = paddleCollider.bounds.center.y;
        float predictedY = ballPredictor.EndPoint.y;
        if (IsBallWithinPaddleRange(predictedY, ballBody.position.y))
        {
            targetPaddleY = paddleY + (paddleY - predictedY);
        }
        else
        {
            targetPaddleY = paddleBody.position.y;
        }
    }
    private void TrackBall()
    {
        targetPaddleY = ballBody.position.y;
    }
}
