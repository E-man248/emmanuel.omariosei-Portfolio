using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlowOnHitEffect")]
public class Slow : OnHitEffector
{
    [Space]
    public float slowAmount;
    [HideInInspector] public bool slowApplied;
    public override OnHitEffector clone()
    {
        Slow clone = (Slow) Slow.CreateInstance("Slow");
        clone.effectName = this.effectName;
        clone.effectDuration = this.effectDuration;
        clone.effectTimer = this.effectDuration;
        clone.VisualEffect = this.VisualEffect;

        visualInstantiated = false;
        clone.slowAmount = this.slowAmount;
        clone.slowApplied = false;
        return clone;
    }

}
