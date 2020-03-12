using UnityEngine;


public class AiController : MonoBehaviour
{
    public enum State { Idle, FollowBall, AvoidBall, ApproachBall };

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

    public void Reset()
    {
        paddleBody.position = initialPosition;
        paddleBody.velocity = Vector2.zero;
        predictedTrajectory.Clear();
        targetY = paddleBody.position.y;
    }

    void Start()
    {
        paddleBody      = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        paddleCollider  = GameObject.Find(paddleName).GetComponent<BoxCollider2D>();
        initialPosition = paddleBody.position;

        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        predictedTrajectory = new PredictedTrajectory();
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
        GameEvents.onZoneIntersection.AddListener(RegisterLastZoneHit);
    }
    void OnDisable()
    {
        GameEvents.onZoneIntersection.RemoveListener(RegisterLastZoneHit);
    }
    public void RegisterLastZoneHit(ZoneIntersectInfo hitZoneInfo)
    {
        bool isAiZone = hitZoneInfo.ContainsPaddle(paddleName);
        bool isAiOnLeftside = paddleBody.position.x < 0;
        bool isAiOnRightside = paddleBody.position.x > 0;

        bool isBallApproachingAiFromOpponent = !isAiZone &&
            (isAiOnLeftside  && hitZoneInfo.IsNearingMidlineFromRight()) ||
            (isAiOnRightside && hitZoneInfo.IsNearingMidlineFromLeft());

        bool isBallBehindAi = isAiZone &&
            (isAiOnLeftside  && hitZoneInfo.IsNearingLeftGoal()) ||
            (isAiOnRightside && hitZoneInfo.IsNearingRightGoal());

        StopAllCoroutines();
        if (isBallApproachingAiFromOpponent)
        {
            targetY = paddleBody.position.y;
            StartCoroutine(
                CoroutineUtils.RunAfter(responseTime, () =>
                {
                    predictedTrajectory.Compute(ball.position, ball.velocity.normalized, paddleBody.position.x);
                    predictedTrajectory.DrawInEditor(Color.green, 1.5f);
                    targetY = predictedTrajectory.EndPoint.y;
                    //Debug.Log("drawing trajectory in editor: " + LastHitPaddleZone.predictedTrajectory);
                })
            );
        }
        else if (isBallBehindAi)
        {
            // todo: handle moving paddle out of balls way
        }
        else
        {
            StartCoroutine(
                CoroutineUtils.RunRepeatedly(Time.fixedDeltaTime, () =>
                {
                    targetY = ball.position.y;
                })
            );
        }
    }
}
