using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GeneralRateBar : MonoBehaviour
{
    public Slider slider;
    public Gradient standardGradient;
    public Gradient overheatedGradient;
    public Gradient chargingGradient;
    public WeaponManager weaponManager;
    public Image fill;
    public float lerpValue = 0.1f;
    private Canvas canvas;
    private bool reverseBar;
    private Gradient currentGradient;

    /** <summary>
        Start method of GeneralRateBar:<br/>
        Sets up gradients, fill color of the image, and sets the canvas
        </summary>
    */
    void Start()
    {
        currentGradient = standardGradient;
        fill.color = standardGradient.Evaluate(1f);
        canvas = gameObject.GetComponentInChildren<Canvas>();

        SceneLoaded();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    protected void SceneLoaded()
    {
        if (PlayerUI.instance != null)
        {
            slider = PlayerUI.instance.transform.Find("General Rate Bar Box").GetComponent<Slider>();
            fill = PlayerUI.instance.transform.Find("General Rate Bar Box/General Rate Bar Fill").GetComponent<Image>();
        }
    }
    protected void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoaded();
    }

    /** <summary>
        Update method of GeneralRateBar:<br/>
        Calls the method updateRateBar() with the parameters for the ShotType assigned
        to currentWeapon of weaponManager.
        </summary>
    */
    public void Update()
    {
        if (weaponManager.currentWeapon != null)
        {
            if (weaponManager.currentWeapon.ShotType is StandardShotType)
            {
                StandardShotType standardShotType = (StandardShotType) weaponManager.currentWeapon.ShotType;
                    reverseBar = true;

                // Standard Weapon:
                if (!standardShotType.hasOverheatMechanic)
                {
                    currentGradient = standardGradient;
                    updateRateBar(standardShotType.fireRateTimer, standardShotType.fireRate, standardShotType.fireRate);
                }
                // Overheat Weapon:
                else
                {
                    if (standardShotType.isOverheating) currentGradient = overheatedGradient;
                    else currentGradient = standardGradient;
                    updateRateBar(standardShotType.currentOverheatAmount, standardShotType.overheatCap, lerpValue);
                }
            }
            // Charged Weapon:
            if (weaponManager.currentWeapon.ShotType is ChargedShotType)
            {
                ChargedShotType chargedShotType = (ChargedShotType) weaponManager.currentWeapon.ShotType;
                reverseBar = false;
                currentGradient = chargingGradient;

                float maxCharge = chargedShotType.damageSegments[chargedShotType.damageSegments.Count - 1].chargeTreshold;
                updateRateBar(chargedShotType.currentChargeValue, maxCharge, lerpValue);
            }
        }
        else
        {
            if (canvas != null && canvas.enabled) canvas.enabled = false;
            reverseBar = false;
            updateRateBar(0f, 1f, lerpValue);
        }
    }

    /**  <summary>
        Updates the visible rate bar with the currentValue and the maxValue:<br/>

        If the bar IS NOT reversed (reversedBar = false), then the slider slides the
        image up to the currentValue from 0 to maxValue.
        This causes the bar to increase toward the right.
        <br/>

        If the bar IS reversed (reversedBar = true), then the slider slides the image
        up to (maxValue - currentValue) from 0 to maxValue.
        This causes the bar to increase toward the left.
        </summary>
    */
    public void updateRateBar(float currentValue, float maxValue, float rate = 1f)
    {
        if(reverseBar)
        {
            setMaxValue(maxValue);
            if (currentValue >= 0)
            {
                setCurrentValue(maxValue - currentValue, rate);
                if (canvas != null && !canvas.enabled) canvas.enabled = true;
            }
            else
            {
                setCurrentValue(maxValue, rate);
                if (canvas != null && canvas.enabled) canvas.enabled = false;
            }
        }
        else
        {
            setMaxValue(maxValue);
            setCurrentValue(currentValue, rate);
            if (canvas != null && !canvas.enabled) canvas.enabled = true;
        }
    }

    /// <summary> Sets the maxValue variable to the specified parameter </summary>
    public void setMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
    }

    /// <summary> Sets the currentValue variable to the specified parameter </summary>
    public void setCurrentValue(float currentValue, float rate = 1f)
    {
        slider.value = Mathf.Lerp(slider.value, currentValue, rate);
        fill.color = currentGradient.Evaluate(slider.normalizedValue);
    }
}
