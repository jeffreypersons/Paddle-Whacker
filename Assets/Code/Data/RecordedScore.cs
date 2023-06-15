

public class RecordedScore
{
    public int WinningScore     { get; private set; }
    public int LeftPlayerScore  { get; private set; }
    public int RightPlayerScore { get; private set; }

    public override string ToString() =>
        $"LeftPlayerScore is {LeftPlayerScore}, and RightPlayerScore is {RightPlayerScore}, with WinningScore set to {WinningScore}";

    public RecordedScore(int winningScore)
    {
        ResetScore(winningScore);
    }

    
    public void IncrementLeftPlayerScore()  => LeftPlayerScore  += 1;
    public void IncrementRightPlayerScore() => RightPlayerScore += 1;
    
    public bool IsTied()                => RightPlayerScore == LeftPlayerScore;
    public bool IsLeftPlayerWinning()   => LeftPlayerScore > RightPlayerScore;
    public bool IsRightPlayerWinning()  => RightPlayerScore > LeftPlayerScore;
    public bool IsWinningScoreReached() => LeftPlayerScore >= WinningScore || RightPlayerScore >= WinningScore;

    public void ResetScore(int winningScore)
    {
        LeftPlayerScore  = 0;
        RightPlayerScore = 0;
        WinningScore     = winningScore;
    }
}
