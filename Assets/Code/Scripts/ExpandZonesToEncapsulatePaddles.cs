using UnityEngine;


[ExecuteAlways]
public class ExpandZonesToEncapsulatePaddles : MonoBehaviour
{
    [SerializeField] private float extraHorizontalPadding = default;
    [SerializeField] private BoxCollider2D leftZone       = default;
    [SerializeField] private BoxCollider2D rightZone      = default;

    [SerializeField] private BoxCollider2D leftPaddle  = default;
    [SerializeField] private BoxCollider2D rightPaddle = default;
    [SerializeField] private BoxCollider2D bottomWall  = default;
    [SerializeField] private BoxCollider2D topWall     = default;

    private float InnerArenaMaxY => topWall   .bounds.min.y;
    private float InnerArenaMinY => bottomWall.bounds.max.y;

    void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {
            if (leftPaddle.transform.hasChanged)
            {
                ResizeZoneColliderToEncapsulatePaddle(leftZone, leftPaddle);
            }
            if (rightPaddle.transform.hasChanged)
            {
                ResizeZoneColliderToEncapsulatePaddle(rightZone, rightPaddle);
            }
        }
    }

    private void ResizeZoneColliderToEncapsulatePaddle(BoxCollider2D zone, BoxCollider2D paddle)
    {
        float height = InnerArenaMaxY - InnerArenaMinY;
        zone.size = new Vector3(paddle.size.x + extraHorizontalPadding, height);
        zone.transform.position = new Vector3(paddle.bounds.center.x, InnerArenaMinY + (height * 0.50f));
    }
}
