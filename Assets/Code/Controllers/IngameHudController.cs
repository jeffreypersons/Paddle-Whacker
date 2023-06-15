using UnityEngine;
using UnityEngine.UI;


public class IngameHudController : MonoBehaviour
{
    [SerializeField] private Button pauseButton = default;
    [SerializeField] private TMPro.TextMeshProUGUI leftScoreLabel  = default;
    [SerializeField] private TMPro.TextMeshProUGUI rightScoreLabel = default;

    private RecordedScore lastRecordedScore;

    void OnEnable()
    {
        GameEventCenter.scoreChange.AddListener(UpdateScore);
        pauseButton.onClick.AddListener(TriggerPauseGameEvent);
    }

    void OnDisable()
    {
        GameEventCenter.scoreChange.RemoveListener(UpdateScore);
        pauseButton.onClick.RemoveListener(TriggerPauseGameEvent);
    }


    private void UpdateScore(RecordedScore recordedScore)
    {
        lastRecordedScore    = recordedScore;
        leftScoreLabel.text  = recordedScore.LeftPlayerScore.ToString();
        rightScoreLabel.text = recordedScore.RightPlayerScore.ToString();
    }

    private void TriggerPauseGameEvent()
    {
        if (lastRecordedScore == null)
        {
            Debug.LogError($"LastRecordedScore received by {GetType().Name} is null");
        }
        GameEventCenter.pauseGame.Trigger(lastRecordedScore);
    }
}
