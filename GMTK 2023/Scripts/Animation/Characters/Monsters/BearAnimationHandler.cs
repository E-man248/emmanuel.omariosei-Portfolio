using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAnimationHandler : MonsterAnimationHandler
{
    private string walkAltAnimationString;
    private string walkNightAltAnimationString;

    private bool walkAltAnimationToggle = false;

    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions

        walkAltAnimationString = walkAnimationString + "Alt";
        walkNightAltAnimationString = walkNightAnimationString + "Alt";

        #endregion
    }

    #region Mutually Exclusive Animations

    protected override void walkAnimation()
    {
        if (isWalking())
        {
            if (!getCurrentAnimation().Contains("BearWalk"))
            {
                walkAltAnimationToggle = !walkAltAnimationToggle;
            }

            if (isNight())
            {
                if (walkAltAnimationToggle)
                {
                    nextAnimation = walkNightAltAnimationString;
                }
                else
                {
                    nextAnimation = walkNightAnimationString;
                }
            }
            else
            {
                if (walkAltAnimationToggle)
                {
                    nextAnimation = walkAltAnimationString;
                }
                else
                {
                    nextAnimation = walkAnimationString;
                }
            }
        }
    }

    #endregion
}
