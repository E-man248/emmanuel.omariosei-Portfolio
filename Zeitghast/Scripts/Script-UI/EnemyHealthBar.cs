using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : HealthBar
{
    [Header("Enemy Health Bar Settings")]
    private bool displayRevealed;

    protected override void Start()
    {
        base.Start();
        
        displayRevealed = false;
    }

    protected override void Update()
    {
        base.Update();

        // Displays Health After First Hit:
        if (health.health != health.maxHealth)
        {
            displayRevealed = true;
        }

        if (displayRevealed)
        {
            if (!canvas.enabled) canvas.enabled = true;
        }
        else
        {
            if (canvas.enabled) canvas.enabled = false;
        }
    }
}
