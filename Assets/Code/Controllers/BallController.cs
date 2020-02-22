using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed;
    public Vector2 initialDirection;

    private Vector2 initialPosition;
    private Rigidbody2D ballBody;
    private Collider2D ballCollider;

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
            ballBody.velocity = ballSpeed * ComputeBounceDirection(ballCollider.bounds, collision.collider.bounds);
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

    private static Vector2 ComputeBounceDirection(Bounds ballBounds, Bounds paddleBounds)
    {
        float invertedXDirection = ballBounds.center.x + paddleBounds.center.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ballBounds.center.y - paddleBounds.center.y) / paddleBounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
    }
}
