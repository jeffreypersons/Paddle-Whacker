using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public string inputAxisName;

    private int score;
    private Rigidbody2D paddle;
    private Vector2 inputVelocity;

    private Rigidbody2D ball;
    
    void Start()
    {
        paddle = GameObject.Find("PlayerPaddle").GetComponent<Rigidbody2D>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();

        score = 0;
        inputVelocity = Vector2.zero;
        paddle.velocity = inputVelocity;
    }
    
    void Update()
    {
        inputVelocity = new Vector2(0, moveSpeed * Input.GetAxisRaw(inputAxisName));
    }

    void FixedUpdate()
    {
        paddle.velocity = inputVelocity;
    }
}
