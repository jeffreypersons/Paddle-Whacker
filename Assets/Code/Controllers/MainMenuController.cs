using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private SubmainMenuController submenuController;

    private List<Button> buttonsToHideWhenActive;
    private List<TMPro.TextMeshProUGUI> labelsToHideWhenActive;

    void Awake()
    {
        buttonsToHideWhenActive = GameObjectUtils.FindAllObjectsWithTags<Button>("Button");
        labelsToHideWhenActive  = GameObjectUtils.FindAllObjectsWithTags<TMPro.TextMeshProUGUI>("Subtitle");

        submenuController.SetActionOnStartPressed(() => LoadGame());
        submenuController.SetActionOnPanelOpen(()    => HideMainMenu(true));
        submenuController.SetActionOnPanelClose(()   => HideMainMenu(false));
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

    private void HideMainMenu(bool enableMainMenu)
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
