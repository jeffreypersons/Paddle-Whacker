using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton    = default;
    [SerializeField] private Button settingsButton = default;
    [SerializeField] private Button aboutButton    = default;
    [SerializeField] private Button quitButton     = default;
    [SerializeField] private MainMenuPanelController submenuController = default;

    private List<Button> buttonsToHideWhenActive;
    private List<TMPro.TextMeshProUGUI> labelsToHideWhenActive;

    void Awake()
    {
        buttonsToHideWhenActive = GameObjectUtils.FindAllObjectsWithTags<Button>("Button");
        labelsToHideWhenActive  = GameObjectUtils.FindAllObjectsWithTags<TMPro.TextMeshProUGUI>("Subtitle");

        submenuController.SetActionOnStartPressed(() => LoadGame());
        submenuController.SetActionOnPanelOpen(()    => ToggleMenuVisibility(true));
        submenuController.SetActionOnPanelClose(()   => ToggleMenuVisibility(false));

        #if UNITY_WEBGL
            quitButton.SetActive(false);
        #endif
    }

    void OnEnable()
    {
        startButton.onClick.AddListener(submenuController.OpenStartPanel);
        settingsButton.onClick.AddListener(submenuController.OpenSettingsPanel);
        aboutButton.onClick.AddListener(submenuController.OpenAboutPanel);
        quitButton.onClick.AddListener(SceneUtils.QuitGame);
    }
    void OnDisable()
    {
        startButton.onClick.RemoveListener(submenuController.OpenStartPanel);
        settingsButton.onClick.RemoveListener(submenuController.OpenSettingsPanel);
        aboutButton.onClick.RemoveListener(submenuController.OpenAboutPanel);
        quitButton.onClick.RemoveListener(SceneUtils.QuitGame);
    }

    private void LoadGame()
    {
        SceneUtils.LoadScene("Game", () =>
        {
            GameEventCenter.startNewGame.Trigger(submenuController.GetGameSettings());
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
