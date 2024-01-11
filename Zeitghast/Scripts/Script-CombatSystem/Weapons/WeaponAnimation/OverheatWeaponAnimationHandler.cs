using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatWeaponAnimationHandler : WeaponAnimationHandler
{
    #region Animation Variables

    [Header("Overheat Weapon Animation")]
    private StandardShotType standardShotType;
    public Gradient overheatColorGradient;
    public ParticleSystem overheatingParitcleEffect;
    #endregion

    #region Animations
    private string bustAnimationString;
    #endregion

    #region UnityFunctions
    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions
        bustAnimationString = weaponName + "Bust";
        #endregion
    }

    protected void Start()
    {
        if(weaponScript.ShotType is StandardShotType)
        {
            standardShotType = (StandardShotType) weaponScript.ShotType;
        }
        else
        {
            Debug.LogError("[OverheatWeaponAnimationHandler] Weapon ShotType is not StandardShotType for " + name);
        }
    }

    protected override void Update()
    {
        base.Update();

        // Non-Mutually Exclusive Animations:
        overheatingAnimation();
    }
    #endregion

    #region Animations
    public void bustAnimation()
    {
        playAnimationOnceFull(bustAnimationString, dashAnimationInterupt);
    }

    #endregion

    #region Animation Helper Functions:
    private void overheatingAnimation()
    {
        weaponSpriteRenderer.material.color = overheatColorGradient.Evaluate(standardShotType.currentOverheatAmount/standardShotType.overheatCap);

        if (standardShotType.isOverheating)
        {
            if (!overheatingParitcleEffect.isPlaying) overheatingParitcleEffect.Play();
        }
        else if (!standardShotType.isOverheating && overheatingParitcleEffect.isPlaying)
        {
            overheatingParitcleEffect.Stop();
        }
    }
    #endregion
}
