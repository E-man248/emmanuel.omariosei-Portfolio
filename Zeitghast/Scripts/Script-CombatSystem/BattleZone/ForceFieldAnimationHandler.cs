using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldAnimationHandler : enemyAnimationHandler
{
    #region Animations Names
    protected string IdleAnimationString;
    protected string ActivateAnimationString;
    protected string DeathAnimationString;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        Enemy = transform.parent;
        enemyHealth = transform.parent.GetComponent<EnemyHealth>();
        enemyName = "ForceField";

        #region Animation String Definitions
        IdleAnimationString = enemyName + "Idle";
        ActivateAnimationString = enemyName + "Activate";
        DeathAnimationString = enemyName + "Death";
        #endregion
    }

    protected override void Start()
    {
        base.Start();

        nextAnimation = IdleAnimationString;
        changeAnimation(IdleAnimationString);
    }

    public virtual void idleAnimation()
    {
        nextAnimation = IdleAnimationString;
    }

    public virtual void activateAnimation()
    {
        playAnimationOnceFull(ActivateAnimationString);
    }

    public override void deathAnimation()
    {
        playAnimationOnceFull(DeathAnimationString);
    }
}
