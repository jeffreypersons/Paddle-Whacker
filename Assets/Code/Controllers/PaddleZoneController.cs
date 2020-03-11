using UnityEngine;

// assumes paddle zone it NOT touching any vertical walls
public class PaddleZoneController : MonoBehaviour
{
    private BoxCollider2D paddleZone;
    private Vector2 lastRecordedInVelocity;
    private Vector2 lastRecordedOutVelocity;

    void Start()
    {
        paddleZone = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            lastRecordedInVelocity = collision.GetComponent<Rigidbody2D>().velocity;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            lastRecordedOutVelocity = collision.GetComponent<Rigidbody2D>().velocity;
            GameEvents.onZoneIntersection.Invoke(
                new ZoneIntersectInfo(
                    gameObject.name,
                    lastRecordedInVelocity,
                    lastRecordedOutVelocity
                )
            );
        }
    }
}
