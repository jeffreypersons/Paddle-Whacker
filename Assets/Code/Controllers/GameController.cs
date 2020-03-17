using UnityEngine;


public class GameController : MonoBehaviour
{
    // todo: replace with Paddle/MoveController interfaces, and use like `MoveController.Reset()`
    public GameObject ball;
    public GameObject playerPaddle;
    public GameObject aiPaddle;

    public GameObject leftGoal;
    public GameObject rightGoal;

    void Start()
    {
        GameData.Init();
        if ((playerPaddle.transform.position.x < 0 && aiPaddle.transform.position.x < 0) ||
            (playerPaddle.transform.position.x > 0 && aiPaddle.transform.position.x > 0))
        {
            Debug.LogError("Both player and paddle cannot be on the same side of the arena.");
        }
    }

    void OnEnable()
    {
        GameEvents.onGoalHit.AddListener(MoveToNextRound);
    }
    void OnDisable()
    {
        GameEvents.onGoalHit.RemoveListener(MoveToNextRound);
    }
    public void MoveToNextRound(string goalName)
    {
        ResetMovingObjects();
        IncrementScoreBasedOnGoal(goalName);
        LoadSceneIfWinningScore("EndMenu");
        GameEvents.onScoreChanged.Invoke();
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
            GameData.leftPlayerScore += 1;
        }
        else if (goalName == leftGoal.name)
        {
            GameData.rightPlayerScore += 1;
        }
        else
        {
            Debug.LogError("Goal name '" + goalName + "' does not match registered goal names");
        }
    }
    private void LoadSceneIfWinningScore(string sceneName)
    {
        if (GameData.leftPlayerScore == GameData.winningScore ||
            GameData.rightPlayerScore == GameData.winningScore)
        {
            GameScenes.Load(sceneName);
        }
    }
}
