using System.Collections.Generic;
using UnityEngine;

public class PredictedTrajectory
{
    public int maxNumPoints = 5;
    public float lineExtentOnMaxBounce = 0.5f;
    public string bounceableColliderTag = "HorizontalWall";
    private List<Vector2> path;

    public bool Empty { get { return path.Count == 0; } }
    public int Size   { get { return path.Count; } }
    public Vector2 this[int index] { get { return path[index]; } }
    public Vector2 StartPoint { get { return path[0]; } }
    public Vector2 EndPoint   { get { return path[path.Count - 1]; } }

    public PredictedTrajectory()
    {
        path = new List<Vector2>(maxNumPoints);
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
    // compute trajectory by recursively extending line from last position, reflecting each bounce until
    // either the target x value is reached, or the maximum number of points is met
    // note: overrides all previous internal data
    public void Compute(Vector2 startPosition, Vector2 startDirection, float maxDistance)
    {
        path.Clear();
        path.Add(startPosition);
        ComputeTrajectory(startPosition, startDirection, startPosition.x + maxDistance);
    }

    private void ComputeTrajectory(Vector2 position, Vector2 direction, float maxX)
    {
        if (position.x == maxX)
        {
            //path.Add(position);
        }
        else if (path.Count == maxNumPoints - 1)
        {
            path.Add(lineExtentOnMaxBounce * ExtrapolateEndPoint(position, direction, maxX));
        }

        RaycastHit2D hit;
        if (!Raycast(position, direction, maxX, out hit))
        {
            path.Add(ExtrapolateEndPoint(position, direction, maxX));
        }
        else
        {
            path.Add(hit.point);
            ComputeTrajectory(hit.point, Vector2.Reflect(direction, hit.normal), maxX);
        }
    }
    private bool Raycast(Vector2 position, Vector2 direction, float maxX, out RaycastHit2D hit)
    {
        hit = Physics2D.Raycast(position, direction, Mathf.Abs(maxX - position.x));
        return hit.transform != null && hit.transform.CompareTag(bounceableColliderTag);
    }
    private Vector2 ExtrapolateEndPoint(Vector2 position, Vector2 direction, float x)
    {
        return position + (direction * (x - position.x));
    }
}
