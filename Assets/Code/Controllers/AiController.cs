using UnityEngine;

public class LastPaddleLineTriggered
{
    public Vector2 ballDirection { get; private set; }
    public string paddleLineName { get; private set; }
    public PredictedTrajectory predictedTrajectory { get; private set; }

    public LastPaddleLineTriggered()
    {
        paddleLineName = "";
        predictedTrajectory = new PredictedTrajectory();
    }
    public void Reset()
    {
        paddleLineName = "";
        predictedTrajectory.Clear();
    }
    public void RegisterPaddleLineTriggered(string paddleLineName, Vector2 ballPosition, Vector2 ballVelocity, float targetX)
    {
        this.paddleLineName = paddleLineName;
        predictedTrajectory.Compute(ballPosition, ballVelocity.normalized, targetX);
        ballDirection = ballVelocity.normalized;
    }
}

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public float randomSlowDownVariance;
    private Vector2 initialPosition;
    private Rigidbody2D paddleBody;

    public float maxToleratedDistanceFromBall;
    private BoxCollider2D paddleCollider;

    public float responseTime;
    private Rigidbody2D ball;
    LastPaddleLineTriggered lastPaddleLineTriggered;

    public void Reset()
    {
        paddleBody.position = initialPosition;
        paddleBody.velocity = Vector2.zero;
        lastPaddleLineTriggered.Reset();
    }

    public AiController()
    {
        lastPaddleLineTriggered = new LastPaddleLineTriggered();
    }

    void Start()
    {
        paddleBody = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        paddleCollider = GameObject.Find(paddleName).GetComponent<BoxCollider2D>();
        initialPosition = paddleBody.position;

        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        lastPaddleLineTriggered = new LastPaddleLineTriggered();
    }

    // if ball's last paddle hit = AI: keep horizontally aligned with ball
    // if ball's last paddle hit = Opponent: start moving after time delay towards predicted ball trajectory
    void FixedUpdate()
    {
        if (lastPaddleLineTriggered.paddleLineName.StartsWith(paddleName))
        {
            paddleBody.position = MoveVerticallyTowards(ball.position.y);
        }
        else if (!lastPaddleLineTriggered.predictedTrajectory.Empty)
        {
            paddleBody.position = MoveVerticallyTowards(lastPaddleLineTriggered.predictedTrajectory.EndPoint.y);
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
        GameEvents.onPaddleHit.AddListener(RegisterPaddleLineCrossed);
    }
    void OnDisable()
    {
        GameEvents.onPaddleHit.RemoveListener(RegisterPaddleLineCrossed);
    }
    public void RegisterPaddleLineCrossed(string paddleLineName)
    {
        if (paddleLineName.StartsWith(paddleName))
        {
            StartCoroutine(
                CoroutineUtils.RunAfter(responseTime, () =>
                {
                    lastPaddleLineTriggered.RegisterPaddleLineTriggered(paddleLineName, ball.position, ball.velocity, paddleBody.position.x);
                    lastPaddleLineTriggered.predictedTrajectory.DrawInEditor(Color.green, 1.5f);
                    Debug.Log("drawing trajectory in editor: " + lastPaddleLineTriggered.predictedTrajectory);
                    lastPaddleLineTriggered.predictedTrajectory.Clear();
                })
            );
        }
        else
        {
            StartCoroutine(
                CoroutineUtils.RunAfter(responseTime, () =>
                {
                    lastPaddleLineTriggered.RegisterPaddleLineTriggered(
                        paddleLineName, ball.position, ball.velocity, -21
                    );
                    lastPaddleLineTriggered.predictedTrajectory.DrawInEditor(Color.green, 1.5f);
                    Debug.Log("drawing trajectory in editor: " + lastPaddleLineTriggered.predictedTrajectory);
                })
            );
        }
    }
}
