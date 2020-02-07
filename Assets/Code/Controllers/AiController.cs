using System.Collections;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;

    // needed for ball trajectory predictions
    public float responseTime;
    private bool isTargetPredicted;
    private bool isMovementSuspended;
    private Vector2 positionToMoveTowards;
    private Rigidbody2D ball;

    private Vector2 initialPosition;
    private Rigidbody2D paddle;

    public void Reset()
    {
        paddle.position = initialPosition;
        positionToMoveTowards = initialPosition;
        paddle.velocity = Vector2.zero;
        isMovementSuspended = false;
        isTargetPredicted = false;
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
        if (isMovementSuspended || positionToMoveTowards == initialPosition)
        {
            return;
        }

        if (!isTargetPredicted)
        {
            positionToMoveTowards = new Vector2(paddle.position.x, ball.position.y);
        }

        // float currentSpeed = Random.Range(0.10f, 1.0f) * paddleSpeed;
        float currentSpeed = paddleSpeed;
        paddle.position = Vector2.MoveTowards(paddle.position, positionToMoveTowards, currentSpeed * Time.deltaTime);
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
        if (paddleName == this.paddleName)
        {
            // we want to track follow the ball position just after our paddle hits it
            isTargetPredicted = false;
        }
        else
        {
            // we want to delay our response time and then compute optimal y
            // and then set this target prediction just once
            isTargetPredicted = true;
            isMovementSuspended = true;
            yield return new WaitForSeconds(responseTime);

            isMovementSuspended = false;
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
            return ball.position.y;
        }

        /*
        int count = 0;
        Vector2 rayHitPosition = ball.position;
        while (rayHitPosition.x < paddle.position.x)
        {
            if (Physics2D.Raycast(rayHitPosition, Vector2.right))
            {
                Debug.Log("HIT!");
            }
            if (++count == 2) {
                break;
            }
        }
        */

        float slope = ball.velocity.x / ball.velocity.y;
        float yOffset = ball.position.y - (slope * ball.position.x);
        result = slope * x + yOffset;
        Debug.Log("Predicted result=" + result);
        return result;
    }

    void Laser()
    {
        DrawReflectionPattern(ball.position + Vector2.right * 0.75f, this.transform.forward, maxReflectionCount);
    }
    public int maxReflectionCount = 5;
    public float maxStepDistance = 200f;
    private void DrawReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
    {
        if (reflectionsRemaining == 0)
        {
            return;
        }

        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }
        else
        {
            position += direction * maxStepDistance;
        }

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(startingPosition, position);
        Debug.DrawLine(startingPosition, position, Color.blue);
        DrawReflectionPattern(position, direction, reflectionsRemaining - 1);
    }
}
