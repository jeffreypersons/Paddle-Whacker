using System.Collections.Generic;
using UnityEngine;


// note: assumes an arena with raycast layers defined,
public class BallTrajectoryPredictor
{
    public const int MinNumVertsPossible = 2;
    public readonly int MaxNumVerticesAllowed;

    public readonly LayerMask LayersUsedDuringComputation;
    private List<Vector2> path;

    public bool Empty              { get { return path.Count == 0;      } }
    public Vector2 this[int index] { get { return path[index];          } }
    public Vector2 StartPoint      { get { return path[0];              } }
    public Vector2 EndPoint        { get { return path[path.Count - 1]; } }

    public BallTrajectoryPredictor(LayerMask layersUsedDuringComputation, int maxNumVertsAllowed=10)
    {
        MaxNumVerticesAllowed = maxNumVertsAllowed;
        path = new List<Vector2>(MaxNumVerticesAllowed);
        LayersUsedDuringComputation = layersUsedDuringComputation;
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
            Vector2 extrapolatedPoint = ExtrapolatePoint(position, direction, targetX);
            hit = Physics2D.Raycast(position, direction, Vector2.Distance(position, extrapolatedPoint), LayersUsedDuringComputation);
            if (hit.transform != null)
            {
                position  = hit.point;
                direction = Vector2.Reflect(direction, hit.normal);
            }
            else
            {
                position = extrapolatedPoint;
            }
            path.Add(position);
        }
    }


    private bool HasMetOrSurpassedTarget(float x, float targetX, Vector2 startDirection)
    {
        if (path.Count <= MinNumVertsPossible)
        {
            return false;
        }
        if (targetX == x || path.Count >= MaxNumVerticesAllowed)
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
