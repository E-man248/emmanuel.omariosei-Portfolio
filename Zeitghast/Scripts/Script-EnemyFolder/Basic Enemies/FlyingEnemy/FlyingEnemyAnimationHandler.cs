using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyAnimationHandler : enemyAnimationHandler
{
    #region Animations Names
    protected string IdleAnimationString;
    protected string FlyingAnimationString;
    protected string AttackAnimationString;
    protected string AttackWindupAnimationString;
    protected string DeathAnimationString;
    #endregion

    protected FlyingEnemy flyingEnemyAI;

    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions
        IdleAnimationString = enemyName + "Idle";
        AttackAnimationString = enemyName + "Attack";
        AttackWindupAnimationString = enemyName + "AttackWindup";
        DeathAnimationString = enemyName + "Death";
        #endregion
    }

    protected override void Start()
    {
        base.Start();
        flyingEnemyAI = (FlyingEnemy)enemyMovement;
    }
    
    protected override void Update()
    {
        base.Update();
    }

    #region Animation Booleans:
    
    // Boolean Functions that are used to dictate animation switching:
    public bool isFlying()
    {
        return Mathf.Abs(enemyMovement.entityRigidbody.velocity.x) > 0f || Mathf.Abs(enemyMovement.entityRigidbody.velocity.y) > 0f;
    }

    #endregion

    protected override void animate()
    {
        base.animate();

        // List of Mutually Exclusive Animations:        
        idleAnimation();
        attackWindUpAnimation();
        stunAnimation();
    }

    #region Mutually Exclusive Animations
    
    private void idleAnimation()
    {
        nextAnimation = IdleAnimationString;
    }

    private void flyingAnimation()
    {
        if(isFlying())
        {
            nextAnimation = FlyingAnimationString;
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

    public override void deathAnimation()
    {
        playAnimationOnceFull(DeathAnimationString);
    }
    #endregion
}
