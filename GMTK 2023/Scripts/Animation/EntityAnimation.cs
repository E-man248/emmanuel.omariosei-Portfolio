using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EntityAnimation : MonoBehaviour
{
    #region Animation Variables

    [Header("Animation")]
    private Animator entityAnimator;

    // Animation Sources:
    [SerializeField] private string currentAnimation;
    private Dictionary<string, float> allAnimationClipsToLength;
    [SerializeField] protected bool autoAnimate = true;
    private Func<bool> animationInterupt;

    // Untility Variables:
    
    //private float lastHealthValue;

    #endregion
    
    #region Animation Strings

    protected string nextAnimation;
    // private const string idleAnimationString = "PlayerIdle";

    #endregion

    #region UnityFunctions
    protected virtual void Awake()
    {
        entityAnimator = GetComponent<Animator>();
        allAnimationClipsToLength = new Dictionary<string, float>();
        
        autoAnimate = true;

        // Set up as the 'source' for animation transitions:

        // Ex: (DELETE UPON IMPLEMENTATION !!)
        /* 
            if (Player != null)
            {
                playerHealth = Player.GetComponent<Health>();
                playerInput = Player.GetComponent<PlayerInput>();
                playerRigidbody = Player.GetComponent<Rigidbody2D>();
            }
            else
            {   
                Debug.LogError("[playerAnimationHandler] No 'Player' Transform for " + name);
            }
        */

        // Set up as the definitions for animation interrupts: (Func<Bool>)

        // Ex: (DELETE UPON IMPLEMENTATION !!)
        /*
            // Animation String Definitions:
            dashAnimationInterupt = () =>
            {
                return weaponScript.weaponManager.dashingWithDashAttackWeapon();
            };
        */

        storeAnimationAllLengths();
    }

    protected virtual void Update()
    {
        if (autoAnimate)
        {
            // Mutually Exclusive Animations:
            animate();
        }
        else if (animationInterupt != null)
        {
            // Check for Interrupting Animations:
            checkAnimationInterupt();
        }

        // Non-Mutually Exclusive Animations:
        
    }

    #endregion

    #region Animation Booleans:
    
    // Boolean Functions that are used to dictate animation switching:

    // Ex: (DELETE UPON IMPLEMENTATION !!)
    /*
        public bool isCrouching()
        {
            return playerInput.isCrouching;
        }
    */

    #endregion

    protected virtual void animate()
    {
        // List of Mutually Exclusive Animations:

        // crouchAnimation();

        if (autoAnimate) changeAnimation(nextAnimation);
    }
    
    #region Mutually Exclusive Animations

    // Ex: (DELETE UPON IMPLEMENTATION !!)
    /*
        private void crouchAnimation()
        {
            if (isCrouching())
            {
                nextAnimation = crouchAnimationString;
            }
        }
    */
    #endregion

    #region Non-Mutually Exclusive Animations
    
    // Ex: (DELETE UPON IMPLEMENTATION !!)
    /*
        private void invincibilityAnimation()
        {
            if (playerHealth.invincible)
            {
                playerSpriteRenderer.material.color = transparentColor;
            }
        }
    */
    #endregion

    #region Animation Helper Functions
    protected void changeAnimation(string animation)
    {
        if (entityAnimator == null || entityAnimator.runtimeAnimatorController == null) return;

        currentAnimation = animation;

        if (!String.IsNullOrEmpty(animation) && !allAnimationClipsToLength.ContainsKey(animation))
        {
            Debug.LogWarning("[EntityAnimationHandler] Animation \"" + animation + "\" does not exist in " + entityAnimator.runtimeAnimatorController.ToString() + "!");
            return;
        }

        entityAnimator.speed = 1f;
        entityAnimator.Play(animation);
    }

    private void checkAnimationInterupt()
    {
        if (animationInterupt == null) return;

        if (animationInterupt())
        {
            StopCoroutine("turnOnAnimationSwitchOnFrame");
            CancelInvoke("turnOnAnimationSwitchOnFrame");
            turnOnAnimationSwitchOnFrame();
        }
    }

    public string getCurrentAnimation()
    {
        return currentAnimation;
    }

    private void storeAnimationAllLengths()
    {
        if (entityAnimator == null || entityAnimator.runtimeAnimatorController == null) return;

        AnimationClip[] allAnimationClips = entityAnimator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in allAnimationClips)
        {
            allAnimationClipsToLength.Add(clip.name, clip.length);
        }
    }

    public float getAnimationLength(string animation)
    {
        if (allAnimationClipsToLength.Count == 0) return -1;

        if (!String.IsNullOrEmpty(animation) && !allAnimationClipsToLength.ContainsKey(animation))
        {
            Debug.LogWarning("[EntityAnimationHandler] Animation \"" + animation + "\" does not exist in " + entityAnimator.runtimeAnimatorController.ToString() + "!");
            return -1f;
        }

        return allAnimationClipsToLength[animation];
    }

    protected void pauseAnimator()
    {
        entityAnimator.speed = 0f;
    }

    protected void playAnimationOnceFull(string animation, Func<bool> interuption = null)
    {
        if (entityAnimator == null || entityAnimator.runtimeAnimatorController == null) return;

        autoAnimate = false;
        entityAnimator.speed = 1f;
        entityAnimator.Play(animation);
        currentAnimation = animation;
        animationInterupt = interuption;

        StopCoroutine("turnOnAnimationSwitchOnFrame");
        CancelInvoke("turnOnAnimationSwitchOnFrame");
        Invoke("turnOnAnimationSwitchOnFrame", getAnimationLength(animation));
    }

    protected void playAnimationTillInterupt(string animation, Func<bool> interuption)
    {
        if (entityAnimator == null || entityAnimator.runtimeAnimatorController == null) return;

        if (interuption == null) return;

        autoAnimate = false;
        entityAnimator.speed = 1f;
        entityAnimator.Play(animation);
        currentAnimation = animation;
        animationInterupt = interuption;
    }

    private void turnOnAnimationSwitchOnFrame() { autoAnimate = true;}

    protected bool getAutoAnimate()
    {
        return autoAnimate;
    }

    protected void setAutoAnimate(bool toggle)
    {
        autoAnimate = toggle;
    }
    #endregion

}
