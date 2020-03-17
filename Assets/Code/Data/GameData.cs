using UnityEngine;

// todo: look into an alternative that allows setting initial values in the inspector (eg player pref's?)
public static class GameData
{
    public static int winningScore { get; set; }
    public static int leftPlayerScore { get; set; }
    public static int rightPlayerScore { get; set; }

    // note that unlike scores, start positions are set once and never again.
    public static void Init()
    {
        winningScore = 10;
        leftPlayerScore = 0;
        rightPlayerScore = 0;
    }
}
