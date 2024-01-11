using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TransitionAnimationHandler : EntityAnimation
{
    public void playTransition(TransitionType transition)
    {
        string transitionString = translateTransitionType(transition);        
        playAnimationOnceFull(transitionString);
    }

    protected override void animate()
    {
        nextAnimation = "None";
        base.animate();
    }

    public string translateTransitionType(TransitionType transition)
    {
        return transition.ToString();
    }

    public enum TransitionType
    {
        None,
        FadeIn,
        FadeOut,
        CircleCutIn,
        CircleCutOut,
        SkullCutIn,
        SkullCutOut,
        LoadingScreen,
    };
}