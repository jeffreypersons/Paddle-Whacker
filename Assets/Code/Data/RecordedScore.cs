using UnityEngine;


public class RecordedScore
{
    public int WinningScore { get; private set; }

    public int LeftPlayerScore  { get; private set; }
    public int RightPlayerScore { get; private set; }

    public void IncrementLeftPlayerScore()  { LeftPlayerScore += 1; }
    public void IncrementRightPlayerScore() { RightPlayerScore += 1; }

    public RecordedScore(int winningScore)
    {
        ResetScore(winningScore);
    }
    public void ResetScore(int winningScore)
    {
        LeftPlayerScore  = 0;
        RightPlayerScore = 0;
        WinningScore = winningScore;
    }

    public override string ToString()
    {
        return $"LeftPlayerScore is {LeftPlayerScore}, and RightPlayerScore is {RightPlayerScore}, with WinningScore set to {WinningScore}";
    }

    public bool IsTied()               { return RightPlayerScore == LeftPlayerScore; }
    public bool IsLeftPlayerWinning()  { return LeftPlayerScore > RightPlayerScore;  }
    public bool IsRightPlayerWinning() { return RightPlayerScore > LeftPlayerScore;  }

    public bool IsWinningScoreReached()
    {
        return LeftPlayerScore >= WinningScore || RightPlayerScore >= WinningScore;
    }
}
