using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float paddleMoveSpeed = 30;

    private Rigidbody2D paddleBody;
    private Vector2 directionOfMovement;
    
    void Start()
    {
        paddleBody = GameObject.Find("PlayerPaddle").GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        directionOfMovement = new Vector2(0, paddleMoveSpeed * Mathf.Sign(Input.GetAxisRaw("Vertical")));
    }

    void FixedUpdate()
    {
        paddleBody.velocity = paddleMoveSpeed * directionOfMovement;
    }
}
