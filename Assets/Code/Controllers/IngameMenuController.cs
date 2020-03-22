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

        GameEventCenter.pauseGame.AddListener(OpenAsPauseMenu);
        GameEventCenter.winningScoreReached.AddListener(OpenAsEndGameMenu);
    }
    void OnDestroy()
    {
        GameEventCenter.pauseGame.RemoveListener(OpenAsPauseMenu);
        GameEventCenter.winningScoreReached.RemoveListener(OpenAsEndGameMenu);
    }

    void OnEnable()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(MoveToMainMenu);
        restartButton.onClick.AddListener(TriggerRestartGameEvent);
        quitButton.onClick.AddListener(SceneUtils.QuitGame);
        // todo: show menu and arena walls, but make pauseButton, labels, paddles, and midline non-visible
    }
    void OnDisable()
    {
        resumeButton.onClick.RemoveListener(ResumeGame);
        mainMenuButton.onClick.RemoveListener(MoveToMainMenu);
        restartButton.onClick.RemoveListener(TriggerRestartGameEvent);
        quitButton.onClick.RemoveListener(SceneUtils.QuitGame);
    }

    private void SetPauseMode(bool isPause)
    {
        ingameMenu.SetActive(isPause);
        Time.timeScale = isPause ? 0 : 1;

        GameObjectUtils.SetAlpha(GameObject.Find("MidLine").GetComponent<SpriteRenderer>(), 0.0f);
        // todo: show menu and arena walls, but make pauseButton, labels, paddles, and midline non-visible
    }

    private void OpenAsPauseMenu(RecordedScore recordedScore)
    {
        SetPauseMode(true);
        title.text    = recordedScore.IsLeftPlayerWinning() ? "Game Won" : "Game Lost";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();
        resumeButton.enabled = true;
    }
    private void OpenAsEndGameMenu(RecordedScore recordedScore)
    {
        SetPauseMode(true);
        title.text    = "Game Paused";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();
        resumeButton.enabled = false;
    }

    private void ResumeGame()
    {
        SetPauseMode(false);
        GameEventCenter.resumeGame.Trigger("Resuming game");
    }
    private void MoveToMainMenu()
    {
        SetPauseMode(false);
        GameEventCenter.gotoMainMenu.Trigger("Opening main menu");
        SceneUtils.LoadScene("MainMenu");
    }
    private void TriggerRestartGameEvent()
    {
        SetPauseMode(false);
        GameEventCenter.restartGame.Trigger("Restarting game");
    }
}
