using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Knockback : MonoBehaviour
{
    [Header("Knockback")]
    protected Rigidbody2D rigidBody;
    public TagList attackers;
    public Vector2 knockbackForce;
    private bool launching;
    private Vector2 currentLaunchDirection;

    [Header("Immovability")]
    public bool knockbackDisabled;

    [Header("Stunned")]
    public bool stunned;
    public float knockbackTimer;
    public float knockbackDuration;
    public bool hasBeenReset;

    [Space]
    public bool trueImmobility = false;

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        trueImmobility = false;

        hasBeenReset = false;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (!trueImmobility)
        {
            if (knockbackTimer > 0)
            {
                stunned = true;
                hasBeenReset = false;
            }
            else
            {
                stunned = false;
                if (!hasBeenReset) afterknockbackReset();
            }
        }
        knockbackTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (launching)
        {
            rigidBody.AddForce(currentLaunchDirection, ForceMode2D.Impulse);
            launching = false;
        }
    }

    public virtual void knockbackFrames()
    {
        if (!knockbackDisabled)
        {
            knockbackTimer = knockbackDuration;
        }
    }

    public virtual void knockbackFrames(float time)
    {
        if (!knockbackDisabled)
        {
            knockbackTimer = time;
        }
    }

    protected virtual void afterknockbackReset()
    {
        hasBeenReset = true;
    }

    public virtual void applyKnockBackDirection(int launchDirection)
    {
        if (!stunned && !knockbackDisabled)
        {
            rigidBody.velocity = Vector3.zero;
            currentLaunchDirection = new Vector2(launchDirection * knockbackForce.x, knockbackForce.y);
            launching = true;
        }
    }

    public virtual void applyKnockBackDirection(int launchDirection, Vector2 knockbackForce)
    {
        if (!stunned && !knockbackDisabled)
        {
            rigidBody.velocity = Vector3.zero;
            currentLaunchDirection = new Vector2(launchDirection * knockbackForce.x, knockbackForce.y);
            launching = true;
        }
    }

    public virtual void applyKnockBackPoint(Vector2 explosionPoint)
    {
        if (!stunned && !knockbackDisabled)
        {
            int launchDirection = 1;
            if (explosionPoint.x - transform.position.x > 0) {
                launchDirection = -1;
            }
            else
            {
                launchDirection = 1;
            }
            applyKnockBackDirection(launchDirection);
        }
    }

    public virtual void applyKnockBackPoint(Vector2 explosionPoint, Vector2 knockbackForce)
    {
        if (!stunned && !knockbackDisabled)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            int launchDirection = 1;
            if (explosionPoint.x - transform.position.x > 0)
            {
                launchDirection = -1;
            }
            else
            {
                launchDirection = 1;
            }
            applyKnockBackDirection(launchDirection, knockbackForce);
        }
    }
}
