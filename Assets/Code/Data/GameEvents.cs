using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

// name of hit zone and the incoming and outgoing ball velocities
[System.Serializable]
public class ZoneIntersectEvent : UnityEvent<ZoneIntersectInfo> { }

// note events are triggered and handled programmatically (via listeners and invocations)
public static class GameEvents
{
    public static StringEvent onPaddleHit         = new StringEvent();
    public static StringEvent onHorizontalWallHit = new StringEvent();
    public static StringEvent onVerticalWallHit   = new StringEvent();
    public static StringEvent onGoalHit           = new StringEvent();
    public static UnityEvent  onScoreChanged      = new UnityEvent();

    public static ZoneIntersectEvent onZoneIntersection = new ZoneIntersectEvent();
}
