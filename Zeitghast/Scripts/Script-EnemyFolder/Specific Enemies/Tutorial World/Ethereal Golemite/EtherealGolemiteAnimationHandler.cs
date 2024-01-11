using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealGolemiteAnimationHandler : GroundedEnemyAnimationHandler
{
    #region Animations Names
    protected string RunAnimationString;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions
        RunAnimationString = enemyName + "Run";
        #endregion
    }

    public bool isRunning()
    {
        return isWalking() && groundedEnemyAI.state.Equals(EnemyMovement.State.Pursuit);
    }

    protected override void walkAnimation()
    {
        if(isRunning())
        {
            nextAnimation = RunAnimationString;
        }
        else if (isWalking())
        {
            nextAnimation = WalkAnimationString;
        }
    }
}
