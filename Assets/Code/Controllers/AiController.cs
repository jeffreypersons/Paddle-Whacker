using UnityEngine;

public class LastHitPaddleZone
{
    public Vector2 ballDirection { get; private set; }
    public string paddleZoneName { get; private set; }
    public PredictedTrajectory predictedTrajectory { get; private set; }

    public LastHitPaddleZone()
    {
        paddleZoneName = "";
        predictedTrajectory = new PredictedTrajectory();
    }
    public void Reset()
    {
        paddleZoneName = "";
        predictedTrajectory.Clear();
    }
    public void RegisterIntersection(string paddleZoneName, Vector2 ballPosition, Vector2 ballVelocity, float targetX)
    {
        this.paddleZoneName = paddleZoneName;
        predictedTrajectory.Compute(ballPosition, ballVelocity.normalized, targetX);
        ballDirection = ballVelocity.normalized;
    }
}

public class AiController : MonoBehaviour
{
    public string paddleName;
    public string goalName;
    public float paddleSpeed;
    public float randomSlowDownVariance;
    private Vector2 initialPosition;
    private Rigidbody2D paddleBody;

    public float maxToleratedDistanceFromBall;
    private BoxCollider2D goalCollider;
    private BoxCollider2D paddleCollider;

    public float responseTime;
    private Rigidbody2D ball;
    LastHitPaddleZone LastHitPaddleZone;

    public void Reset()
    {
        paddleBody.position = initialPosition;
        paddleBody.velocity = Vector2.zero;
        LastHitPaddleZone.Reset();
    }

    public AiController()
    {
        LastHitPaddleZone = new LastHitPaddleZone();
    }

    void Start()
    {
        goalCollider = GameObject.Find(goalName).GetComponent<BoxCollider2D>();
        paddleBody = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        paddleCollider = GameObject.Find(paddleName).GetComponent<BoxCollider2D>();
        initialPosition = paddleBody.position;

        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        LastHitPaddleZone = new LastHitPaddleZone();
    }

    // if ball's last paddle hit = AI: keep horizontally aligned with ball
    // if ball's last paddle hit = Opponent: start moving after time delay towards predicted ball trajectory
    void FixedUpdate()
    {
        if (LastHitPaddleZone.paddleZoneName.StartsWith(paddleName))
        {
            Vector2 projectedBallPosition = new Vector2(paddleBody.position.x, LastHitPaddleZone.predictedTrajectory.EndPoint.y);
            if (paddleCollider.bounds.Contains(projectedBallPosition))
            {
                // add proper move out the way logic if ball is behind paddle
                /*
                if (paddleBody.position.y > 5)
                {
                    paddleBody.position = MoveVerticallyTowards(paddleBody.position.y - 5);
                }
                else if (paddleBody.position.y < 5)
                {
                    paddleBody.position = MoveVerticallyTowards(paddleBody.position.y + 5);
                }
                */
            }
            else
            {
                paddleBody.position = MoveVerticallyTowards(ball.position.y);
            }
        }
        else if (!LastHitPaddleZone.predictedTrajectory.Empty)
        {
            paddleBody.position = MoveVerticallyTowards(LastHitPaddleZone.predictedTrajectory.EndPoint.y);
        }
        else
        {
            paddleBody.velocity = Vector2.zero;
        }
    }
    private Vector2 MoveVerticallyTowards(float targetY)
    {
        if (Mathf.Abs(targetY - paddleBody.position.y) < maxToleratedDistanceFromBall) {
            return paddleBody.position;
        }

        float maxDistance = Random.Range(1.00f - randomSlowDownVariance, 1.00f) * paddleSpeed * Time.fixedDeltaTime;
        return Vector2.MoveTowards(paddleBody.position, new Vector2(paddleBody.position.x, targetY), maxDistance);
    }

    void OnEnable()
    {
        GameEvents.onPaddleHit.AddListener(RegisterPaddleZoneHit);
    }
    void OnDisable()
    {
        GameEvents.onPaddleHit.RemoveListener(RegisterPaddleZoneHit);
    }
    public void RegisterPaddleZoneHit(string paddleZoneName)
    {
        if (paddleZoneName.StartsWith(paddleName))
        {
            StartCoroutine(
                CoroutineUtils.RunNow(() =>
                {
                    LastHitPaddleZone.RegisterIntersection(paddleZoneName, ball.position, ball.velocity, goalCollider.bounds.min.x);
                    LastHitPaddleZone.predictedTrajectory.DrawInEditor(Color.green, 1.5f);
                    //Debug.Log("drawing trajectory in editor: " + LastHitPaddleZone.predictedTrajectory);
                    LastHitPaddleZone.predictedTrajectory.Clear();
                })
            );
        }
        else
        {
            StartCoroutine(
                CoroutineUtils.RunAfter(responseTime, () =>
                {
                    LastHitPaddleZone.RegisterIntersection(paddleZoneName, ball.position, ball.velocity, paddleBody.position.x);
                    LastHitPaddleZone.predictedTrajectory.DrawInEditor(Color.green, 1.5f);
                    //Debug.Log("drawing trajectory in editor: " + LastHitPaddleZone.predictedTrajectory);
                })
            );
        }
    }
}
