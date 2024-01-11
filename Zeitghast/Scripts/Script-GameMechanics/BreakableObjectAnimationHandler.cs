using System;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjectAnimationHandler : EntityAnimationHandler
{
    #region Animation Strings:

    private string IdleAnimationString;
    private string HurtAnimationString;
    private string DeathAnimationString;

    #endregion

    #region Animation Interrupts:
    private Func<bool> hurtAnimationInterrupt;
    #endregion

    // Animation Handler Utilities:
    private int currentAnimationIndex = 0;
    public Health objectHealth;
    private EnemyMovement enemyMovement;

    public List<float> animationHealthThresholds;

    protected override void Awake()
    {
        base.Awake();

        enemyMovement = GetComponentInParent<EnemyMovement>();
    }

    private void Start()
    {
        currentAnimationIndex = 0;

        #region Animation String Definitions
        IdleAnimationString = enemyMovement.entityName + "Idle";
        HurtAnimationString = enemyMovement.entityName + "Hurt";
        DeathAnimationString = enemyMovement.entityName + "Death";
        #endregion

        #region Animation Interrupt Definitions:
        hurtAnimationInterrupt = () => objectHealth.isDead;
        #endregion

        if (objectHealth == null)
        {
            objectHealth = GetComponentInParent<Health>();
        }

        subcribeToEvents();
    }

    private void OnEnable()
    {
        subcribeToEvents();
    }

    private void OnDisable()
    {
        unsubcribeToEvents();
    }

    private void OnDestroy()
    {
        unsubcribeToEvents();
    }

    private void subcribeToEvents()
    {
        objectHealth?.onHealthChanged.AddListener(onHealthChanged);
        objectHealth?.onDeathEvent.AddListener(playDeathAnimation);
    }

    private void unsubcribeToEvents()
    {
        objectHealth?.onHealthChanged.RemoveListener(onHealthChanged);
        objectHealth?.onDeathEvent.RemoveListener(playDeathAnimation);
    }

    protected override void animate()
    {
        idleAnimation();

        base.animate();
    }

    private void idleAnimation()
    {
        if (objectHealth == null || animationHealthThresholds.Count == 0)
        {
            nextAnimation = IdleAnimationString + 0;
            return;
        }

        nextAnimation = IdleAnimationString + currentAnimationIndex;
    }

    private void onHealthChanged(int changeInHealth)
    {
        if (changeInHealth < 0)// display Damage number
        {
            playHurtAnimation();
        }
    }

    public void playHurtAnimation()
    {
        if (objectHealth == null || animationHealthThresholds.Count == 0)
        {
            playAnimationOnceFull(HurtAnimationString + 0, hurtAnimationInterrupt);
        }
        else
        {
            playAnimationOnceFull(HurtAnimationString + currentAnimationIndex, hurtAnimationInterrupt);
        }

        if (currentAnimationIndex >= animationHealthThresholds.Count - 1)
        {
            return;
        }
        
        if (objectHealth.health <= animationHealthThresholds[currentAnimationIndex + 1])
        {
            currentAnimationIndex++;
        }
    }

    public void playDeathAnimation()
    {
        playAnimationOnceFull(DeathAnimationString);
    }
}
