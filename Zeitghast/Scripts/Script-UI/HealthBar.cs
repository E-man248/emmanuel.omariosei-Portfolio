using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar GameObject")]
    public Slider slider;
    public Gradient gradient;
    public Health health;
    public Image fill;
    public float lerpValue = 0.1f;

    protected Canvas canvas;

    protected virtual void Start()
    {
        SetMaxHealth(health.maxHealth);
        fill.color = gradient.Evaluate(1f);
        canvas = gameObject.GetComponentInChildren<Canvas>();
    }

    protected virtual void Update()
    {
        SetHealth(health.health);
    }

    public virtual void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    
    public virtual void SetHealth(int Health)
    {
        slider.value = Health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
