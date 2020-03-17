using UnityEngine;

public class HudController : MonoBehaviour
{
    public string leftScoreLabelName;
    public string rightScoreLabelName;

    private TMPro.TextMeshProUGUI leftScoreLabel;
    private TMPro.TextMeshProUGUI rightScoreLabel;

    void Start()
    {
        leftScoreLabel = GameObject.Find(leftScoreLabelName).GetComponent<TMPro.TextMeshProUGUI>();
        rightScoreLabel = GameObject.Find(rightScoreLabelName).GetComponent<TMPro.TextMeshProUGUI>();
    }

    void OnEnable()
    {
        GameEvents.onScoreChanged.AddListener(UpdateScore);
    }
    void OnDisable()
    {
        GameEvents.onScoreChanged.RemoveListener(UpdateScore);
    }
    public void UpdateScore()
    {
        leftScoreLabel.text = GameData.leftPlayerScore.ToString();
        rightScoreLabel.text = GameData.rightPlayerScore.ToString();
    }
}
