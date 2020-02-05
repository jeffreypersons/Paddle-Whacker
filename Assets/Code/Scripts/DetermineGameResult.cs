using UnityEngine;

public class DetermineGameResult : MonoBehaviour
{
    void Start()
    {
        // todo: consider doing the change through on state change, similar to the in-game score labels
        var resultsLabel = GameObject.Find(transform.name).GetComponent<TMPro.TextMeshProUGUI>();

        resultsLabel.text = GameData.player1Score == GameData.winningScore ? "Game Won" : "Game Lost";
        resultsLabel.text += "\n" + GameData.player1Score.ToString() + " - " + GameData.player2Score.ToString();
    }
}
