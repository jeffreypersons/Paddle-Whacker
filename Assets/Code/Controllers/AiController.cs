using System.Collections;
using UnityEngine;


public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public float responseTime;
    public float randomSlowDownVariance;
    public float minDistanceBeforeAvoiding;
    public float minDistanceBeforeApproaching;

    private Vector2 initialPosition;
    private Rigidbody2D paddleBody;
    private BoxCollider2D paddleCollider;

    private Rigidbody2D ball;
    private PredictedTrajectory predictedTrajectory;
    private float targetY;
    private Coroutine updateTargetCoroutine;

    public void Reset()
    {
        paddleBody.position = initialPosition;
        paddleBody.velocity = Vector2.zero;
        predictedTrajectory.Clear();
        targetY = paddleBody.position.y;
        StopCoroutine(updateTargetCoroutine);
    }

    void Start()
    {
        paddleBody      = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        paddleCollider  = GameObject.Find(paddleName).GetComponent<BoxCollider2D>();
        initialPosition = paddleBody.position;

        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        predictedTrajectory = new PredictedTrajectory();
        targetY = paddleBody.position.y;
        updateTargetCoroutine = StartCoroutine(CoroutineUtils.RunAfter(responseTime, PredictBallPosition));
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(targetY - paddleBody.position.y) >= minDistanceBeforeApproaching)
        {
            paddleBody.position = Vector2.MoveTowards(
                paddleBody.position,
                new Vector2(paddleBody.position.x, targetY),
                paddleSpeed * Time.fixedDeltaTime * Random.Range(1.00f - randomSlowDownVariance, 1.00f)
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
            task = CoroutineUtils.RunAfter(responseTime, HitBallFromHorizontalEdge);
        }
        else
        {
            task = CoroutineUtils.RunRepeatedly(Time.fixedDeltaTime, TrackBall);
        }

        targetY = paddleBody.position.y;
        StopCoroutine(updateTargetCoroutine);
        updateTargetCoroutine = StartCoroutine(task);
    }
    private void PredictBallPosition()
    {
        predictedTrajectory.Compute(ball.position, ball.velocity.normalized, paddleBody.position.x);
        predictedTrajectory.DrawInEditor(Color.green, 1.5f);
        targetY = predictedTrajectory.EndPoint.y;
    }
    private void HitBallFromHorizontalEdge()
    {
        // todo: fix trajectory to reflect back properly in opposite direction
        predictedTrajectory.Compute(ball.position, ball.velocity.normalized, paddleBody.position.x);
        predictedTrajectory.DrawInEditor(Color.red, 1.5f);
        targetY = predictedTrajectory.EndPoint.y;
    }
    private void TrackBall()
    {
        targetY = ball.position.y;
    }
}
