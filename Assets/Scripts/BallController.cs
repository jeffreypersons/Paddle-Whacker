using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed;
    public Vector2 initialDirection;

    private Rigidbody2D ball;

    void Start()
    {
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        ball.velocity = ballSpeed * initialDirection;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ball.velocity = ballSpeed * ComputeBounceDirection(ball.position, collision.rigidbody.position, collision.collider);
        }
        if (collision.gameObject.CompareTag("HorizontalWall"))
        {
            // desired behavior already handled by collider defaults
        }
        if (collision.gameObject.CompareTag("VerticalWall"))
        {
            // for now scoring logic handled within ball class, but external event system would be preferred
            // see https://github.com/jeffreypersons/Pong/issues/9
            UpdateScoreOnHittingWall(collision.gameObject.name);
        }
    }
    private Vector2 ComputeBounceDirection(Vector2 ballPosition, Vector2 paddlePosition, Collider2D paddleCollider)
    {
        float invertedXDirection = ballPosition.x - paddlePosition.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ball.position.y - paddlePosition.y) / paddleCollider.bounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
    }
    private void UpdateScoreOnHittingWall(string wallName)
    {
        if (wallName == "LeftWall")
        {
            GameObject.Find("RightPaddle").GetComponent<AiController>().score += 1;
        }
        else if (wallName == "RightWall")
        {
            GameObject.Find("LeftPaddle").GetComponent<PlayerController>().score += 1;
        }
    }
}
