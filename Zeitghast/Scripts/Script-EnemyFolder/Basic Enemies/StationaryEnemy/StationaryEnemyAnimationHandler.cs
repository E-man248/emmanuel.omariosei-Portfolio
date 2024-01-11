using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemyAnimationHandler : enemyAnimationHandler
{
    #region Animations Names
    protected string IdleAnimationString;
    protected string AttackAnimationString;
    protected string AttackWindupAnimationString;
    protected string DeathAnimationString;
    #endregion

    protected StationaryEnemy stationaryEnemyAI;

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
        stationaryEnemyAI = (StationaryEnemy)enemyMovement;
    }
    
    protected override void Update()
    {
        base.Update();
    }

    #region Animation Booleans:
    
    // Boolean Functions that are used to dictate animation switching:
    
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
    
    protected virtual void idleAnimation()
    {
        nextAnimation = IdleAnimationString;
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
    #endregion
}
