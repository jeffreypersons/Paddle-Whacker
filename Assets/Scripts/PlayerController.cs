using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public string inputAxisName;

    private Rigidbody2D paddle;
    private Vector2 moveDirection;
    
    void Start()
    {
        paddle = GameObject.Find("PlayerPaddle").GetComponent<Rigidbody2D>();
        paddle.velocity = Vector2.zero;
    }

    void Update()
    {
        moveDirection = new Vector2(0, Input.GetAxisRaw(inputAxisName));
    }

    void FixedUpdate()
    {
        paddle.velocity = moveSpeed * moveDirection;
    }
}
