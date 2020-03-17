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
        GameEvents.onScoreChanged.AddListener(UpdateScore);
    }
    void OnDisable()
    {
        GameEvents.onScoreChanged.RemoveListener(UpdateScore);
    }
    public void UpdateScore()
    {
        leftScoreLabel.text  = GameData.leftPlayerScore.ToString();
        rightScoreLabel.text = GameData.rightPlayerScore.ToString();
    }
}
