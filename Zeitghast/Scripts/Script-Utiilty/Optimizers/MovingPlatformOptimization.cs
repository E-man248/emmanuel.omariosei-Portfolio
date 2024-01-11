using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformOptimization : Optimizer
{
    private MovingPlatforms movingPlatform;
    protected override void Start()
    {
        base.Start();
        movingPlatform = GetComponent<MovingPlatforms>();

        if (movingPlatform == null) Debug.LogError(name + "could not find a movingPlatform script");
    }
    protected override void setInactive()
    {
        movingPlatform.toggleMovingPlatform(false);
    }

    protected override void setActive()
    {
        movingPlatform.toggleMovingPlatform(true);
    }
}
