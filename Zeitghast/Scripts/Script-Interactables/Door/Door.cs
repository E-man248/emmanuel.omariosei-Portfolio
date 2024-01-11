using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public abstract class Door : PortalDoor
{
    [Header("Door")]
    public bool restoreHealth;
    public bool keepInventory;
    public bool targetPositionIsACheckpoint;

    protected Transform playerTransfrom;
    protected PlayerHealth playerHealth;
    private PlayerOnHitReciever playerOnHitReciever;
    protected WeaponManager playerWeaponManager;
    protected Animator animator;

    [Header("Sound")]
    [EventRef][SerializeField] private string openSound;

    protected override void triggerEnteredAction(Collider2D collision)
    {
        base.triggerEnteredAction(collision);

        playerTransfrom = collision.transform;
        playerHealth = collision.GetComponent<PlayerHealth>();
        playerWeaponManager = collision.GetComponentInChildren<WeaponManager>();
        playerOnHitReciever = collision.GetComponent<PlayerOnHitReciever>();
    }

    protected override void interactAction()
    {
        //Play Sound
        if (!string.IsNullOrEmpty(openSound))
        {
            RuntimeManager.PlayOneShot(openSound, transform.position);
        }

        if (restoreHealth)
        {
            playerHealth.ResetHealth();
            playerOnHitReciever.removeAllEffects();
        }
        
        if (!keepInventory)
        {
            resetPlayerWeaponManager();
        }

        if(targetPositionIsACheckpoint)
        {
            setLastCheckpoint();
        }
    }

    public void removeLastCheckpoint()
    {
        if (playerHealth.lastCheckPointPosition.Equals(transform) && playerHealth.isDead)
        {
            return;
        }

        if (playerHealth.lastCheckPointPosition.checkpointScript != null)
        {
            Checkpoints lastCheckpoint = playerHealth.lastCheckPointPosition.checkpointScript;
            if (lastCheckpoint != null)
            {
                lastCheckpoint.removeCheckpoint();
            }
        }
    }

    protected virtual void setLastCheckpoint()
    {
        Debug.Log("Cannot use this function as this class is abstract");
    }

    protected virtual void resetPlayerWeaponManager()
    {
        playerWeaponManager.clearWeaponArsenal();
    }
}
