using UnityEngine;

[ExecuteInEditMode]
public class ExpandColliderToPaddleRange : MonoBehaviour
{
    private BoxCollider2D topWall;
    private BoxCollider2D bottomWall;

    public GameObject paddle;
    public float extraWidth;
    private BoxCollider2D paddleCollider;
    private BoxCollider2D movementRegion;

    void Start()
    {
        topWall = GameObject.Find("TopWall").GetComponent<BoxCollider2D>();
        bottomWall = GameObject.Find("BottomWall").GetComponent<BoxCollider2D>();

        paddleCollider = paddle.GetComponent<BoxCollider2D>();
        movementRegion = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        ComputeMovementRange(
            new Vector3(paddleCollider.bounds.min.x - (extraWidth * 0.5f), bottomWall.bounds.max.y),
            new Vector3(paddleCollider.bounds.max.x + (extraWidth * 0.5f), topWall.bounds.min.y)
        );
    }
    private void ComputeMovementRange(Vector3 min, Vector3 max)
    {
        movementRegion.size = max - min;
        movementRegion.transform.position = min + movementRegion.bounds.extents;
    }
}
