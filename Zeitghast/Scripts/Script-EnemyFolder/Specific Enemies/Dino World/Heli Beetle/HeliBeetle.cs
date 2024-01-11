using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliBeetle : GroundedEnemy
{
    [Header("Heli Beetle Setting")]
    [SerializeField] private float flyingAttackSpeedMulitplier = 1.7f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    
    protected override void combatMovementCycle()
    {
        
    }

    protected override void combat()
    {
        attackWindUpCycle();

        if (attackWindUpTimer > 0)
        {
            return;
        }

        attackCycle();
    }

    public bool isAttacking() // Tells Animator Attack is Taking Place
    {
        return attackSpeedTimer > 0 && attackWindUpTimer <= 0;
    }

    protected override void attackWindUpCycle()
    {
        if (attackAnimationIsPlaying) return;

        attackWindUpTimer -= Time.deltaTime;

        if (attackWindUpTimer > 0)
        {
            attackSpeedTimer = attackSpeed;
            lookAtTarget();
            return;
        }
    }


    protected override void attackCycle()
    {
        if (attackSpeedTimer > 0)
        {
            orientateLook();
            flyingAttack();
            attackSpeedTimer -= Time.deltaTime;
        }
        else
        {
            coolDown();
            //Checks if we can Leave combat state and turns off animation
            leaveCombatState();
        }
    }

    //path finding and setting gravity
    protected virtual void flyingAttack()
    {
        //path finding 
        FlyingAttackUpdatePath(flyingAttackSpeedMulitplier);

        //Turning off gravity
        changeGravity(0f);
    }

    private void changeGravity(float gravityScale)
    {
        entityRigidbody.gravityScale = gravityScale;
    }
    protected override void leaveCombatState()
    {
        print("Leave Combat");
        base.leaveCombatState();

        //Turning on gravity
        changeGravity(1f);
    }

    //Shut down Phase
    protected override void coolDown()
    {
        base.coolDown();

        moveX(0);

        //Turning on gravity
        changeGravity(1f);
    }


    protected virtual void FlyingAttackUpdatePath(float speedMultiplier = 1f)
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(entityRigidbody.position, target.position, OnPathComplete);
        }

        flyingPathFollow(speedMultiplier);
    }

    private void flyingPathFollow(float speedMultiplier = 1f)
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        if (currentWaypoint + 1 >= path.vectorPath.Count)
        {
            return;
        }

        moveDirection = ((Vector2)path.vectorPath[currentWaypoint + 1] - entityRigidbody.position).normalized;

        moveDirection *= speedMultiplier;

        moveXY(moveDirection.x, moveDirection.y);

        float distance = Vector2.Distance(entityRigidbody.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }
    }
}
