using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public readonly float SPEED = 30;
    private Rigidbody2D racket;
    private Rigidbody2D ball; // change this later, its disgusting. this class doesn't own that object...

    void Start()
    {
        racket = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>();
        racket.velocity = new Vector2(0, SPEED);
    }
    void FixedUpdate()
    {
        float input = Input.GetAxisRaw("Vertical");
        if (input == 0)
        {
            racket.velocity = Vector2.zero;
        }
        else
        {
            Vector2 directionToMove = input > 0 ? Vector2.up : Vector2.down;
            racket.velocity = directionToMove * SPEED;
        }
    }
}
