using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeTrackerUIAnimationHandler : EntityAnimationHandler
{
    #region Animation Strings

    public const string AppearAnimationString = "RealTimeTrackerUIAppear";
    public const string DisappearAnimationString = "RealTimeTrackerUIDisappear";
    public const string ShownAnimationString = "RealTimeTrackerUIShown";
    public const string HiddenAnimationString = "RealTimeTrackerUIHidden";

    #endregion

    private RealTimeTrackerUI realTimeTrackerUI;

    protected override void Awake()
    {
        base.Awake();

        if (realTimeTrackerUI == null)
        {
            realTimeTrackerUI = GetComponentInParent<RealTimeTrackerUI>();
            if (realTimeTrackerUI == null)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void PlayAppearAnimation()
    {
        if (getCurrentAnimation() == ShownAnimationString) return;

        playAnimationOnceFull(AppearAnimationString);
    }

    public void PlayDisappearAnimation()
    {
        if (getCurrentAnimation() == HiddenAnimationString) return;

        playAnimationOnceFull(DisappearAnimationString);
    }

    public void PlayShownAnimation()
    {
        playAnimationOnceFull(ShownAnimationString);
    }

    public void PlayHiddenAnimation()
    {
        playAnimationOnceFull(HiddenAnimationString);
    }

    #region Animation Cycle Functions

    protected override void animate()
    {
        AnimateDisplayActivity();

        base.animate();
    }

    private void AnimateDisplayActivity()
    {
        if (IsDisplayActive())
        {
            nextAnimation = ShownAnimationString;
        }
        else
        {
            nextAnimation = HiddenAnimationString;
        }
    }

    #endregion

    #region Animation Booleans

    private bool IsDisplayActive()
    {
        return realTimeTrackerUI.displayActive;
    }

    #endregion
}
