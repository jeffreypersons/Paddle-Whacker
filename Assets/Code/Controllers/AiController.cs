using UnityEngine;

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public float responseTime;

    [HideInInspector] public Vector2 initialPosition;

    private Rigidbody2D paddle;
    private Rigidbody2D ball;
    private Vector2 positionToMoveTowards;

    void OnEnable()
    {
        GameEvents.onPaddleHit.AddListener(UpdateTargetPosition);
    }
    void OnDisable()
    {
        GameEvents.onPaddleHit.RemoveListener(UpdateTargetPosition);
    }

    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();

        initialPosition = paddle.position;
    }

    void FixedUpdate()
    {
        Vector2 current = paddle.position;
        positionToMoveTowards = new Vector2(current.x, ball.position.y);
        float currentSpeed = Random.Range(0.10f, 1.0f) * paddleSpeed;
        paddle.position = Vector2.MoveTowards(current, positionToMoveTowards, currentSpeed * Time.deltaTime);
    }
    void UpdateTargetPosition(string paddleName)
    {
        if (paddleName == "LeftPaddle")
        {
            positionToMoveTowards = Vector2.zero;
        }
        // TODO: logic will go here for predicting position see ticket 'Make ai feel less robotic'
        Debug.Log(paddleName);
    }
}
