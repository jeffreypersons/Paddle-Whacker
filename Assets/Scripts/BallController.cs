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
            // desired behavior already handled by collider defaults
        }
        // REALLY UGLY, I KNOW! this tech debt will be addressed in the next issue through thorough refactoring.
        if (collision.gameObject.CompareTag("VerticalWall"))
        {
            // for now scoring logic handled within ball class, but an external event system
            // would be preferred, with less code duplication
            // see https://github.com/jeffreypersons/Pong/issues/9
            var data = DataManager.instance;
            switch (collision.gameObject.name)
            {
                case "LeftWall":  data.player2Score += 1; break;
                case "RightWall": data.player1Score += 1; break;
            }

            GameObject.Find("LeftPlayerScore").GetComponent<TMPro.TextMeshProUGUI>().text = DataManager.instance.player1Score.ToString();
            GameObject.Find("RightPlayerScore").GetComponent<TMPro.TextMeshProUGUI>().text = DataManager.instance.player2Score.ToString();
            if (data.player1Score == data.WINNING_SCORE || data.player2Score == data.WINNING_SCORE)
            {
                SceneManager.LoadScene("EndMenu");
            }
            data.ResetAll();
            ResetPositions();
        }
    }

    private Vector2 ComputeBounceDirection(Vector2 ballPosition, Vector2 paddlePosition, Collider2D paddleCollider)
    {
        float invertedXDirection = ballPosition.x - paddlePosition.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ball.position.y - paddlePosition.y) / paddleCollider.bounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
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
