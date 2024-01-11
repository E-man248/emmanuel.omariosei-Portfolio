using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemyAnimationHandler : enemyAnimationHandler
{
    #region Animation Variables
    
    // Untility Variables:
    private bool hasPlayedFallingEaseInAnimation = false;

    #endregion

    #region Animations Names
    protected string IdleAnimationString;
    protected string WalkAnimationString;
    protected string JumpAnimationString;
    protected string JumpWindupAnimationString;
    protected string FallingAnimationString;
    protected string FallingEaseInAnimationString;
    protected string AttackAnimationString;
    protected string AttackWindupAnimationString;
    protected string LandingAnimationString;
    protected string DeathAnimationString;
    #endregion

    protected GroundedEnemy groundedEnemyAI;
    [SerializeField] private float walkingSpeedThreshold = 0.9f;

    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions
        IdleAnimationString = enemyName + "Idle";
        WalkAnimationString = enemyName + "Walk";
        JumpAnimationString = enemyName + "Jump";
        JumpWindupAnimationString = enemyName + "JumpWindup";
        FallingAnimationString = enemyName + "Falling";
        FallingEaseInAnimationString = enemyName + "FallingEaseIn";
        AttackAnimationString = enemyName + "Attack";
        AttackWindupAnimationString = enemyName + "AttackWindup";
        LandingAnimationString = enemyName + "Landing";
        DeathAnimationString = enemyName + "Death";
        #endregion
    }

    protected override void Start()
    {
        base.Start();
        groundedEnemyAI = (GroundedEnemy)enemyMovement;
    }
    
    protected override void Update()
    {
        base.Update();
    }

    #region Animation Booleans:
    
    // Boolean Functions that are used to dictate animation switching:
    public bool isJumping()
    {
        return groundedEnemyAI.isJumping();
    }

    public bool isWindingUpJump()
    {
        return groundedEnemyAI.jumpWindupTimer > 0 && enemyMovement.state == EnemyMovement.State.Pursuit;
    }

    public virtual bool isFalling()
    {
        return groundedEnemyAI.isFalling();
    }

    public bool isWalking()
    {
        return Mathf.Abs(enemyMovement.entityRigidbody.velocity.x) > walkingSpeedThreshold;
    }

    #endregion

    protected override void animate()
    {
        base.animate();

        // List of Mutually Exclusive Animations:        
        idleAnimation();
        walkAnimation();
        jumpWindupAnimation();
        jumpAnimation();
        fallingAnimation();
        attackWindUpAnimation();
        stunAnimation();
    }

    #region Mutually Exclusive Animations
    
    protected virtual void idleAnimation()
    {
        nextAnimation = IdleAnimationString;
    }

    protected virtual void walkAnimation()
    {
        if(isWalking())
        {
            nextAnimation = WalkAnimationString;
        }
    }

    public override void attackAnimation()
    {
        playAnimationOnceFull(AttackAnimationString);
    }

    public override void attackWindUpAnimation()
    {
        if (isWindingUpAttack())
        {
            nextAnimation = AttackWindupAnimationString;
        }
    }

    protected void jumpAnimation()
    {
        if (groundedEnemyAI.jumpEnabled && isJumping())
        {
            nextAnimation = JumpAnimationString;
        }
    }

    protected void jumpWindupAnimation()
    {
        if (groundedEnemyAI.jumpEnabled && isWindingUpJump())
        {
            nextAnimation = JumpWindupAnimationString;
        }
    }

    protected void fallingAnimation()
    {
        if (isFalling())
        {
            nextAnimation = FallingAnimationString;
        
            if (!hasPlayedFallingEaseInAnimation)
            {
                Invoke( "resetFallingEaseInAnimation" , getAnimationLength(FallingEaseInAnimationString) );
                nextAnimation = FallingEaseInAnimationString;
            }
        }

        if (getCurrentAnimation() != FallingAnimationString && getCurrentAnimation() != FallingEaseInAnimationString)
        {
            hasPlayedFallingEaseInAnimation = false;
        }
    }

    private void resetFallingEaseInAnimation()
    {
        hasPlayedFallingEaseInAnimation = true;
    }

    public virtual void landingAnimation()
    {
        playAnimationOnceFull(LandingAnimationString);
    }

    public override void deathAnimation()
    {
        playAnimationOnceFull(DeathAnimationString);
    }
    #endregion
}
