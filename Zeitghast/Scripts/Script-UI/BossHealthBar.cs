using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : HealthBar
{
    private GameObject bossBar;
    [SerializeField] private int phaseNumber = 1;

    protected override void Start()
    {
        if (BossUI.instance != null)
        {
            SetUpUtilities();

            SetMaxHealth(health.maxHealth);
            fill.color = gradient.Evaluate(1f);
        }
    }

    private void SetUpUtilities()
    {
        bossBar = BossUI.instance.bossBar.gameObject;
        slider = BossUI.instance.transform.Find("Boss Bar/Health Bar Box").GetComponent<Slider>();
        fill = BossUI.instance.transform.Find("Boss Bar/Health Bar Box/Health Bar Fill").GetComponent<Image>();
    }

    private void OnEnable()
    {
        SetUpUtilities();

        bossBar.SetActive(true);
        BossUI.instance.SetPhaseNumber(phaseNumber);
    }

    private void OnDisable()
    {
        bossBar?.SetActive(false);
    }

    private void OnDestroy()
    {
        bossBar?.SetActive(false);
    }

    public override void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = Mathf.Lerp(slider.value, health, lerpValue);
    }
    
    public override void SetHealth(int Health)
    {
        slider.value = Mathf.Lerp(slider.value, Health, lerpValue);

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
