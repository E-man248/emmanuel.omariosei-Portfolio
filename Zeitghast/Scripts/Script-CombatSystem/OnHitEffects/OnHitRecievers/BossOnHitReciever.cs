using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOnHitReciever : OnHitReciever
{
    [Header("Boss On-Hit Reciever")]
    [SerializeField]private Vector2 effectOffset = Vector2.zero;
    [SerializeField]private Vector2 effectScale = Vector2.one;
    [SerializeField]private Vector3 effectRotation = Vector3.zero;

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
                BossAnimationHandler animationHandler = GetComponentInChildren<BossAnimationHandler>();
                
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
}
