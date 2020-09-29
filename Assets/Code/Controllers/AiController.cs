using System.Collections;
using UnityEngine;


public class AiController : MonoBehaviour
{
    [SerializeField] private float paddleSpeedAtMinDifficulty  = default;
    [SerializeField] private float paddleSpeedAtMaxDifficulty  = default;
    [SerializeField] private float responseTimeAtMinDifficulty = default;
    [SerializeField] private float responseTimeAtMaxDifficulty = default;

    [SerializeField] private float initialTimeDelayAfterReset = default;
    [SerializeField] private float minVerticalDistanceBeforeMoving = default;
    [SerializeField] private LayerMask layersUsedWhenPredictingTrajectory = default;

    private bool  wasDifficultySet;
    private float difficultyRatio;
    private float paddleSpeed;
    private float responseTime;
    private static float DEFAULT_DIFFICULTY_RATIO = 0.5f;

    private Rigidbody2D paddleBody;
    private BoxCollider2D paddleCollider;
    private Vector2 initialPaddlePosition;

    private Rigidbody2D ballBody;
    private BoxCollider2D ballCollider;
    private BallTrajectoryPredictor ballPredictor;
    private float targetPaddleY;
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
    }
    public void SetDifficultyLevel(float ratio)
    {
        if (MathUtils.IsWithinRange(ratio, 0.00f, 1.00f))
        {
            wasDifficultySet = true;
            difficultyRatio  = ratio;
            responseTime = Mathf.Lerp(responseTimeAtMinDifficulty, responseTimeAtMaxDifficulty, ratio);
            paddleSpeed  = Mathf.Lerp(paddleSpeedAtMinDifficulty,  paddleSpeedAtMaxDifficulty,  ratio);
        }
        else
        {
            Debug.LogError($"DifficultyScale must be a ratio between 0.00 and 1.00, recieved {ratio} instead");
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
        if (!wasDifficultySet)
        {
            Debug.Log($"Difficulty level was not selected, defaulting to a {DEFAULT_DIFFICULTY_RATIO}");
            SetDifficultyLevel(DEFAULT_DIFFICULTY_RATIO);
        }

        Reset();
        StartTargetUpdateRoutine(CoroutineUtils.RunAfter(initialTimeDelayAfterReset, TargetPredictedBallPosition));
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
            task = CoroutineUtils.RunAfter(responseTime, TargetPredictedBallPosition);
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
    private void TargetPredictedBallPosition()
    {
        Vector2 initialBallDirection = ballBody.velocity.normalized;
        float targetX = paddleCollider.ClosestPoint(ballBody.position).x;
        ballPredictor.ComputeNewTrajectory(ballBody.position, initialBallDirection, targetX);
        targetPaddleY = ballPredictor.EndPoint.y;
        if (IsBallWithinPaddleRange(paddleCollider.bounds.center.y, ballPredictor.StartPoint.y) &&
            IsBallWithinPaddleRange(paddleCollider.bounds.center.y, ballPredictor.EndPoint.y))
        {
            TargetRandomPositionWithinBounds(ballPredictor.EndPoint.y, BallHalfHeight * 0.50f, BallHalfHeight);
        }
        #if UNITY_EDITOR
            ballPredictor.DrawInEditor(Color.red, 1.50f);
        #endif
    }
    private void TryToHitBallFromHorizontalEdge()
    {
        float targetX = paddleCollider.ClosestPoint(ballBody.position).x;
        ballPredictor.ComputeNewTrajectory(ballBody.position, ballBody.velocity.normalized, targetX);
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
    // returns a randomized number within the range [ratioOffsetFromPaddleCenter
    private void TargetRandomPositionWithinBounds(float paddleY, float minDistance, float maxDistance)
    {
        targetPaddleY = paddleY + (MathUtils.RandomSign() * Random.Range(minDistance, maxDistance));
    }
}
