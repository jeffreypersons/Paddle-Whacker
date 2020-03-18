

// all game events are held here (as static data, so it can be accessed across scenes)
// note events are triggered and handled ENTIRELY programmatically (via listeners and invocations)
public static class GameEventCenter
{
    public class HitEvent                 : GameEvent<string> { }
    public class PaddleZoneIntersectEvent : GameEvent<PaddleZoneIntersectInfo> { }
    public class ScoreChangeEvent         : GameEvent<ScoreInfo> { }
    public class GameFinishedEvent        : GameEvent<ScoreInfo> { }

    public static HitEvent paddleHit           = new HitEvent();
    public static HitEvent horizontalWallHit   = new HitEvent();
    public static HitEvent verticalWallHit     = new HitEvent();
    public static HitEvent goalHit             = new HitEvent();
    public static PaddleZoneIntersectEvent zoneIntersection = new PaddleZoneIntersectEvent();
    public static ScoreChangeEvent scoreChange   = new ScoreChangeEvent();
    public static GameFinishedEvent gameFinished = new GameFinishedEvent();
}
