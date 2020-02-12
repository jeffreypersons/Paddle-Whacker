using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    private Rigidbody2D ball;
    private List<Vector2> trajectory;
    private List<string> path;

    // Start is called before the first frame update
    void Start()
    {

    }

    // conceptually: draw line from ball position until a reflection occurs, repeating until target is met,
    // or no recursion depth (maxReflections) is met (which case it calculates partway off the last bounce)
    // when nothing to bounce off of is hit, let the trajectory go all the way to the paddle
    private void ComputeTrajectory(Vector2 position, Vector2 direction, float targetX, int reflectionsRemaining)
    {
        // todo: make a trajectory path class with node for each and then use that abstraction and move this stuff to over there...
        // todo: figure out why it only works for a single bounce!
        if (position.x == targetX)
        {
            trajectory.Add(position);
            return;
        }
        else if (reflectionsRemaining == 0)
        {
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
