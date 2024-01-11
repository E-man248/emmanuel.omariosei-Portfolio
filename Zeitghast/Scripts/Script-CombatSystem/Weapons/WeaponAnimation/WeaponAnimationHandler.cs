using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponAnimationHandler : EntityAnimationHandler
{
    #region Animation Variables

    [Header("Weapon Animation")]
    public Transform WeaponTransform;
    protected Rigidbody2D playerRigidbody;
    protected string weaponName;

    // Utility Variables:
    [Header("Animation Settings")]
    [SerializeField] protected float stillIdleAnimationDuration = 0.7f;
    protected float stillIdleAnimationTimer;

    // Player Animation Sources:
    protected Weapon weaponScript;
    [HideInInspector] public SpriteRenderer weaponSpriteRenderer;

    #endregion

    #region Sound Variables
    //Sound
    
    //Fmod
    #endregion
    
    #region Animations
    public string floatingIdleAnimationString;
    public string stillIdleAnimationString;
    public string attackAnimationString;
    public string dashAttackAnimationString;
    public string reloadAnimationString;
    #endregion

    #region Animation Interupts
    public Func<bool> dashAnimationInterupt;
    #endregion

    #region UnityFunctions
    protected override void Awake()
    {
        #region Animation Interupt Definitions
        dashAnimationInterupt = () =>
        {
            return weaponScript.weaponManager.dashingWithDashAttackWeapon();
        };
        #endregion

        base.Awake();

        weaponSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        //Sets up the Player as the 'source' for animation transitions
        if (transform.parent.tag == "Weapon")
        {
            WeaponTransform = transform.parent;
        }

        if (WeaponTransform != null)
        {
            weaponScript = WeaponTransform.GetComponent<Weapon>();
            weaponName = weaponScript.weaponName.Replace(" ", "");
        }
        else
        {   
            Debug.LogError("[weaponAnimationHandler] No 'Weapon' Transform for " + name);
        }

        #region Animation String Definitions
        floatingIdleAnimationString = weaponName + "FloatingIdle";
        stillIdleAnimationString = weaponName + "StillIdle";
        attackAnimationString = weaponName + "Attack";
        dashAttackAnimationString = weaponName + "DashAttack";
        reloadAnimationString = weaponName + "Reload";
        #endregion
        
        stillIdleAnimationTimer = stillIdleAnimationDuration;
    }

    protected override void Update()
    {   
        if (Timer.gamePaused) return;

        if (animationCycleEnabled)
        {
            if (!DisableAutoAnimate)
            {
                if (weaponInactive())
                {
                    inactiveAnimation();
                }
                else
                {
                    // Normal Weapon Animation:
                    base.Update();
                }
            }
        }
        else if (animationInterupt != null)
        {
            // Check for Interrupting Animations:
            checkAnimationInterupt();
        }

        // Non-Mutually Exclusive Animations:
        playerHandGlowAnimation();
        inWorldGlowGraphicsAnimation();
    }
    #endregion

    #region Animation Booleans:
    public bool isDashAttacking()
    {
        return weaponScript.weaponManager != null && weaponScript.weaponManager.dashingWithDashAttackWeapon();
    }

    #endregion

    #region Animations
    protected override void animate()
    {
        stillIdleAnimationTimer -= Time.deltaTime;

        idleAnimation();
        dashAttackAnimation();

        base.animate();
    }

    public bool weaponInactive()
    {
        // Weapon should not animate when:
        if (weaponScript.weaponManager == null)
        {
            return true;
        }
        else if (weaponScript.weaponManager.currentWeapon != weaponScript)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void inactiveAnimation()
    {
        changeAnimation(stillIdleAnimationString);
    }

    protected void playerHandGlowAnimation()
    {
        if (weaponInactive()) return;
    }

    protected void inWorldGlowGraphicsAnimation()
    {
        if (weaponInactive()) return;
    }

    public void playAttackAnimation()
    {
        stillIdleAnimationTimer = stillIdleAnimationDuration;
        playAnimationOnceFull(attackAnimationString, dashAnimationInterupt);
    }

    protected void idleAnimation()
    {
        nextAnimation = floatingIdleAnimationString;
        if (stillIdleAnimationTimer > 0f)
        {
            nextAnimation = stillIdleAnimationString;
        }
    }

    protected void dashAttackAnimation()
    {
        if (isDashAttacking())
        {
            stillIdleAnimationTimer = stillIdleAnimationDuration;
            nextAnimation = dashAttackAnimationString;
        }
    }

    public void playReloadAnimation()
    {
        if (!getCurrentAnimation().Equals(attackAnimationString))
        {
            playAnimationOnceFull(reloadAnimationString);
        }
    }
    #endregion

}