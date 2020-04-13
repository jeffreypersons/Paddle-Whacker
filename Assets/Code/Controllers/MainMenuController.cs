using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton    = default;
    [SerializeField] private Button settingsButton = default;
    [SerializeField] private Button aboutButton    = default;
    [SerializeField] private Button quitButton     = default;
    [SerializeField] private MainMenuPanelController mainMenuPanelController = default;

    private List<Button> buttonsToHideWhenActive;
    private List<TMPro.TextMeshProUGUI> labelsToHideWhenActive;

    void Awake()
    {
        buttonsToHideWhenActive = GameObjectUtils.FindAllObjectsWithTags<Button>("Button");
        labelsToHideWhenActive  = GameObjectUtils.FindAllObjectsWithTags<TMPro.TextMeshProUGUI>("Subtitle");

        mainMenuPanelController.SetActionOnStartPressed(() => LoadGame());
        mainMenuPanelController.SetActionOnPanelOpen(()    => ToggleMenuVisibility(true));
        mainMenuPanelController.SetActionOnPanelClose(()   => ToggleMenuVisibility(false));

        #if UNITY_WEBGL
            quitButton.SetActive(false);
        #endif
    }

    void OnEnable()
    {
        startButton.onClick.AddListener(mainMenuPanelController.OpenStartPanel);
        settingsButton.onClick.AddListener(mainMenuPanelController.OpenSettingsPanel);
        aboutButton.onClick.AddListener(mainMenuPanelController.OpenAboutPanel);
        quitButton.onClick.AddListener(SceneUtils.QuitGame);
    }
    void OnDisable()
    {
        startButton.onClick.RemoveListener(mainMenuPanelController.OpenStartPanel);
        settingsButton.onClick.RemoveListener(mainMenuPanelController.OpenSettingsPanel);
        aboutButton.onClick.RemoveListener(mainMenuPanelController.OpenAboutPanel);
        quitButton.onClick.RemoveListener(SceneUtils.QuitGame);
    }

    private void LoadGame()
    {
        SceneUtils.LoadScene("Game", () =>
        {
            GameEventCenter.startNewGame.Trigger(mainMenuPanelController.GetGameSettings());
        });
    }

    private void ToggleMenuVisibility(bool isVisible)
    {
        bool hideBackground = !isVisible;
        for (int i = 0; i < buttonsToHideWhenActive.Count; i++)
        {
            buttonsToHideWhenActive[i].gameObject.SetActive(hideBackground);
        }
        for (int i = 0; i < labelsToHideWhenActive.Count; i++)
        {
            labelsToHideWhenActive[i].enabled = hideBackground;
        }
        #if UNITY_WEBGL
            quitButton.SetActive(false);
        #endif
    }
}
