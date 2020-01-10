using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed;
    public Vector2 initialDirection;

    [HideInInspector] public Vector2 initialPosition;

    private Rigidbody2D ball;

    void Start()
    {
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        ball.velocity = ballSpeed * initialDirection;

        initialPosition = ball.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ball.velocity = ballSpeed * ComputeBounceDirection(ball.position, collision.rigidbody.position, collision.collider);
        }
        if (collision.gameObject.CompareTag("HorizontalWall"))
        {
            // desired behavior already handled by collider defaults
        }
        if (collision.gameObject.CompareTag("VerticalWall"))
        {
            // for now scoring logic handled within ball class, but external event system would be preferred
            // see https://github.com/jeffreypersons/Pong/issues/9
            switch (collision.gameObject.name)
            {
                case "LeftWall":  IncrementRightPlayerScore(); break;
                case "RightWall": IncrementLeftPlayerScore();  break;
            }

            ResetPositions();
        }
    }
    private Vector2 ComputeBounceDirection(Vector2 ballPosition, Vector2 paddlePosition, Collider2D paddleCollider)
    {
        float invertedXDirection = ballPosition.x - paddlePosition.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ball.position.y - paddlePosition.y) / paddleCollider.bounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
    }
    private void IncrementLeftPlayerScore()
    {
        PlayerController controller = GameObject.Find("LeftPaddle").GetComponent<PlayerController>();
        controller.score += 1;
        GameObject.Find("LeftScoreLabel").GetComponent<TMPro.TextMeshProUGUI>().text = controller.score.ToString();
    }
    private void IncrementRightPlayerScore()
    {
        AiController controller = GameObject.Find("RightPaddle").GetComponent<AiController>();
        controller.score += 1;
        GameObject.Find("RightScoreLabel").GetComponent<TMPro.TextMeshProUGUI>().text = controller.score.ToString();
    }
    private void ResetPositions()
    {
        ball.position = initialPosition;
        ball.velocity = ballSpeed * initialDirection;

        PlayerController leftController = GameObject.Find("LeftPaddle").GetComponent<PlayerController>();
        leftController.GetComponent<Rigidbody2D>().position = leftController.initialPosition;

        AiController rightController = GameObject.Find("RightPaddle").GetComponent<AiController>();
        rightController.GetComponent<Rigidbody2D>().position = rightController.initialPosition;
    }
}
