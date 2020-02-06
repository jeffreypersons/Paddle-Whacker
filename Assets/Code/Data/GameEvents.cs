using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }
public class CollisionEvent : UnityEvent<Collision2D> { }

// note events are triggered and handled programmatically (via listeners and invocations)
public static class GameEvents
{
    public static CollisionEvent onPaddleHit       = new CollisionEvent();
    public static CollisionEvent onVerticalWallHit = new CollisionEvent();
    public static UnityEvent  onScoreChanged       = new UnityEvent();
}
