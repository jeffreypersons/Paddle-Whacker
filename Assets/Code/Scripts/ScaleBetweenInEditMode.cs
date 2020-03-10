using UnityEngine;

[ExecuteInEditMode]
public class ScaleBetweenInEditMode : MonoBehaviour
{
    private BoxCollider2D topWall;
    private BoxCollider2D bottomWall;
    private BoxCollider2D leftPaddle;
    private BoxCollider2D rightPaddle;

    private BoxCollider2D leftRangeChecker;
    private BoxCollider2D rightRangeChecker;

    void Start()
    {
        topWall     = GameObject.Find("TopWall").GetComponent<BoxCollider2D>();
        bottomWall  = GameObject.Find("BottomWall").GetComponent<BoxCollider2D>();
        leftPaddle  = GameObject.Find("LeftPaddle").GetComponent<BoxCollider2D>();
        rightPaddle = GameObject.Find("RightPaddle").GetComponent<BoxCollider2D>();

        leftRangeChecker  = GameObject.Find("LeftRangeChecker").GetComponent<BoxCollider2D>();
        rightRangeChecker = GameObject.Find("RightRangeChecker").GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        float innerArenaHeight = topWall.bounds.min.y - bottomWall.bounds.max.y;

        leftRangeChecker.transform.position = leftPaddle.bounds.center;
        leftRangeChecker.size = new Vector2(leftPaddle.bounds.size.x, innerArenaHeight);

        rightRangeChecker.transform.position = rightPaddle.bounds.center;
        rightRangeChecker.size = new Vector2(rightPaddle.bounds.size.x, innerArenaHeight);
    }
}
