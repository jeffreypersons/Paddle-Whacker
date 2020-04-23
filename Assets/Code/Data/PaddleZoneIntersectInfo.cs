using UnityEngine;


// assumes directly vertical bounces are not velocity possible, and thus zones are never continually occupied!
// (unless of course the initial ball velocity is set that way!)
public class PaddleZoneIntersectInfo
{
    public string  PaddleName  { get; private set; }
    public Vector2 InPosition  { get; private set; }
    public Vector2 InVelocity  { get; private set; }
    public Vector2 OutPosition { get; private set; }
    public Vector2 OutVelocity { get; private set; }
    public override string ToString()
    {
        return $"Ball entered {PaddleName}Zone at {InPosition} and left at {OutPosition} " +
               $"with velocities of {InVelocity} and {OutVelocity}";
    }

    public PaddleZoneIntersectInfo(string paddleName,
        Vector2 inPosition, Vector2 inVelocity,
        Vector2 outPosition, Vector2 outVelocity)
    {
        PaddleName  = paddleName;
        InVelocity  = inVelocity;
        InPosition  = inPosition;
        OutPosition = outPosition;
        OutVelocity = outVelocity;
    }

    public bool ContainsPaddle(string paddleName) { return paddleName == PaddleName; }

    public bool IsNearingLeftGoalWall()     { return OutPosition.x < 0 && OutVelocity.x < 0; }
    public bool IsNearingMidlineFromLeft()  { return OutPosition.x < 0 && OutVelocity.x > 0; }

    public bool IsNearingRightGoalWall()    { return OutPosition.x > 0 && OutVelocity.x > 0; }
    public bool IsNearingMidLineFromRight() { return OutPosition.x > 0 && OutVelocity.x < 0; }

    public bool IsNearingGoalWall()
    {
        return IsNearingLeftGoalWall() || IsNearingRightGoalWall();
    }
    public bool IsNearingMidline()
    {
        return IsNearingMidlineFromLeft() || IsNearingMidLineFromRight();
    }
    public bool BallPassedThrough()
    {
        return (InVelocity.x > 0 && OutVelocity.x > 0) || (InVelocity.x < 0 && OutVelocity.x < 0);
    }
    public bool BallReflectedBack()
    {
        return (InVelocity.x > 0 && OutVelocity.x < 0) || (InVelocity.x < 0 && OutVelocity.x > 0);
    }
}
