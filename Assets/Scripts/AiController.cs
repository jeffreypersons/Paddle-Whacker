using UnityEngine;

public class AiController : MonoBehaviour
{
    public readonly float SPEED = 30;

    private Rigidbody2D racket;
    private Rigidbody2D ball; // change this later, its disgusting. this class doesn't own that object...

    void Start()
    {
        racket = GameObject.FindGameObjectWithTag("Ai").GetComponent<Rigidbody2D>();
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>();
        racket.velocity = new Vector2(0, SPEED);
    }
    void FixedUpdate()
    {
        if (racket.position.y == ball.position.y)
        {
            racket.velocity = new Vector2(0, 0);
        }
        else
        {
            Vector2 directionToMove = racket.position.y < ball.position.y ? Vector2.up : Vector2.down;
            racket.velocity = directionToMove * SPEED;
        }
    }
}
