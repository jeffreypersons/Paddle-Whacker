using System.Collections.Generic;
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
    private int maxReflectionCount = 5;
    private Vector2 positionToMoveTowards;
    private Rigidbody2D ball;
    private List<Vector2> trajectory;

    private Vector2 initialPosition;
    private Rigidbody2D paddle;

    public AiController()
    {
        trajectory = new List<Vector2>();
    }
    public void Reset()
    {
        trajectory.Clear();
        paddle.position = initialPosition;
        positionToMoveTowards = initialPosition;
        paddle.velocity = Vector2.zero;
        isMovementSuspended = false;
        isTargetPredicted = false;
    }
    void Start()
    {
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        initialPosition = paddle.position;
        Reset();
    }

    // if ball's last paddle hit = AI: keep horizontally aligned with ball
    // if ball's last paddle hit = Opponent: start moving after time delay towards predicted ball trajectory
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
        paddle.position = Vector2.MoveTowards(paddle.position, positionToMoveTowards, paddleSpeed * Time.deltaTime);
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

    // when opponent's paddle is hit, predict the ball trajectory after some time T
    private IEnumerator UpdateTargetPosition(string paddleName)
    {
        if (paddleName == this.paddleName)
        {
            isTargetPredicted = false;
        }
        else
        {
            isTargetPredicted = true;
            isMovementSuspended = true;
            yield return new WaitForSeconds(responseTime);
            isMovementSuspended = false;

            positionToMoveTowards = PredictOptimalDefensivePosition();
            Debug.Log(string.Join(",", trajectory));
            Debug.Log("pos("        + ball.position            + "), " +
                      "dir( "       + ball.velocity.normalized + "), " +
                      "bounces("    + trajectory.Count         + "), " +
                      "prediction(" + positionToMoveTowards    + ")");
        }
    }
    // note: ball hit position not needed since time/reflections/friction is not yet accounted for
    // note: since x is fixed (currently only vertical paddle movement is possible), we only need to predict optimal y
    private Vector2 PredictOptimalDefensivePosition()
    {
        trajectory.Clear();
        float targetX = paddle.position.x;
        ComputeTrajectory(ball.position, ball.velocity.normalized, targetX, maxReflectionCount);
        return new Vector2(targetX, trajectory[trajectory.Count - 1].y);
    }

    // conceptually: draw line from ball position until a reflection occurs, repeating until target is met,
    // or no recursion depth (maxReflections) is met
    private void ComputeTrajectory(Vector2 position, Vector2 direction, float targetX, int reflectionsRemaining)
    {
        if (position.x == targetX)
        {
            trajectory.Add(position);
            return;
        }
        else if (reflectionsRemaining == 0)
        {
            // couldn't make it all the way to target, so stop halfway off the last bounce
            float horizontalStepForPartialBounce = 1.0f * Mathf.Abs(targetX - ball.position.x);
            Vector2 maximumForecastedPosition = ball.position + (direction * horizontalStepForPartialBounce);
            trajectory.Add(maximumForecastedPosition);
            return;
        }

        float distanceToPaddle = Mathf.Abs(targetX - position.x);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, float.PositiveInfinity);

        Vector2 hitPosition, hitBounceDirection;
        if (hit.transform != null && hit.transform.CompareTag("HorizontalWall"))
        {
            Debug.Log(hit.transform.name);
            hitPosition = hit.point;
            hitBounceDirection = Vector2.Reflect(direction, hit.normal);
        }
        else
        {
            // nothing to bounce off of, so let the trajectory go all the way to the paddle
            hitPosition = position + (direction * distanceToPaddle);
            hitBounceDirection = direction;
        }

        trajectory.Add(position);
        ComputeTrajectory(hitPosition, hitBounceDirection, targetX, reflectionsRemaining - 1);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i + 1 < trajectory.Count; i++)
        {
            Gizmos.DrawLine(trajectory[i], trajectory[i + 1]);
        }
    }
}
