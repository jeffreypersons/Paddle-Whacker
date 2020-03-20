using System.Collections;
using UnityEngine;


public class AiController : MonoBehaviour
{
    public float paddleSpeed;
    public float responseTime;
    public float randomSlowDownVariance;
    public float minVerticalDistanceBeforeMoving;

    private Rigidbody2D paddleBody;
    private BoxCollider2D paddleCollider;
    private Vector2 initialPaddlePosition;

    private Rigidbody2D ballBody;
    private BoxCollider2D ballCollider;

    private float targetPaddleY;
    private BallTrajectoryPredictor ballPredictor;
    private Coroutine updateTargetCoroutine;

    private string PaddleName       { get { return paddleCollider.name; } }
    private float  BallHalfHeight   { get { return ballCollider.bounds.extents.y;   } }
    private float  PaddleHalfHeight { get { return paddleCollider.bounds.extents.y; } }
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

    void Awake()
    {
        paddleBody     = gameObject.transform.GetComponent<Rigidbody2D>();
        paddleCollider = gameObject.transform.GetComponent<BoxCollider2D>();
        initialPaddlePosition = paddleBody.position;

        ballBody      = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        ballCollider  = GameObject.Find("Ball").GetComponent<BoxCollider2D>();
        ballPredictor = new BallTrajectoryPredictor();
        targetPaddleY = initialPaddlePosition.y;
    }
    void Start()
    {
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
        GameEventCenter.zoneIntersection.StartListening(UpdateTargetTask);
    }
    void OnDisable()
    {
        GameEventCenter.zoneIntersection.StopListening(UpdateTargetTask);
    }
    public void UpdateTargetTask(PaddleZoneIntersectInfo hitZoneInfo)
    {
        bool isOnAiSide         =  hitZoneInfo.ContainsPaddle(PaddleName);
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
        ballPredictor.Compute(ballBody.position, ballBody.velocity.normalized, paddleBody.position.x);
        targetPaddleY = ballPredictor.EndPoint.y;
    }
    private void TryToHitBallFromHorizontalEdge()
    {
        ballPredictor.Compute(ballBody.position, ballBody.velocity.normalized, paddleBody.position.x);
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
