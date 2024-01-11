using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

public class Bullet : MonoBehaviour
{
    public string owner;
    public WeaponFireType firingType;
    public int damage;
    public Vector2 knockbackForce;
    public float knockbackTime;
    public OnHitEffector onHitEffect;

    public float lifeTime;
    public Vector2 speed;
    public Vector2 acceleration;

    public LayerMaskObject destroyCollisions;

    public bool automaticDamageDisabled;

    protected Collider2D bulletCollider;
    [SerializeField] protected float hitboxOffset = 0.7f;

    [Header("Pierce Projectile settings")]
    public bool canPierce;
    public float damageTickSpeedDurations = 0.2f;
    public Dictionary<int, float> damageTickSpeedTimers;

    //Sound vars
    protected FMOD.Studio.EventInstance AmbientSoundEvent;
    private float abientSoundPositionUpdateTimer;
    private float abientSoundPositionUpdateRate = 0.3f;
    [EventRef]
    [Header("Sound")]
    public string HitSound = null;
    [EventRef]
    public string SpawnSound = null;
    [EventRef]
    public string DeathSound = null;
    [EventRef]
    public string AmbientSound = null;

    [Header("ScreenShake")]
    public float screenShakeOnHitIntensity;
    public float screenShakeOnHitDuration;
    [Space]
    public float screenShakeOnDeathIntensity;
    public float screenShakeOnDeathDuration;

    [Header("Special Effects")]
    public GameObject deathEffect;
    public GameObject hitMarker;

    [Header("Spawn Bullet On Death")]
    public bool spawnBulletOnDeath = false;
    protected bool hasSpawnedBulletOnDeath = false;
    public GameObject bulletToSpawnDeath;

    [Header("Trail settings")]
    public bool hasTrail;

    protected WeaponTrailController[] trailControllers;
    private bool hasLandedAHit;

    protected virtual void Awake()
    {
        if (lifeTime > -1)
        {
            Invoke("destroyBullet", lifeTime);
        }
    }

    protected virtual void Start()
    {
        bulletCollider = GetComponent<Collider2D>();

        damageTickSpeedTimers = new Dictionary<int, float>();

        if (spawnBulletOnDeath && bulletToSpawnDeath == null)
        {
            Debug.LogError("There is no Bullet to Spawn on Death for " + name);
        }

        trailControllers = GetComponentsInChildren<WeaponTrailController>();
        if (trailControllers.Length != 0)
        {
            hasTrail = true;
        }
        else
        {
            hasTrail = false;
        }

        #region Sound
        if (!String.IsNullOrEmpty(AmbientSound))
        {
            AmbientSoundEvent = RuntimeManager.CreateInstance(AmbientSound);
            AmbientSoundEvent.start();
        }

        if (!String.IsNullOrEmpty(SpawnSound))
        {
            RuntimeManager.PlayOneShot(SpawnSound,transform.position);
        }
        #endregion
    }


    protected virtual void FixedUpdate()
    {
        speed += acceleration * 0.1f;
        transform.Translate(speed * Time.deltaTime);
        automaticDamage();
    }

    protected virtual void Update()
    {
        updateAbmientSoundPosition();
    }

    public virtual void destroyBullet()
    {
        if (AmbientSound != null)
        {
            AmbientSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            AmbientSoundEvent.release();
        }

        //DeathSound
        if (!string.IsNullOrEmpty(DeathSound)) RuntimeManager.PlayOneShot(DeathSound, transform.position);


        if (deathEffect != null)
        {
            GameObject currentDeathEffect = Instantiate(deathEffect, transform.position, transform.rotation);
        }

        if (hasTrail)
        {
            foreach (WeaponTrailController controller in trailControllers)
            {
                controller.transform.SetParent(null);
                controller.selfDestruct = true;
            }
        }

        if (spawnBulletOnDeath)
        {
            GameObject currentBullet = Instantiate(bulletToSpawnDeath, transform.position, Quaternion.identity);
        }

        ScreenShake.Instance.ShakeScreen(screenShakeOnDeathIntensity, screenShakeOnDeathDuration); //<---------------------------------------------------------------------------------------ScreenShake

        Destroy(gameObject);
    }

    protected virtual void destroyBullet(RaycastHit2D hitInfo)
    {
        Debug.DrawLine(transform.position, hitInfo.point, Color.red, 5f);   

        transform.position = getDestroyPosition(hitInfo);
        destroyBullet();
    }

    /**
        <summary>
            Calculation to teleport the Bullet to the Hit Position without losing its Shot Trajectory (Move Direction)
        </summary>
    **/
    public Vector3 getDestroyPosition(RaycastHit2D hitInfo)
    {
        // Get Current Trajectory
        Vector3 currentTrajectory = new Vector3 (transform.forward.x, Vector2.Perpendicular(transform.forward).y, 0f);
        
        // Get Distance to Hit Position -> Augmented Trajectory (We Only Need the X)
        float trajectoryForHitPositionX = transform.position.x - hitInfo.point.x;

        // Convert Augmented Trajectory to Current (We Don't Include Augmented Trajectory Y so that we travel Straight)
        Vector3 destroyDistanceWithTrajectory = new Vector3 (currentTrajectory.x * trajectoryForHitPositionX, Vector2.Perpendicular(currentTrajectory).y, 0f);   

        // Add Current Trajectory (Distance) to Current Position (Giving New Position)
        Vector3 newPosition = transform.position + destroyDistanceWithTrajectory;

        return newPosition;
    }

    public virtual void automaticDamage()
    {
        if (destroyCollisions == null)
        {
            return;
        }

        if (!automaticDamageDisabled)
        {
            RaycastHit2D[] allHitInfo = Physics2D.BoxCastAll(bulletCollider.bounds.center, bulletCollider.bounds.size, 0f, transform.right, hitboxOffset, destroyCollisions.layerMask);

            if (allHitInfo.Length == 0) return;

            foreach (RaycastHit2D hitInfo in allHitInfo)
            {
                Health targetHealth = hitInfo.collider.GetComponentInParent<Health>();
                if (targetHealth != null)
                {
                    int targetID = targetHealth.gameObject.GetInstanceID();

                    try { damageTickSpeedTimers.Add(targetID, -0.05f); }
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    if (!canPierce)
                    {
                        applyHit(hitInfo);
                    }
                    else if (canPierce && damageTickSpeedTimers[targetID] < 0f)
                    {
                        applyHit(hitInfo);
                        damageTickSpeedTimers[targetID] = damageTickSpeedDurations;
                    }
                }
                else
                {
                    performOnHitProperty(hitInfo);
                    performCollisionProperty(hitInfo);
                }
            }

            foreach (RaycastHit2D hitInfo in allHitInfo)
            {
                Health targetHealth = hitInfo.collider.GetComponentInParent<Health>();
                if (targetHealth == null)
                {
                    continue;
                }

                int targetID = targetHealth.gameObject.GetInstanceID();

                try { damageTickSpeedTimers.Add(targetID, -0.05f); }
                catch (ArgumentException) 
                {
                    continue;
                }

                damageTickSpeedTimers[targetID] -= Time.deltaTime;
            }
        }

    }

    protected virtual void applyHit(RaycastHit2D hitInfo)
    {
        //Checking if your health are part of the list. if not add yourself 
        Health targetHealth = hitInfo.collider.GetComponentInParent<Health>();
        if (targetHealth != null)
        {
            if (targetHealth.attackers.list.Contains(tag) && !hasLandedAHit)
            {
                if (hitMarker != null)
                {
                    GameObject currentDamageEffect = Instantiate(hitMarker, transform.position, transform.rotation);

                    ScreenShake.Instance.ShakeScreen(screenShakeOnHitIntensity, screenShakeOnHitDuration);//<---------------------------------------------------------------------------------------ScreenShake
                }

                dealDamage(damage, targetHealth);
            }
        }

        Knockback targetKnockback = hitInfo.collider.GetComponentInParent<Knockback>();
        if (targetKnockback != null)
        {
            if (targetKnockback.attackers.list.Contains(tag) && !hasLandedAHit)
            {
                if (!knockbackForce.Equals(Vector2.zero))
                {
                    dealKnockback(targetKnockback.knockbackForce + knockbackForce, targetKnockback);
                }
            }
        }

        #region Hit Sound
        if (!String.IsNullOrEmpty(HitSound))
        {
            RuntimeManager.PlayOneShot(HitSound, transform.position);
        }
        #endregion

        performOnHitProperty(hitInfo);
        performCollisionProperty(hitInfo);
    }

    public void dealDamage(int damage, Health targetHealth)
    {
        targetHealth.changeHealth(-damage, owner);
        if (!targetHealth.invincible && damage > 0)
        {
            targetHealth.invincibilityFrames();
        }
    }

    public void dealKnockback(Vector2 knockbackForce, Knockback targetKnockback)
    {
        targetKnockback.applyKnockBackPoint(transform.position, knockbackForce);
        //print("Knockback " + targetKnockback.name + " with a force of " + knockbackForce + "\nKnockback Time: " + targetKnockback.knockbackDuration + knockbackTime);
        if (!targetKnockback.knockbackDisabled)
        {
            targetKnockback.knockbackFrames(targetKnockback.knockbackDuration + knockbackTime);
        }
    }

    protected virtual void performCollisionProperty(RaycastHit2D hitInfo)
    {
        if (canPierce && hitInfo.collider.tag != "Ground")
        {
            return;
        }
        else if (firingType == WeaponFireType.Melee)
        {
            if (hitInfo.collider.tag != "Ground")
            {
                hasLandedAHit = true;
                damage = 0;
            }
        }
        else
        {
            destroyBullet(hitInfo);
        }
    }

    protected void performOnHitProperty(RaycastHit2D hitInfo)
    {
        OnHitReciever targetEffectReciever = hitInfo.collider.GetComponentInParent<OnHitReciever>();
        if (targetEffectReciever != null)
        {
            if (onHitEffect != null && targetEffectReciever.attackers.list.Contains(tag))
            {
                targetEffectReciever.addEffect(onHitEffect);
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (bulletCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(bulletCollider.bounds.center.x + hitboxOffset, bulletCollider.bounds.center.y, bulletCollider.bounds.center.z), bulletCollider.bounds.size);
        }
    }

    //updates the positon of the sound in the world space
    private void updateAbmientSoundPosition()
    {
        abientSoundPositionUpdateTimer += Time.deltaTime;

        if (abientSoundPositionUpdateTimer >= abientSoundPositionUpdateRate)
        {
            // You can set the position based on your game object's position
            FMOD.ATTRIBUTES_3D attributes = RuntimeUtils.To3DAttributes(transform.position);
            AmbientSoundEvent.set3DAttributes(attributes);

            abientSoundPositionUpdateTimer = 0;
        }
    }
}
