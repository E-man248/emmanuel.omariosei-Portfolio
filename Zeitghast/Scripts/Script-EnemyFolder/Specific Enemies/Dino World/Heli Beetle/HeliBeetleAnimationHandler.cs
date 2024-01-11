using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliBeetleAnimationHandler : GroundedEnemyAnimationHandler
{
    private HeliBeetle heliBeetleAI;

    protected override void Start()
    {
        base.Start();
        heliBeetleAI = (HeliBeetle) groundedEnemyAI;
    }

    protected override void animate()
    {
        base.animate();
        
        attackAnimation(); // Added Attack Animation to the State Order Loop
    }

    public override void attackAnimation()
    {
        if(isAttacking()) nextAnimation = AttackAnimationString;
    }

    private bool isAttacking()
    {
        return heliBeetleAI.isAttacking();
    }

    public override bool isFalling()
    {
        return !heliBeetleAI.isGrounded();
    }
}
