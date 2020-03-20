using UnityEngine;
using UnityEngine.UI;


public class IngameMenuController : MonoBehaviour
{
    public GameObject ingameMenu;
    public TMPro.TextMeshProUGUI title;
    public TMPro.TextMeshProUGUI subtitle;

    public Button resumeButton;
    public Button mainMenuButton;
    public Button restartButton;
    public Button quitButton;

    void Awake()
    {
        title    = title.GetComponent<TMPro.TextMeshProUGUI>();
        subtitle = subtitle.GetComponent<TMPro.TextMeshProUGUI>();

        resumeButton   = resumeButton.GetComponent<Button>();
        mainMenuButton = mainMenuButton.GetComponent<Button>();
        restartButton  = restartButton.GetComponent<Button>();
        quitButton     = quitButton.GetComponent<Button>();
    }

    void OnEnable()
    {
        GameEventCenter.pauseGame.StartListening(OpenMenuOnPause);
        GameEventCenter.winningScoreReached.StartListening(OpenMenuOnGameFinish);

        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        restartButton.onClick.AddListener(TriggerRestartGameEvent);
        quitButton.onClick.AddListener(SceneUtils.QuitGame);
    }
    void OnDisable()
    {
        GameEventCenter.pauseGame.StopListening(OpenMenuOnPause);
        GameEventCenter.winningScoreReached.StopListening(OpenMenuOnGameFinish);

        resumeButton.onClick.RemoveListener(ResumeGame);
        mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
        restartButton.onClick.RemoveListener(TriggerRestartGameEvent);
        quitButton.onClick.RemoveListener(SceneUtils.QuitGame);
    }

    private void ResumeGame()
    {

    }
    private void ReturnToMainMenu()
    {
        SceneUtils.LoadScene("MainMenu");
    }
    private void TriggerRestartGameEvent()
    {
        GameEventCenter.restartGame.Trigger("Restarting game");
        GameEventCenter.pauseGame.Trigger("Pausing");
    }

    private void OpenMenuOnPause(string status)
    {
        Time.timeScale = 0;
        // todo: show menu and arena walls, but make pauseButton, labels, paddles, and midline non-visible
        // todo: show result in case of endgame
        ingameMenu.SetActive(true);
    }
    private void OpenMenuOnGameFinish(RecordedScore scoreInfo)
    {
        ingameMenu.SetActive(true);
        Time.timeScale = 0;
        // todo: show menu and arena walls, but make pauseButton, labels, paddles, and midline non-visible
        if (scoreInfo.IsWinningScoreReached())
        {
            title.text = scoreInfo.IsLeftPlayerWinning() ? "Game Won" : "Game Lost";
            subtitle.text  = scoreInfo.LeftPlayerScore.ToString() + " - " + scoreInfo.RightPlayerScore.ToString();
        }
    }
}
