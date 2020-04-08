using System;
using UnityEngine;
using UnityEngine.UI;


// controller for opening sub panels of the main menu
// - only one panel can be open at a time
// - all panels have a back button, and some may have a continue button
public class SubmainMenuController : MonoBehaviour
{
    private Action actionOnStartPress;
    private Action actionOnPanelOpen;
    private Action actionOnPanelClose;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject aboutPanel;

    [SerializeField] private SliderSettingController difficultySetting;
    [SerializeField] private SliderSettingController numberOfGoalsSetting;
    [SerializeField] private SliderSettingController soundVolumeSetting;
    [SerializeField] private SliderSettingController musicVolumeSetting;

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

    public GameSettings GetGameSettings()
    {
        return new GameSettings(
            numberOfGoals:   (int)numberOfGoalsSetting.SliderValue,
            difficultyLevel: (int)difficultySetting.SliderValue,
            soundVolume:     (int)soundVolumeSetting.SliderValue,
            musicVolume:     (int)musicVolumeSetting.SliderValue
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
        if (MathUtils.CountTrueValues(startPanel.active, settingsPanel.active, aboutPanel.active) != 0)
        {
            Debug.LogError($"Cannot open {submenuPanel.name}, since only one sub-mainmenu panel can be active at a time.");
        }

        Button startButton = GameObjectUtils.FindFirstChildWithTag<Button>(submenuPanel, "ContinueButton");
        Button closeButton = GameObjectUtils.FindFirstChildWithTag<Button>(submenuPanel, "CancelButton");
        if (startButton)
        {
            GameObjectUtils.AddAutoUnsubscribeListenerToButton(startButton, actionOnStartPress);
        }
        if (closeButton)
        {
            GameObjectUtils.AddAutoUnsubscribeListenerToButton(closeButton, () =>
            {
                DeactivePanels();
                actionOnPanelClose();
            });
        }
        actionOnPanelOpen();
        submenuPanel.SetActive(true);
    }
}
