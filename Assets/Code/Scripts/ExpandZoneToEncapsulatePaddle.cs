using UnityEngine;


[ExecuteAlways]
public class ExpandZoneToEncapsulatePaddle : MonoBehaviour
{
    [SerializeField] private float extraWidth = default;

    private BoxCollider2D zoneCollider;
    private BoxCollider2D paddleCollider;

    private BoxCollider2D bottomWallCollider;
    private BoxCollider2D topWallCollider;

    void Start()
    {
        zoneCollider       = gameObject.transform.GetComponent<BoxCollider2D>();
        paddleCollider     = gameObject.transform.parent.GetComponent<BoxCollider2D>();
        bottomWallCollider = GameObject.Find("BottomWall").GetComponent<BoxCollider2D>();
        topWallCollider    = GameObject.Find("TopWall").GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {
            SetZoneColliderBounds(
                new Vector3(paddleCollider.bounds.min.x - (extraWidth * 0.5f), bottomWallCollider.bounds.max.y),
                new Vector3(paddleCollider.bounds.max.x + (extraWidth * 0.5f), topWallCollider.bounds.min.y)
            );
        }
    }
    private void SetZoneColliderBounds(Vector3 min, Vector3 max)
    {
        zoneCollider.size = max - min;
        zoneCollider.transform.localScale = Vector2.one / paddleCollider.transform.localScale;
        zoneCollider.transform.position = min + zoneCollider.bounds.extents;
    }
}
