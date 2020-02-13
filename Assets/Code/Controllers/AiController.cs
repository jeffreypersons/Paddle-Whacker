using UnityEngine;

public class LastHit
{
    public string paddleName { get; private set; }
    public PredictedTrajectory predictedTrajectory { get; private set; }
    public LastHit()
    {
        paddleName = "";
        predictedTrajectory = new PredictedTrajectory();
    }
    public void Reset()
    {
        paddleName = "";
        predictedTrajectory.Clear();
    }
    public void RegisterHit(string paddleName)
    {
        this.paddleName = paddleName;
        predictedTrajectory.Clear();
    }
    public void RegisterHit(string paddleName, Vector2 ballPosition, Vector2 ballVelocity, float maxX)
    {
        this.paddleName = paddleName;
        predictedTrajectory.Compute(ballPosition, ballVelocity.normalized, maxX);
    }
}

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public float randomSlowDownVariance;
    private Vector2 initialPosition;
    private Rigidbody2D paddle;

    public float responseTime;
    private Rigidbody2D ball;
    LastHit lastHit;

    public void Reset()
    {
        paddle.position = initialPosition;
        paddle.velocity = Vector2.zero;
        lastHit.Reset();
    }

    public AiController()
    {
        lastHit = new LastHit();
    }

    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        initialPosition = paddle.position;

        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        lastHit = new LastHit();
    }

    // if ball's last paddle hit = AI: keep horizontally aligned with ball
    // if ball's last paddle hit = Opponent: start moving after time delay towards predicted ball trajectory
    void FixedUpdate()
    {
        if (lastHit.paddleName == paddleName)
        {
            paddle.position = MoveVerticallyTowards(ball.position.y);
        }
        else if (!lastHit.predictedTrajectory.Empty)
        {
            paddle.position = MoveVerticallyTowards(lastHit.predictedTrajectory.EndPoint.y);
        }
        else
        {
            paddle.velocity = Vector2.zero;
        }
    }
    private Vector2 MoveVerticallyTowards(float targetY)
    {
        float maxDistance = Random.Range(1.00f - randomSlowDownVariance, 1.00f) * paddleSpeed * Time.deltaTime;
        return Vector2.MoveTowards(paddle.position, new Vector2(paddle.position.x, targetY), maxDistance);
    }

    void OnEnable()
    {
        GameEvents.onPaddleHit.AddListener(RegisterPaddleHit);
    }
    void OnDisable()
    {
        GameEvents.onPaddleHit.RemoveListener(RegisterPaddleHit);
    }
    public void RegisterPaddleHit(string paddleName)
    {
        if (paddleName == this.paddleName)
        {
            lastHit.RegisterHit(paddleName);
        }
        else
        {
            StartCoroutine(
                CoroutineUtils.RunAfter(responseTime, () =>
                {
                    lastHit.RegisterHit(paddleName, ball.position, ball.velocity, paddle.position.x);
                    lastHit.predictedTrajectory.DrawInEditor(Color.green, 1.5f);
                    Debug.Log("drawing trajectory in editor: " + lastHit.predictedTrajectory);
                })
            );
        }
    }
}
