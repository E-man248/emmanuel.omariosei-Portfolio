using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityRaptor : GroundedEnemy
{
    [Space]

    [Header("Velocity Raptor Settings")]
    [SerializeField] private float pursueJumpHeight = 6f;
    private bool hasInitiatedJumped = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        jumpEnabled = true;
        hasInitiatedJumped = false;
    }

    protected override void pursue()
    {
        jumpEnabled = true;

        pursueOrientationCheck();

        jumpInitiationCycle();

        // If Jump is Enabled, this IF statement will allow the pursue cycle to
        // return when it needs to (so that the enemy does not move when its jumping)
        // If Jump Is Not Enabled Skip This!
        if (!pursueJumpCycle())
        {  
            return;
        }
    }

    protected override bool pursueJumpCycle()
    { 
        jumpWindupTimer -= Time.deltaTime;
        
        // If enemy is not ready to jump | OR | Is falling (airborne)
        if (jumpWindupTimer > 0f || !isGrounded())
        {
            if (!isGrounded())
            {
                hasLandedAfterJump = false;
                hasPlayedLandingAnimation = false;
                if (isGoingToHitWall()) moveDirection.x *= -1;
            }
            else
            {
                orientLookDirectionForJump();
            }
            return false;
        }
        // If fully wound up and have not jumped, initiate jump!
        // This forces the jump to only happen once and not happen
        // again until you are fully airborne (completely not touching ground)
        else if (!hasJumpedAfterWindup)
        {
            impulseJump(target.position, pursueJumpHeight);
            
            orientLookDirectionForJump();

            jumpCooldownTimer = jumpCooldownDuration;
            hasJumpedAfterWindup = true;
            hasLandedAfterJump = false;
            hasPlayedLandingAnimation = false;
            return false;
        }

        // Allow landing animation to play and reset (and only play once)
        if (!hasLandedAfterJump && isGrounded())
        {
            hasLandedAfterJump = true;
            if (groundedEnemyAnimationHandler != null)
            {
                groundedEnemyAnimationHandler.landingAnimation();
                Invoke("resetHasLanded", groundedEnemyAnimationHandler.getAnimationLength(entityName + "Landing"));
            }
            else
            {
                resetHasLanded();
            }
            return false;
        }

        // Do not move while playing landing animations
        if (!hasPlayedLandingAnimation && isGrounded())
        {
            moveX(0);
            return false;
        }

        // Continues the pursuit cycle
        // (returning false would break the cycle and re-run this function)
        return true;
    }

    protected override void resetHasLanded()
    {
        hasPlayedLandingAnimation = true;
        hasInitiatedJumped = false;
    }

    /**
        <summary>
            This cycle is to be called in every Update while in the pursue state
            and initiates a jump for the Velocity Raptor. The cooldown for the jump
            is to be handled in pursueJumpCycle(), this only starts the jump.
        </summary>
    **/
    protected void jumpInitiationCycle()
    {
        if(!hasInitiatedJumped && jumpCooldownTimer <= 0 && isGrounded())
        {
            // Reset Jump Windup Timer (Initiates the Jump Windup)
            jumpWindupTimer = jumpWindupDuration;

            // Stops the Enemy From Moving Before Windup
            moveX(0f);

            // Reset Jump State Variables
            hasJumpedAfterWindup = false;
            hasInitiatedJumped = true;

            orientLookDirectionForJump();
        }
    }

    protected void orientLookDirectionForJump()
    {
        // Set the Move Direction
        // (Used for the Sprite Orientation, NOT Used for the Jump)
        float distanceFromTarget = targetPosition.x - transform.position.x;
        moveDirection.x = Mathf.Sign(distanceFromTarget);
    }

    protected override bool pursueCheck()
    {
        return inHostileRange() && hasLineOfSight(target.position);
    }
}
