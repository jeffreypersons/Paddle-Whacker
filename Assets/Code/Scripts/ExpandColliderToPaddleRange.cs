using UnityEngine;

[ExecuteInEditMode]
public class ExpandColliderToPaddleRange : MonoBehaviour
{
    private BoxCollider2D topWall;
    private BoxCollider2D bottomWall;

    public GameObject paddle;
    public float extraWidth;
    private BoxCollider2D paddleCollider;
    private BoxCollider2D paddleZone;

    void Start()
    {
        topWall = GameObject.Find("TopWall").GetComponent<BoxCollider2D>();
        bottomWall = GameObject.Find("BottomWall").GetComponent<BoxCollider2D>();

        paddleCollider = paddle.GetComponent<BoxCollider2D>();
        paddleZone = GetComponent<BoxCollider2D>();
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
        paddleZone.size = max - min;
        paddleZone.transform.position = min + paddleZone.bounds.extents;
    }
}
