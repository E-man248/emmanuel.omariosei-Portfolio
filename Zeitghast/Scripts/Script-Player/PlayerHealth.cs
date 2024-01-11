using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct lastCheckPointObject
{
    public string lastScene;
    public Vector3 position;
    public Checkpoints checkpointScript;
}

public class PlayerHealth : Health
{
    public LayerMaskObject enemyLayerMask;
    public float contactDistance;
    private WeaponManager weaponManager;
    public Collider2D playerCollider;
    public bool debugOn;
    public playerAnimationHandler playerAnimationHandler;
    
    private long playerPauseDisableKey;
    
    internal lastCheckPointObject lastCheckPointPosition;

    [Header("Respawn Settings")]
    public float respawnInvincibilityTime = 0.5f;
    public float additionalRespawnDelay;
    private float respawnDelayTimer;
    [SerializeField] protected TransitionType respawnTransitionIn;
    [SerializeField] protected TransitionType respawnTransitionOut;
    private bool isRespawning = false;

    public event Action onHurt;

    #region Unity Functions

    /** 
     * Start Function inherited from parent
    */
    protected override void Start()
    {
        base.Start();
        GetComponent<Movement>().entityName = "Player";
        weaponManager = GetComponentInChildren<WeaponManager>();
        playerCollider = GetComponent<PlayerInput>().entityCollider;
        playerAnimationHandler = GetComponentInChildren<playerAnimationHandler>();
        
        if (lastCheckPointPosition.position == null || string.IsNullOrEmpty(lastCheckPointPosition.lastScene))
        {
            resetLastCheckPointPosition(transform.position);
        }

        isRespawning = false;

        subscribeToEvents();
        SceneLoaded();
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
        SceneManager.sceneLoaded += SceneLoaded;

        PlayerInfo.PlayerDeathEvent.AddListener(OnPlayerDeath);
    } 

    private void unsubscribeToEvents()
    {
        SceneManager.sceneLoaded -= SceneLoaded;

        PlayerInfo.PlayerDeathEvent.RemoveListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        trueInvincible = true;
        invincible = true;
    }

    #endregion

    protected void SceneLoaded()
    {
        if (healthBarPrefab != null && GetComponentInChildren<HealthBar>() == null)
        {
            GameObject prefab = Instantiate(healthBarPrefab, transform);
            prefab.transform.localPosition = healthBarOffset;
            prefab.GetComponent<HealthBar>().health = this;
            isDead = false;
        }
    }

    protected void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (healthBarPrefab != null && GetComponentInChildren<HealthBar>() == null)
        {
            GameObject prefab = Instantiate(healthBarPrefab, transform);
            prefab.transform.localPosition = healthBarOffset;
            prefab.GetComponent<HealthBar>().health = this;
            isDead = false;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (debugOn && enemyContact())
        {
            Debug.Log("Enemy Contact: " + enemyContact());
        }

        if (isRespawning)
        {
            RespawnCycle();
        }
    }

    /**
        <summary>
        Follows with the inherited 'changeHealth()' function, but
        changes the functionality to be dependent on if 'dashAttackActivated'
        from the player 'WeaponManager' is true. If so, the health value will
        NOT be changed. 
        </summary>
    **/
    public override void changeHealth(int healthAdded, string lastHitBy = null)
    {
        if (!isDead)
        {
            if (weaponManager.dashAttackActivated())
            {
                return;
            }

            // Check if Health was Lost or Gained:

            if ((health + healthAdded) > health) 
            {
                // Display Healing Number
                spawnHealNumber(healthAdded, transform);
            }
            else if ((health + healthAdded) < health && !invincible)
            {
                // Display Damage Number
                spawnHurtNumber(healthAdded, transform);
                
                //Broadcast hurt
                onHurt.Invoke();
            }


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

                if (lastHitBy != null || !lastHitBy.Equals(""))
                {
                    this.lastHitBy = lastHitBy;
                }
            }
        }
    }

    /**
        <summary>
        Follows with the inherited 'invincibilityFrames()' function, but
        changes the functionality to be dependent on if 'dashAttackActivated'
        from the player 'WeaponManager' is true. If so, the invincibility timer
        will NOT be reset.
        </summary>
    **/
    public override void invincibilityFrames()
    {
        if (weaponManager.dashAttackActivated())
        {
            return;
        }

        if (invicibilityTimer <= 0)
        {
            invicibilityTimer = invincibilityDuration;
        }
    }

    /**
        <summary>
        Follows with the inherited 'invincibilityFrames()' function, but
        changes the functionality to be dependent on if 'dashAttackActivated'
        from the player 'WeaponManager' is true. If so, the invincibility timer
        will NOT be reset.
        </summary>
    **/
    public override void invincibilityFrames(float time)
    {
        if (weaponManager.dashAttackActivated())
        {
            return;
        }

        if (invicibilityTimer <= 0)
        {
            invicibilityTimer = time;
        }
    }

    public void resetInvincibilityFrames(float time = 0f)
    {
        invicibilityTimer = time;
    }

    /** 
     *  <summary>
     *  Returns true if the player has contact with an object in enemyLayerMask
     *  </summary>
    */
    public bool enemyContact()
    {
        if(getEnemyInContact() != null)
        {
            return true;
        }

        return false;
    }

    /** 
     *  <summary>
     *  Returns the enemy a object has contact with that is in enemyLayerMask.
     *  If there is no contact, the function returns null.
     *  There is a bias towards the object that is closest to the player.
     *  </summary>
    **/
    public Transform getEnemyInContact()
    {
        RaycastHit2D hitLeft = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.left, contactDistance, enemyLayerMask.layerMask);
        RaycastHit2D hitRight = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.right, contactDistance, enemyLayerMask.layerMask);

        Transform contactedObject = null;

        // Figures out which collider has the closest object hit:
        if (hitLeft.collider != null && hitRight.collider != null)
        {
            contactedObject = hitLeft.collider.transform;

            float distanceToLeftObject = Vector3.Distance(hitLeft.collider.transform.position, transform.position);
            float distanceToRightObject = Vector3.Distance(hitRight.collider.transform.position, transform.position);

            if (distanceToLeftObject < distanceToRightObject)
            {
                contactedObject = hitRight.collider.transform;
            }
        }
        else if (hitLeft.collider != null)
        {
            contactedObject = hitLeft.collider.transform;
        }
        else if (hitRight.collider != null)
        {
            contactedObject = hitRight.collider.transform;
        }

        return contactedObject;
    }

    /// <summary> Death of the Player. Player Death Event is Called and Player is Prepared for Respawn. </summary>
    public override void die() 
    {
        if (isDead)
        {
            return;
        }

        base.die();

        // Stop Timer:
        Timer.Instance.stopTimer();

        // Disable Player Pause:
        playerPauseDisableKey = PauseMenuUI.Instance.TryDisablePlayerPause();
        
        // Death Event Is Called: (All Attached Processes Can Now Respond)
        PlayerInfo.Instance.PlayerDeathEventTrigger();

        // Play Death Animation:
        playDeathAnimation();

        // Calculate and Set Time Till Respawn:
        float deathAnimationLength = getDeathAnimationLength();
        respawnDelayTimer = deathAnimationLength + additionalRespawnDelay;

        isRespawning = true;
    }

    private void playDeathAnimation()
    {
        string deathAnimationString = playerAnimationHandler.deathAnimationString;

        playerAnimationHandler.playAnimationOnceFull(deathAnimationString);
    }

    private float getDeathAnimationLength()
    {
        string deathAnimationString = playerAnimationHandler.deathAnimationString;
        
        float deathAnimationLength = playerAnimationHandler.getAnimationLength(deathAnimationString);

        return deathAnimationLength;
    }

    public void RespawnCycle()
    {
        if (respawnDelayTimer > 0f)
        {
            respawnDelayTimer -= Time.deltaTime;
            return;
        }

        isRespawning = false; // Setting this Boolean False Ends the Cycle By End of this Frame!

        // Respawn the Player:
        RespawnPlayer(lastCheckPointPosition.lastScene);
    }

    public void RespawnPlayer(string spawnScene)
    {
        bool isSameScene = AdvancedSceneManager.Instance.isSameScene(spawnScene);

        Action PostTransitionAction = () => 
        {
            // Enable Player Pause:
            PauseMenuUI.Instance.TryEnablePlayerPause(playerPauseDisableKey, 0);            

            // Ask Level Manager to Play Respawn Sequence:
            LevelManager.Instance.PlayPlayerRespawnSequence();
        };

        if (isSameScene)
        {
            AdvancedSceneManager.Instance.inSceneTransition(respawnTransitionIn, TransitionType.LoadingScreen, PostTransitionAction);
        }
        else
        {
            AdvancedSceneManager.Instance.loadScene(spawnScene, respawnTransitionIn, TransitionType.LoadingScreen, PostTransitionAction);
        }
    }

    public void resetLastCheckPointPosition()
    {
        lastCheckPointPosition.position = transform.position;
        lastCheckPointPosition.lastScene = AdvancedSceneManager.Instance.getCurrentScene();
        lastCheckPointPosition.checkpointScript = null;
    }

    public void resetLastCheckPointPosition(Vector3 targetPosition)
    {
        lastCheckPointPosition.position = targetPosition;
        lastCheckPointPosition.lastScene = AdvancedSceneManager.Instance.getCurrentScene();
        lastCheckPointPosition.checkpointScript = null;
    }

    public void setLastCheckPointPosition(Vector3 targetPosition,string targetSceneName,Checkpoints targetCheckpoint = null)
    {
        lastCheckPointPosition.position = targetPosition;
        lastCheckPointPosition.lastScene = targetSceneName;
        lastCheckPointPosition.checkpointScript = targetCheckpoint;
    }

    public Vector3 getLastCheckPointPosition()
    {
        return lastCheckPointPosition.position;
    }

    public void Reset()
    {
        // Re-Enable Player Damage:
        trueInvincible = false;
        resetInvincibilityFrames();

        // Revert Death State:
        isDead = false;

        // Restore Player Health:
        ResetHealth();

        // Reset Last Checkpoint Position:
        resetLastCheckPointPosition();
    }

    /**
     * <summary>
     * Draws Debug Gizmos
     * </summary>
    */
    private void OnDrawGizmos()
    {
        if (debugOn)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(new Vector3(playerCollider.bounds.center.x + contactDistance, playerCollider.bounds.center.y, playerCollider.bounds.center.z), playerCollider.bounds.size);
            Gizmos.DrawWireCube(new Vector3(playerCollider.bounds.center.x - contactDistance, playerCollider.bounds.center.y, playerCollider.bounds.center.z), playerCollider.bounds.size);
        }
    }
}
