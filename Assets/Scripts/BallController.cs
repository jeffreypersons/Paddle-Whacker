using UnityEngine;

public class BallController : MonoBehaviour
{
    public float moveSpeed;
    public Vector2 initialDirection;

    private Vector2 moveDirection;
    private Rigidbody2D ball;

    void Start()
    {
        ball = GetComponent<Rigidbody2D>();
        moveDirection = initialDirection;
        ball.velocity = moveSpeed * moveDirection;
    }
    
    // upon hitting a paddle, we invert the direction of the ball
    // TODO: add checking for walls
    // TODO: add handling for hitting top of paddle
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            float invertedXDirection = moveDirection.x > 0 ? -1 : 1;
            float offsetFromPaddleCenterToBall = (ball.position.y - collision.transform.position.y) / collision.collider.bounds.size.y;
            moveDirection = new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
            ball.velocity = moveSpeed * moveDirection;
        }
    }
}
