using System;


// lightweight event, as a wrapper over the native c# events, and as an alternative to the much slower UnityEvent
// no support for: serialization (since delegate body is null), threading, or assignment via Unity inspector
// note: only allows single parameter events, but its no problem as it's better to use custom objects in that case anyways
public class GameEvent<EventData>
{
    Action<EventData> action;

    public int NumListeners { get { return action.GetInvocationList().Length; } }

    public GameEvent()
    {
        action = delegate { };
    }
    public void AddListener(Action<EventData> listener)
    {
        action += listener;
    }
    // adds a listener that automatically removes itself just after executing
    public void AddAutoUnsubscribeListener(Action<EventData> oneTimeUseListener)
    {
        // note null initialization is required to force nonlocal scope of the handler, see https://stackoverflow.com/a/1362244
        Action<EventData> handler = null;
        handler = (data) =>
        {
            action -= handler;
            oneTimeUseListener.Invoke(data);
        };
        action += handler;
    }
    public void RemoveListener(Action<EventData> listener)
    {
        action -= listener;
    }
    public void StopAllListeners()
    {
        if (action != null)
        {
            foreach (Delegate d in action.GetInvocationList())
            {
                action -= (Action<EventData>)d;
            }
        }
    }
    public void Trigger(in EventData eventData)
    {
        action.Invoke(eventData);
    }
}
