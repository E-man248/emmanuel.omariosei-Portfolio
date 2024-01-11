using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "StandardShotType")]
public class StandardShotType : ShotType
{
    [HideInInspector] public float fireRateTimer;
    public float fireRate;
    public bool canFire;


    [Header("Over Heat Mechanic")]
    public bool hasOverheatMechanic;
    [EventRef]
    public string OverheatSound = null;
    [EventRef]
    public string OverheatReloadSound = null;


    //[HideInInspector]
    public bool isOverheating;
    [Space]
    public float overheatCap;
    public float currentOverheatAmount;
    [Space]
    public float overheatAddRate;
    public float overheatSubtractRate;


    public void initalizeStandardShotType()
    {
        canFire = true;
        isOverheating = false;
        currentOverheatAmount = 0;
    }
    
    public override ShotType clone()
    {
        StandardShotType clone = (StandardShotType)StandardShotType.CreateInstance("StandardShotType");
        clone.name = this.name + " (Clone)";
        clone.fireRateTimer = this.fireRateTimer;
        clone.fireRate = this.fireRate;
        clone.canFire = this.canFire;
        clone.hasOverheatMechanic = this.hasOverheatMechanic;
        clone.OverheatSound = this.OverheatSound;
        clone.isOverheating = this.isOverheating;
        clone.overheatCap = this.overheatCap;
        clone.currentOverheatAmount = this.currentOverheatAmount;
        clone.overheatAddRate = this.overheatAddRate;
        clone.overheatSubtractRate = this.overheatSubtractRate;

        return clone;
    }

}
