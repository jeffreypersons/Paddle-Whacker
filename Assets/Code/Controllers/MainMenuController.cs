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
    
    private List<Button> buttonsToHideWhenPanelIsOpen;
    private List<TMPro.TextMeshProUGUI> labelsToHideWhenPanelIsOpen;
    [TagSelector] [SerializeField] private string[] tagsOfButtonsToHideWhenPanelIsOpen = new string[] { };
    [TagSelector] [SerializeField] private string[] tagsOfLabelsToHideWhenPanelIsOpen  = new string[] { };
    
    void Awake()
    {
        buttonsToHideWhenPanelIsOpen = GameObjectUtils.FindAllObjectsWithTags<Button>(tagsOfButtonsToHideWhenPanelIsOpen);
        labelsToHideWhenPanelIsOpen  = GameObjectUtils.FindAllObjectsWithTags<TMPro.TextMeshProUGUI>(tagsOfLabelsToHideWhenPanelIsOpen);

        mainMenuPanelController.SetActionOnStartPressed(() => LoadGame());
        mainMenuPanelController.SetActionOnPanelOpen(()    => ToggleMenuVisibility(true));
        mainMenuPanelController.SetActionOnPanelClose(()   => ToggleMenuVisibility(false));

        #if UNITY_WEBGL
            GameObjectUtils.SetButtonActiveAndEnabled(quitButton, false);
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
        for (int i = 0; i < buttonsToHideWhenPanelIsOpen.Count; i++)
        {
            GameObjectUtils.SetButtonVisibility(buttonsToHideWhenPanelIsOpen[i], hideBackground);
        }
        for (int i = 0; i < labelsToHideWhenPanelIsOpen.Count; i++)
        {
            GameObjectUtils.SetLabelVisibility(labelsToHideWhenPanelIsOpen[i], hideBackground);
        }
        #if UNITY_WEBGL
            GameObjectUtils.SetButtonActiveAndEnabled(quitButton, false);
        #endif
    }
}
