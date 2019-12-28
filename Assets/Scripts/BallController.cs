using UnityEngine;

public class BallController : MonoBehaviour
{
    public readonly float SPEED = 30;
    public readonly Vector2 INITIAL_DIRECTION = Vector2.right;

    private Vector2 directionOfMovement;
    private Rigidbody2D ballBody;

    void Start()
    {
        ballBody = GetComponent<Rigidbody2D>();
        directionOfMovement = INITIAL_DIRECTION;
        ballBody.velocity = directionOfMovement * SPEED;
    }

    // upon hitting a paddle, we invert the direction of the ball
    // TODO: add checking for walls
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 ballPosition = transform.position;
        Vector2 otherPosition = collision.transform.position;

        if (collision.gameObject.GetComponent("Paddle") != null)
        {
            float invertedXDirection = directionOfMovement.x > 0? -1 : 1;
            float offsetFromPaddleCenterToBall = (otherPosition.y - ballPosition.y) / collision.collider.bounds.size.y;
            directionOfMovement = new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
            ballBody.velocity = directionOfMovement * SPEED;
        }
    }
}
