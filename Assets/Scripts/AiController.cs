using UnityEngine;

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;

    [HideInInspector] public int score;
    [HideInInspector] public Vector2 initialPosition;

    private Rigidbody2D paddle;
    private Rigidbody2D ball;

    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();

        score = 0;
        initialPosition = paddle.position;
    }

    void FixedUpdate()
    {
        Vector2 current = paddle.position;
        Vector2 target = new Vector2(current.x, ball.position.y);
        paddle.position = Vector2.MoveTowards(current, target, paddleSpeed * Time.deltaTime);
    }
}
