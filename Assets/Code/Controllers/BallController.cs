using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed;
    public Vector2 initialDirection;

    private Vector2 initialPosition;
    private Rigidbody2D ballBody;
    private BoxCollider2D ballCollider;

    public void Reset()
    {
        ballBody.position = initialPosition;
        ballBody.velocity = ballSpeed * initialDirection;
    }
    void Start()
    {
        ballBody = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        ballCollider = GameObject.Find("Ball").GetComponent<BoxCollider2D>();
        initialPosition = ballBody.position;
        Reset();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ballBody.velocity = ballSpeed *
                ComputeBounceDirection(ballBody.position, ballCollider, collision.rigidbody.position, collision.collider);
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

    private static Vector2 ComputeBounceDirection(Vector2 ballPosition, Collider2D ballCollider,
        Vector2 paddlePosition, Collider2D paddleCollider)
    {
        float invertedXDirection = ballPosition.x + paddlePosition.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ballPosition.y - paddlePosition.y) / paddleCollider.bounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
    }
}
