using System.Collections;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;

    // needed for ball trajectory predictions
    public float responseTime;
    private Rigidbody2D ball;
    private Vector2 positionToMoveTowards;

    private Vector2 initialPosition;
    private Rigidbody2D paddle;

    public void Reset()
    {
        paddle.position = initialPosition;
        positionToMoveTowards = initialPosition;
        paddle.velocity = Vector2.zero;
    }
    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        initialPosition = paddle.position;
        Reset();

        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // todo: invoke a predict ball position script here instead
        float ballPositionY = GameObject.Find("Ball").GetComponent<Rigidbody2D>().position.y;

        Vector2 current = paddle.position;
        positionToMoveTowards = new Vector2(current.x, ballPositionY);
        float currentSpeed = Random.Range(0.10f, 1.0f) * paddleSpeed;

        if (positionToMoveTowards != initialPosition)
        {
            paddle.position = Vector2.MoveTowards(current, positionToMoveTowards, currentSpeed * Time.deltaTime);
        }
    }

    void OnEnable()
    {
        GameEvents.onPaddleHit.AddListener(PredictBallTrajectory);
    }
    void OnDisable()
    {
        GameEvents.onPaddleHit.RemoveListener(PredictBallTrajectory);
    }
    public void PredictBallTrajectory(string paddleName)
    {
        StartCoroutine(UpdateTargetPosition(paddleName));
    }

    // since x is fixed (currently only vertical paddle movement is possible), we only need to predict optimal y
    private IEnumerator UpdateTargetPosition(string paddleName)
    {
        yield return new WaitForSeconds(responseTime);

        if (paddleName != this.paddleName)
        {
            float predictedYPosition = ComputeYForGivenXAlongTrajectory(paddle.position.x);
            positionToMoveTowards = new Vector2(paddle.position.x, predictedYPosition);
        }
    }
    // determines assumes constant speed with no reflections
    // note: ball hit position not needed since time/reflections/friction is not yet accounted for
    private float ComputeYForGivenXAlongTrajectory(float x)
    {
        float result;
        if (ball.velocity.y == 0)
        {
            result = ball.position.y;
        }
        else
        {
            float slope = ball.velocity.x / ball.velocity.y;
            float yOffset = ball.position.y - (slope * ball.position.x);
            result = slope * x + yOffset;
        }
        Debug.Log("CCC");
        return result;
    }
}
