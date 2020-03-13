using System.Collections.Generic;
using UnityEngine;


public class PredictedTrajectory
{
    public int minNumIterations = 2;
    public int maxNumIterations = 20;

    public int NumBounces { get; private set; }
    public static HashSet<string> wallTags = new HashSet<string> { "HorizontalWall", "VerticalWall" };

    private List<Vector2> path;

    public float TargetX { get; private set; }
    public bool Empty { get { return path.Count == 0; } }
    public Vector2 this[int index] { get { return path[index]; } }
    public Vector2 StartPoint { get { return path[0]; } }
    public Vector2 EndPoint   { get { return path[path.Count - 1]; } }

    public Vector2 StartDirection { get; private set; }
    public Vector2 EndDirection   { get; private set; }

    public PredictedTrajectory()
    {
        path = new List<Vector2>(minNumIterations);
    }
    public override string ToString()
    {
        return "[" + string.Join(",", path) + "]";
    }
    public void Clear()
    {
        path.Clear();
    }
    public void DrawInEditor(Color lineColor, float lineDuration)
    {
        for (int i = 0; i + 1 < path.Count; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], lineColor, lineDuration);
        }
    }
    // compute trajectory by extending line from last position, reflecting each bounce,
    // and extending the line until either the target x value is reached, or the maximum number of points is met
    // note: overrides all previous internal data
    public void Compute(Vector2 startPosition, Vector2 startDirection, float targetX)
    {
        StartDirection = startDirection;
        TargetX = targetX;
        NumBounces = 0;

        path.Clear();
        path.Add(startPosition);
        RaycastHit2D hit;
        Vector2 position  = startPosition;
        Vector2 direction = startDirection;
        while (!HasMetOrSurpassedTarget(position.x, targetX))
        {
            hit = Physics2D.Raycast(position, direction, Mathf.Abs(targetX - position.x));
            if (hit.transform != null && wallTags.Contains(hit.transform.tag))
            {
                position = hit.point;
                direction = Vector2.Reflect(direction, hit.normal);
                NumBounces++;
            }
            else
            {
                position = ExtrapolatePoint(position, direction, targetX);
            }
            path.Add(position);
        }
        EndDirection = direction;
    }

    private bool HasMetOrSurpassedTarget(float x, float targetX)
    {
        if (path.Count <= minNumIterations)
        {
            return false;
        }
        if (targetX == x || path.Count >= maxNumIterations)
        {
            return true;
        }

        if (targetX == StartPoint.x)
        {
            return StartDirection.x < 0? x > targetX : x < targetX;
        }
        else if (targetX < StartPoint.x)
        {
            return x > targetX;
        }
        else
        {
            return x > targetX;
        }
    }
    private Vector2 ExtrapolatePoint(Vector2 position, Vector2 direction, float x)
    {
        return position + (direction * Mathf.Abs(x - position.x));
    }
}
