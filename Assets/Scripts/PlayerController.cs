using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public readonly float SPEED = 30;
    public Rigidbody2D paddleBody;

    private Vector2 directionOfMovement;
    
    void Start()
    {
        paddleBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        directionOfMovement = new Vector2(0, Mathf.Sign(Input.GetAxisRaw("Vertical")) * SPEED);
    }

    void FixedUpdate()
    {
        paddleBody.velocity = directionOfMovement * SPEED;
    }
}
