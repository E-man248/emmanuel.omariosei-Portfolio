using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeNotificationAnimationHandler : EntityAnimationHandler
{
    private const string InactiveAnimationString = "TimeNotificationInactive";
    private const string NotifyAnimationString = "TimeNotificationNotify";

    protected override void animate()
    {
        nextAnimation = InactiveAnimationString;

        base.animate();
    }

    public void PlayTimeNotification()
    {
        playAnimationOnceFull(NotifyAnimationString);
    }
}
