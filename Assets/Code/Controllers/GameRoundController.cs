using UnityEngine;


public class GameRoundController : MonoBehaviour
{
    private ScoreInfo scoreInfo;

    // todo: replace with Paddle/MoveController interfaces, and use like `MoveController.Reset()`
    public GameObject ball;
    public GameObject playerPaddle;
    public GameObject aiPaddle;

    public GameObject leftGoal;
    public GameObject rightGoal;

    void Start()
    {
        // todo: replace winning score with a value supplied from startmenu
        scoreInfo = new ScoreInfo(5);
        if ((playerPaddle.transform.position.x < 0 && aiPaddle.transform.position.x < 0) ||
            (playerPaddle.transform.position.x > 0 && aiPaddle.transform.position.x > 0))
        {
            Debug.LogError("Both player and paddle cannot be on the same side of the arena.");
        }
    }

    void OnEnable()
    {
        GameEventCenter.goalHit.StartListening(MoveToNextRound);
    }
    void OnDisable()
    {
        GameEventCenter.goalHit.StopListening(MoveToNextRound);
    }
    public void MoveToNextRound(string goalName)
    {
        ResetMovingObjects();
        IncrementScoreBasedOnGoal(goalName);
        LoadSceneIfWinningScore("EndMenu");
        GameEventCenter.scoreChange.Trigger(scoreInfo);
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
            scoreInfo.LeftPlayerScore += 1;
        }
        else if (goalName == leftGoal.name)
        {
            scoreInfo.RightPlayerScore += 1;
        }
        else
        {
            Debug.LogError("Goal name '" + goalName + "' does not match registered goal names");
        }
    }
    private void LoadSceneIfWinningScore(string sceneName)
    {
        if (scoreInfo.IsWinningScoreReached())
        {
            GameEventCenter.gameFinished.Trigger(scoreInfo);
            SceneUtils.Load(sceneName);
        }
    }
}
