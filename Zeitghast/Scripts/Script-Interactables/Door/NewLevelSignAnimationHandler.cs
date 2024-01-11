using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLevelSignAnimationHandler : EntityAnimationHandler
{
    #region Animation Strings
    public const string ActiveAnimationString = "NewLevelSignActive";
    public const string InactiveAnimationString = "NewLevelSignInactive";

    #endregion

    // Utility Variables:
    private LevelCompleteIndicator levelCompleteIndicator;

    private void Start()
    {
        levelCompleteIndicator = GetComponentInParent<LevelCompleteIndicator>();
        
        if (levelCompleteIndicator == null)
        {
            Debug.LogError("A level complete indicator is not available in the parent of " + name + "!");
        }
    }

    public bool IsActive()
    {
        return !levelCompleteIndicator.LevelComplete;
    }

    protected override void animate()
    {
        if (IsActive())
        {
            nextAnimation = ActiveAnimationString;
        }
        else
        {
            nextAnimation = InactiveAnimationString;
        }

        base.animate();
    }
}
