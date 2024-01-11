using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntagBossAnimationHandler : BossAnimationHandler
{
    [Header("Antag Boss Animation Handler Settings")]

    [Header("Laser Cannon Graphics Settings")]
    
    [SerializeField] private Transform normalLaserCannonGraphic;
    [SerializeField] private Transform aimingLaserCannonGraphic;

    #region Animation Strings

    public const string TeleportInAnimationString = "DrAntagTeleportIn";
    public const string TeleportOutAnimationString = "DrAntagTeleportOut";

    public const string AimedLaserAttackWindupAnimationString = "DrAntagAimedLaserAttackWindup";
    public const string AimedLaserAttackAimingAnimationString = "DrAntagAimedLaserAttackAiming";
    public const string AimedLaserAttackEaseInAnimationString = "DrAntagAimedLaserAttackEaseIn";
    public const string AimedLaserAttackShootingAnimationString = "DrAntagAimedLaserAttackShooting";
    public const string AimedLaserAttackEaseOutAnimationString = "DrAntagAimedLaserAttackEaseOut";

    public const string RockThrow_WindupAnimationString = "DrAntagRockThrow_Windup";
    public const string RockThrowingAnimationString = "DrAntagRockThrowing";

    public const string CreateTimeRiftsSummoningAnimationString = "DrAntagCreateTimeRiftsSummoning";


    public const string FlyingStompWindUpAnimationString = "DrAntagFlyingStompWindUp";
    public const string FlyingStompFlyUpAnimationString = "DrAntagFlyingStompFlyUp";
    public const string FlyingStompTrackingAnimationString = "DrAntagFlyingStompTracking";
    public const string FlyingStompFlyDownAnimationString = "DrAntagFlyingStompFlyDown";
    public const string FlyingStompLandingAnimationString = "DrAntagFlyingStompLanding";
    public const string FlyingStompCoolDownAnimationString = "DrAntagFlyingStompCoolDown";

    [Header("Hurt Flash Animation")]
    [SerializeField] protected bool hurtFlashEnabled = true;
    public float FlashRate = 0.3f;
    [Range(0, 1)] public float FlashIntensity = 0.62f;
    public float flashDuration = 0.1f;
    protected float flashDurationTimer = 1f;
    protected SpriteRenderer[] sprites;

    protected bool flashState = false;
    protected bool StartFlashing = false;

    protected float flashValue;
    protected int storedHealth;

    #endregion

    // Utilities:
    private AntagBoss antagBoss;
    private BossHealth antagHealth;

    protected override void Awake()
    {
        base.Awake();

        antagBoss = (AntagBoss) boss;
        antagHealth = GetComponentInParent<BossHealth>();

        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        // Non-Mutually Exclusive Animations:
        FlashAnimationUpdate();
    }

    #region Mutually Exclusive Animations:

    protected override void animateAttack()
    {
        base.animateAttack();

        if (antagBoss?.currentAntagBossAttackState?.state == null) return;

        switch(antagBoss.currentAntagBossAttackState.state)
        {
            case AntagBoss.AttackState.RockThrow:
                nextAnimation = IdleAnimationString;
            break;

            case AntagBoss.AttackState.AimedLaserAttack:
                nextAnimation = IdleAnimationString;
            break;

            case AntagBoss.AttackState.CreateTimeRifts:

            break;

            case AntagBoss.AttackState.FlyStomp:
                nextAnimation = IdleAnimationString;
            break;
        }        
    }

    #region Teleport Animations
    public void playTeleportInAnimation()
    {
        playAnimationOnceFull(TeleportInAnimationString);
    }

    public void playTeleportOutAnimation()
    {
        playAnimationOnceFull(TeleportOutAnimationString);
    }

    #endregion

    # region Aimed laser Attack Animation
    public void playAimedLaserAttackWindupAnimation()
    {
        Func<bool> AimedLaserAttackIsWindup = () => antagBoss.aimedLaserAttackWindUpTimer < 0;

        playAnimationOnceFull(AimedLaserAttackWindupAnimationString, AimedLaserAttackIsWindup);
    }

    public void playAimedLaserAttackAimingAnimation()
    {
        Func<bool> AimedLaserAttackIsAiming = () => antagBoss.aimedLaserAttackAimingTimer < 0;

        playAnimationTillInterupt(AimedLaserAttackAimingAnimationString, AimedLaserAttackIsAiming );
    }

    public void playAimedLaserAttackEaseInAnimation()
    {
        Func<bool> AimedLaserAttackIsEaseIn = () => antagBoss.aimedLaserAttackEaseInTimer < 0;

        playAnimationTillInterupt(AimedLaserAttackEaseInAnimationString, AimedLaserAttackIsEaseIn );
    }

    public void playAimedLaserAttackShootingAnimation()
    {
        Func<bool> AimedLaserAttackIsShooting = () => antagBoss.aimedLaserAttackShootingTimer < 0;

        playAnimationTillInterupt(AimedLaserAttackShootingAnimationString , AimedLaserAttackIsShooting );
    }
    

    public void playAimedLaserAttackEaseOutAnimation()
    {
        Func<bool> AimedLaserAttackIsEaseOut = () => antagBoss.aimedLaserAttackEaseOutTimer < 0;

        playAnimationTillInterupt(AimedLaserAttackEaseOutAnimationString, AimedLaserAttackIsEaseOut );
    }
    #endregion

    # region Rock Throw Animation
    public void playRockThrow_WindupAnimation()
    {
        Func<bool> RockThrowingIsWindup = () => antagBoss.rockThrow_WindUpTimer < 0;

        playAnimationTillInterupt(RockThrow_WindupAnimationString, RockThrowingIsWindup );
    }

    public void playRockThrowingAnimation()
    {
        Func<bool> IsRockThrowing = () => antagBoss.rockThrowingTimer < 0;

        playAnimationTillInterupt(RockThrowingAnimationString, IsRockThrowing );
    }

    #endregion

    #region Create Time Rifts Animation

    public void playCreateTimeRiftsSummoningAnimation()
    {
        Func<bool> CreateTimeRiftsIsSummoning = () => antagBoss.createTimeRiftsSummoningTimer < 0;

        playAnimationTillInterupt(CreateTimeRiftsSummoningAnimationString, CreateTimeRiftsIsSummoning );
    }

    #endregion

    #region Flying Stomp Animation
    public void playFlyingStompWindUpAnimation()
    {
        Func<bool> FlyingStompWindUpComplete = () => antagBoss.FlyingStompWindUpTimer < 0;

        playAnimationTillInterupt(FlyingStompWindUpAnimationString, FlyingStompWindUpComplete);
    }

    public void playFlyingStompFlyUpAnimation()
    {
        Func<bool> FlyingStompFlyUpComplete = () => antagBoss.flyingStompFlyUpComplete;

        playAnimationTillInterupt(FlyingStompFlyUpAnimationString, FlyingStompFlyUpComplete);
    }

    public void playFlyingStompTrackingAnimation()
    {
        Func<bool> FlyingStompTrackingComplete = () => antagBoss.FlyingStompTrackingTimer < 0;

        playAnimationTillInterupt(FlyingStompTrackingAnimationString, FlyingStompTrackingComplete);
    }

    public void playFlyingStompFlyDownAnimation()
    {
        Func<bool> FlyingStompFlyDownComplete = () => antagBoss.flyingStompFlyDownComplete;

        playAnimationTillInterupt(FlyingStompFlyDownAnimationString, FlyingStompFlyDownComplete);
    }

    public void playFlyingStompLandingAnimation()
    {
        Func<bool> FlyingStompLandingComplete = () => antagBoss.FlyingStompLandingTimer < 0;

        playAnimationTillInterupt(FlyingStompLandingAnimationString, FlyingStompLandingComplete);
    }

    public void playFlyingStompCoolDownAnimation()
    {
        Func<bool> FlyingStompCoolDownComplete = () => antagBoss.FlyingStompCoolDownTimer < 0;

        playAnimationTillInterupt(FlyingStompCoolDownAnimationString, FlyingStompCoolDownComplete);
    }


    #endregion

    #region Laser Cannon Graphics

    public void setLaserCannonGraphicState(LaserCannonGraphicState state)
    {
        if (state == LaserCannonGraphicState.Aiming)
        {
            normalLaserCannonGraphic.gameObject.SetActive(false);
            aimingLaserCannonGraphic.gameObject.SetActive(true);
        }
        else
        {
            normalLaserCannonGraphic.gameObject.SetActive(true);
            aimingLaserCannonGraphic.gameObject.SetActive(false);
        }
    }

    public enum LaserCannonGraphicState
    {
        Aiming,
        Normal
    }

    #endregion

    #region Non-Mutually Exclusive Animations

    protected virtual void FlashAnimationUpdate()
    {
        if (!hurtFlashEnabled) return;

        if (antagHealth.health != storedHealth)
        {
            storedHealth = antagHealth.health;
            StartFlashing = true;
            flashDurationTimer = flashDuration;
            flashValue = flashDuration;
        }

        if (StartFlashing)
        {
            flashSprite();
        }

        if (flashDurationTimer <= 0f && !flashState)
        {
            StartFlashing = false;

            //reset 
            for (int i = 0; i < sprites.Length; i++)
            {
                Color newColor = new Color(sprites[i].color.r, 1f, 1f);
                sprites[i].color = newColor;
            }
        }
        flashDurationTimer -= Time.deltaTime;
    }

    protected virtual void flashSprite()
    {
        //Handle timer
        if (flashState)
        {
            if (flashValue / FlashRate >= 1)
            {
                flashState = false;
            }
            flashValue += Time.deltaTime;
        }
        else
        {
            if (flashValue / FlashRate <= FlashIntensity)
            {
                flashState = true;
            }
            flashValue -= Time.deltaTime;
        }

        //Flash all objects

        for (int i = 0; i < sprites.Length; i++)
        {
            float newGValue = flashValue / FlashRate;
            float newBValue = flashValue / FlashRate;
            Color newColor = new Color(sprites[i].color.r, newGValue, newBValue);
            sprites[i].color = newColor;
        }

    }

    #endregion
    
    #endregion
}
