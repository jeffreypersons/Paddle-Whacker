using System;


// lightweight event, as a wrapper over the native c# events, and as an alternative to the much slower UnityEvent
// no support for: serialization (since delegate body is null), threading, or assignment via Unity inspector
// note: only allows single parameter events, but its no problem as it's better to use custom objects in that case anyways
// note: like UnityEvents, requires the parameter be specified in a subclass (eg: `public class HitEvent : GameEvent<string> { }`
public abstract class GameEvent<EventData>
{
    Action<EventData> action;
    public GameEvent()
    {

        action = delegate { };
    }
    public void StartListening(Action<EventData> listener)
    {
        action += listener;
    }
    public void StopListening(Action<EventData> listener)
    {
        action += listener;
    }
    public void Trigger(EventData eventData)
    {
        action.Invoke(eventData);
    }
}
