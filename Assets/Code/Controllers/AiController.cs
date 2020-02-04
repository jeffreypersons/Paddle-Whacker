using UnityEngine;

public class AiController : MonoBehaviour
{
    public string paddleName;
    public float paddleSpeed;
    public float responseTime;

    private Vector2 initialPosition;
    private Vector2 positionToMoveTowards;
    private Rigidbody2D paddle;

    public void Reset()
    {
        paddle.position = initialPosition;
        positionToMoveTowards = initialPosition;
        paddle.velocity = Vector2.zero;
    }
    void Start()
    {
        paddle = GameObject.Find(paddleName).GetComponent<Rigidbody2D>();
        initialPosition = paddle.position;
        Reset();
    }

    void FixedUpdate()
    {
        // todo: invoke a predict ball position script here instead
        float ballPositionY = GameObject.Find("Ball").GetComponent<Rigidbody2D>().position.y;

        Vector2 current = paddle.position;
        positionToMoveTowards = new Vector2(current.x, ballPositionY);
        float currentSpeed = Random.Range(0.10f, 1.0f) * paddleSpeed;

        if (positionToMoveTowards != initialPosition)
        {
            paddle.position = Vector2.MoveTowards(current, positionToMoveTowards, currentSpeed * Time.deltaTime);
        }
    }

    void OnEnable()
    {
        GameEvents.onPaddleHit.AddListener(UpdateTargetPosition);
    }
    void OnDisable()
    {
        GameEvents.onPaddleHit.RemoveListener(UpdateTargetPosition);
    }
    public void UpdateTargetPosition(string paddleName)
    {
        if (paddleName == "LeftPaddle")
        {
            positionToMoveTowards = Vector2.zero;
        }
        // TODO: logic will go here for predicting position see ticket 'Make ai feel less robotic'
        Debug.Log(paddleName);
    }
}
