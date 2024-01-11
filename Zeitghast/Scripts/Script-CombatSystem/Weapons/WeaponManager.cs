using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using System;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Position Settings")]
    public int rotationOffest = 90;
    private float weaponPositionOffest = 2f;

    [Header("Inventory")]
    public Weapon arsenalSlot1 = null;
    public Weapon arsenalSlot2 = null;
    public Weapon currentWeapon;
    [SerializeField] protected List<weaponGameObject> weaponsInPickUpRange;

    [Header("Aim and Interact Settings")]
    public Transform playerTransform;
    public bool canAim;
    public bool canInteractWithWeapons;
    public Vector3 currentAimDirection {get; private set;}
    
    [Header("Target Prediction Settings")]
    [SerializeField] protected float targetPredictionDistance = 5f;
    public Transform predictedTarget {get; private set;}
    public Vector3 predictionAimPoint {get; private set;}
    
    [Header("Aim Assist Settings")]
    public bool debugAimAssist = false; // Safely Remove
    [SerializeField] protected Vector2 aimAssistCastSize;
    [SerializeField] private TagList aimAssistTargetTagsToIgnore;
    public Transform currentAimAssistTarget {get; private set;} = null;

    [Header("Crouch Settings")]
    public float crouchingOffset;

    [Header("Player Access")]
    public PlayerInput playerInput;
    public PlayerHealth playerHealth;
    public LayerMaskObject bulletDestroyCollisions;

    [Header("Sound")]
    [EventRef] public string selectSound = null;
    public float selectSoundDelay = 0.1f;
    private float selectSoundDelayTimer;
    public float pickupSoundDelay = 0.1f;
    private float pickupSoundDelayTimer;

    [Header("UI")]
    [SerializeField] private float pickTextDuration = 1.7f;
    [SerializeField] private float pickTextfadeIn = 0.5f;
    [SerializeField] private float pickTextfadeOut = 0.5f;

    [Header("Scroll Wheel Setting")]
    [SerializeField] private float scrollWheelSpeed = 0.2f;
    private float scrollWheelTimer;
    [Header("Weapon Wall Check Settings")]
    
    [SerializeField] public LayerMaskObject weaponWallCheckCollisions;

    [System.Serializable]
    protected struct weaponGameObject
    {
        public int gameObjectID;
        public Weapon weapon;
    }

    private void Awake()
    {
        canAim = true;
        canInteractWithWeapons = true;
        playerHealth = playerTransform.GetComponent<PlayerHealth>();
        playerInput = playerTransform.GetComponent<PlayerInput>();
    }

    private void Start()
    {
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
        PlayerInfo.PlayerDeathEvent?.AddListener(OnPlayerDeath);
    }

    private void unsubscribeToEvents()
    {
        PlayerInfo.PlayerDeathEvent?.RemoveListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        canAim = false;
        canInteractWithWeapons = false;
    }

    #endregion

    private void Update()
    {
        // Pause when game paused:
        if (Timer.gamePaused)
        {
            return;
        }

        checkToshoot();

        // Update and Store Player Aim Input:
        UpdatePlayerAimInput();

        // Orient Physical Weapon with Player Aim:
        if(canAim && !currentWeaponEmpty() && dashingWithDashAttackWeapon())
        {
            Vector3 dashAttackRotation = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(playerInput.dashDirection.normalized.y, playerInput.dashDirection.normalized.normalized.x));
            centerWeapon();
            transform.eulerAngles = dashAttackRotation;
        }
        else if(canAim)
        {
            // Handle Player Target Point:
            orientateWeapon(currentAimDirection);
        }

        // Update and Store Current Aim Predictions:
        UpdateAimPredictions();

        setCurrentWeaponVisibility(canAim);

        correctWeaponRotation();
        
        weaponSwitching();

        // Pick Up Weapon:
        if (Input.GetButtonDown("Pick Up Weapon") && weaponsInPickUpRange.Count > 0)
        {
            // Remove Weapon:
            int selectedGameObjectID = weaponsInPickUpRange[0].gameObjectID;
            Weapon selectedWeapon = weaponsInPickUpRange[0].weapon;
            selectedWeapon.pickUpWeapon();
            int weaponIndex = GetWeaponFromPickUpList(selectedGameObjectID);
            if (weaponIndex != -1) weaponsInPickUpRange.RemoveAt(0);
        }

        soundDelayTimerTick();
    }

    private void setCurrentWeaponVisibility(bool value)
    {
        if (currentWeapon == null) return;

        currentWeapon.gameObject.SetActive(value);
    }

    private void correctWeaponRotation()
    {
        //Get the Z rotation of the weapon manager 
        float zRotation = transform.rotation.eulerAngles.z;

        if (currentWeaponEmpty())
        {
            //Do Nothing 
        }
        else if (zRotation > 90 && zRotation < 270)
        {
            currentWeapon.flipY();
        }
        else
        {
            currentWeapon.unFlipY();
        }
    }

    private void centerWeapon()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f + rotationOffest);
        currentWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void setDefaultWeapons()
    {
        GameObject defaultArsenalSlot1 = null;
        GameObject defaultArsenalSlot2 = null;

        // Check for Level Default Weapons
        if (LevelManager.Instance != null)
        {
            defaultArsenalSlot1 = LevelManager.Instance.defaultPlayerArsenalSlot1;
            defaultArsenalSlot2 = LevelManager.Instance.defaultPlayerArsenalSlot2;
        }

        // Set Default Weapons:
        if (defaultArsenalSlot1 != null)
        {
            GameObject defaultWeapon1 = Instantiate(defaultArsenalSlot1, transform.position, transform.rotation);
            Weapon defaultWeapon1Script = defaultWeapon1.GetComponent<Weapon>();
            if (defaultWeapon1Script == null)
            {
                Debug.LogError("Default Arsenal Slot 1's Weapon is NOT a Weapon!");
                return;
            }

            defaultWeapon1Script.weaponManager = this;
            defaultWeapon1Script.pickUpWeapon(false);
            playerInput.setPlayerDashInfo(defaultWeapon1Script.dashAttack);
        }

        if (defaultArsenalSlot2 != null)
        {
            GameObject defaultWeapon2 = Instantiate(defaultArsenalSlot2, transform.position, transform.rotation);
            Weapon defaultWeapon2Script = defaultWeapon2.GetComponent<Weapon>();
            if (defaultWeapon2Script == null)
            {
                Debug.LogError("Default Arsenal Slot 2's Weapon is NOT a Weapon!");
                return;
            }

            defaultWeapon2Script.weaponManager = this;
            defaultWeapon2Script.pickUpWeapon(false);
            playerInput.setPlayerDashInfo(defaultWeapon2Script.dashAttack);
        }

        // Set Defaults for Player Having No Weapons:
        if (defaultArsenalSlot1 == null && defaultArsenalSlot1 == null)
        {
            playerInput.setPlayerDashInfo(null);
        }
    }

    private void UpdatePlayerAimInput()
    {
        // Get Raw Aim Direction:
        Vector3 calculatedAimDirection = getRawAimDirection();

        // Apply Aim Assist to Aim Direction:
        if (!calculatedAimDirection.Equals(Vector3.zero))
        {
            calculatedAimDirection = getAssistedAimDirection(calculatedAimDirection);
        }

        // Update Current Aim:
        currentAimDirection = calculatedAimDirection;
    }

    private void orientateWeapon(Vector3 aimDirection)
    {
        float rotationZ = math.atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // If AngleShotWeapon, Snap to AngleShotWeapon Fixed Rotation Angle:
        if (!currentWeaponEmpty() && currentWeapon is AngleShotWeapon)
        {
            float snapToAngle = ((AngleShotWeapon) currentWeapon).findSnapToAngle(aimDirection);
            if (snapToAngle < float.MaxValue)
            {
                rotationZ = snapToAngle;
            }
        }

        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + rotationOffest);

        if (!currentWeaponEmpty())
        {
            //Shifts the weapon Manager down if the player is crouching
            if (playerInput.isCrouching)
            {
               transform.localPosition = new Vector3(0f, crouchingOffset, 0f);
            }
            else
            {
                transform.localPosition = new Vector3(0f, 0, 0f);
            }

            currentWeapon.transform.localPosition = new Vector3(weaponPositionOffest, 0f, 0f);
        }
    }

    public Vector3 getRawAimDirection()
    {
        if (playerInput.currentAimInputDevice == PlayerInput.AimInput.Mouse)
        {
            return getAimDirectionFromMouse();
        }
        else
        {
            return getAimDirectionFromJoystick();
        }
    }

    private Vector3 getAimDirectionFromMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 difference = mousePosition - transform.position;
        difference.Normalize();

        return difference;
    }
    
    public Vector3 getAimDirectionFromJoystick()
    {
        Vector3 baseAimDirection = playerInput.getJoystickAimInput();
        if (baseAimDirection.Equals(Vector3.zero)) baseAimDirection = new Vector3(playerInput.lookDirection, 0f, 0f);
        baseAimDirection.Normalize();

        return baseAimDirection;
    }

    public void addWeapon(Weapon weapon, bool playSound  = true)
    {
        weapon.isInWorld = false;

        if (arsenalSlot1 == null)
        {
            arsenalSlot1 = weapon;
            arsenalSlot1.transform.SetParent(transform);
            arsenalSlot1.gameObject.SetActive(false);
            setUpWeapon(arsenalSlot1);
            switchToSlot(1);
        }
        else if (arsenalSlot2 == null)
        {
            arsenalSlot2 = weapon;
            arsenalSlot2.transform.SetParent(transform);
            arsenalSlot2.gameObject.SetActive(false);
            setUpWeapon(arsenalSlot2);
            switchToSlot(2);
        }
        else
        {
            if (currentWeapon == arsenalSlot1)
            {
                dropWeapon(arsenalSlot1);
                arsenalSlot1 = weapon;
                arsenalSlot1.transform.SetParent(transform);
                arsenalSlot1.gameObject.SetActive(false);
                setUpWeapon(arsenalSlot1);
                switchToSlot(1);
            }
            else if (currentWeapon == arsenalSlot2)
            {
                dropWeapon(arsenalSlot2);
                arsenalSlot2 = weapon;
                arsenalSlot2.transform.SetParent(transform);
                arsenalSlot2.gameObject.SetActive(false);
                setUpWeapon(arsenalSlot2);
                switchToSlot(2);
            }
        }

        //Playing pick up sound
        if(pickupSoundDelayTimer <= 0f && playSound)
        {
            pickupSoundDelayTimer = pickupSoundDelay;
            weapon.playPickUpSound();

            //Display Weapon discrption on screen 
            LocationNameManager.Instance.displayMainandSubTextUI(weapon.weaponDisplayName, weapon.weaponPickUpDescription, pickTextDuration, pickTextfadeIn, pickTextfadeOut);
        }
    }

    private void dropWeapon(Weapon weapon)
    {
        if (currentWeaponEmpty() || arsenalEmpty())
        {
            return;
        }

        if (arsenalSlot1 == weapon)
        {
            //Debug.Log("drop slot 1");

            arsenalSlot1.transform.position = transform.position;
            arsenalSlot1.transform.eulerAngles = Vector3.zero;

            arsenalSlot1.gameObject.SetActive(true);
            arsenalSlot1.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(arsenalSlot1.gameObject, SceneManager.GetActiveScene());

            arsenalSlot1.isInWorld = true;
            arsenalSlot1 = null;
        }
        else if (arsenalSlot2 == weapon)
        {
            //Debug.Log("drop slot 2");

            arsenalSlot2.transform.position = transform.position;
            arsenalSlot2.transform.eulerAngles = Vector3.zero;

            arsenalSlot2.gameObject.SetActive(true);
            arsenalSlot2.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(arsenalSlot2.gameObject, SceneManager.GetActiveScene());

            arsenalSlot2.isInWorld = true;
            arsenalSlot2 = null;
        }
        
        if (arsenalSlot1 != null)
        {
            switchToSlot(1);
        }
        else if (arsenalSlot2 != null)
        {
            switchToSlot(2);
        }
        else
        {
            currentWeapon = null;
            playerInput.setPlayerDashInfo(null);
        }
    }

    private void checkToshoot()
    {
        if ((Input.GetButton("Fire") || Input.GetAxisRaw("Fire") > 0) && canAim && !currentWeaponEmpty() && !dashingWithDashAttackWeapon())
        {
            currentWeapon.shoot();
        }
    }

    /** <summary>
        Returns true if dashAttack is active and false otherwise
        </summary>
    **/
    public bool dashAttackActivated()
    {
        if (currentWeapon == null)
        {
            return false;
        }

        if (currentWeapon.dashAttack == null || currentWeapon.dashAttack.spawnBulletInDashCollision.bulletToSpawn == null || !playerHealth.enemyContact())
        {
            return false;
        }

        EnemyHealth enemyHealth = playerHealth.getEnemyInContact().GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null && !enemyHealth.attackers.list.Contains(currentWeapon.dashAttack.spawnBulletInDashCollision.bulletToSpawn.tag))
        {
            return false;
        }

        if (playerInput.isDashing && currentWeapon.dashAttack != null && currentWeapon.dashAttack.spawnBulletInDashCollision.bulletToSpawn != null && playerHealth.enemyContact() && currentWeapon.canShoot)
        {
            return true;
        }

        return false;
    }

    public bool dashingWithDashAttackWeapon()
    {
        if (currentWeaponEmpty())
        {
            return false;
        }

        return currentWeapon.dashAttack != null && currentWeapon.dashAttack.spawnBulletInDashCollision.bulletToSpawn != null && playerInput.isDashing;
    }

    public bool currentWeaponIsFiring()
    {
        if (currentWeaponEmpty())
        {
            return false;
        }
        
        return currentWeapon.isFiring();
    }

    private void switchToSlot(int currentSlot)
    {
        if (getCurrentWeaponSlot() == currentSlot) return;

        if (currentSlot == 1)
        {
            //switch animation
            if(arsenalSlot2 != null)
            {
                arsenalSlot2.gameObject.SetActive(false);
            }
            currentWeapon = arsenalSlot1;
            if (!currentWeaponEmpty())
            {
                currentWeapon.gameObject.SetActive(true);
                setUpWeapon(currentWeapon);
            }
            else
            {
                playerInput.setPlayerDashInfo(null);
            }
        }
        else if (currentSlot == 2)
        {
            //switch animation
            if (arsenalSlot1 != null)
            {
                arsenalSlot1.gameObject.SetActive(false);
            }
            currentWeapon = arsenalSlot2;
            if (!currentWeaponEmpty())
            {
                currentWeapon.gameObject.SetActive(true);
                setUpWeapon(currentWeapon);
            }
            else
            {
                playerInput.setPlayerDashInfo(null);
            }
        }
        else
        {
            Debug.LogError("Incorrect slot number input! (Only 1 or 2)");
        }
    }

    private void setUpWeapon(Weapon weapon)
    {
        weaponPositionOffest = weapon.weaponPostionOffest;

        weapon.transform.localPosition = new Vector3(weaponPositionOffest, 0f, 0f);
        weapon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        weapon.bulletDestroyCollisions = bulletDestroyCollisions;

        playerInput.setPlayerDashInfo(weapon.dashAttack);
    }

    public void clearWeaponArsenal()
    {
        GameObject doomedWeapon1;
        GameObject doomedWeapon2;
        
        if (arsenalSlot1 != null)
        {
            doomedWeapon1 = arsenalSlot1.gameObject;
            dropWeapon(arsenalSlot1.GetComponent<Weapon>());
            Destroy(doomedWeapon1);
        }
        arsenalSlot1 = null;

        if (arsenalSlot2 != null)
        {
            doomedWeapon2 = arsenalSlot2.gameObject;
            dropWeapon(arsenalSlot2.GetComponent<Weapon>());
            Destroy(doomedWeapon2);
        }
        arsenalSlot2 = null;

        setDefaultWeapons();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canInteractWithWeapons || !collision.tag.Equals("Weapon")) return;
        
        Weapon weapon = collision.GetComponent<Weapon>();
        if (weapon == null) return;

        if (!weapon.enteredWeaponManagerCollider) return;

        if (weapon.transform.parent == transform) return;
        
        int weaponIndex = GetWeaponFromPickUpList(weapon.GetInstanceID());
        if (weaponIndex == -1)
        {
            weaponGameObject weaponObject = new weaponGameObject();
            weaponObject.weapon = weapon;
            weaponObject.gameObjectID = weapon.GetInstanceID();
            weaponsInPickUpRange.Add(weaponObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!canInteractWithWeapons || !collision.tag.Equals("Weapon")) return;
        
        Weapon weapon = collision.GetComponent<Weapon>();
        if (weapon != null)
        {
            int weaponIndex = GetWeaponFromPickUpList(weapon.GetInstanceID());
            if (weaponIndex != -1) weaponsInPickUpRange.RemoveAt(weaponIndex);
        }
    }

    private int GetWeaponFromPickUpList(int gameObjectID)
    {
        int foundIndex = -1;

        if (weaponsInPickUpRange.Count <= 0) return foundIndex;

        for (int index = 0; index < weaponsInPickUpRange.Count; index++)
        {
            if (gameObjectID.Equals(weaponsInPickUpRange[index].gameObjectID))
            {
                foundIndex = index;
                break;
            }
        }

        return foundIndex;
    }

    private void playSelectSound()
    {
        //Select Sound
        if (!string.IsNullOrEmpty(selectSound) && selectSoundDelayTimer <= 0f)
        {
            selectSoundDelayTimer = selectSoundDelay;
            RuntimeManager.PlayOneShot(selectSound, transform.position);
        }
    }

    private void soundDelayTimerTick()
    {
        selectSoundDelayTimer -= Time.deltaTime;
        pickupSoundDelayTimer -= Time.deltaTime;
    }

    public int getSelectedPickUpWeaponGameID()
    {
        if (weaponsInPickUpRange.Count <= 0) return 0;

        return weaponsInPickUpRange[0].gameObjectID;
    }

    private int getCurrentWeaponSlot()
    {
        if (currentWeapon == arsenalSlot1)
        {
            return 1;
        }
        else if (currentWeapon == arsenalSlot2)
        {
            return 2;
        }
        else
        {
            return -1;
        }
    }

    private bool currentWeaponEmpty()
    {
        if (currentWeapon == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool arsenalEmpty()
    {
        if (arsenalSlot1 == null && arsenalSlot2 == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public  bool arsenalFilled()
    {
        if (arsenalSlot1 != null && arsenalSlot2 != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void weaponSwitching()
    {
        if (Input.GetButtonDown("Slot One") && arsenalSlot1 != null)
        {
            playSelectSound();
            switchToSlot(1);
        }
        if (Input.GetButtonDown("Slot Two") && arsenalSlot2 != null)
        {
            playSelectSound();
            switchToSlot(2);
        }

        buttonWeaponSwitching();

        scrollWheelWeaponSwitching();
    }

    public void scrollWheelWeaponSwitching()
    {
        scrollWheelTimer -= Time.deltaTime;

        if (scrollWheelTimer > 0f) return;

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            scrollWheelTimer = scrollWheelSpeed;

            cycleWeaponSelectedWeapon();
        }
    }

    private void buttonWeaponSwitching()
    {
        //Weapon switching using R key
        if(Input.GetKeyDown(KeyCode.R))
        {
            cycleWeaponSelectedWeapon();

            playSelectSound();
        }
    }

    private void cycleWeaponSelectedWeapon()
    {
        if (getCurrentWeaponSlot() == 2 && arsenalSlot1 != null)
        {
            switchToSlot(1);
        }
        else if (getCurrentWeaponSlot() == 1 && arsenalSlot2 != null)
        {
            switchToSlot(2);
        }
    }

    private float getAimAssistStrength()
    {
        return DataPersistanceManager.Instance.GetOptionsData().aimAssistStrengthValue;

    }

    private Vector3 getAssistedAimDirection(Vector3 baseAimDirection)
    {
        
        return (getRawAimAssistVector(baseAimDirection) * getAimAssistStrength() + baseAimDirection).normalized;
    }


    protected Vector3 getRawAimAssistVector(Vector3 baseAimDirection)
    {
        currentAimAssistTarget = getAimAssistTarget(baseAimDirection);

        if (currentAimAssistTarget != null)
        {
            Debug.DrawRay(transform.position, baseAimDirection.normalized, Color.magenta);

            Vector3 aimAssistDirection = currentAimAssistTarget.position - transform.position;
            aimAssistDirection.Normalize();

            return aimAssistDirection;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Transform getAimAssistTarget(Vector3 baseAimDirection)
    {
        RaycastHit2D[] allHitInfo = Physics2D.CircleCastAll(transform.position, aimAssistCastSize.y, baseAimDirection.normalized, aimAssistCastSize.x, bulletDestroyCollisions.layerMask);

        currentAimAssistTarget = null;

        if (allHitInfo.Length > 0)
        {
            foreach (RaycastHit2D hitInfo in allHitInfo)
            {
                if (aimAssistTargetTagsToIgnore.list.Contains(hitInfo.collider.tag)) continue;

                Health targetHealth = hitInfo.collider.GetComponentInParent<Health>();
                if (targetHealth != null && targetHealth.attackers.list.Contains("PlayerBullet"))
                {
                    return hitInfo.collider.transform;
                }
            }
        }

        return null;
    }

    private void UpdateAimPredictions()
    {
        // Make Initial Prediction Using Current Aim Direction:
        predictionAimPoint = transform.position + currentAimDirection.normalized * targetPredictionDistance;
        float initialPredictionDistance = Vector2.Distance(transform.position, predictionAimPoint);

        // Check For Raycast Hits In Current Aim Direction:
        var hitInfo = Physics2D.Raycast(transform.position, currentAimDirection, targetPredictionDistance, bulletDestroyCollisions.layerMask);

        // If Raycast Hit In Current Aim Direction, Update Prediction Point If Smaller (Closer) than Initial Prediction:
        if (hitInfo)
        {
            float hitDistance = Vector2.Distance(transform.position, hitInfo.point);
            predictedTarget = hitInfo.transform;

            if (hitDistance < initialPredictionDistance)
            {
                predictionAimPoint = hitInfo.point;
            }
        }
    }

    protected void OnDrawGizmos()
    {  
        if (!Application.IsPlaying(this))
        {
            return;
        }

        if (debugAimAssist)
        {
            Gizmos.color = Color.cyan;
            if (currentAimAssistTarget != null) Gizmos.DrawRay(transform.position, currentAimAssistTarget.position - transform.position);
            Gizmos.DrawWireSphere(transform.position, aimAssistCastSize.y);
        }
    }

    /// <summary>
    /// Resets all Weapon Manager states to their initial state.<br/>
    /// NOTE: Does NOT Clear Weapon arsenal. Weapons in the Weapon Manager arsenal are retained but their states are reset.
    /// </summary>
    public void Reset()
    {
        // Enable Weapon Manager Aim:
        canAim = true;

        // Enable Weapon Manager Interaction:
        canInteractWithWeapons = true;

        // Reset states of current arsenal Weapons:

    }
}
