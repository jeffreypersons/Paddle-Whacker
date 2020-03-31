using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public Button startButton;
    public Button settingsButton;
    public Button aboutButton;
    public Button quitButton;

    public GameObject startPanel;
    public GameObject settingsPanel;
    public GameObject aboutPanel;

    private StartNewGameInfo newGameInfo;

    private List<Button> buttonsToHideWhenActive;
    private List<TMPro.TextMeshProUGUI> labelsToHideWhenActive;

    void Awake()
    {
        newGameInfo = new StartNewGameInfo(3, StartNewGameInfo.Difficulty.Medium);

        startPanel.SetActive(false);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);

        buttonsToHideWhenActive = GameObjectUtils.FindAllObjectsWithTags<Button>("Button");
        labelsToHideWhenActive = GameObjectUtils.FindAllObjectsWithTags<TMPro.TextMeshProUGUI>("Label");
    }

    void OnEnable()
    {
        startButton.onClick.AddListener(OpenStartPanel);
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        aboutButton.onClick.AddListener(OpenAboutPanel);
        quitButton.onClick.AddListener(SceneUtils.QuitGame);

        startPanel.SetActive(false);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }
    void OnDisable()
    {
        startButton.onClick.RemoveListener(OpenStartPanel);
        settingsButton.onClick.RemoveListener(OpenSettingsPanel);
        aboutButton.onClick.RemoveListener(OpenAboutPanel);
        quitButton.onClick.RemoveListener(SceneUtils.QuitGame);
    }

    public void OpenStartPanel()
    {
        startPanel.SetActive(true);
        ToggleMainMenuVisibility(true);
    }
    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        ToggleMainMenuVisibility(true);
    }
    public void OpenAboutPanel()
    {
        aboutPanel.SetActive(true);
        ToggleMainMenuVisibility(true);
    }

    public void TriggerLoadGameEvent()
    {
        SceneUtils.LoadScene("Game", () =>
        {
            GameEventCenter.startNewGame.Trigger(newGameInfo);
        });
    }

    private void ToggleMainMenuVisibility(bool enableMainMenu)
    {
        bool hideBackground = !enableMainMenu;
        for (int i = 0; i < buttonsToHideWhenActive.Count; i++)
        {
            buttonsToHideWhenActive[i].gameObject.SetActive(hideBackground);
            buttonsToHideWhenActive[i].enabled = hideBackground;
        }
        for (int i = 0; i < labelsToHideWhenActive.Count; i++)
        {
            labelsToHideWhenActive[i].enabled = hideBackground;
        }
    }
}
