using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCrystal : EffectApplyer
{
    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null) Debug.LogError(name + " Could not find animator");
    }

    protected override void applyEffect(Collider2D collision)
    {
        OnHitReciever targetEffectReciever = collision.GetComponentInParent<OnHitReciever>();

        if (targetEffectReciever == null) return;

        if (onHitEffect == null) return;

        //Check to see if there is already an effect of my type 
        foreach (var effect in targetEffectReciever.currentEffects)
        {
            if (effect.effectName == onHitEffect.effectName) return;
        }

        //Apply the effect
        targetEffectReciever.addEffect(onHitEffect);
        animator.SetTrigger("Collected");
    }
}
