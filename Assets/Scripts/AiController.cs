using UnityEngine;

public class AiController : MonoBehaviour
{
    public float moveSpeed;
    
    private Rigidbody2D paddle;
    private Rigidbody2D ball;

    void Start()
    {
        paddle = GameObject.Find("AiPaddle").GetComponent<Rigidbody2D>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 current = paddle.position;
        Vector2 target = new Vector2(current.x, ball.position.y);
        paddle.position = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);
    }
}
