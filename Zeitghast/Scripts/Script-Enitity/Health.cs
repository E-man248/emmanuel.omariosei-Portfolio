using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Health : MonoBehaviour
{
    [Header("Health")]
    public TagList attackers;
    public int maxHealth;
    [field: SerializeField] public int health { get; protected set; } = 0;
    protected string lastHitBy;
    //[HideInInspector]
    public bool isDead;
    public UnityEvent onDeathEvent;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector2 healthBarOffset;
    public Vector2 healthBarScale = Vector2.one;

    protected GameObject healthBarCache;

    [Header("Invicibility")]
    public bool invincible;
    public float invincibilityDuration;
    protected float invicibilityTimer;

    [Header("Damage Number")]
    [SerializeField] protected bool showDamageNumber = true;

    [Space]
    public bool trueInvincible = false;

    internal UnityEvent onDamageTaken;
    internal UnityEvent<int> onHealthChanged; // Event parameter returns the current heatlh value (health)

    protected virtual void Awake()
    {
        if (attackers == null)
        {
            attackers = new TagList();
        }

        health = maxHealth;
        onDamageTaken = new UnityEvent();
        onHealthChanged = new UnityEvent<int>();
    }
    protected virtual void Start()
    {
        //Default health bar scale
        if(healthBarScale == Vector2.zero)
        {
            healthBarScale = Vector2.one;
        }
   
        if (healthBarPrefab != null && GetComponentInChildren<HealthBar>() == null)
        {
            Vector3 position = transform.GetComponent<Collider2D>().bounds.center + new Vector3(healthBarOffset.x, healthBarOffset.y, 0f);
            GameObject prefab = Instantiate(healthBarPrefab, position, transform.rotation);
            prefab.transform.SetParent(transform);

            prefab.transform.localScale = healthBarScale;
            prefab.GetComponent<HealthBar>().health = this;

            healthBarCache = prefab;
        }
        isDead = false;
    }

    protected virtual void Update()
    {
        if (!trueInvincible) 
        {
            if (invicibilityTimer > 0 && !isDead)
            {
                invincible = true;
            }
            else
            {
                invincible = false;
            }
        }
        invicibilityTimer -= Time.deltaTime;

        if (health <= 0 && !isDead)
        {
            die();
        }
    }

    public void ResetHealth()
    {
        health = maxHealth;
    }

    /**
        <summary>
        Sets the Health Value 'health' of the GameObject to
        that 'health' + 'healthAdded'. The health value still
        stays between the interval: 0 &lt; 'health' &lt; 'maxHealth'
        </summary>
    **/
    public virtual void changeHealth(int healthAdded, string lastHitBy)
    {
        if (isDead) return;

        /*
        Damage number Dislpay Calculation 
        */
        int totalHealth = health + healthAdded;
        if (totalHealth > health) // display Healing number
        {
            spawnHealNumber(healthAdded, transform);
        }
        else if (totalHealth < health && !invincible)// display Damage number
        {
            onDamageTaken.Invoke();
            spawnDamageNumber(healthAdded, transform);
        }

        /*
            Actual Damage Calculation 
        */
        if (!invincible || healthAdded > 0)
        {
            if (health + healthAdded > maxHealth)
            {
                health = maxHealth;

            }
            else if (health + healthAdded < 0)
            {
                health = 0;
            }
            else
            {
                health += healthAdded;
            }

            if (string.IsNullOrEmpty(lastHitBy))
            {
                this.lastHitBy = lastHitBy;
            }
        }

        onHealthChanged.Invoke(healthAdded);
    }

    public virtual void changeHealth(int healthAdded)
    {
        changeHealth(healthAdded, null);
    }

    public virtual void setHealth(int newHealthValue, string lastHitBy = null)
    {
        int changeInHealth = newHealthValue - health;

        health = Mathf.Clamp(newHealthValue, 0, maxHealth);

        this.lastHitBy = lastHitBy;

        onHealthChanged.Invoke(changeInHealth);
    }

    public virtual void SetMaxHealth(int newMaxHealthValue)
    {
        maxHealth = newMaxHealthValue;

        health = Mathf.Clamp(health, 0, maxHealth);
    }

    /** 
        <summary>
        Changes the 'invincibilityTimer' value to 'invincibilityDuration'
        if the timer less than 0 (to avoid reseting a previous timer set).
        This will cause the player to be invincible for the time of
        'invincibilityTimer'.
        </summary>
    **/
    public virtual void invincibilityFrames()
    {
        if(!invincible)
        {
            if (invicibilityTimer < 0 && !invincible)
            {
                invicibilityTimer = invincibilityDuration;
            }
        }
    }

    /**
        <summary>
        Changes the 'invincibilityTimer' value to the parameter 'time'
        if the timer less than 0 (to avoid reseting a previous timer set).
        This will cause the player to be invincible for the time of
        'invincibilityTimer'.
        </summary>
    **/
    public virtual void invincibilityFrames(float time)
    {
        if (invicibilityTimer < 0 && !invincible)
        {
            invicibilityTimer = time;
        }
    }

    public virtual void die() 
    {
        if (!isDead)
        {
            if (lastHitBy != null)
            {
                string myName = gameObject.name;
                if (GetComponent<Movement>() != null) myName = GetComponent<Movement>().entityName;

                TextEventCache.annouceTextEvent(myName + " was killed by " + lastHitBy);
            }
            CursorManager.Instance.targetObjectTag = null;
            CursorManager.Instance.targetObjectName = null;
            isDead = true;

            onDeathEvent.Invoke();
        }
    }

    protected void spawnHealNumber(int healthAdded, Transform transform)
    {
        if(showDamageNumber && DevTools.Instance != null && DevTools.Instance.devToolsActive)
        {
            UINumberManager.Instance.createHealNumber(healthAdded, transform);
        }
    }

    protected void spawnDamageNumber(int healthAdded, Transform transform)
    {
        if (showDamageNumber && DevTools.Instance != null && DevTools.Instance.devToolsActive)
        {
            UINumberManager.Instance.createDamageNumber(healthAdded, transform);
        }
    }

    protected void spawnHurtNumber(int healthAdded, Transform transform)
    {
        if (showDamageNumber && DevTools.Instance != null && DevTools.Instance.devToolsActive)
        {
            UINumberManager.Instance.createHurtNumber(healthAdded, transform);
        }
    }
    /// <summary> Enemy Cursor Function </summary>
    private void OnMouseEnter()
    {        
        string myName = gameObject.name;
        if (GetComponent<Movement>() != null) myName = GetComponent<Movement>().entityName;
        CursorManager.Instance.targetObjectName = myName;
        CursorManager.Instance.targetObjectTag = tag;
    }

    private void OnMouseExit()
    {
        CursorManager.Instance.targetObjectTag = null;
        CursorManager.Instance.targetObjectName = null;
    }
}
