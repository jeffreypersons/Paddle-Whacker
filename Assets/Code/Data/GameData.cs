using UnityEngine;

public static class GameData
{
    public static void Init()
    {
        // todo: find a way to have these initial values set in inspector, even though this is a static class...
        // maybe a file? player prefs...?
        player1Score = 0;
        player2Score = 0;
        winningScore = 5;
    }
    public static int winningScore { get; set; }
    public static int player1Score { get; set; }
    public static int player2Score { get; set; }
}
