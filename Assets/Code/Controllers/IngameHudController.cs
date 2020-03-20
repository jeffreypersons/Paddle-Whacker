using UnityEngine;
using UnityEngine.UI;

public class IngameHudController : MonoBehaviour
{
    public Button pauseButton;

    public TMPro.TextMeshProUGUI leftScoreLabel;
    public TMPro.TextMeshProUGUI rightScoreLabel;

    void Awake()
    {
        pauseButton     = pauseButton.GetComponent<Button>();
        leftScoreLabel  = leftScoreLabel.GetComponent<TMPro.TextMeshProUGUI>();
        rightScoreLabel = rightScoreLabel.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void OnEnable()
    {
        GameEventCenter.scoreChange.StartListening(UpdateScore);
        pauseButton.onClick.AddListener(TriggerPauseGameEvent);
    }
    void OnDisable()
    {
        GameEventCenter.scoreChange.StopListening(UpdateScore);
        pauseButton.onClick.RemoveListener(TriggerPauseGameEvent);
    }

    private void UpdateScore(RecordedScore scoreInfo)
    {
        leftScoreLabel.text  = scoreInfo.LeftPlayerScore.ToString();
        rightScoreLabel.text = scoreInfo.RightPlayerScore.ToString();
    }
    private void TriggerPauseGameEvent()
    {
        GameEventCenter.pauseGame.Trigger("Pausing");
    }
}
