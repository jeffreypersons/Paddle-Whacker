using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNextRound : MonoBehaviour
{
    private BallController ballController;
    private PlayerController leftPaddleController;
    private AiController rightPaddleController;

    private TMPro.TextMeshProUGUI leftScoreLabel;
    private TMPro.TextMeshProUGUI rightScoreLabel;

    void Start()
    {
        // todo: add listeners for player scores instead in a separate script or something,
        // so they simply just change when `data.player1Score` and `data.player2Score` change
        ballController = GameObject.Find("Ball").GetComponent<BallController>();
        leftPaddleController = GameObject.Find("LeftPaddle").GetComponent<PlayerController>();
        rightPaddleController = GameObject.Find("RightPaddle").GetComponent<AiController>();

        leftScoreLabel = GameObject.Find("LeftPlayerScore").GetComponent<TMPro.TextMeshProUGUI>();
        rightScoreLabel = GameObject.Find("RightPlayerScore").GetComponent<TMPro.TextMeshProUGUI>();
    }

    void OnEnable()
    {
        GameEvents.onVerticalWallHit.AddListener(MoveToNextRound);
    }
    void OnDisable()
    {
        GameEvents.onVerticalWallHit.RemoveListener(MoveToNextRound);
    }
    public void MoveToNextRound(string goalName)
    {
        IncrementScoreBasedOnGoal(goalName);
        if (HasWinningPlayer())
        {
            SceneManager.LoadScene("EndMenu");
        }
        ballController.Reset();
        leftPaddleController.Reset();
        rightPaddleController.Reset();
    }
    private void IncrementScoreBasedOnGoal(string goalName)
    {
        var data = DataManager.instance;
        if (goalName == "LeftWall")
        {
            data.player2Score += 1;
            rightScoreLabel.text = data.player2Score.ToString();
        }
        else if (goalName == "RightWall")
        {
            data.player1Score += 1;
            leftScoreLabel.text = data.player1Score.ToString();
        }
    }
    private bool HasWinningPlayer()
    {
        var data = DataManager.instance;
        return (data.player1Score == data.WINNING_SCORE || data.player2Score == data.WINNING_SCORE);
    }
}
