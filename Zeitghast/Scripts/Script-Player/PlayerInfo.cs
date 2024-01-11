using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance = null;

    [field: SerializeField] public float enemy_AI_Update_Distance {get; private set;} = 60f;

    public Action<Vector3> SetPlayerPosition;
    public Action<Vector3> ResetPlayerAndPosition;

    public static UnityEvent PlayerDeathEvent;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (PlayerDeathEvent == null)
        {
            PlayerDeathEvent = new UnityEvent();
        }

        SetPlayerPosition += (targetPosition) => 
        {
            transform.position = targetPosition;
        };

        ResetPlayerAndPosition += (targetPosition) =>
        {
            Instance.SetPlayerPosition(targetPosition);
            Instance.ResetPlayer(targetPosition);
        };
    }

    public void PlayerDeathEventTrigger()
    {
        if (PlayerDeathEvent == null)
        {
            PlayerDeathEvent = new UnityEvent();
        }

        PlayerDeathEvent.Invoke();
    }

    private void Start()
    {
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
    }

    protected virtual void OnEnable()
    {
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
    }

    protected virtual void OnDisable()
    {
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    protected virtual void OnDestroy()
    {
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    protected void LoadingScreenAction()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(this.gameObject);
    }

    public void ResetPlayer(Vector3 lastcheckPointPosition, bool clearWeaponArsenal = true)
    {
        // Retrieve Resources:
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        PlayerKnockback playerKnockback = GetComponent<PlayerKnockback>();
        PlayerOnHitReciever playerOnHitReciever = GetComponent<PlayerOnHitReciever>();
        WeaponManager weaponManager = GetComponentInChildren<WeaponManager>();
        PlayerInput playerMovement = GetComponent<PlayerInput>();
        PlayerHatHolder playerHatHolder = GetComponentInChildren<PlayerHatHolder>();

        if (playerHealth != null)
        {
            // Reset Player Health:
            playerHealth.Reset();

            // Set Last Player Checkpoint:
            playerHealth.resetLastCheckPointPosition(lastcheckPointPosition);
        }

        if (playerKnockback != null)
        {
            // Reset Player Knockback:
            playerKnockback.Reset();
        }

        if (playerOnHitReciever != null)
        { 
            // Re-Enable On Hit Effect Recieving:
            playerOnHitReciever.Reset();
        }

        if (weaponManager != null) 
        {
            // Reset Player Weapon Manager:
            weaponManager.Reset();

            if (clearWeaponArsenal)
            {
                // Clear Weapon Arsenal:
                weaponManager.clearWeaponArsenal();
            }
        }

        if (playerMovement != null)
        {
            // Reset Player Movement:
            playerMovement.Reset();  
        }

        if (playerHatHolder != null)
        {
            // Set Player Hat to Recent Saved Player Hat:
            var recentPlayerHat = HatManager.Instance.GetRecentPlayerHat();
            playerHatHolder.Equip(recentPlayerHat);
        }

    }
}
