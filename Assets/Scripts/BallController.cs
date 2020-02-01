using UnityEngine;
using UnityEngine.SceneManagement;

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
            // desired bounce behavior already handled by collider defaults
        }
        if (collision.gameObject.CompareTag("VerticalWall"))
        {
            // for now some scoring logic/position-resetting logic is still handled within ball class,
            // despite recent decoupling, but an external event system would be better...
            IncrementScoreBaseOnGoal(collision.gameObject.name);
            if (HasWinningPlayer())
            {
                SceneManager.LoadScene("EndMenu");
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
    private void IncrementScoreBaseOnGoal(string goalName)
    {
        var data = DataManager.instance;
        if (goalName == "LeftWall")
        {
            data.player2Score += 1;
            GameObject.Find("RightPlayerScore").GetComponent<TMPro.TextMeshProUGUI>().text = data.player2Score.ToString();
        }
        else if (goalName == "RightWall")
        {
            data.player1Score += 1;
            GameObject.Find("LeftPlayerScore").GetComponent<TMPro.TextMeshProUGUI>().text = data.player1Score.ToString();
        }
    }
    private bool HasWinningPlayer()
    {
        var data = DataManager.instance;
        return (data.player1Score == data.WINNING_SCORE || data.player2Score == data.WINNING_SCORE);
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
