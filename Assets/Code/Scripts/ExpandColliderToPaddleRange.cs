using UnityEngine;

[ExecuteInEditMode]
public class ExpandColliderToPaddleRange : MonoBehaviour
{
    public string paddleName;

    private BoxCollider2D topWall;
    private BoxCollider2D bottomWall;

    private BoxCollider2D paddle;
    private BoxCollider2D movementRegion;

    void Start()
    {
        topWall = GameObject.Find("TopWall").GetComponent<BoxCollider2D>();
        bottomWall = GameObject.Find("BottomWall").GetComponent<BoxCollider2D>();

        paddle = GetComponentInParent<BoxCollider2D>();
        movementRegion = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        ComputeMovementRange(
            new Vector2(paddle.bounds.min.x, bottomWall.bounds.max.y),
            new Vector2(paddle.bounds.max.x, topWall.bounds.min.y),
            new Vector2(paddle.transform.localScale.x, 1)
        );
    }
    private void ComputeMovementRange(Vector2 min, Vector2 max, Vector2 scale)
    {
        movementRegion.size = scale * (max - min);
        movementRegion.transform.position = min + (movementRegion.size * 0.5f);
    }

}
