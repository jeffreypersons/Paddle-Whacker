using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float  paddleSpeed   = default;
    [SerializeField] private string inputAxisName = default;
    [SerializeField] private float  minVerticalDistanceBeforeMoving = default;

    private bool moveWithKeys;
    private Vector2 initialPosition;
    private float targetPaddleY;
    private Rigidbody2D paddleBody;

    public void Reset()
    {
        paddleBody.velocity = Vector2.zero;
        paddleBody.position = initialPosition;
        targetPaddleY       = initialPosition.y;
        moveWithKeys        = true;
    }

    void Awake()
    {
        paddleBody          = gameObject.transform.GetComponent<Rigidbody2D>();
        initialPosition     = paddleBody.position;
        targetPaddleY       = paddleBody.position.y;
        paddleBody.velocity = Vector2.zero;
        moveWithKeys        = true;
    }
    void Update()
    {
        float directionalKeysInputStrength = Input.GetAxisRaw(inputAxisName);
        moveWithKeys = !Mathf.Approximately(directionalKeysInputStrength, 0.00f);
        if (moveWithKeys)
        {
            targetPaddleY = paddleBody.position.y + (paddleSpeed * directionalKeysInputStrength);
        }
        else
        {
            targetPaddleY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        }
    }
    void FixedUpdate()
    {
        if (Mathf.Abs(targetPaddleY - paddleBody.position.y) >= minVerticalDistanceBeforeMoving)
        {
            paddleBody.position = Vector2.MoveTowards(
                paddleBody.position,
                new Vector2(paddleBody.position.x, targetPaddleY),
                paddleSpeed * Time.fixedDeltaTime);
        }
    }
}
