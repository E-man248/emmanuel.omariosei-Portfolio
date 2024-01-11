using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAnimationHandler : StationaryEnemyAnimationHandler
{
    protected override void animate()
    {
        // List of Mutually Exclusive Animations:
        attackWindUpAnimation();
        stunAnimation();

        if (animationCycleEnabled) changeAnimation(nextAnimation);
    }
}
