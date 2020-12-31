using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float minVerticalDistanceBeforeMoving = default;

    private float minPaddleY;
    private float maxPaddleY;
    private Vector2 initialPosition;
    private Vector2 previousMousePosition;

    private float targetPaddleY;
    private Rigidbody2D paddleBody;
    private BoxCollider2D paddleCollider;

    public void Reset()
    {
        paddleBody.velocity   = Vector2.zero;
        paddleBody.position   = initialPosition;
        targetPaddleY         = initialPosition.y;
        previousMousePosition = Input.mousePosition;

        minPaddleY = Physics2D.Raycast(initialPosition, Vector2.down).centroid.y + paddleCollider.bounds.extents.y;
        maxPaddleY = Physics2D.Raycast(initialPosition, Vector2.up)  .centroid.y - paddleCollider.bounds.extents.y;
    }

    void Awake()
    {
        paddleBody      = gameObject.transform.GetComponent<Rigidbody2D>();
        paddleCollider  = gameObject.transform.GetComponent<BoxCollider2D>();
        initialPosition = paddleBody.position;
        Reset();
    }
    void Update()
    {
        if (Mathf.Approximately(Input.mousePosition.y, previousMousePosition.y))
        {
            targetPaddleY = Mathf.Clamp(
                value: Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                min:   minPaddleY,
                max:   maxPaddleY);
        }
        previousMousePosition = Input.mousePosition;
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(targetPaddleY - paddleBody.position.y) >= minVerticalDistanceBeforeMoving)
        {
            paddleBody.position = Vector2.MoveTowards(
                paddleBody.position,
                new Vector2(paddleBody.position.x, targetPaddleY),
                float.MaxValue);
        }
    }
}
