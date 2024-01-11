using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageOverTimeEffect")]
public class DamageOverTime : OnHitEffector
{
    [Space]
    public int damageAmount;
    public float damageTickRate;
    [HideInInspector] public float damageTickRateTimer;
    public override OnHitEffector clone()
    {
        DamageOverTime clone = (DamageOverTime) DamageOverTime.CreateInstance("DamageOverTime");
        clone.effectName = this.effectName;
        clone.effectDuration = this.effectDuration;
        clone.effectTimer = this.effectDuration;
        clone.VisualEffect = this.VisualEffect;

        clone.damageAmount = this.damageAmount;
        clone.damageTickRate = this.damageTickRate;
        clone.damageTickRateTimer = 0f;
        return clone;
    }

}

