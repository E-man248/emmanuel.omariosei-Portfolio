using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSaurusAnimationHandler : GroundedEnemyAnimationHandler
{
    LaserSaurus laserSaurusAI;

    protected override void Start()
    {
        base.Start();

        laserSaurusAI = (LaserSaurus) groundedEnemyAI;
    }

    public bool isAttacking()
    {
        return laserSaurusAI.isAttacking();
    }

    protected override void animate()
    {
        base.animate();
        attackAnimation();
    }

    public override void attackAnimation()
    {
        if(isAttacking())
        {
            nextAnimation = AttackAnimationString;
        }
    }
}
