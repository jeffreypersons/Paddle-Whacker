﻿using UnityEngine;


public class BallController : MonoBehaviour
{
    [SerializeField] private float   ballSpeed        = default;
    [SerializeField] private Vector2 initialDirection = default;

    private Vector2 initialPosition;
    private Rigidbody2D ballBody;
    private Collider2D ballCollider;

    public void Reset()
    {
        ballBody.position = initialPosition;
        ballBody.velocity = ballSpeed * initialDirection;
    }
    void Awake()
    {
        ballBody        = gameObject.transform.GetComponent<Rigidbody2D>();
        ballCollider    = gameObject.transform.GetComponent<BoxCollider2D>();
        initialPosition = ballBody.position;
    }
    void Start()
    {
        ballBody.velocity = ballSpeed * initialDirection;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ballBody.velocity = ballSpeed * ComputeBounceDirectionOffPaddle(ballCollider.bounds, collision.collider.bounds);
            GameEventCenter.paddleHit.Trigger(collision.gameObject.name);
        }
        if (collision.gameObject.CompareTag("Goal"))
        {
            GameEventCenter.goalHit.Trigger(collision.gameObject.name);
        }
        if (collision.gameObject.CompareTag("HorizontalWall"))
        {
            GameEventCenter.horizontalWallHit.Trigger(collision.gameObject.name);
        }
        if (collision.gameObject.CompareTag("VerticalWall"))
        {
            GameEventCenter.verticalWallHit.Trigger(collision.gameObject.name);
        }
    }

    private static Vector2 ComputeBounceDirectionOffPaddle(Bounds ballBounds, Bounds paddleBounds)
    {
        float invertedXDirection = ballBounds.center.x + paddleBounds.center.x > 0 ? -1 : 1;
        float offsetFromPaddleCenterToBall = (ballBounds.center.y - paddleBounds.center.y) / paddleBounds.size.y;
        return new Vector2(invertedXDirection, offsetFromPaddleCenterToBall).normalized;
    }
}
