using System.Collections.Generic;
using UnityEngine;


public class BallTrajectoryPredictor
{
    private const int minNumPoints = 2;
    private const int maxNumBounces = 10;
    private const float maxRaycastDistance = 30;
    private const int maxNumPoints = minNumPoints + maxNumBounces;

    private readonly HashSet<string> wallTags = new HashSet<string> { "HorizontalWall", "VerticalWall" };

    private List<Vector2> path;

    public float NumBounces        { get { return path.Count - minNumPoints; } }
    public bool Empty              { get { return path.Count == 0;           } }
    public Vector2 this[int index] { get { return path[index];               } }
    public Vector2 StartPoint      { get { return path[0];                   } }
    public Vector2 EndPoint        { get { return path[path.Count - 1];      } }

    public BallTrajectoryPredictor()
    {
        path = new List<Vector2>(minNumPoints);
    }
    public override string ToString()
    {
        return "PredictedTrajectory=[" + string.Join(",", path) + "]";
    }
    public void Reset()
    {
        path.Clear();
    }
    // example usage: `DrawInEditor(Color.red, 1.50f)`
    public void DrawInEditor(Color lineColor, float lineDuration)
    {
        for (int i = 0; i + 1 < path.Count; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], lineColor, lineDuration);
        }
    }
    // compute trajectory by extending line from last position, reflecting each bounce,
    // and extending the line until either the target x value is reached, or the maximum number of points is met
    // note: overrides previous list of points, and for consistency considers goal as a wall to bounce off of
    public void ComputeNewTrajectory(Vector2 startPosition, Vector2 startDirection, float targetX)
    {
        path.Clear();
        path.Add(startPosition);

        RaycastHit2D hit;
        Vector2 position  = startPosition;
        Vector2 direction = startDirection;
        while (!HasMetOrSurpassedTarget(position.x, targetX, startDirection))
        {
            hit = Physics2D.Raycast(position, direction, Vector2.Distance(position, new Vector2(targetX, ExtrapolatePoint(position, direction, targetX).y)));
            if (hit.transform != null && (wallTags.Contains(hit.transform.tag) || hit.transform.CompareTag("Goal")))
            {
                position = hit.point;
                direction = Vector2.Reflect(direction, hit.normal);
            }
            else
            {
                position = ExtrapolatePoint(position, direction, targetX);
            }
            path.Add(position);
        }
    }

    private bool HasMetOrSurpassedTarget(float x, float targetX, Vector2 startDirection)
    {
        if (path.Count <= minNumPoints)
        {
            return false;
        }
        if (targetX == x || path.Count >= maxNumPoints)
        {
            return true;
        }

        if (targetX < StartPoint.x)
        {
            return x < targetX;
        }
        if (targetX > StartPoint.x)
        {
            return x > targetX;
        }
        return startDirection.x < 0 ? x > targetX : x < targetX;
    }
    private Vector2 ExtrapolatePoint(Vector2 position, Vector2 direction, float x)
    {
        return position + (direction * Mathf.Abs(x - position.x));
    }
}
