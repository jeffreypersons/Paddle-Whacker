using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    
    private Rigidbody2D paddle;
    private Vector2 moveDirection;
    
    void Start()
    {
        paddle = GameObject.Find("PlayerPaddle").GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveDirection = new Vector2(0, moveSpeed * Mathf.Sign(Input.GetAxisRaw("Vertical")));
    }

    void FixedUpdate()
    {
        paddle.velocity = moveSpeed * moveDirection;
    }
}
