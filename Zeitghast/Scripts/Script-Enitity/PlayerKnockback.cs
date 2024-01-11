using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : Knockback
{
    private PlayerInput playerInput;
    private WeaponManager weaponManager;

    [Header("Player Knockback")]
    [Header("Player Immovability")]
    private float immovabilityTimer;
    public float immovabilityDuration;
    private PlayerHealth playerHealth;

    protected override void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        
        weaponManager = GetComponentInChildren<WeaponManager>();

        if (playerInput == null) Debug.LogError("No 'PlayerInput' Script in 'PlayerKnockback' Script for " + name);
        if (weaponManager == null) Debug.LogError("No 'WeaponManager' Script in 'PlayerKnockback' Script for " + name);
        playerHealth = GetComponent<PlayerHealth>();
        
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
        trueImmobility = true;
        knockbackDisabled = true;
    }

    #endregion

    protected override void Update()
    {
        if (!trueImmobility)
        {
            if (knockbackTimer >= 0 && immovabilityTimer >=0 )
            {
                knockbackDisabled = true;
                stunned = true;
                hasBeenReset = false;
            }
            else if(knockbackTimer < 0 && !playerHealth.isDead)
            {
                if (immovabilityTimer >= 0)
                {
                    knockbackDisabled = true;
                    stunned = false;
                    if (!hasBeenReset) afterknockbackReset();
                }
                else
                {
                    knockbackDisabled = false;
                    if (!hasBeenReset) afterknockbackReset();
                }
                immovabilityTimer -= Time.deltaTime;
            }
        }
        knockbackTimer -= Time.deltaTime;
    }

    public override void knockbackFrames()
    {
        if (weaponManager.dashAttackActivated())
        {
            return;
        }

        if (immovabilityTimer < 0)
        {
            knockbackTimer = knockbackDuration;
            immovabilityTimer = immovabilityDuration;
        }

        if (playerInput.isDashing) playerInput.dashReset();

        if (playerInput != null && !knockbackDisabled)
        {
            playerInput.setControlsDisable(true);
        }
    }

    public override void knockbackFrames(float time)
    {
        if (weaponManager.dashAttackActivated())
        {
            return;
        }

        if (immovabilityTimer < 0)
        {
            knockbackTimer = time;
            immovabilityTimer = immovabilityDuration;
        }
        
        if (playerInput.isDashing) playerInput.dashReset();

        if (playerInput != null && !knockbackDisabled)
        {
            playerInput.setControlsDisable(true);
        }
    }

    public void knockbackFrames(float time, float immovabilityDuration)
    {  
        if (weaponManager.dashAttackActivated())
        {
            return;
        }

        if (immovabilityTimer < 0)
        {
            knockbackTimer = time;
            immovabilityTimer = immovabilityDuration;
        }
        
        if (playerInput.isDashing) playerInput.dashReset();

        if (playerInput != null && !knockbackDisabled)
        {
            playerInput.setControlsDisable(true);
        }
    }

    public void resetImmovabilityFrames(float time = 0f)
    {
        immovabilityTimer = time;
    }

    protected override void afterknockbackReset()
    {
        if (playerInput != null)
        {
            playerInput.setControlsDisable(false);
        }
        hasBeenReset = true;
    }

    public void Reset()
    {
        // Re-Enable Player Damage:
        trueImmobility = false;
        resetImmovabilityFrames();
    }

    public override void applyKnockBackDirection(int launchDirection)
    {
        if (weaponManager.dashAttackActivated())
        {
            return;
        }

        if (playerInput.isDashing) playerInput.dashReset();
        base.applyKnockBackDirection(launchDirection);
    }

    public override void applyKnockBackDirection(int launchDirection, Vector2 knockbackForce)
    {
        if (weaponManager.dashAttackActivated())
        {
            return;
        }

        if (playerInput.isDashing) playerInput.dashReset();
        base.applyKnockBackDirection(launchDirection, knockbackForce);
    }
}
