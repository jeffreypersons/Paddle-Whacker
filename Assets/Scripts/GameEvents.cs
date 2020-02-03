using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }

public class GameEvents : MonoBehaviour
{
    public static StringEvent onPaddleHit = new StringEvent();
}
