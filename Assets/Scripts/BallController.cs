using UnityEngine;

public class BallController : MonoBehaviour
{
    public float moveSpeed;
    public Vector2 initialDirection;

    private Rigidbody2D ball;

    void Start()
    {
        ball = GetComponent<Rigidbody2D>();
        ball.velocity = moveSpeed * initialDirection;
    }

    // upon hitting a paddle, reflect the ball. everything else is taken care (ie walls) of automatically by colliders
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ball.velocity = moveSpeed * ComputeBounceDirection(ball.position, collision.rigidbody.position, collision.collider);
        }
    }
    private Vector2 ComputeBounceDirection(Vector2 ballPosition, Vector2 paddlePosition, Collider2D paddleCollider)
    {
        float invertedXDirection = ballPosition.x - paddlePosition.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ball.position.y - paddlePosition.y) / paddleCollider.bounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
    }
}
