using UnityEngine;


public class MenuController : MonoBehaviour
{
    void Start()
    {
    }

    void OnEnable()
    {

    }
    void OnDisable()
    {

    }
    void ShowScore(ScoreInfo scoreInfo)
    {
        // todo: consider doing the change through on state change, similar to the in-game score labels
        var endMenuMainTitle = GameObject.Find("Title").GetComponent<TMPro.TextMeshProUGUI>();
        var endMenuSubtitle = GameObject.Find("Subtitle").GetComponent<TMPro.TextMeshProUGUI>();


        if (scoreInfo.IsWinningScoreReached())
        {
            endMenuMainTitle.text = scoreInfo.IsLeftPlayerWinning() ? "Game Won" : "Game Lost";
            endMenuSubtitle.text  = scoreInfo.LeftPlayerScore.ToString() + " - " + scoreInfo.RightPlayerScore.ToString();
        }
    }
}
