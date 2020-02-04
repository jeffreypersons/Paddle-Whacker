using UnityEngine;

public class GameController : MonoBehaviour
{
    private BallController ballController;
    private PlayerController leftPaddleController;
    private AiController rightPaddleController;

    private TMPro.TextMeshProUGUI leftScoreLabel;
    private TMPro.TextMeshProUGUI rightScoreLabel;

    void Start()
    {
        Debug.Log("Starting Game");
        GameData.Init();

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
        EndGameIfWinner();
        ResetRound();
    }
    private void IncrementScoreBasedOnGoal(string goalName)
    {
        if (goalName == "LeftWall")
        {
            GameData.player2Score += 1;
            rightScoreLabel.text = GameData.player2Score.ToString();
        }
        else if (goalName == "RightWall")
        {
            GameData.player1Score += 1;
            leftScoreLabel.text = GameData.player1Score.ToString();
        }
    }
    private void EndGameIfWinner()
    {
        if (GameData.player1Score == GameData.winningScore ||
            GameData.player2Score == GameData.winningScore)
        {
            GameScenes.Load("EndMenu");
        }
    }
    private void ResetRound()
    {
        ballController.Reset();
        leftPaddleController.Reset();
        rightPaddleController.Reset();
    }
}
