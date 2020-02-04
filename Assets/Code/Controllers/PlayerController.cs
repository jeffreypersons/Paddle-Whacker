using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public string inputAxisName;

    private Vector2 initialPosition;
    private Vector2 inputVelocity;
    private Rigidbody2D paddle;

    public void Reset()
    {
        inputVelocity = Vector2.zero;
        paddle.velocity = inputVelocity;
        paddle.position = initialPosition;
    }
    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        initialPosition = paddle.position;
        Reset();
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
