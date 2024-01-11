using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockback : Knockback
{
    [Header("Enemy Knockback")]
    public float afterKnockbackStunDuration = 0.1f;
    private EnemyMovement enemy;

    protected override void Awake()
    {
        base.Awake();
        hasBeenReset = true;
    }

    protected override void Start()
    {
        enemy = GetComponent<EnemyMovement>();
    }

    public override void applyKnockBackDirection(int launchDirection, Vector2 knockbackForce)
    {
        if (!knockbackDisabled)
        {
            base.applyKnockBackDirection(launchDirection, knockbackForce * rigidBody.mass);
        }
    }

    public override void knockbackFrames()
    {
        if (!knockbackDisabled)
        {
            knockbackTimer = knockbackDuration;
            enemy.stunEnemy(knockbackDuration + afterKnockbackStunDuration);
        }
    }

    public override void knockbackFrames(float time)
    {
        if (!knockbackDisabled)
        {
            knockbackTimer = time;
            enemy.stunEnemy(time + afterKnockbackStunDuration);
        }
    }

    protected override void afterknockbackReset()
    {
        //enemy.stunEnemy();
        hasBeenReset = true;
    }
}
