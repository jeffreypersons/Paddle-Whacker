using UnityEngine;

public class AiController : MonoBehaviour
{
    public float moveSpeed;
    
    private Rigidbody2D ball;
    private Rigidbody2D paddle;

    void Start()
    {
        ball = GetComponent<Rigidbody2D>();
        paddle = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 current = paddle.transform.position;
        Vector2 target = new Vector2(current.x, ball.transform.position.y);

        transform.position = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);
    }
}
