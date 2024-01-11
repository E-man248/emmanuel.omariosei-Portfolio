using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TransitionAnimationHandler : EntityAnimationHandler
{
    public void playTransition(TransitionType transition, Func<bool> interuption)
    {
        string transitionString = translateTransitionType(transition);        
        playAnimationTillInterupt(transitionString, interuption);
    }

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
}
