using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAnimationHandler : EntityAnimationHandler
{
    //Enemy Animation Sources:
    [Header("Enemy Animation Handler")]
    [HideInInspector] public string enemyName;
    public Transform Enemy;
    protected EnemyMovement enemyMovement;
    protected EnemyHealth enemyHealth;
    protected int storedHealth;
    protected Func<bool> attackAnimationInterupt;
    protected Func<bool> deathAnimationInterupt;


    [Header("Hurt Flash Animation")]
    [SerializeField] protected bool hurtFlashEnabled = true;
    public float FlashRate = 0.3f;
    [Range(0, 1)] public float FlashIntensity = 0.62f;
    public float flashDuration = 0.1f;
    protected float flashDurationTimer = 1f;
    protected SpriteRenderer[] sprites;

    protected bool flashState = false;
    protected bool StartFlashing = false;

    protected float flashValue;
   

    protected override void Awake()
    {
        #region Animation String Definitions
        attackAnimationInterupt = () =>
        {
            return Mathf.Abs(enemyMovement.entityRigidbody.velocity.x) > 0 && enemyHealth.health > 0;
        };
        #endregion
        base.Awake();

        enemyMovement = GetComponentInParent<EnemyMovement>();
        if (enemyMovement != null)
        {
            Enemy = enemyMovement.transform;
            enemyHealth = enemyMovement.GetComponent<EnemyHealth>();
            enemyName = new string(enemyMovement.entityName);

            sprites = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    protected virtual void Start()
    {
        storedHealth = enemyHealth.health;
    }

    protected override void Update()
    {
        base.Update();

        //Non-Mutually Exclusive Animations
        orientateEnemy();
        FlashAnimationUpdate();
    }

    #region Animation Booleans:
    
    // Boolean Functions that are used to dictate animation switching:
    public bool isWindingUpAttack()
    {
        return enemyMovement.attackWindUpTimer > 0 && enemyMovement.state == EnemyMovement.State.Combat;
    }

    public bool isStunned()
    {
        return enemyMovement.stunTimer > 0f;
    }

    #endregion

    #region Mutually Exclusive Animations
    public virtual void attackAnimation()
    {

    }

    public virtual void attackWindUpAnimation()
    {

    }

    public virtual void deathAnimation()
    {
        
    }

    public virtual void stunAnimation()
    {
        if (isStunned())
        {
            pauseAnimator();
        }
    }
    #endregion

    #region Non-Mutually Exclusive Animations
    protected virtual void orientateEnemy()
    {
        if (enemyMovement == null)
        {
            return;
        }

        if (!enemyMovement.canOrientate)
        {
            return;
        }


        //Checking to correct the orientation if it's flipped
        float lookDirectionCorrection = 0f;

        if (enemyMovement.flipOrientation)
        {
            lookDirectionCorrection = -1f;
        }
        else
        {
            lookDirectionCorrection = 1f;
        }

        if (enemyMovement.lookDirection == (1 * lookDirectionCorrection))
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (enemyMovement.lookDirection == (-1 * lookDirectionCorrection))
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    protected virtual void FlashAnimationUpdate()
    {
        if (!hurtFlashEnabled) return;

        if (enemyHealth.health != storedHealth)
        {
            storedHealth = enemyHealth.health;
            StartFlashing = true;
            flashDurationTimer = flashDuration;
            flashValue = flashDuration;
        }

        if (StartFlashing)
        {
            flashSprite();
        }

        if (flashDurationTimer <= 0f && !flashState)
        {
            StartFlashing = false;

            //reset 
            for (int i = 0; i < sprites.Length; i++)
            {
                Color newColor = new Color(sprites[i].color.r, 1f, 1f);
                sprites[i].color = newColor;
            }
        }
        flashDurationTimer -= Time.deltaTime;
    }

    protected virtual void flashSprite()
    {
        //Handle timer
        if (flashState)
        {
            if (flashValue / FlashRate >= 1)
            {
                flashState = false;
            }
            flashValue += Time.deltaTime;
        }
        else
        {
            if (flashValue / FlashRate <= FlashIntensity)
            {
                flashState = true;
            }
            flashValue -= Time.deltaTime;
        }

        //Flash all objects

        for (int i = 0; i < sprites.Length; i++)
        {
            float newGValue = flashValue / FlashRate;
            float newBValue = flashValue / FlashRate;
            Color newColor = new Color(sprites[i].color.r, newGValue, newBValue);
            sprites[i].color = newColor;
        }

    }

    #endregion

}
