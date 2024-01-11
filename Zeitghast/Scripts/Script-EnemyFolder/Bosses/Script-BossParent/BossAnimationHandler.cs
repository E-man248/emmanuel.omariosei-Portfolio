using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAnimationHandler : EntityAnimationHandler
{
    #region Animation Strings

    internal string IdleAnimationString;
    internal string DeadAnimationString;

    #endregion

    [Header("Boss Animation Settings")]
    public bool flipOrientation = false;

    // Utilities:
    protected Boss boss;

    protected override void Awake()
    {
        base.Awake();

        boss = GetComponentInParent<Boss>();

        IdleAnimationString = boss.bossName + "Idle";
        DeadAnimationString = boss.bossName + "Dead";
    }

    protected override void Update()
    {
        if (Timer.gamePaused) return;

        base.Update();

        // Non-Mutually Exclusive Animations:
        orientate();
    }

    #region Mutually Exclusive Animations:

    protected override void animate()
    {
        switch(boss.currentState)
        {
            case Boss.State.Idle:
                animateIdle();
            break;

            case Boss.State.Attack:
                animateAttack();
            break;

            case Boss.State.Dead:
                animateDead();
            break;
        }

        base.animate();
    }

    protected virtual void animateIdle()
    {
        idle();
    }

    protected virtual void animateAttack()
    {
        idle();
    }

    protected virtual void animateDead()
    {
        dead();
    }

    protected virtual void idle()
    {
        nextAnimation = IdleAnimationString;
    }

    protected virtual void dead()
    {
        nextAnimation = DeadAnimationString;
    }

    public virtual void playDeathAnimation()
    {
        playAnimationOnceFull(DeadAnimationString);
    }

    public void orientate()
    {
        int graphicsDirection = flipOrientation ? (int) boss.lookDirection : (int) boss.lookDirection * -1 ;

        switch((Boss.LookDirection) graphicsDirection)
        {
            case Boss.LookDirection.Left:
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            break;
            case Boss.LookDirection.Right:
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            break;
        }
    }

    #endregion
}

