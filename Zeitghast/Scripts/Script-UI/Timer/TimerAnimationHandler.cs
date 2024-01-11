using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerAnimationHandler : EntityAnimationHandler
{
    public const string IdleAnimationString = "TimerIdle";
    public const string LevelStartAnimationString = "TimerLevelStart";
    public const string PlayerRespawnAnimationString = "TimerPlayerRespawn";
    protected override void animate()
    {
        // List of Mutually Exclusive Animations:

        idleAnimation();

        base.animate();
    }

    private void idleAnimation()
    {
        nextAnimation = IdleAnimationString;
    }

    public void playLevelStartAnimation()
    {
        playAnimationOnceFull(LevelStartAnimationString);
    }

    public void playIdleAnimation()
    {
        playAnimationOnceFull(IdleAnimationString);
    }

    public void playPlayerRespawnAnimation()
    {
        playAnimationOnceFull(PlayerRespawnAnimationString);
    }
}
