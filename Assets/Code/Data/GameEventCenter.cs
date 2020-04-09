

// all game events are held here (as static data, so it can be accessed across scenes)
// note events are triggered and handled ENTIRELY programmatically (via listeners and invocations)
public static class GameEventCenter
{
    public static GameEvent<string> paddleHit         = new GameEvent<string>();
    public static GameEvent<string> horizontalWallHit = new GameEvent<string>();
    public static GameEvent<string> verticalWallHit   = new GameEvent<string>();
    public static GameEvent<string> goalHit           = new GameEvent<string>();

    public static GameEvent<RecordedScore> scoreChange                = new GameEvent<RecordedScore>();
    public static GameEvent<PaddleZoneIntersectInfo> zoneIntersection = new GameEvent<PaddleZoneIntersectInfo>();

    public static GameEvent<GameSettings> startNewGame        = new GameEvent<GameSettings>();
    public static GameEvent<RecordedScore>    pauseGame           = new GameEvent<RecordedScore>();
    public static GameEvent<string>           resumeGame          = new GameEvent<string>();
    public static GameEvent<string>           gotoMainMenu        = new GameEvent<string>();
    public static GameEvent<RecordedScore>    winningScoreReached = new GameEvent<RecordedScore>();
    public static GameEvent<string>           restartGame         = new GameEvent<string>();
}
