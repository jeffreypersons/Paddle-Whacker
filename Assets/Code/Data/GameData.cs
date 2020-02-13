using UnityEngine;

// todo: look into an alternative that allows setting initial values in the inspector (eg player pref's?)
public static class GameData
{
    public static int winningScore { get; set; }
    public static int player1Score { get; set; }
    public static int player2Score { get; set; }

    // note that unlike scores, start positions are set once and never again.
    public static void Init()
    {
        winningScore = 10;
        player1Score = 0;
        player2Score = 0;
    }
}
