using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformAnimationHandler : EntityAnimationHandler
{
    private string IdleAnimationString = "MovingPlatformIdle";
    private string MovingAnimationString = "MovingPlatformMoving";

    private MovingPlatforms movingPlatform;

    [Header("Moving Platform Animation Handler Settings:")]
    [SerializeField] private string entityName;
    [SerializeField] private bool canOrientate = false;
    [SerializeField] private bool flipOrientation = false;
    private Vector2 lookDirection = Vector2.zero;
    
    protected override void Awake()
    {
        base.Awake();

        IdleAnimationString = entityName + "Idle";
        MovingAnimationString = entityName + "Moving";
    }

    private void Start()
    {
        movingPlatform = GetComponentInParent<MovingPlatforms>();
    }

    protected override void Update()
    {
        base.Update();

        // Non-Mutually Exclusive Animations:
        orientateEntity();
    }

    #region Mutually Exclusive Animations

    public bool IsMoving()
    {
        return movingPlatform.isMoving();
    }

    #endregion

    #region Mutually Exclusive Animations

    protected override void animate()
    {
        idleAnimation();
        movingAnimation();

        base.animate();
    }

    private void idleAnimation()
    {
        nextAnimation = IdleAnimationString;
    }

    private void movingAnimation()
    {
        if (IsMoving())
        {
            nextAnimation = MovingAnimationString;
        }
    }

    #endregion

    #region Non-Mutually Exclusive Animations
    protected virtual void orientateEntity()
    {
        if (!canOrientate)
        {
            return;
        }

        //Checking to correct the orientation if it's flipped
        float lookDirectionCorrection = 0f;

        if (flipOrientation)
        {
            lookDirectionCorrection = -1f;
        }
        else
        {
            lookDirectionCorrection = 1f;
        }

        lookDirection = movingPlatform.getMoveDirection();

        if (lookDirection.x == (1 * lookDirectionCorrection))
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (lookDirection.x == (-1 * lookDirectionCorrection))
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        #endregion
    }
}
