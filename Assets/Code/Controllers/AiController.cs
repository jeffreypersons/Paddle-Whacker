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
    private Vector2 positionToMoveTowards;
    private Rigidbody2D ball;
    private List<Vector2> ballTrajectory;

    private Vector2 initialPosition;
    private Rigidbody2D paddle;

    public AiController()
    {
        ballTrajectory = new List<Vector2>();
    }
    public void Reset()
    {
        ballTrajectory.Clear();
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
            positionToMoveTowards = PredictOptimalDefensivePosition();
            yield return new WaitForSeconds(responseTime);
            isMovementSuspended = false;
            Debug.Log("Computed trajectory=[" + string.Join(",", ballTrajectory) + "]");
        }
    }
    // note: ball hit position not needed since time/reflections/friction is not yet accounted for
    // note: since x is fixed (currently only vertical paddle movement is possible), we only need to predict optimal y
    private Vector2 PredictOptimalDefensivePosition()
    {
        ballTrajectory.Clear();
        ballTrajectory.Add(ball.position);
        float targetX = paddle.position.x;
        ComputeTrajectory(ball.position, ball.velocity.normalized, targetX, maxReflectionCount);
        return new Vector2(targetX, ballTrajectory[ballTrajectory.Count - 1].y);
    }

    // conceptually: draw line from ball position until a reflection occurs, repeating until target is met,
    // or no recursion depth (maxReflections) is met (which case it calculates partway off the last bounce)
    // when nothing to bounce off of is hit, let the trajectory go all the way to the paddle
    private void ComputeTrajectory(Vector2 position, Vector2 direction, float targetX, int reflectionsRemaining)
    {
        // todo: figure out why it only works for a single bounce!
        // todo: figure out why the ray is backwards!
        if (position.x == targetX)
        {
            ballTrajectory.Add(position);
            return;
        }
        else if (reflectionsRemaining == 0)
        {
            Debug.Log("ComputeTrajectory::hit=used all reflections");
            float horizontalStepForPartialBounce = 0.5f * Mathf.Abs(targetX - position.x);
            ballTrajectory.Add(position + (direction * horizontalStepForPartialBounce));
            return;
        }

        float distanceToPaddle = Mathf.Abs(targetX - position.x);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distanceToPaddle);
        if (hit.transform != null && hit.transform.CompareTag("HorizontalWall"))
        {
            Debug.Log("ComputeTrajectory::hit=" + hit.transform.name);
            ballTrajectory.Add(hit.point);
            Debug.DrawLine(position, hit.point, Color.green, 2.5f);
            ComputeTrajectory(hit.point, Vector2.Reflect(direction, hit.normal), targetX, reflectionsRemaining - 1);
        }
        else
        {
            Debug.Log("ComputeTrajectory::hit=targetX");
            ballTrajectory.Add(position + (direction * distanceToPaddle));
            Debug.DrawLine(position, ballTrajectory[ballTrajectory.Count - 1], Color.green, 2.5f);
        }
    }
}
