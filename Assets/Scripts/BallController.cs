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

    // upon hitting a paddle, we invert the direction of the ball
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector2 paddlePosition = collision.rigidbody.position;
            Vector2 paddleSize = collision.collider.bounds.size;

            float invertedXDirection = ball.position.x - paddlePosition.x > 0 ? -1 : 1;
            float offsetFromPaddleCenterToBall = (ball.position.y - paddlePosition.y) / paddleSize.y;
            ball.velocity = moveSpeed * new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
        }
    }
} 
