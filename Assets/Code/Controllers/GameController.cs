using UnityEngine;

public class GameController : MonoBehaviour
{
    public string player1PaddleName;
    public string player1OpposingGoalName;

    public string player2PaddleName;
    public string player2OpposingGoalName;

    private BallController ballController;
    private PlayerController player1Controller;
    private AiController player2Controller;

    private TMPro.TextMeshProUGUI leftScoreLabel;
    private TMPro.TextMeshProUGUI rightScoreLabel;

    void Start()
    {
        GameData.Init();

        ballController = GameObject.Find("Ball").GetComponent<BallController>();
        player1Controller = GameObject.Find(player1PaddleName).GetComponent<PlayerController>();
        player2Controller = GameObject.Find(player2PaddleName).GetComponent<AiController>();
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
        ResetRound();
        IncrementScoreBasedOnGoal(goalName);
        FinishGameIfWinner();
    }

    private void ResetRound()
    {
        ballController.Reset();
        player1Controller.Reset();
        player2Controller.Reset();
    }
    private void IncrementScoreBasedOnGoal(string goalName)
    {
        if (goalName == player1OpposingGoalName)
        {
            GameData.player1Score += 1;
        }
        else if (goalName == player2OpposingGoalName)
        {
            GameData.player2Score += 1;
        }
        else
        {
            Debug.LogError("Goal name '" + goalName + "' does not match registered goal names");
        }
    }
    private void FinishGameIfWinner()
    {
        if (GameData.player1Score == GameData.winningScore ||
            GameData.player2Score == GameData.winningScore)
        {
            GameScenes.Load("EndMenu");
        }
    }
}
