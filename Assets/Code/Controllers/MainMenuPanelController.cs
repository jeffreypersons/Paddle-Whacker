using System;
using UnityEngine;
using UnityEngine.UI;


// controller for opening sub panels of the main menu
// - only one panel can be open at a time
// - all panels have a back button, and some may have a continue button
public class MainMenuPanelController : MonoBehaviour
{
    private Action actionOnStartPress;
    private Action actionOnPanelOpen;
    private Action actionOnPanelClose;

    [SerializeField] private GameObject startPanel    = default;
    [SerializeField] private GameObject settingsPanel = default;
    [SerializeField] private GameObject aboutPanel    = default;

    [SerializeField] private SliderSettingController difficultySetting    = default;
    [SerializeField] private SliderSettingController numberOfGoalsSetting = default;
    [SerializeField] private SliderSettingController soundVolumeSetting   = default;
    [SerializeField] private SliderSettingController musicVolumeSetting   = default;

    void OnEnable()
    {
        DeactivePanels();
    }
    public void OpenStartPanel()    { OpenPanel(startPanel);    }
    public void OpenSettingsPanel() { OpenPanel(settingsPanel); }
    public void OpenAboutPanel()    { OpenPanel(aboutPanel);    }

    public void SetActionOnStartPressed(Action actionOnStartPress) { this.actionOnStartPress = actionOnStartPress; }
    public void SetActionOnPanelOpen(Action actionOnPanelOpen)     { this.actionOnPanelOpen  = actionOnPanelOpen;  }
    public void SetActionOnPanelClose(Action actionOnPanelClose)   { this.actionOnPanelClose = actionOnPanelClose; }

    public GameSettingsInfo GetGameSettings()
    {
        return new GameSettingsInfo(
            numberOfGoals:      (int)numberOfGoalsSetting.SliderValue,
            difficultyPercent:  (int)difficultySetting.SliderValue,
            soundVolumePercent: (int)soundVolumeSetting.SliderValue,
            musicVolumePercent: (int)musicVolumeSetting.SliderValue
        );
    }

    private void DeactivePanels()
    {
        startPanel.SetActive(false);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    private void OpenPanel(GameObject submenuPanel)
    {
        if (startPanel.activeInHierarchy || settingsPanel.activeInHierarchy || aboutPanel.activeInHierarchy)
        {
            Debug.LogError($"Cannot open {submenuPanel.name}, since only one sub-mainmenu panel can be active at a time.");
        }

        Button startButton = GameObjectUtils.FindFirstChildWithTag<Button>(submenuPanel, "ContinueButton");
        Button closeButton = GameObjectUtils.FindFirstChildWithTag<Button>(submenuPanel, "CancelButton");
        if (startButton)
        {
            GameObjectUtils.AddAutoUnsubscribeOnClickListenerToButton(startButton, () =>
            {
                actionOnStartPress();
            });
        }
        if (closeButton)
        {
            GameObjectUtils.AddAutoUnsubscribeOnClickListenerToButton(closeButton, () =>
            {
                DeactivePanels();
                actionOnPanelClose();
            });
        }
        actionOnPanelOpen();
        submenuPanel.SetActive(true);
    }
}
