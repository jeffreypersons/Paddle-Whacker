using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float  paddleSpeedForKeys              = default;
    [SerializeField] private string inputAxisName                   = default;
    [SerializeField] private float  minVerticalDistanceBeforeMoving = default;

    private bool moveWithCursor;
    private Vector2 previousMousePosition;
    private Vector2 initialPosition;
    private float targetPaddleY;
    private Rigidbody2D paddleBody;

    public void Reset()
    {
        paddleBody.velocity   = Vector2.zero;
        paddleBody.position   = initialPosition;
        targetPaddleY         = initialPosition.y;
        moveWithCursor        = true;
        previousMousePosition = Input.mousePosition;
    }

    void Awake()
    {
        paddleBody = gameObject.transform.GetComponent<Rigidbody2D>();
        initialPosition = paddleBody.position;
        Reset();
    }
    void Update()
    {
        float distanceCursorMoved = Input.mousePosition.y - previousMousePosition.y;
        float directionalKeysInputStrength = Input.GetAxisRaw(inputAxisName);
        moveWithCursor = Mathf.Approximately(directionalKeysInputStrength, 0.00f);
        if (Mathf.Approximately(Input.mousePosition.y, previousMousePosition.y))
        {
            targetPaddleY = paddleBody.position.y + (paddleSpeedForKeys * directionalKeysInputStrength);
        }
        else
        {
            targetPaddleY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        }
    }
    void FixedUpdate()
    {
        if (Mathf.Abs(targetPaddleY - paddleBody.position.y) < minVerticalDistanceBeforeMoving)
        {
            return;
        }

        float maxDistanceCanMove = moveWithCursor? paddleSpeedForKeys : float.MaxValue;
        paddleBody.position = Vector2.MoveTowards(
            paddleBody.position,
            new Vector2(paddleBody.position.x, targetPaddleY),
            maxDistanceCanMove);
    }
}
