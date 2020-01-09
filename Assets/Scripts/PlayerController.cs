using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public string inputAxisName;

    [HideInInspector] public int score;

    private Vector2 inputVelocity;
    private Rigidbody2D paddle;
    private Rigidbody2D ball;

    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();

        score = 0;
        inputVelocity = Vector2.zero;
        paddle.velocity = inputVelocity;
    }
    
    void Update()
    {
        inputVelocity = new Vector2(0, paddleSpeed * Input.GetAxisRaw(inputAxisName));
    }

    void FixedUpdate()
    {
        paddle.velocity = inputVelocity;
    }
}
