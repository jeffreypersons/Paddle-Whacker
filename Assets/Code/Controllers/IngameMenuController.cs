using System.Collections.Generic;
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

    private List<TMPro.TextMeshProUGUI> labelsToHideWhenMenuIsActive;

    void Awake()
    {
        TMPro.TextMeshProUGUI[] labels = GameObject.Find("Hud").GetComponentsInChildren<TMPro.TextMeshProUGUI>();

        var labelsToHideWhenMenuIsActive = new List<TMPro.TextMeshProUGUI>(labels.Length);
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i].transform.parent.name != gameObject.name)
            {
                labelsToHideWhenMenuIsActive.Add(labels[i]);
            }
        }
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

    private void SetBackgroundVisibility(bool isMakeVisible)
    {
        float alpha = isMakeVisible ? 255 : 0;
        TMPro.TextMeshProUGUI[] labels = GameObject.Find("Hud").GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i].transform.parent.name != gameObject.name)
            {
                labels[i].enabled = isMakeVisible;
            }
        }

        GameObject.Find("LeftPlayerName").GetComponent<TMPro.TextMeshProUGUI>();
        GameObject.Find("LeftPlayerName").GetComponent<TMPro.TextMeshProUGUI>().enabled = isMakeVisible;
        GameObjectUtils.SetLabelVisibility(GameObject.Find("LeftPlayerName").GetComponent<TMPro.TextMeshProUGUI>(), isMakeVisible);
        GameObjectUtils.SetLabelVisibility(GameObject.Find("RightPlayerName").GetComponent<TMPro.TextMeshProUGUI>(), isMakeVisible);
        //GameObjectUtils.SetButtonVisibility(GameObject.Find("PauseButton").GetComponent<Button>(), isMakeVisible);
        GameObject.Find("PauseButton").GetComponent<Button>().enabled = isMakeVisible;
        GameObject.Find("PauseButton").GetComponent<Button>().transform.localScale = isMakeVisible? Vector2.one : Vector2.zero;
        GameObjectUtils.SetAlpha(GameObject.Find("MidLine").GetComponent<SpriteRenderer>(), alpha);

        GameObjectUtils.SetLabelVisibility(GameObject.Find("LeftPlayerScore").GetComponent<TMPro.TextMeshProUGUI>(), isMakeVisible);
        GameObjectUtils.SetLabelVisibility(GameObject.Find("RightPlayerScore").GetComponent<TMPro.TextMeshProUGUI>(), isMakeVisible);

        GameObjectUtils.SetAlpha(GameObject.Find("MidLine").GetComponent<SpriteRenderer>(), alpha);
        GameObjectUtils.SetAlpha(GameObject.Find("AiPaddle").GetComponent<SpriteRenderer>(), alpha);
        GameObjectUtils.SetAlpha(GameObject.Find("PlayerPaddle").GetComponent<SpriteRenderer>(), alpha);
        GameObjectUtils.SetAlpha(GameObject.Find("Ball").GetComponent<SpriteRenderer>(), alpha);
    }

    private void ActivateMenu()
    {
        Time.timeScale = 0;
        SetBackgroundVisibility(false);
        gameObject.SetActive(true);
    }
    private void DeactivateMenu()
    {
        Time.timeScale = 1;
        SetBackgroundVisibility(true);
        gameObject.SetActive(false);
    }
    private void OpenAsPauseMenu(RecordedScore recordedScore)
    {

        title.text    = "Game Paused";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();
        ActivateMenu();
        GameObjectUtils.SetButtonVisibility(resumeButton, true);
    }
    private void OpenAsEndGameMenu(RecordedScore recordedScore)
    {
        title.text    = recordedScore.IsLeftPlayerWinning() ? "Game Won" : "Game Lost";
        subtitle.text = recordedScore.LeftPlayerScore.ToString() + " - " + recordedScore.RightPlayerScore.ToString();
        ActivateMenu();
        GameObjectUtils.SetButtonVisibility(resumeButton, false);
    }

    private void ResumeGame()
    {
        GameEventCenter.resumeGame.Trigger("Resuming game");
        DeactivateMenu();
    }
    private void MoveToMainMenu()
    {
        GameEventCenter.gotoMainMenu.Trigger("Opening main menu");
        Time.timeScale = 1;
        SceneUtils.LoadScene("MainMenu");
    }
    private void TriggerRestartGameEvent()
    {
        GameEventCenter.restartGame.Trigger("Restarting game");
        DeactivateMenu();
    }
}
