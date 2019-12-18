using UnityEngine;

public class BallController : MonoBehaviour
{
    public readonly float SPEED = 30;
    public readonly Vector2 INITIAL_DIRECTION = Vector2.right;
    private Rigidbody2D ball;
    
    void Start()
    {
        ball = GetComponent<Rigidbody2D>();
        ball.velocity = INITIAL_DIRECTION * SPEED;
    }

    float hitFactor(Vector2 ballPos, Vector2 racketPos,
                float racketHeight)
    {
        // ascii art:
        // ||  1 <- at the top of the racket
        // ||
        // ||  0 <- at the middle of the racket
        // ||
        // || -1 <- at the bottom of the racket
        return (ballPos.y - racketPos.y) / racketHeight;
    }
}
