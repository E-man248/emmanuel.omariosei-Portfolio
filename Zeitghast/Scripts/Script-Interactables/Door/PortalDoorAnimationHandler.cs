using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoorAnimationHandler : EntityAnimationHandler
{
    #region Animation Strings

    private const string activeAnimationString = "DoorActive";
    private const string inactiveAnimationString = "DoorInactive";
    private const string hiddenAnimationString = "DoorHidden";

    #endregion

    private PortalDoor portalDoor;
    private bool activePortalAnimationIsPlaying;

    protected override void Awake()
    {
        base.Awake();

        portalDoor = GetComponentInParent<PortalDoor>();

        DisableAutoAnimate = true;
    }

    private void Start()
    {
        // Update animation with portal door:
        activePortalAnimationIsPlaying = PortalActive();
        
        if (PortalActive())
        {
            playAnimationOnceFull(activeAnimationString);
        }
        else
        {
            playAnimationOnceFull(hiddenAnimationString);
        }
    }

    protected override void Update()
    {
        if (Timer.gamePaused) return;

        base.Update();
        
        UpdatePortalDoorAnimation();
    }

    #region Animation Booleans

    public bool PortalActive()
    {
        return portalDoor.IsActive();
    }

    public bool PortalInactive()
    {
        return !portalDoor.IsActive();
    }

    #endregion

    private void UpdatePortalDoorAnimation()
    {
        if (PortalActive() && !activePortalAnimationIsPlaying)
        {
            playAnimationOnceFull(activeAnimationString);
            activePortalAnimationIsPlaying = true;
        }
        
        if (PortalInactive() && activePortalAnimationIsPlaying)
        {
            playAnimationOnceFull(inactiveAnimationString);
            activePortalAnimationIsPlaying = false;
        }
    }
}
