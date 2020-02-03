using UnityEngine;
using UnityEngine.Events;

public class EventTriggers : MonoBehaviour
{
    public UnityEvent onPaddleHit;

    public void HandlePaddleHit()
    {
        onPaddleHit.Invoke();
    }
}
