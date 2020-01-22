using UnityEngine;

public class ComputeGameOutcome : MonoBehaviour
{
    void Start()
    {
        var data = DataManager.instance;
        var resultsLabel = GameObject.Find(transform.name).GetComponent<TMPro.TextMeshProUGUI>();

        resultsLabel.text = data.player1Score == data.WINNING_SCORE ? "Game Won" : "Game Lost";
        resultsLabel.text += "\n" + data.player1Score.ToString() + " - " + data.player2Score.ToString();
    }
}
