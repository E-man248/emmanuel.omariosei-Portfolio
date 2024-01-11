using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerOnHitReciever : OnHitReciever
{
    // Utility Variables:
    private float initialPlayerGravity;

    // Stun Variables 
    public int maxStunMashAmount;
    public float stunMashPerSec;
    private float stunMashtimer;
    [HideInInspector] public int currentStunMashAmount;
    private bool playerHasSlowEffect;

    private void Start()
    {
        PlayerInput targetMovement = GetComponent<PlayerInput>();
        initialPlayerGravity = targetMovement.entityRigidbody.gravityScale;
        
        subscribeToEvents();
    }

    protected void OnEnable()
    {
        subscribeToEvents();
    }

    protected void OnDisable()
    {
        unsubscribeToEvents();
    }

    protected void OnDestroy()
    {
        unsubscribeToEvents();
    }

    #region Event Functions

    private void subscribeToEvents()
    {
        PlayerInfo.PlayerDeathEvent.AddListener(OnPlayerDeath);
    }

    private void unsubscribeToEvents()
    {
        PlayerInfo.PlayerDeathEvent.RemoveListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        EffectImunity = true;
    }

    #endregion

    #region Apply Effect
    public override void applySlowEffect(Slow effect)
    {
        base.applySlowEffect(effect);
        effect.refreshOnHit = false;

        PlayerInput targetMovement = GetComponent<PlayerInput>();

        if (effect.effectTimer > 0 && !effect.slowApplied)
        {
            effect.slowAmount = -targetMovement.addToVariableSpeed(-effect.slowAmount);
            effect.slowApplied = true;
        }

        // Visual Effect:
        if (effect.VisualEffect != null && !effect.visualInstantiated && !playerHasSlowEffect)
        {
            effect.visualInstantiated = true;
            playerHasSlowEffect = true;
            playerAnimationHandler spriteRenderer = GetComponentInChildren<playerAnimationHandler>();
            GameObject particleEffect = Instantiate(effect.VisualEffect, spriteRenderer.transform);
            SelfDestruct selfDestruct = particleEffect.GetComponent<SelfDestruct>();
            effect.VisualEffectReference = selfDestruct;
            selfDestruct.lifeTime = effect.effectDuration;

            selfDestruct.InitiateDestruct();
        }
    }
    public override void applyStunnedEffect(Stuned effect)
    {
        base.applyStunnedEffect(effect);
        PlayerInput targetMovement = GetComponent<PlayerInput>();
        Collider2D targetCollider = targetMovement.entityCollider;
        WeaponManager targetWeaponManager = GetComponentInChildren<WeaponManager>();

        stunMashtimer += Time.deltaTime;

        /*                              Stun Mashing
             *                              
                Reads for any input from the player and add to the currentStunMashAmount.
                if currentStunMashAmount == maxStunMashAmount you get unstunned 
            */
        if (Input.anyKeyDown && effect.effectTimer > 0f && stunMashtimer >= stunMashPerSec)
        {
            stunMashtimer = 0;
            currentStunMashAmount += 1;
            // Stun Mash Sound
            if (effect.StunBreakSound != null)
            {
                RuntimeManager.PlayOneShot(effect.StunBreakSound, transform.position);
            }
            
            if (currentStunMashAmount >= maxStunMashAmount)
            {
                effect.effectTimer = 0f;
                currentStunMashAmount = 0;
                if(effect.visualInstantiated)
                {
                    effect.VisualEffectReference.Destruct();
                }
            }
        }

        if (effect.effectTimer > 0f)
        {
            targetMovement.entityRigidbody.gravityScale = effect.gravityEffect;
            
            if (effect.disableAttacking)
            {
                targetWeaponManager.canAim = false;
            }

            if (effect.disableMovement)
            {
                targetMovement.stunned = true;
                targetMovement.setControlsDisable(true);
                targetMovement.dashReset();
                targetMovement.crouchReset();
            }
        }

        // Visual Effect:
        if (effect.VisualEffect != null && !effect.visualInstantiated)
        {
            effect.visualInstantiated = true;
            currentStunMashAmount = 0;
            playerAnimationHandler spriteRenderer = GetComponentInChildren<playerAnimationHandler>();
            GameObject particleEffect = Instantiate(effect.VisualEffect, spriteRenderer.transform);
            SelfDestruct selfDestruct = particleEffect.GetComponent<SelfDestruct>();
            effect.VisualEffectReference = selfDestruct;
            CapsuleCollider2D iceCollider2D = particleEffect.GetComponent<CapsuleCollider2D>();

            if (targetCollider != null && targetCollider.bounds.size.x / 2f > targetCollider.bounds.size.y / 2f)
            {
                particleEffect.transform.localScale = new Vector3(targetCollider.bounds.size.x / 2f + effect.VisualScaleOffest.x, targetCollider.bounds.size.x / 2f + effect.VisualScaleOffest.x, particleEffect.transform.localScale.z);
            }
            else
            {
                particleEffect.transform.localScale = new Vector3(targetCollider.bounds.size.y / 2f + effect.VisualScaleOffest.y, targetCollider.bounds.size.y / 2f + effect.VisualScaleOffest.y, particleEffect.transform.localScale.z);
            }

            StunEffectAnimationHandler stunEffectAnimationHandler = particleEffect.GetComponent<StunEffectAnimationHandler>();
            if (stunEffectAnimationHandler != null)
            {
                stunEffectAnimationHandler.playerOnHitReciever = this;
                stunEffectAnimationHandler.effectName = effect.effectName;
            }

            selfDestruct.lifeTime = effect.effectDuration;
            selfDestruct.InitiateDestruct();
        }
    }

    public override void applyDamageOverTimeEffect(DamageOverTime effect)
    {
        base.applyDamageOverTimeEffect(effect);
        if (effect.damageTickRateTimer < 0f)
        {
            Health targetHealth = GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.changeHealth(-effect.damageAmount, effect.name);
                effect.damageTickRateTimer = effect.damageTickRate;
            }

            // Visual Effect:
            if (effect.VisualEffect != null)
            {
                playerAnimationHandler spriteRenderer = GetComponentInChildren<playerAnimationHandler>();
                GameObject particleEffect = Instantiate(effect.VisualEffect, spriteRenderer.transform);
            }
        }
        effect.damageTickRateTimer -= Time.deltaTime;
    }
    #endregion

    #region Remove Effect

    public override void removeSlowEffect(Slow effect) 
    {
        if (effect.slowApplied)
        {
            PlayerInput targetMovement = GetComponent<PlayerInput>();
            targetMovement.addToVariableSpeed(effect.slowAmount);
            effect.slowApplied = false;
        }

        // Visual Effect:
        if (effect.visualInstantiated && effect.VisualEffectReference != null)
        {
            playerHasSlowEffect = false;
            effect.VisualEffectReference.Destruct();
        }
    }
    
    public override void removeStunnedEffect(Stuned effect) 
    {
        PlayerInput targetMovement = GetComponent<PlayerInput>();
        WeaponManager targetWeaponManager = GetComponentInChildren<WeaponManager>();

        targetMovement.entityRigidbody.gravityScale = initialPlayerGravity;

        if (effect.disableAttacking)
        {
            targetWeaponManager.canAim = true;
        }

        if (effect.disableMovement)
        {
            targetMovement.stunned = false;
            targetMovement.setControlsDisable(false);
        }

        if (effect.visualInstantiated && effect.VisualEffectReference != null)
        {
            //Stun Free Sound
            if (effect.StunFreeSound != null)
            {
                RuntimeManager.PlayOneShot(effect.StunFreeSound, transform.position);
            }
            
            effect.VisualEffectReference.Destruct();
        }
    }

    public override void removeDamageOverTimeEffect(DamageOverTime effect) 
    {
        
    }

    #endregion

    public void Reset()
    {
        EffectImunity = false;
        removeAllEffects();
    }
}
