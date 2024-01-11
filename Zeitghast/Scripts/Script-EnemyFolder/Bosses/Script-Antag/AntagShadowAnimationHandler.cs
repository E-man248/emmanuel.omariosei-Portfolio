using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntagShadowAnimationHandler : EntityAnimationHandler
{
    public const string VisibleAnimationString = "AntagShadowVisible";
    public const string AppearAnimationString = "AntagShadowAppear";
    public const string DisappearAnimationString = "AntagShadowDisappear";
    public const string HiddenAnimationString = "AntagShadowHidden";

    public void playAppearAnimation()
    {
        playAnimationOnceFull(AppearAnimationString);
    }

    public void playDisappearAnimation()
    {
        playAnimationOnceFull(DisappearAnimationString);
    }
    
    public void playHideAnimation()
    {
        playAnimationOnceFull(HiddenAnimationString);
    }
}
