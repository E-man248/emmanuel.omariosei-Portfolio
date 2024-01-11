using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandler : EntityAnimation
{
    protected string performActionAnimationString = "PerformAction";
    protected string walkAnimationString = "Walk";
    protected string idleAnimationString = "Idle";
    protected string deathAnimationString = "Death";

    protected TileBasedMovment tileBasedMovmentScript;
    protected CharacterController characterController;
    protected Health healthScript;


    public string CharacterName;

    [Header("Settings")]
    [SerializeField] private bool flipOrientation = false;

    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions
        
        performActionAnimationString = CharacterName + performActionAnimationString;
        walkAnimationString = CharacterName + walkAnimationString;
        idleAnimationString = CharacterName + idleAnimationString;
        deathAnimationString = CharacterName + deathAnimationString;

        #endregion
    }

    protected virtual void Start()
    {
        tileBasedMovmentScript = GetComponentInParent<TileBasedMovment>();
        healthScript = GetComponentInParent<Health>();
        characterController = GetComponentInParent<CharacterController>();

        if (healthScript != null)
        {
            healthScript.deathEvent.AddListener(playDeathAnimation);
        }

        //GameStateManger.Instance.PlayerActionPerformedEvent.AddListener(playPerformAction);
    }

    protected void OnEnable()
    {
        healthScript?.deathEvent?.AddListener(playDeathAnimation);
        GameStateManger.Instance.PlayerActionPerformedEvent.AddListener(playPerformAction);
    }
    protected void OnDisable()
    {
        healthScript?.deathEvent?.RemoveListener(playDeathAnimation);
        GameStateManger.Instance.PlayerActionPerformedEvent.RemoveListener(playPerformAction);
    }
    protected void OnDestroy()
    {
        healthScript?.deathEvent?.RemoveListener(playDeathAnimation);
        GameStateManger.Instance.PlayerActionPerformedEvent.RemoveListener(playPerformAction);
    }

    protected override void Update()
    {
        base.Update();

        // Non-Mutually Exclusive Animations:
        orientate();
    }

    protected override void animate()
    {
        // List of Mutually Exclusive Animations:

        idleAnimation();
        walkAnimation();

        base.animate();
    }

    #region Animation Booleans:

    protected bool isWalking()
    {
        return tileBasedMovmentScript != null && tileBasedMovmentScript.hasNotReachedTargetPosition();
    }

    #endregion


    #region Mutually Exclusive Animations

    protected virtual void idleAnimation()
    {
        nextAnimation = idleAnimationString;
    }

    protected virtual void walkAnimation()
    {
        if (isWalking())
        {
            nextAnimation = walkAnimationString;
        }
    }

    protected virtual void playDeathAnimation()
    {
        playAnimationOnceFull(deathAnimationString);
    }

    #endregion

    #region Non-Mutually Exclusive Animations
    private void orientate()
    {
        // Correct Orientation:
        float lookDirectionCorrection = 0f;

        if (flipOrientation)
        {
            lookDirectionCorrection = -1f;
        }
        else
        {
            lookDirectionCorrection = 1f;
        }

        if (tileBasedMovmentScript.lastMoveDirection.x == (1 * lookDirectionCorrection))
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else if (tileBasedMovmentScript.lastMoveDirection.x == (-1 * lookDirectionCorrection))
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }
    #endregion

    public virtual void playPerformAction()
    {
        if (characterController != null && characterController.charcterIsActive && !isWalking())
        {
            playAnimationOnceFull(performActionAnimationString);
        }
    }
}
