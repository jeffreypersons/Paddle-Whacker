using UnityEngine;

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;

    [HideInInspector] public Vector2 initialPosition;

    private Rigidbody2D paddle;
    private Rigidbody2D ball;

    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();

        initialPosition = paddle.position;
    }

    void FixedUpdate()
    {
        Vector2 current = paddle.position;
        Vector2 target = new Vector2(current.x, ball.position.y);
        float currentSpeed = Random.Range(0.10f, 1.0f) * paddleSpeed;
        paddle.position = Vector2.MoveTowards(current, target, currentSpeed * Time.deltaTime);
    }
}
