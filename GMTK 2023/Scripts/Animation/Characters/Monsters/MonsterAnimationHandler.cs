using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationHandler : CharacterAnimationHandler
{
    protected string performActionNightAnimationString;
    protected string walkNightAnimationString;
    protected string idleNightAnimationString;
    protected string deathNightAnimationString;

    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions

        performActionNightAnimationString = performActionAnimationString + "Night";
        performActionAnimationString = performActionAnimationString + "Day";
        walkNightAnimationString = walkAnimationString + "Night";
        walkAnimationString = walkAnimationString + "Day";
        idleNightAnimationString = idleAnimationString + "Night";
        idleAnimationString = idleAnimationString + "Day";
        deathAnimationString = deathAnimationString + "Day";
        deathNightAnimationString = deathAnimationString + "Night";

        #endregion
    }

    #region Animation Booleans:

    protected bool isNight()
    {
        return GameStateManger.Instance.currentTime == GameStateManger.DayNightState.Night;
    }

    #endregion

    #region Mutually Exclusive Animations

    protected override void walkAnimation()
    {
        if (isWalking())
        {
            if (isNight())
            {
                nextAnimation = walkNightAnimationString;
            }
            else
            {
                nextAnimation = walkAnimationString;
            }
        }
    }

    protected override void idleAnimation()
    {
        if (isNight())
        {
            nextAnimation = idleNightAnimationString;
        }
        else
        {
            nextAnimation = idleAnimationString;
        }
    }

    #endregion

    
    public override void playPerformAction()
    {
        if (characterController != null && characterController.charcterIsActive && !isWalking())
        {
            if (isNight())
            {
                playAnimationOnceFull(performActionNightAnimationString);
            }
            else
            {
                playAnimationOnceFull(performActionAnimationString);
            }
        }
    }
}
