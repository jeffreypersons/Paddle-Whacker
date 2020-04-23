using System.Collections;
using UnityEngine;


public class AiController : MonoBehaviour
{
    [SerializeField] private float paddleSpeedAtMaxDifficulty      = default;
    [SerializeField] private float responseTimeAtMaxDifficulty     = default;
    [SerializeField] private float minVerticalDistanceBeforeMoving = default;
    [SerializeField] private LayerMask layersUsedWhenPredictingTrajectory = default;

    bool wasDifficultySet = false;
    private int percentOfMaxDifficulty;
    private float paddleSpeed;
    private float responseTime;

    private Rigidbody2D paddleBody;
    private BoxCollider2D paddleCollider;
    private Vector2 initialPaddlePosition;

    private Rigidbody2D ballBody;
    private BoxCollider2D ballCollider;

    private float targetPaddleY;
    private BallTrajectoryPredictor ballPredictor;
    private Coroutine updateTargetCoroutine;

    private string PaddleName       { get { return paddleCollider.name;             } }
    private float  BallHalfHeight   { get { return ballCollider.bounds.extents.y;   } }
    private float  PaddleHalfHeight { get { return paddleCollider.bounds.extents.y; } }
    private bool IsBallWithinPaddleRange(float paddleY, float ballY)
    {
        return MathUtils.IsOverlappingRange(
            paddleY - PaddleHalfHeight,
            paddleY + PaddleHalfHeight,
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
        targetPaddleY       = initialPaddlePosition.y;
        StartTargetUpdateRoutine(CoroutineUtils.RunRepeatedly(Time.fixedDeltaTime, TrackBall));
    }
    public void SetDifficultyLevel(int percent)
    {
        if (MathUtils.IsWithinRange(percent, 0, 100))
        {
            wasDifficultySet = true;
            percentOfMaxDifficulty = percent;
        }
        else
        {
            Debug.LogError("Ai's difficulty level cannot be set to a percentage outside the range [0, 100]");
        }
    }

    void Awake()
    {
        paddleBody     = gameObject.transform.GetComponent<Rigidbody2D>();
        paddleCollider = gameObject.transform.GetComponent<BoxCollider2D>();
        initialPaddlePosition = paddleBody.position;

        GameObject ball = GameObject.Find("Ball");
        ballBody      = ball.GetComponent<Rigidbody2D>();
        ballCollider  = ball.GetComponent<BoxCollider2D>();
        ballPredictor = new BallTrajectoryPredictor(layersUsedWhenPredictingTrajectory);
        targetPaddleY = initialPaddlePosition.y;
    }
    void Start()
    {
        if (wasDifficultySet)
        {
            float aiHandicap = percentOfMaxDifficulty / 100.0f;
            paddleSpeed  = paddleSpeedAtMaxDifficulty * aiHandicap;
            responseTime = responseTimeAtMaxDifficulty - (responseTimeAtMaxDifficulty * aiHandicap);
        }
        else
        {
            Debug.LogError("Difficulty level was not set, defaulting to a 100%");
            paddleSpeed  = paddleSpeedAtMaxDifficulty;
            responseTime = responseTimeAtMaxDifficulty;
        }
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
        GameEventCenter.zoneIntersection.AddListener(UpdateTargetTask);
    }
    void OnDisable()
    {
        GameEventCenter.zoneIntersection.RemoveListener(UpdateTargetTask);
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
        ballPredictor.ComputeNewTrajectory(ballBody.position, ballBody.velocity.normalized, paddleBody.position.x);
        targetPaddleY = ballPredictor.EndPoint.y;

        #if UNITY_EDITOR
            ballPredictor.DrawInEditor(Color.red, 1.50f);
        #endif
    }
    private void TryToHitBallFromHorizontalEdge()
    {
        ballPredictor.ComputeNewTrajectory(ballBody.position, ballBody.velocity.normalized, paddleBody.position.x);
        float paddleY    = paddleCollider.bounds.center.y;
        float predictedY = ballPredictor.EndPoint.y;
        #if UNITY_EDITOR
            ballPredictor.DrawInEditor(Color.green, 1.50f);
        #endif

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
