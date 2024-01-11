using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


[System.Serializable]
public struct ChargedWeaponComponent
{
    public float chargeTreshold;
    public int damage;
    public GameObject bullet;
};

[CreateAssetMenu(fileName = "ChargedShotType")]
public class ChargedShotType : ShotType
{
    public float currentChargeValue;

    public List<ChargedWeaponComponent> damageSegments;

    [HideInInspector] public int currentChargeIndex;
    [HideInInspector] public int nextChargeIndex;

    public float baseFireRate;
    [HideInInspector] public float baseFireRateTimer;

    [Header("Sound")]
    #region Sound Variable
    public float maxChargePitchValue;
    [HideInInspector]public float chargePitchValue;
    [HideInInspector]public bool hasPlayedChargingSound;
    public FMOD.Studio.EventInstance chargingEvent;
    [EventRef]
    public string chargingSound = null;
    #endregion

    public float maxChargeValue;
    public float chargeRate;
    public bool isCharging;

    public override ShotType clone()
    {
        ChargedShotType clone = (ChargedShotType)ChargedShotType.CreateInstance("ChargedShotType");
        clone.name = this.name + " (Clone)";
        clone.currentChargeValue = this.currentChargeValue;
        clone.damageSegments = this.damageSegments;
        clone.currentChargeIndex = this.currentChargeIndex;
        clone.nextChargeIndex = this.nextChargeIndex;
        clone.baseFireRate = this.baseFireRate;
        clone.baseFireRateTimer = this.baseFireRateTimer;

        //Sound
        clone.maxChargePitchValue = this.maxChargePitchValue;
        clone.chargePitchValue = this.chargePitchValue;
        clone.hasPlayedChargingSound = this.hasPlayedChargingSound;
        clone.chargingEvent = this.chargingEvent;
        clone.chargingSound = this.chargingSound;

        //Charge values
        clone.maxChargeValue = this.maxChargeValue;
        clone.chargeRate = this.chargeRate;
        clone.isCharging = this.isCharging;

        return clone;
    }

    public void initalizeChargedShotType()
    {
        isCharging = false;
        hasPlayedChargingSound = false;
        currentChargeIndex = -1;
        nextChargeIndex = 0;

        maxChargeValue = damageSegments[damageSegments.Count-1].chargeTreshold;
        if (!string.IsNullOrEmpty(chargingSound))
        {
            chargingEvent = RuntimeManager.CreateInstance(chargingSound);
        }

        currentChargeValue = 0;
        baseFireRateTimer = 0f;
    }
}
