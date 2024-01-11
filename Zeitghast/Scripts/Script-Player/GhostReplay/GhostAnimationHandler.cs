using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimationHandler : EntityAnimationHandler
{
    private const string PlayerIdleAnimationString = "PlayerIdle";

    public string animationToPlayNext;

    protected override void animate()
    {
        if (!string.IsNullOrWhiteSpace(nextAnimation))
        {
            nextAnimation = animationToPlayNext;
        }
        else
        {
            nextAnimation = PlayerIdleAnimationString;
        }

        base.animate();
    }
}
