using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float movementSpeed;
    public float jumpVelocity;
    public float feetThreshold = 0.45F;
    public float airControl = 0.5F;
    public int groundDecayTicks = 10;

    [NonSerialized] public Vector2 rawInput;
    [NonSerialized] public bool onGround;
    [NonSerialized] public bool effectiveOnGround;

    private Rigidbody2D rb;
    private CircleCollider2D col;
    private List<ContactPoint2D> contactPoints = new();
    private int groundTimer;

    private void OnCollisionEnter2D(Collision2D _)
    {
        UpdateOnGround();
    }

    private void OnCollisionExit2D(Collision2D _)
    {
        UpdateOnGround();
    }

    private void UpdateOnGround()
    {
        col.GetContacts(contactPoints);
        var newOnGround = false;
        foreach (ContactPoint2D point in contactPoints)
        {
            Vector2 pos = point.point;
            if (pos.y < transform.position.y + col.offset.y - col.radius + Mathf.Lerp(0, col.radius * 2, feetThreshold))
                newOnGround = true;
        }

        if (onGround && !newOnGround) groundTimer = groundDecayTicks;
        else effectiveOnGround = newOnGround;
        onGround = newOnGround;
    }

    private void OnDrawGizmosSelected()
    {
        var c = GetComponent<CircleCollider2D>();
        if (c is null) return;
        float y = transform.position.y + c.offset.y - c.radius + Mathf.Lerp(0, c.radius * 2, feetThreshold);
        var start = new Vector2(transform.position.x - 0.5F, y);
        var end = new Vector2(transform.position.x + 0.5F, y);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start, end);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        rawInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 vel = rb.velocity;
        float speed = movementSpeed;
        if (!effectiveOnGround) speed *= airControl;
        vel.x = rawInput.x * speed;
        if (effectiveOnGround && rawInput.y > 0 && vel.y < jumpVelocity)
        {
            vel.y = jumpVelocity;
            effectiveOnGround = false;
        }

        if (groundTimer > 0 && --groundTimer <= 0) effectiveOnGround = onGround;

        rb.velocity = vel;
    }
}