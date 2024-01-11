using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponFireType
{
    Ranged,
    Melee,
}

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName;
    public ShotType ShotType;
    public float weaponPostionOffest;
    public int damage;
    public int maxAmountOfBullets;
    [Space]
    [Space]
    public string weaponDisplayName;
    [TextArea] public string weaponDescription;

    [Header("Weapon States")]
    public bool isInWorld;
    public bool enteredWeaponManagerCollider;
    public bool canShoot;

    public WeaponManager weaponManager;
    private PlayerInput playerInput;
    protected Rigidbody2D weaponRigidbody;
    protected PolygonCollider2D weaponPhysicsCollider;

    [Header("Weapon Sounds")]
    [EventRef] public string AttackSound = null;
    [EventRef] public string DashAttackSound = null;
    [EventRef] public string PickUpSound = null;

    [Header("Dash Attack Stats")]
    public DashInfo dashAttack;
    protected List<int> enemiesLastHit;

    [Header("Wall Check Settings")]
    [SerializeField] private bool wallCheckEnabled = true;
    [SerializeField] private float wallCheckDistance = 2f;
    [SerializeField] private float wallCheckWidth = 0.2f;
    [SerializeField] private float wallCheckRadialOffset = 0f;

    [field: Header("Weapon UI Settings")]
    [field: SerializeField] public Sprite displaySprite { get; protected set; }
    public HotBarUIParameter hotBarUI;
    public HotBarUIParameter shopUI;
    [TextArea] public string weaponPickUpDescription = "Press [Fire] To Shoot";
    public Vector2 additionalLasersightOffset;

    [Header("Required Game Objects")]
    public Transform FiringPoint;
    public GameObject bullet;
    public GameObject muzzleFlash;
    public Vector2 muzzleFlashOffest;
    [Space]
    public GameObject glowEffect;
    private List<Bullet> listOfBullets;
    private WeaponAnimationHandler weaponAnimationHandler;
    
    [Header("Pick Up Prompt Settings")]
    [SerializeField] protected GameObject pickUpPrompt;
    private GameObject currentPickUpPrompt;
    [SerializeField] protected Vector2 pickUpPromptOffset;
    [SerializeField] protected float pickUpPromptAnimationLength;
    private Animator pickUpPromptAnimator;

    private StandardShotType standardShot;
    private ChargedShotType chargedShot;

    [Space]
    [HideInInspector] public LayerMaskObject bulletDestroyCollisions;
    protected WeaponWallCollisionDetection weaponWallCollision;

    private bool playedOverHeatSound;

    protected virtual void Awake()
    {
        if(string.IsNullOrEmpty(weaponName))
        {
            Debug.LogError("[Weapon Script] " + name + " has no weapon name filled in !!");
        }
        canShoot = true;
        isInWorld = true; //<----------------------------------------- Need to make this smarter so it know if in the world  
        gameComponentCheck();
        weaponRigidbody = GetComponent<Rigidbody2D>();
        weaponPhysicsCollider = GetComponent<PolygonCollider2D>();
        listOfBullets = new List<Bullet>();

        weaponWallCollision = GetComponentInChildren<WeaponWallCollisionDetection>();
        weaponAnimationHandler = GetComponentInChildren<WeaponAnimationHandler>();

        if (weaponAnimationHandler != null && weaponAnimationHandler.weaponSpriteRenderer != null)
        {
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (displaySprite == null && spriteRenderer != null) displaySprite = GetComponentInChildren<SpriteRenderer>().sprite;

            if (displaySprite == null) Debug.LogError(name + " could not find a displaySprite");
        }

        if (dashAttack != null)
        {
            enemiesLastHit = new List<int>();
        }

        ShotType = ShotType.clone();

        if (ShotType is StandardShotType)
        {
            standardShot = (StandardShotType)ShotType;
            standardShot.initalizeStandardShotType();
        }

        if (ShotType is ChargedShotType)
        {
            chargedShot = (ChargedShotType)ShotType;
            chargedShot.initalizeChargedShotType();
        }
    }

    public virtual void Start()
    {
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
        addweaponToSceneManagerDictionary();

        playerInput = PlayerInfo.Instance.GetComponent<PlayerInput>();
    }

    protected virtual void OnEnable()
    {
        if (chargedShot != null)
        {
            chargedShot.isCharging = false;
        }

        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
    }

    protected virtual void OnDisable()
    {
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;

        if (ShotType is ChargedShotType && chargedShot != null)
        {
            chargedShot.isCharging = false;
            chargedShot.currentChargeValue = 0f;
        }
    }

    protected virtual void OnDestroy()
    {
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;

        GameObjectScenePair tempPair = new GameObjectScenePair(gameObject, AdvancedSceneManager.Instance.getCurrentScene());
        AdvancedSceneManager.Instance.weaponDictionary.Remove(gameObject.GetInstanceID());
    }

    protected void LoadingScreenAction()
    {
        addweaponToSceneManagerDictionary();
        carryOverToNextScene();
    }

    private void addweaponToSceneManagerDictionary()
    {
        if (!isInWorld) return;

        //Check if we are already in the list
        if (AdvancedSceneManager.Instance.weaponDictionary.ContainsKey(gameObject.GetInstanceID())) return;

        GameObjectScenePair tempPair = new GameObjectScenePair(gameObject, AdvancedSceneManager.Instance.getCurrentScene());
        AdvancedSceneManager.Instance.weaponDictionary.Add(gameObject.GetInstanceID(), tempPair);
    }

    private void carryOverToNextScene ()
    {
        if (!isInWorld) return;

        //Carrying myself over to the next  scene
        transform.SetParent(AdvancedSceneManager.Instance.transform);
        gameObject.SetActive(false);
    }


    protected virtual void Update()
    {
        if (Timer.gamePaused)
        {
            return;
        }

        if (ShotType is StandardShotType)
        {
            standardShot.fireRateTimer -= Time.deltaTime;

            if (standardShot.hasOverheatMechanic)
            {
                overheatMechanic();//<--------------------------------------------------------------------------------------Overheat Mechanic
            }
        }

        if (ShotType is ChargedShotType)
        {
            chargedShot.baseFireRateTimer -= Time.deltaTime;
            if (chargedShot.isCharging)
            {
                if(!string.IsNullOrEmpty(chargedShot.chargingSound))
                {
                    if (!chargedShot.hasPlayedChargingSound)
                    {
                        chargedShot.chargingEvent = RuntimeManager.CreateInstance(chargedShot.chargingSound);
                        chargedShot.chargingEvent.start();
                        chargedShot.hasPlayedChargingSound = true;
                    }
                    chargedShot.chargePitchValue = Mathf.Clamp(chargedShot.currentChargeValue, 0, chargedShot.maxChargeValue) / chargedShot.maxChargeValue;
                    chargedShot.chargingEvent.setParameterByName("ChargeValue", chargedShot.chargePitchValue - chargedShot.maxChargePitchValue);
                } 
            }

            if (weaponManager != null && getPlayerShootInput() && !isInWorld)
            {
                unloadChargedShoot();
                chargedShot.currentChargeValue = 0;
                chargedShot.currentChargeIndex = -1;
                chargedShot.nextChargeIndex = 0;

                //Stop Charging Sound//
                if (chargedShot.chargingSound != "")
                {
                    chargedShot.chargingEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    chargedShot.chargingEvent.release();
                    chargedShot.hasPlayedChargingSound = false;
                }
            }

            if (isInWorld || weaponManager.currentWeapon != this)
            {
                chargedShot.isCharging = false;
                chargedShot.currentChargeValue = 0f;
            }
        }

        checkToTurnOnPhysics();
    
        glowing();

        displayPickUpPrompt();

        // Dash Attack Update:
        if (dashAttack != null)
        {
            DashAttack();
        }

        nullClearListOfBullets();
    }

    private bool getPlayerShootInput()
    {
        switch (playerInput.currentAimInputDevice)
        {
            case PlayerInput.AimInput.Mouse:
                return Input.GetButtonUp("Fire");
            case PlayerInput.AimInput.Joystick:
                return Input.GetAxisRaw("Fire") == 0;
            default:
                return false;
        }
    }


    /** <summary>
        Shooting function for the Charged Shot Type which
        adds gradual charge to the currentChargeValue of the 
        Charged Shot Type and updated the currentChargeIndex that
        corresponds to the level of charge achieved by the currentChargeValue
        </summary>
    **/
    public void ChargedShoot()
    {
        ChargedWeaponAnimationHandler chargedWeaponAnimationHandler = (ChargedWeaponAnimationHandler) weaponAnimationHandler;
        chargedShot.isCharging = true;

        // Charging Shot:
        if(canShoot && chargedShot.currentChargeValue >= chargedShot.damageSegments[chargedShot.nextChargeIndex].chargeTreshold)
        {
            chargedShot.currentChargeIndex = chargedShot.nextChargeIndex;
            if (chargedShot.currentChargeIndex < chargedShot.damageSegments.Count - 1)
            {
                chargedShot.nextChargeIndex = chargedShot.currentChargeIndex + 1;
            }
            if (weaponAnimationHandler != null) chargedWeaponAnimationHandler.levelChargedAnimation(chargedShot.currentChargeIndex);
        }
        chargedShot.currentChargeValue += Time.deltaTime * chargedShot.chargeRate;

        // Shoot at Base Fire Rate:
        if (chargedShot.currentChargeIndex < 0 && canShoot && bullet != null && chargedShot.baseFireRateTimer < 0f)
        {
            chargedShot.baseFireRateTimer = chargedShot.baseFireRate;
            
            if (weaponAnimationHandler != null)
            {
                weaponAnimationHandler.playAttackAnimation();
            }
            //Shoot Sound
            if(!string.IsNullOrEmpty(AttackSound))
            {
                RuntimeManager.PlayOneShot(AttackSound, transform.position);
            }

            if (canInstantiateBullet())
            {
                GameObject currentBullet = bullet;
                currentBullet = Instantiate(bullet, FiringPoint.position, FiringPoint.rotation);
                currentBullet.GetComponent<Bullet>().destroyCollisions = bulletDestroyCollisions;
                currentBullet.tag = "PlayerBullet";
                currentBullet.layer = 13;
                currentBullet.GetComponent<Bullet>().owner = "Player";

                listOfBullets.Add(currentBullet.GetComponent<Bullet>());

                currentBullet.GetComponent<Bullet>().damage = damage;

                if (currentBullet.GetComponent<Bullet>().firingType == WeaponFireType.Melee)
                {
                    currentBullet.transform.SetParent(transform);
                }
            }

            spawnMuzzleFlash();
        }
    }

    protected void spawnMuzzleFlash()
    {
        if (muzzleFlash != null && weaponManager)
        {
            Transform muzzleFlashParent = transform;
            if (weaponAnimationHandler != null) muzzleFlashParent = weaponAnimationHandler.transform;

            Vector3 newMuzzleFlashlocation = new Vector3(muzzleFlashOffest.x,muzzleFlashOffest.y, 0f);
            GameObject currentMuzzleFlash = Instantiate(muzzleFlash, muzzleFlashParent);

            currentMuzzleFlash.transform.localPosition = newMuzzleFlashlocation;
        }
    }
    /** <summary>
        Unleashes the charged bullet corresponding to the current
        level of charge indicated by the currentChargeIndex as an index
        to the damageSegments array.
        If the level of charge is less than 0, this indicates that there
        is no level of charge stored and so the function returns.
        </summary>
    **/
    public void unloadChargedShoot()
    {
        chargedShot.isCharging = false;

        if (weaponManager == null || weaponManager.playerHealth == null)
        {
            return;
        }

        if (weaponManager.playerHealth.isDead)
        {
            return;
        }

        if (!canShoot || chargedShot.currentChargeIndex < 0) return;

        ChargedWeaponAnimationHandler chargedWeaponAnimationHandler = (ChargedWeaponAnimationHandler) weaponAnimationHandler;

        if (weaponAnimationHandler != null) chargedWeaponAnimationHandler.levelUnloadAnimation(chargedShot.currentChargeIndex);
        //Shoot Sound
        if (AttackSound != null)
        {
            RuntimeManager.PlayOneShot(AttackSound, transform.position);
        }

        chargedShot.baseFireRateTimer = chargedShot.baseFireRate;

        if (canInstantiateBullet())
        {
            GameObject currentBullet = chargedShot.damageSegments[chargedShot.currentChargeIndex].bullet;
            currentBullet = Instantiate(currentBullet, FiringPoint.position, FiringPoint.rotation);
            currentBullet.GetComponent<Bullet>().destroyCollisions = bulletDestroyCollisions;
            currentBullet.tag = "PlayerBullet";
            currentBullet.layer = 13;
            currentBullet.GetComponent<Bullet>().owner = "Player";
            listOfBullets.Add(currentBullet.GetComponent<Bullet>());
            
            currentBullet.GetComponent<Bullet>().damage = chargedShot.damageSegments[chargedShot.currentChargeIndex].damage;

            if (currentBullet.GetComponent<Bullet>().firingType == WeaponFireType.Melee)
            {
                currentBullet.transform.SetParent(transform);
            }
        }

        spawnMuzzleFlash();
    }


    public void StandardShoot()
    {
        if (standardShot.canFire && standardShot.fireRateTimer <= 0 && canShoot)
        {
            //Checks if this weapon has Overheating and add to the current over heat amount whne firing 
            if (standardShot.hasOverheatMechanic)
            {
                standardShot.currentOverheatAmount += standardShot.overheatAddRate;
            }

            if (weaponAnimationHandler != null)
            {
                weaponAnimationHandler.playAttackAnimation();
            }
            //Attack Sound
            if (!string.IsNullOrEmpty( AttackSound ))
            {
                RuntimeManager.PlayOneShot(AttackSound, transform.position);
            }

            if (canInstantiateBullet())
            {
                GameObject currentBullet = Instantiate(bullet, FiringPoint.position, FiringPoint.rotation);
                currentBullet.GetComponent<Bullet>().destroyCollisions = bulletDestroyCollisions;
                currentBullet.tag = "PlayerBullet";
                currentBullet.layer = 13;
                currentBullet.GetComponent<Bullet>().owner = "Player";
                listOfBullets.Add(currentBullet.GetComponent<Bullet>());

                currentBullet.GetComponent<Bullet>().damage = damage;

                if (currentBullet.GetComponent<Bullet>().firingType == WeaponFireType.Melee)
                {
                    currentBullet.transform.SetParent(transform);
                }
            }

            spawnMuzzleFlash();

            standardShot.fireRateTimer = standardShot.fireRate;
        }
    }

    public void overheatMechanic()
    {
        standardShot = (StandardShotType)ShotType;
        //Checks if currentOverheatAmount has reach the cap and starts the Overheating phase 
        if (standardShot.currentOverheatAmount >= standardShot.overheatCap)
        {
            OverheatWeaponAnimationHandler overheatWeaponAnimationHandler = (OverheatWeaponAnimationHandler) weaponAnimationHandler;
            if (weaponAnimationHandler != null && !standardShot.isOverheating) overheatWeaponAnimationHandler.bustAnimation();

            standardShot.canFire = false;
            standardShot.isOverheating = true;
            if (!playedOverHeatSound)
            {
                //Playing overheat Sound 
                if(standardShot.OverheatSound != "")
                {
                    RuntimeManager.PlayOneShot(standardShot.OverheatSound, transform.position);
                    playedOverHeatSound = true;
                }                
            }
            
        }

        //The Overheating phase 
        if (standardShot.isOverheating)
        {
            //Reduces the currentOverheatAmount by overheatRecoveryRate over time 
            standardShot.currentOverheatAmount -= Time.deltaTime * standardShot.overheatSubtractRate;

            //Overheating phase is over
            if (standardShot.currentOverheatAmount <= 0)
            {
                standardShot.canFire = true;
                standardShot.isOverheating = false;
                standardShot.currentOverheatAmount = 0;
                playedOverHeatSound = false;

                playOverHeatReloadSound();
            }
        }

        //The Not shooting phase 
        if (!standardShot.isOverheating)
        {
            //Reduces the currentOverheatAmount by overheatRecoveryRate over time 
            standardShot.currentOverheatAmount -= Time.deltaTime * standardShot.overheatSubtractRate;

            //Caps the currentOverheatAmount  at 0
            if (standardShot.currentOverheatAmount <= 0)
            {
                standardShot.currentOverheatAmount = 0;
            }
        }
    }

    /** <summary>
        This function fires a bullet by the shooting function of specified Shot Type.
        If the amount of bullets fired by the weapon is greater than the
        maxAmountOfBullets then it destroy the oldest bullet that it fired.
        </summary>
    **/
    public virtual void shoot()
    {
        if (weaponManager == null || weaponManager.playerHealth == null)
        {
            return;
        }

        if (weaponManager.playerHealth.isDead)
        {
            return;
        }
        
        if (ShotType is StandardShotType)
        {
            StandardShoot();
        }

        if (ShotType is ChargedShotType)
        {
            ChargedShoot();
        }

        if (maxAmountOfBullets != 0 && listOfBullets.Count > maxAmountOfBullets)
        {
            Bullet doomedBullet = listOfBullets[0];
            listOfBullets.RemoveAt(0);
            doomedBullet.destroyBullet();
        }
    }

    public bool canInstantiateBullet()
    {
        return !wallCheckEnabled || (wallCheckEnabled && !isInWall());
    }

    private bool isInWall()
    {
        Vector3 playerPosition = weaponManager.transform.position; // Ensure player transform is correct for crouching too...
        Vector3 firingPointPosition = FiringPoint.transform.position;

        Vector3 wallCheckDirection = (firingPointPosition - playerPosition).normalized;
        Vector3 wallCheckBoxSize = new Vector3(0.05f, wallCheckWidth, 0f);
        Vector3 calulatedWallCheckRadialOffset = wallCheckDirection * wallCheckRadialOffset;

        var hitWall = Physics2D.BoxCast(playerPosition + calulatedWallCheckRadialOffset, wallCheckBoxSize, 0f, wallCheckDirection, wallCheckDistance, weaponManager.weaponWallCheckCollisions.layerMask);

        return hitWall.collider != null;
    }

    public bool isFiring()
    {
        if (ShotType is StandardShotType)
        {
            return standardShot.fireRateTimer > 0;
        }

        else if (ShotType is ChargedShotType)
        {
            return chargedShot.baseFireRateTimer > 0;
        }

        else return false;
    }

    public void nullClearListOfBullets()
    {
        for (int i = listOfBullets.Count - 1; i > -1; i--)
        {
            Bullet potentialNull = listOfBullets[i];
            if (potentialNull == null)
            {
                listOfBullets.Remove(potentialNull);
            }
        }
    }

    /*
       This function instatiates a dashSlash for the player's dash attack
       when the player has a melee weapon, is dashing, and has hit an enemy 
    */
    public void DashAttack()
    {
        if (weaponManager == null || weaponManager.currentWeapon != this)
        {
            return;
        }

        if (weaponManager.dashAttackActivated())
        {
            //Looking for a parent with a health script and returning it's transform 
            EnemyHealth enemyHealth = weaponManager.playerHealth.getEnemyInContact().GetComponentInParent<EnemyHealth>();
            Transform enemyTransform = enemyHealth.transform;
            int enemyGameObjectID = enemyTransform.GetInstanceID();
           
            if (enemiesLastHit.Contains(enemyGameObjectID) || !enemyHealth.attackers.list.Contains(dashAttack.spawnBulletInDashCollision.bulletToSpawn.tag))
            {
                return;
            }
            else
            {
                if (enemiesLastHit.Count == 0)
                {
                    weaponManager.playerInput.extendDashTime(weaponManager.playerInput.getStartDashTime()/2);
                    weaponManager.playerInput.triggerResetDashWhenCurrentDashComplete();
                }

                enemiesLastHit.Add(enemyGameObjectID);

                //Dash Attack instantiation 
                Vector3 slashRotation = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(weaponManager.playerInput.dashDirection.normalized.y, weaponManager.playerInput.dashDirection.normalized.normalized.x) - 180);
                Vector3 enemyPosition = enemyTransform.GetComponent<Collider2D>().bounds.center;
                GameObject spawn = Instantiate(dashAttack.spawnBulletInDashCollision.bulletToSpawn, enemyPosition, Quaternion.Euler(slashRotation));

                if (dashAttack.spawnBulletInDashCollision.parentOfSpawn == DashInfo.parentOfSpawnInfo.Player)
                {
                    spawn.transform.position = weaponManager.playerTransform.position;
                    spawn.transform.parent = weaponManager.playerTransform;
                }
                else if (dashAttack.spawnBulletInDashCollision.parentOfSpawn == DashInfo.parentOfSpawnInfo.WeaponFiringPoint)
                {
                    spawn.transform.position = FiringPoint.position;
                    spawn.transform.parent = FiringPoint;
                }
                spawn.GetComponent<Bullet>().owner = "Player";

                // Check if Spawn was a Bullet:
                Bullet spawnBulletScript = spawn.GetComponent<Bullet>();
                if (spawnBulletScript != null) spawnBulletScript.destroyCollisions = weaponManager.bulletDestroyCollisions;

                if(!string.IsNullOrEmpty(DashAttackSound ))
                {
                    RuntimeManager.PlayOneShot(DashAttackSound, transform.position);
                }
            }
        }

        if (!weaponManager.playerInput.isDashing)
        {
            enemiesLastHit.Clear();
        }
    }

    public void flipY()
    {
        gameObject.transform.localEulerAngles = new Vector3(-180f, 0f, 0f);
    }

    public void unFlipY()
    {
        gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "WeaponManager" && isInWorld)
        {
            enteredWeaponManagerCollider = true;
            weaponManager = collision.GetComponent<WeaponManager>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "WeaponManager" && isInWorld)
        {
            enteredWeaponManagerCollider = false;
            weaponManager = collision.GetComponent<WeaponManager>();
        }
    }

    private void gameComponentCheck()
    {
        if (FiringPoint == null)
        {
            Debug.LogError(gameObject.name + " FiringPoint not set");
            FiringPoint = gameObject.transform;
        }

        if (bullet == null)
        {
            Debug.LogError(gameObject.name + " bullet not set");
        }
    }

    private void checkToTurnOnPhysics()
    {
        if (isInWorld)
        {
            weaponRigidbody.bodyType = RigidbodyType2D.Dynamic;
            weaponRigidbody.WakeUp();
            weaponPhysicsCollider.enabled = true;
        }
        else
        {
            weaponRigidbody.bodyType = RigidbodyType2D.Kinematic;
            weaponRigidbody.Sleep();
            weaponPhysicsCollider.enabled = false;
        }
    }

    public void pickUpWeapon(bool playPickUpSound = true)
    {
        weaponManager.addWeapon(this, playPickUpSound);
    }

    public void glowing()
    {

        if (enteredWeaponManagerCollider && isInWorld)
        {
            if (weaponManager != null && weaponManager.getSelectedPickUpWeaponGameID() == GetInstanceID())
            {
                glowEffect.SetActive(true);
            }
        }
        else
        {
            
            glowEffect.SetActive(false);
        }
    }

    public void displayPickUpPrompt()
    {
        //null Check
        if (pickUpPrompt == null)
        {
            return;
        }

        //Setting up the position
        Vector3 pickUpPromptPosition;
        void settingPosition()
        {
            float xPosition = transform.position.x + pickUpPromptOffset.x;
            float yPosition = transform.position.y + pickUpPromptOffset.y;
            pickUpPromptPosition = new Vector3(xPosition, yPosition, transform.position.z);
        }

        //Creating and setting up the object if it doesn't exist
        if (currentPickUpPrompt == null)
        {
            currentPickUpPrompt = Instantiate(pickUpPrompt, Vector3.zero, Quaternion.identity);
            currentPickUpPrompt.SetActive(false);

            //Getting it's animator
            if (pickUpPromptAnimator == null)
            {
                pickUpPromptAnimator = currentPickUpPrompt.GetComponent<Animator>();
            }
        }

        //Checking if the player is near and then displaying if so
        if (enteredWeaponManagerCollider && isInWorld)
        {
            if (weaponManager != null && weaponManager.getSelectedPickUpWeaponGameID() == GetInstanceID())
            {
                settingPosition();
                currentPickUpPrompt.transform.position = pickUpPromptPosition;
                pickUpPromptAnimator.SetBool("playerOutOfRange", false);
                currentPickUpPrompt.SetActive(true);
            }
        }
        else
        {
            //Skiping work if the object has already been set inactive
            if (!currentPickUpPrompt.activeSelf)
            {
                return;
            }

            pickUpPromptAnimator.SetBool("playerOutOfRange", true);
            Invoke("hidePickUpPrompt", pickUpPromptAnimationLength);
        }
    }

    public void hidePickUpPrompt()
    {
        currentPickUpPrompt.SetActive(false);
    }

    public Sprite getSprite()
    {
        if(displaySprite != null)
        {
            return displaySprite;
        }
        else if (weaponAnimationHandler != null && displaySprite == null)
        {
            return weaponAnimationHandler.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            SpriteRenderer weaponSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            return weaponSpriteRenderer.sprite;
        }
    }

    public void playPickUpSound()
    {
        //Pick up Sound 
        if (!string.IsNullOrEmpty(PickUpSound))
        {
            RuntimeManager.PlayOneShot(PickUpSound, transform.position);
        }
    }
   

    private void playOverHeatReloadSound()
    {
        if (!string.IsNullOrEmpty(standardShot.OverheatReloadSound))
        {
            RuntimeManager.PlayOneShot(standardShot.OverheatReloadSound, transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        // Debug Wall Check:
        
        if (wallCheckEnabled)
        {
            DrawWallCheckGizmos();
        }
    }

    private void DrawWallCheckGizmos()
    {
        if (weaponManager == null) return;

        if (FiringPoint == null) return;

        if (isInWorld) return;
        
        Vector3 playerPosition = weaponManager.transform.position; // Ensure player transform is correct for crouching too...
        Vector3 firingPointPosition = FiringPoint.transform.position;

        Vector3 wallCheckDirection = (firingPointPosition - playerPosition).normalized;

        Vector3 calculatedWallCheckDistanceOffset = wallCheckDirection * wallCheckDistance;
        Vector3 calculatedWallCheckWidthOffset = Vector2.Perpendicular(wallCheckDirection) * wallCheckWidth / 2;
        Vector3 calulatedWallCheckRadialOffset = wallCheckDirection * wallCheckRadialOffset;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(playerPosition + calulatedWallCheckRadialOffset + calculatedWallCheckWidthOffset, playerPosition + calculatedWallCheckDistanceOffset + calulatedWallCheckRadialOffset + calculatedWallCheckWidthOffset);
        Gizmos.DrawLine(playerPosition + calulatedWallCheckRadialOffset - calculatedWallCheckWidthOffset, playerPosition + calculatedWallCheckDistanceOffset + calulatedWallCheckRadialOffset - calculatedWallCheckWidthOffset);
    }
}

[System.Serializable]
public struct HotBarUIParameter
{
    public Vector3 Position;
    public Vector3 Scale;
    public Vector3 Rotation;
}