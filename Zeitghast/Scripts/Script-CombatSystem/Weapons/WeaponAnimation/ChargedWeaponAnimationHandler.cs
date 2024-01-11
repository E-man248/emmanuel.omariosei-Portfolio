using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedWeaponAnimationHandler : WeaponAnimationHandler
{
    #region Animation Variables
    private struct ChargeLevelPair
    {
        public string chargedAnimation;
        public string unloadAnimation;
    };

    [Header("Charged Weapon Animation")]
    [SerializeField] private ChargedShotType chargedShotType;
    #endregion

    #region Animations
    private List<ChargeLevelPair> chargeLevelAnimationPairs;
    private string chargingAnimationString;
    #endregion

    #region UnityFunctions
    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions
        chargingAnimationString = weaponName + "Charging";
        #endregion
    }

    protected void Start()
    {
        if(weaponScript.ShotType is ChargedShotType)
        {
            chargedShotType = (ChargedShotType) weaponScript.ShotType;
            chargeLevelAnimationPairs = new List<ChargeLevelPair>();
            loadChargeLevelAnimationPairs();
        }
        else
        {
            Debug.LogError("[ChargedWeaponAnimationHandler] Weapon ShotType is not ChargedShot for " + name);
        }
    }
    #endregion

    #region Animation Booleans:
    public bool isCharging()
    {
        return chargedShotType.isCharging;
    }
    #endregion

    #region Animations
    protected override void animate()
    {
        stillIdleAnimationTimer -= Time.deltaTime;
        
        idleAnimation();
        dashAttackAnimation();
        chargingAnimation();

        if (animationCycleEnabled) changeAnimation(nextAnimation);
    }

    public void levelChargedAnimation(int chargeLevel)
    {
        playAnimationTillInterupt(chargeLevelAnimationPairs[chargeLevel].chargedAnimation, dashAnimationInterupt);
    }

    public void levelUnloadAnimation(int chargeLevel)
    {
        stillIdleAnimationTimer = stillIdleAnimationDuration;
        playAnimationOnceFull(chargeLevelAnimationPairs[chargeLevel].unloadAnimation, dashAnimationInterupt);
    }

    public void chargingAnimation()
    {
        if (isCharging())
        {
            nextAnimation = chargingAnimationString;
        }
    }
    #endregion

    #region Animation Helper Functions:
    private void loadChargeLevelAnimationPairs()
    {
        if (chargedShotType.damageSegments.Count == 0) return;

        for (int i = 0; i < chargedShotType.damageSegments.Count; i++)
        {
            ChargeLevelPair chargePair = new ChargeLevelPair();
            chargePair.chargedAnimation = weaponName + "Level" + i + "Charged";
            chargePair.unloadAnimation = weaponName + "Level" + i + "Unload";
            chargeLevelAnimationPairs.Add(chargePair);
        }
    }
    #endregion
}
