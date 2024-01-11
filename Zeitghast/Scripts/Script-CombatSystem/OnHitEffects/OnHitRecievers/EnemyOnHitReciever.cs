using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnHitReciever : OnHitReciever
{
    [Header("Enemy On-Hit Reciever")]
    [SerializeField]private Vector2 effectOffset = Vector2.zero;
    [SerializeField]private Vector2 effectScale = Vector2.one;
    [SerializeField]private Vector3 effectRotation = Vector3.zero;

    public override void applySlowEffect(Slow effect)
    {
        base.applySlowEffect(effect);
        effect.refreshOnHit = false;

        EnemyMovement targetMovement = GetComponent<EnemyMovement>();
        if (targetMovement != null && effect.effectTimer > 0f && !effect.slowApplied)
        {
            effect.slowAmount = -targetMovement.addToVariableSpeed(-effect.slowAmount);
            effect.slowApplied = true;
        }
        
        // Visual Effect:
        if (effect.VisualEffect != null && !effect.visualInstantiated)
        {
            effect.visualInstantiated = true;
            EntityAnimationHandler graphicsObject = GetComponentInChildren<EntityAnimationHandler>();
            GameObject particleEffect = Instantiate(effect.VisualEffect,  graphicsObject.transform);   

            SelfDestruct selfDestruct = particleEffect.GetComponent<SelfDestruct>();
            selfDestruct.lifeTime = effect.effectDuration;

            Vector3 newScale = new Vector3(particleEffect.transform.localScale.x * effectScale.x, particleEffect.transform.localScale.y * effectScale.y, particleEffect.transform.localScale.z);
            particleEffect.transform.localScale = newScale;
            particleEffect.transform.position += (Vector3) effectOffset;

            selfDestruct.InitiateDestruct();
        }
    }
    
    public override void applyStunnedEffect(Stuned effect)
    {
        base.applyStunnedEffect(effect);
        EnemyMovement targetMovement = GetComponent<EnemyMovement>();

        Collider2D targetCollider;
        if (targetMovement != null)
        {
            targetCollider = targetMovement.entityCollider;
        }
        else targetCollider = GetComponent<Collider2D>();

        if (targetMovement != null && effect.effectTimer > 0f)
        {
            targetMovement.stunEnemy(effect.effectDuration, effect.gravityEffect);
        }
        
        // Visual Effect:
        if (effect.VisualEffect != null && !effect.visualInstantiated)
        {
            effect.visualInstantiated = true;
            enemyAnimationHandler animationHandler = GetComponentInChildren<enemyAnimationHandler>();
            
            GameObject particleEffect;
            if (animationHandler != null)
            {
                particleEffect = Instantiate(effect.VisualEffect, animationHandler.transform);
            }
            else
            {
                particleEffect = Instantiate(effect.VisualEffect, transform);
            }
            
            SelfDestruct selfDestruct = particleEffect.GetComponent<SelfDestruct>();

            Vector3 newScale = new Vector3(particleEffect.transform.localScale.x * effectScale.x, particleEffect.transform.localScale.y * effectScale.y, particleEffect.transform.localScale.z);
            particleEffect.transform.localScale = newScale;
            particleEffect.transform.position += (Vector3) effectOffset;

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
                enemyAnimationHandler animationHandler = GetComponentInChildren<enemyAnimationHandler>();
                
                GameObject particleEffect;
                if (animationHandler != null)
                {
                    particleEffect = Instantiate(effect.VisualEffect, animationHandler.transform);
                }
                else
                {
                    particleEffect = Instantiate(effect.VisualEffect, transform);
                }

                Vector3 newScale = new Vector3(particleEffect.transform.localScale.x * effectScale.x, particleEffect.transform.localScale.y * effectScale.y, particleEffect.transform.localScale.z);
                particleEffect.transform.localScale = newScale;
                particleEffect.transform.position += (Vector3) effectOffset;

                //Applying rotaion offset
                Vector3 oldRotation = particleEffect.transform.eulerAngles;
                Vector3 newRotation = oldRotation + effectRotation;
                particleEffect.transform.rotation = Quaternion.Euler(newRotation);
            }
        }
        effect.damageTickRateTimer -= Time.deltaTime;
    }

    public override void removeSlowEffect(Slow effect) 
    {
        if (effect.slowApplied)
        {
            EnemyMovement targetMovement = GetComponent<EnemyMovement>();
            targetMovement.addToVariableSpeed(effect.slowAmount);
            effect.slowApplied = false;
        }
    }
    
    public override void removeStunnedEffect(Stuned effect) 
    {

    }

    public override void removeDamageOverTimeEffect(DamageOverTime effect) 
    {
        
    }
}
