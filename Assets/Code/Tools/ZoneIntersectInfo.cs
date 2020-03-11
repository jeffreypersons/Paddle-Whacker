using UnityEngine;

// assumes directly vertical bounces are not velocity possible, and thus zones are never continually occupied!
// (unless of course the initial ball velocity is set that way!)
public class ZoneIntersectInfo
{
    public string  ZoneName    { get; private set; }
    public Vector2 InVelocity  { get; private set; }
    public Vector2 OutVelocity { get; private set; }

    public bool WasInZone(string zoneName)     { return ZoneName == zoneName;       }
    public bool WasClosestToPaddle(string playerName) { return ZoneName.StartsWith(playerName); }

    public bool WasBallPassedThroughZone()
    {
        return (InVelocity.x > 0 && OutVelocity.x > 0) || (InVelocity.x < 0 && OutVelocity.x < 0);
    }
    public bool WasBallReflectedFromZone()
    {
        return (InVelocity.x > 0 && OutVelocity.x < 0) || (InVelocity.x < 0 && OutVelocity.x > 0);
    }
    public ZoneIntersectInfo(string zoneName, Vector2 inVelocity, Vector2 outVelocity)
    {
        ZoneName    = zoneName;
        InVelocity  = inVelocity;
        OutVelocity = outVelocity;
    }
}
