using UnityEngine;


public class PaddleZoneController : MonoBehaviour
{
    private BoxCollider2D paddleZoneCollider;
    private Vector2 lastRecordedInPosition;
    private Vector2 lastRecordedInVelocity;
    private Vector2 lastRecordedOutPosition;
    private Vector2 lastRecordedOutVelocity;

    void Start()
    {
        paddleZoneCollider = gameObject.transform.GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ball = collision.GetComponent<Rigidbody2D>();
            lastRecordedInVelocity = ball.velocity;
            lastRecordedInPosition = ball.position;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ball = collision.GetComponent<Rigidbody2D>();
            lastRecordedOutVelocity = ball.velocity;
            lastRecordedOutPosition = ball.position;
            GameEvents.onZoneIntersection.Invoke(
                new PaddleZoneIntersectInfo(
                    paddleZoneCollider.transform.parent.name,
                    lastRecordedInPosition,
                    lastRecordedInVelocity,
                    lastRecordedOutPosition,
                    lastRecordedOutVelocity
                )
            );
        }
    }
}
