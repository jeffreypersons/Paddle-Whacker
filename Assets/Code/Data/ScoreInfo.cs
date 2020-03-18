

public class ScoreInfo
{
    public static int WinningScore { get; set; }

    public int LeftPlayerScore     { get; set; }
    public int RightPlayerScore    { get; set; }

    public override string ToString()
    {
        return $"LeftPlayerScore score is {LeftPlayerScore}, and " +
               $"RightPlayerScore score is {RightPlayerScore}";
    }

    public ScoreInfo(int winningScore)
    {
        LeftPlayerScore  = 0;
        RightPlayerScore = 0;
        WinningScore     = winningScore;
    }

    public bool IsTied()               { return RightPlayerScore == LeftPlayerScore; }
    public bool IsLeftPlayerWinning()  { return LeftPlayerScore > RightPlayerScore;  }
    public bool IsRightPlayerWinning() { return RightPlayerScore > LeftPlayerScore;  }

    public bool IsWinningScoreReached()
    {
        return LeftPlayerScore >= WinningScore || RightPlayerScore >= WinningScore;
    }
}
