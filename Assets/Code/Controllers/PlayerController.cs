using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float paddleSpeed;
    public string inputAxisName;

    private Vector2 initialPosition;
    private Vector2 inputVelocity;
    private Rigidbody2D paddleBody;

    public void Reset()
    {
        inputVelocity = Vector2.zero;
        paddleBody.velocity = inputVelocity;
        paddleBody.position = initialPosition;
    }
    void Start()
    {
        paddleBody = gameObject.transform.GetComponent<Rigidbody2D>();
        initialPosition = paddleBody.position;
        Reset();
    }
    void Update()
    {
        inputVelocity = new Vector2(0, paddleSpeed * Input.GetAxisRaw(inputAxisName));
    }
    void FixedUpdate()
    {
        paddleBody.velocity = inputVelocity;
    }
}
