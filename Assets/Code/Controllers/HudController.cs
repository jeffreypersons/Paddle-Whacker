using UnityEngine;


public class HudController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI leftScoreLabel;
    public TMPro.TextMeshProUGUI rightScoreLabel;

    void Start()
    {
        leftScoreLabel  = leftScoreLabel.GetComponent<TMPro.TextMeshProUGUI>();
        rightScoreLabel = rightScoreLabel.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void OnEnable()
    {
        GameEventCenter.scoreChange.StartListening(UpdateScore);
    }
    void OnDisable()
    {
        GameEventCenter.scoreChange.StopListening(UpdateScore);
    }
    public void UpdateScore(ScoreInfo scoreInfo)
    {
        leftScoreLabel.text  = scoreInfo.LeftPlayerScore.ToString();
        rightScoreLabel.text = scoreInfo.RightPlayerScore.ToString();
    }
}
