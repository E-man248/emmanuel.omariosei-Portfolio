using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentalHarm : MonoBehaviour
{
    public string owner;

    [Header("Damage")]
    public TagList hurtCollisions;
    public int damage;
    [SerializeField] private float damageTickSpeed = 0f;
    private float damageTickSpeedTimer;

    [Header("KnockBack")]
    public bool canKnockBack = true;
    public Vector2 knockbackForce;
    public float knockbackTime;

    private Dictionary<int, Health> currentTargetHealth;
    private Health healthScript;

    [Header("On Hit Effects")]
    public OnHitEffector onHitEffect;

    #region Unity Functions
    private void Start()
    {
        currentTargetHealth = new Dictionary<int, Health>();
        if (transform.parent != null && (gameObject.tag.Equals("EnemyHurtBox") || gameObject.tag.Equals("PlayerHurtBox")))
        {
            owner = transform.parent.name;
            if (transform.parent.GetComponent<Movement>() != null)
            {
                owner = transform.parent.GetComponent<Movement>().entityName;
            }
        }

        healthScript = GetComponentInParent<Health>();
        subscribeToEvents();
    }

    protected void OnEnable()
    {
        subscribeToEvents();
    }

    protected void OnDisable()
    {
        unsubscribeToEvents();
    }

    protected void OnDestroy()
    {
        unsubscribeToEvents();
    }

    #endregion

    #region Event Functions

    private void subscribeToEvents()
    {
        PlayerInfo.PlayerDeathEvent.AddListener(OnPlayerDeath);
    } 

    private void unsubscribeToEvents()
    {
        PlayerInfo.PlayerDeathEvent.RemoveListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        if (currentTargetHealth != null)
        {
            currentTargetHealth.Clear();
        }
        else
        {
            currentTargetHealth = new();
        }
    }

    #endregion

    private void Update()
    {
        //damage timer  
        damageTickSpeedTimer -= Time.deltaTime;

        //if the is no items in the currentTargetHealth list we can stop checking to deal damage 
        if (currentTargetHealth?.Count == 0) return;

        //if there is a health script in the parent and the parents is dead, we can stop checking to deal damage 
        if (healthScript != null && healthScript.isDead)
        {
            return;
        }

        dealDamageToAllHitinfo();
    }


    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (currentTargetHealth == null)
        {
            currentTargetHealth = new Dictionary<int, Health>();
        }

        if (hitInfo != null && (hurtCollisions.list.Contains(hitInfo.tag)))
        {
            Health tempHealth = hitInfo.GetComponentInParent<Health>();

            if(!currentTargetHealth.ContainsKey(tempHealth.GetInstanceID()) && !tempHealth.isDead)
            {
                currentTargetHealth.Add(tempHealth.GetInstanceID(), tempHealth);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo != null && (hurtCollisions.list.Contains(hitInfo.tag)))
        {
            Health tempHealth = hitInfo.GetComponentInParent<Health>();
            currentTargetHealth.Remove(tempHealth.GetInstanceID());
        }
    }

    public void dealDamage(int damage, Health targetHealth)
    {    
        targetHealth.changeHealth(-damage, owner);
        if(!targetHealth.invincible && damage > 0)
        {
            targetHealth.invincibilityFrames();
        }
    }

    public void dealKnockback(Vector2 knockbackForce, Knockback targetKnockback)
    {
        targetKnockback.applyKnockBackPoint(transform.position, knockbackForce);
        if (!targetKnockback.knockbackDisabled)
        {
            targetKnockback.knockbackFrames(targetKnockback.knockbackDuration + knockbackTime);
        }
    }

    //Deals damage to all hit info  stored  every tick interval
    private void dealDamageToAllHitinfo()
    {
        //Do nothing if the tick timer is not up
        if (damageTickSpeedTimer > 0f)
        {
            return;
        }
       
        //Dealing damage to all health 
        foreach (KeyValuePair  <int, Health> targetHealthID_Pair in currentTargetHealth)
        {
            //Continue if there is no health script
            if (targetHealthID_Pair.Value == null)
            {
                continue;
            }

            //Deal damage if the target health is not invincible and we can attack it 
            if (!targetHealthID_Pair.Value.invincible) 
            {
                if (targetHealthID_Pair.Value.attackers.list.Contains(tag))
                {
                    dealDamage(damage, targetHealthID_Pair.Value);
                }
            }

            //Finds an Knockback scripts and applies the knockback
            Knockback targetKnockback = targetHealthID_Pair.Value.GetComponentInParent<Knockback>();
            if (targetKnockback != null)
            {
                if (targetKnockback.attackers.list.Contains(tag))
                {
                    if (canKnockBack) dealKnockback(targetKnockback.knockbackForce + knockbackForce, targetKnockback);
                }
            }


            //Finds an On-Hit reciver and applies the onhit effect to it 
            OnHitReciever targetEffectReciever = targetHealthID_Pair.Value.GetComponentInParent<OnHitReciever>();
            if (targetEffectReciever != null)
            {
                if (onHitEffect != null && targetEffectReciever.attackers.list.Contains(tag))
                {
                    targetEffectReciever.addEffect(onHitEffect);
                }
            }

            //Reseting the damage Tick Timer 
            damageTickSpeedTimer = damageTickSpeed;
        }
    }
}