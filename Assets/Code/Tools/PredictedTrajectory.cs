using System.Collections.Generic;
using UnityEngine;

public class PredictedTrajectory
{
    public int maxNumIterations = 10;
    public HashSet<string> wallTags = new HashSet<string> { "HorizontalWall", "VerticalWall" };

    private List<Vector2> path;

    public bool Empty { get { return path.Count == 0; } }
    public Vector2 this[int index] { get { return path[index]; } }
    public Vector2 StartPoint { get { return path[0]; } }
    public Vector2 EndPoint   { get { return path[path.Count - 1]; } }

    public PredictedTrajectory()
    {
        path = new List<Vector2>(maxNumIterations);
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
        path.Clear();
        path.Add(startPosition);

        RaycastHit2D hit;
        Vector2 position = startPosition;
        Vector2 direction = startDirection;
        while (position.x < targetX && path.Count < maxNumIterations)
        {
            hit = Physics2D.Raycast(position, direction, Mathf.Abs(targetX - position.x));
            if (hit.transform != null && wallTags.Contains(hit.transform.tag))
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
    private Vector2 ExtrapolatePoint(Vector2 position, Vector2 direction, float x)
    {
        return position + (direction * (x - position.x));
    }
}
