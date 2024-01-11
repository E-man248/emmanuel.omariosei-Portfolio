using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitEffector : ScriptableObject
{
    public string effectName;
    public GameObject VisualEffect;
    [HideInInspector]
    public SelfDestruct VisualEffectReference;
    public Vector2 VisualScaleOffest;

    [HideInInspector] public bool visualInstantiated = false;

    public bool refreshOnHit;

    public float effectDuration;
    [HideInInspector] public float effectTimer;

    public virtual OnHitEffector clone()
    {
        OnHitEffector clone = (OnHitEffector) OnHitEffector.CreateInstance("OnHitEffector");
        clone.effectName = this.effectName;
        clone.effectDuration = this.effectDuration;
        clone.effectTimer = effectDuration;
        clone.VisualEffect = this.VisualEffect;
        return clone;
    }

}
