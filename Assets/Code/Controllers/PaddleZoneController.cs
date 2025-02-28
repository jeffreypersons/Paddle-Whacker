﻿using UnityEngine;


public class PaddleZoneController : MonoBehaviour
{
    [SerializeField] private GameObject paddle = default;

    private string paddleName;
    private BoxCollider2D paddleZoneCollider;
    private Vector2 lastRecordedInPosition;
    private Vector2 lastRecordedInVelocity;
    private Vector2 lastRecordedOutPosition;
    private Vector2 lastRecordedOutVelocity;

    void Awake()
    {
        paddleName = paddle.name;
        paddleZoneCollider = gameObject.transform.GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ball = collision.GetComponent<Rigidbody2D>();
            lastRecordedInVelocity = ball.linearVelocity;
            lastRecordedInPosition = ball.position;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ball = collision.GetComponent<Rigidbody2D>();
            lastRecordedOutVelocity = ball.linearVelocity;
            lastRecordedOutPosition = ball.position;
            GameEventCenter.zoneIntersection.Trigger(
                new PaddleZoneIntersectInfo(
                    paddleName,
                    lastRecordedInPosition,
                    lastRecordedInVelocity,
                    lastRecordedOutPosition,
                    lastRecordedOutVelocity
                )
            );
        }
    }
}
