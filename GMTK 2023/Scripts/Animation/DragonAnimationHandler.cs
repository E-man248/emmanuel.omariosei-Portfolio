using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimationHandler : EntityAnimation
{
    protected string suspicionAnimationString = "DragonSuspicion";

    [Header("Settings")]
    public float maxSuspicionAnimations = 5f;

    protected override void Awake()
    {
        base.Awake();

        nextAnimation = suspicionAnimationString + "1";
    }

    private void Start()
    {
        playAnimationOnceFull(suspicionAnimationString + "1");
        SuspicionMeter.instance.suspicionMeterChangeEvent.AddListener(playSuspicionAnimation);
    }
    protected void OnEnable()
    {
        SuspicionMeter.instance?.suspicionMeterChangeEvent.AddListener(playSuspicionAnimation);
    }
    protected void OnDisable()
    {
        SuspicionMeter.instance.suspicionMeterChangeEvent.RemoveListener(playSuspicionAnimation);
    }
    protected void OnDestroy()
    {
        SuspicionMeter.instance.suspicionMeterChangeEvent.RemoveListener(playSuspicionAnimation);
    }

    protected override void animate()
    {
        // List of Mutually Exclusive Animations:

        idleAnimation();

        base.animate();
    }

    #region Mutually Exclusive Animations

    protected virtual void idleAnimation()
    {
        nextAnimation = getCurrentAnimation();
    }

    #endregion

    protected void playSuspicionAnimation(int suspicionLevel)
    {
        int animationNumber = Mathf.FloorToInt(((float)suspicionLevel / SuspicionMeter.instance.suspicionCap) * maxSuspicionAnimations) + 1;

        string animationName = suspicionAnimationString + animationNumber;

        playAnimationOnceFull(animationName);
    }
}
