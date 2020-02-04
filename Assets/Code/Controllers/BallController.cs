using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed;
    public Vector2 initialDirection;

    private Vector2 initialPosition;
    private Rigidbody2D ball;

    public void Reset()
    {
        ball.position = initialPosition;
        ball.velocity = ballSpeed * initialDirection;
    }
    void Start()
    {
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        initialPosition = ball.position;
        Reset();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ball.velocity = ballSpeed * ComputeBounceDirection(ball.position, collision.rigidbody.position, collision.collider);
            GameEvents.onPaddleHit.Invoke(collision.gameObject.name);
        }
        if (collision.gameObject.CompareTag("HorizontalWall"))
        {
            // desired bounce behavior already handled by collider defaults
        }
        if (collision.gameObject.CompareTag("VerticalWall"))
        {
            GameEvents.onVerticalWallHit.Invoke(collision.gameObject.name);
        }
    }

    private Vector2 ComputeBounceDirection(Vector2 ballPosition, Vector2 paddlePosition, Collider2D paddleCollider)
    {
        float invertedXDirection = ballPosition.x - paddlePosition.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ball.position.y - paddlePosition.y) / paddleCollider.bounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
    }
}
