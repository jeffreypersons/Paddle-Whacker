using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory
{
    public int maxNumBounces;

    private int numInUse;

    public struct Node
    {
        public Vector2 position;
        public Vector2 direction;
        public string contact;
    }

    private Node[] path;

    Trajectory()
    {
        numInUse = 0;
        path = new Node[maxNumBounces];
        for (int i = 0; i < maxNumBounces; i++)
        {
            path[i] = new Node();
        }
    }
    private void AddPoint(Vector2 position, Vector2 direction, string contact)
    {
        if (numInUse + 1 < maxNumBounces)
        {
            path[numInUse].position  = position;
            path[numInUse].direction = direction;
            path[numInUse].contact   = contact;
            numInUse++;
        }
    }
    public void ComputeNewTrajectory(Vector2 startPosition, Vector2 startDirection, float targetX)
    {
        AddPoint(startPosition, startDirection, "start");
        while ()
        {
            SetPoint()
        }
        ComputeTrajectory();
    }
    // conceptually: draw line from ball position until a reflection occurs, repeating until target is met,
    // or no recursion depth (maxReflections) is met (which case it calculates partway off the last bounce)
    // when nothing to bounce off of is hit, let the trajectory go all the way to the paddle
    // todo: figure out why it only works for a single bounce!
    private void ComputeTrajectory(Node node, float targetX, int index)
    {
        if (node.position.x == targetX)
        {
            AddPoint(node.position, node.direction, "targetX");
            return;
        }
        else if (index == maxNumBounces)
        {
            AddPoint(node.position, node.direction, "targetX");
            path.Add("used all reflections");
            float horizontalStepForPartialBounce = 0.5f * Mathf.Abs(targetX - position.x);
            trajectory.Add(position + (direction * horizontalStepForPartialBounce));
            return;
        }

        float distanceToPaddle = Mathf.Abs(targetX - position.x);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distanceToPaddle);
        if (hit.transform != null && hit.transform.CompareTag("HorizontalWall"))
        {
            path.Add(hit.transform.name);
            trajectory.Add(hit.point);
            Debug.DrawLine(position, hit.point, Color.green, 2.5f);
            ComputeTrajectory(hit.point, Vector2.Reflect(direction, hit.normal), targetX, reflectionsRemaining - 1);
        }
        else
        {
            path.Add("targetX");
            trajectory.Add(position + (direction * distanceToPaddle));
            Debug.DrawLine(position, trajectory[trajectory.Count - 1], Color.green, 2.5f);
        }
    }

}
