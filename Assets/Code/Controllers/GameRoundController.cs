using UnityEngine;


// note: relies on startNewGameEvent being triggered (from main menu), so if running in editor
// expect null parameters, and just run from main menu instead
public class GameRoundController : MonoBehaviour
{
    private RecordedScore recordedScore;

    [SerializeField] private GameObject ball         = default;
    [SerializeField] private GameObject playerPaddle = default;
    [SerializeField] private GameObject aiPaddle     = default;

    [SerializeField] private GameObject leftGoal     = default;
    [SerializeField] private GameObject rightGoal    = default;

    void Awake()
    {
        if ((playerPaddle.transform.position.x < 0 && aiPaddle.transform.position.x < 0) ||
            (playerPaddle.transform.position.x > 0 && aiPaddle.transform.position.x > 0))
        {
            Debug.LogError("Both player and paddle cannot be on the same side of the arena.");
        }
        if (leftGoal.name == rightGoal.name)
        {
            Debug.LogError($"Left and Right goals must have unique names to identify which player scored, " +
                           $"both are nonuniquely named {leftGoal.name}");
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

    private void StartNewGame(GameSettingsInfo gameSettings)
    {
        recordedScore = new RecordedScore(gameSettings.NumberOfGoals);
        aiPaddle.GetComponent<AiController>().SetDifficultyLevel(gameSettings.DifficultyLevel);
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
        ResetMovingObjects();

        if (recordedScore == null)
        {
            Debug.LogError($"RecordedScore that is set upon starting a new game {GetType().Name} is missing, " +
                           $"perhaps the event wasn't fired or listened to? " +
                           $"If running from game scene in play mode, try starting from main menu instead");
            return;
        }

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
