using UnityEngine;


// todo: handle difficulty level, maybe when ai controller is reset, pass it in?
public class GameRoundController : MonoBehaviour
{
    private RecordedScore recordedScore;

    // todo: replace with Paddle/MoveController interfaces, and use like `MoveController.Reset()`
    public GameObject ball;
    public GameObject playerPaddle;
    public GameObject aiPaddle;

    public GameObject leftGoal;
    public GameObject rightGoal;

    void Awake()
    {
        if ((playerPaddle.transform.position.x < 0 && aiPaddle.transform.position.x < 0) ||
            (playerPaddle.transform.position.x > 0 && aiPaddle.transform.position.x > 0))
        {
            Debug.LogError("Both player and paddle cannot be on the same side of the arena.");
        }
        GameEventCenter.startNewGame.AddAutoUnsubscribeListener(StartNewGame);
    }

    void OnEnable()
    {
        GameEventCenter.goalHit.AddListener(MoveToNextRound);
        GameEventCenter.restartGame.AddListener(RestartGame);
    }
    void OnDisable()
    {
        GameEventCenter.goalHit.RemoveListener(MoveToNextRound);
        GameEventCenter.restartGame.RemoveListener(RestartGame);
    }

    private void StartNewGame(StartNewGameInfo startGameInfo)
    {
        Debug.Log(startGameInfo);
        recordedScore = new RecordedScore(startGameInfo.NumberOfGoals);
        GameEventCenter.scoreChange.Trigger(recordedScore);
    }
    private void RestartGame(string status)
    {
        ResetMovingObjects();
        recordedScore = new RecordedScore(recordedScore.WinningScore);
        GameEventCenter.scoreChange.Trigger(recordedScore);
    }
    private void MoveToNextRound(string goalName)
    {
        if (recordedScore == null)
        {
            Debug.LogError("RecordedScore object received in StartNewGame event is missing, event must not been triggered");
        }
        ResetMovingObjects();
        IncrementScoreBasedOnGoal(goalName);
        GameEventCenter.scoreChange.Trigger(recordedScore);
        if (recordedScore.IsWinningScoreReached())
        {
            GameEventCenter.winningScoreReached.Trigger(recordedScore);
        }
    }

    private void ResetMovingObjects()
    {
        ball.GetComponent<BallController>().Reset();
        playerPaddle.GetComponent<PlayerController>().Reset();
        aiPaddle.GetComponent<AiController>().Reset();
    }
    private void IncrementScoreBasedOnGoal(string goalName)
    {
        if (goalName == rightGoal.name)
        {
            recordedScore.IncrementLeftPlayerScore();
        }
        else if (goalName == leftGoal.name)
        {
            recordedScore.IncrementRightPlayerScore();
        }
        else
        {
            Debug.LogError("Goal name '" + goalName + "' does not match registered goal names");
        }
    }
}
