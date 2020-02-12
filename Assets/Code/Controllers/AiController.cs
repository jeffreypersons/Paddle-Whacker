using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class LastHit
{
    public string paddleName { get; private set; }
    public PredictedTrajectory predictedTrajectory { get; private set; }
    public LastHit()
    {
        predictedTrajectory = new PredictedTrajectory();
        paddleName = "";
    }
    public void Reset()
    {
        paddleName = "";
        predictedTrajectory.Clear();
    }
    public void RegisterHit(string paddleName, Vector2 ballPosition, Vector2 ballVelocity, float maxTrajectoryDistanceX)
    {
        this.paddleName = paddleName;
        predictedTrajectory.Compute(ballPosition, ballVelocity.normalized, maxTrajectoryDistanceX);
    }
}

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    private Vector2 initialPosition;
    private Rigidbody2D paddle;

    public double responseTime;
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
        return Vector2.MoveTowards(paddle.position, new Vector2(paddle.position.x, targetY), paddleSpeed * Time.deltaTime);
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
            lastHit.Reset();
        }
        else
        {
            float distance = Mathf.Abs(paddle.position.x - ball.position.x);
            StartCoroutine(
                WaitAtLeast(responseTime, () =>
                {
                    lastHit.RegisterHit(paddleName, ball.position, ball.velocity, distance);
                    lastHit.predictedTrajectory.DrawInEditor(Color.green, 2.5f);
                })
            );
            //StartCoroutine(PredictOpponentTrajectory(paddleName));
        }
    }

    private IEnumerator WaitAtLeast(double timeDelay, Action action)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        action();
        stopWatch.Stop();

        double elapsedTime = stopWatch.Elapsed.TotalSeconds;
        if (elapsedTime < timeDelay)
        {
            yield return new WaitForSeconds((float)(timeDelay - elapsedTime));
        }
    }
}
