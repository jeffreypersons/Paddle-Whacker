using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

// note events are triggered and handled programmatically (via listeners and invocations)
public class GameEvents : MonoBehaviour
{
    public static StringEvent onPaddleHit = new StringEvent();
    public static StringEvent onVerticalWallHit = new StringEvent();
}
