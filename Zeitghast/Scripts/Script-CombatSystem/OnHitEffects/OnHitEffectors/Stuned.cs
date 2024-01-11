using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "StunedEffect")]
public class Stuned : OnHitEffector
{
    [Range(1, 100)]
    public int stunChance;
    public float gravityEffect = 1f;

    public bool disableMovement;
    public bool disableAttacking;

    [EventRef]
    public string stunSound = null;
    [EventRef]
    public string StunBreakSound = null;
    [EventRef]
    public string StunFreeSound = null;
    [HideInInspector]
    public bool stunSoundPlayed = false;

    public override OnHitEffector clone()
    {
        Stuned clone = (Stuned)Stuned.CreateInstance("Stuned");
        clone.effectName = this.effectName;
        clone.effectDuration = this.effectDuration;
        clone.effectTimer = this.effectDuration;
        clone.VisualEffect = this.VisualEffect;
        clone.VisualScaleOffest = this.VisualScaleOffest;

        clone.disableAttacking = this.disableAttacking;
        clone.disableMovement = this.disableMovement;

        clone.stunSound = this.stunSound;
        clone.StunBreakSound = this.StunBreakSound;
        clone.StunFreeSound = this.StunFreeSound;
        clone.stunSoundPlayed = this.stunSoundPlayed;

        visualInstantiated = false;
        clone.stunChance = this.stunChance;
        clone.gravityEffect = this.gravityEffect;
        int chance = Random.Range(1, 101);
        if (chance > this.stunChance)
        {
            clone.effectTimer = 0f;
        }

        return clone;
    }
}
