using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PortalAnimationHandler : EntityAnimationHandler
{
    private SelfDestruct selfDestruct = null;
    [Header("Portal Sounds")]
    [EventRef] public string PortalIn = null;
    [EventRef] public string PortalOut = null;


    private const string portalSpawnAnimation = "PortalSpawnAnimation";

    private void Start()
    {
        nextAnimation = portalSpawnAnimation;
        changeAnimation(portalSpawnAnimation);

        selfDestruct = transform.parent.GetComponent<SelfDestruct>();

        if (selfDestruct != null)
        {
            selfDestruct.lifeTime = getAnimationLength(getCurrentAnimation());
            selfDestruct.InitiateDestruct();
        }
    }

    //Triggered through animations
    public void playPortalInSound()
    {
        if (!string.IsNullOrEmpty(PortalIn))
        {
            RuntimeManager.PlayOneShot(PortalIn, transform.position);
        }
    } 
    
    //Triggered through animations
    public void playPortalOutSound()
    {
        if (!string.IsNullOrEmpty(PortalOut))
        {
            RuntimeManager.PlayOneShot(PortalOut, transform.position);
        }
    }
}
