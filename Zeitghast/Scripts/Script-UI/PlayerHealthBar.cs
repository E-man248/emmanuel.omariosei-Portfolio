using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthBar : HealthBar
{
    private bool hasSetup = false;

    #region Unity Functions
    protected override void Start()
    {
        base.Start();

        if (!hasSetup)
        {
            SetUpBar();
        }
    }

    protected override void Update()
    {
        base.Update();
    }
    #endregion

    public void SetUpBar()
    {
        if (PlayerUI.instance == null) return;

        slider = PlayerUI.instance.transform.Find("Health Bar Box")?.GetComponent<Slider>();
        fill = PlayerUI.instance.transform.Find("Health Bar Box/Health Bar Fill").GetComponent<Image>();

        SetMaxHealth(health.maxHealth);
        fill.color = gradient.Evaluate(1f);

        hasSetup = true;
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
