using DG.Tweening;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class AntagBoss : Boss
{
    public enum AttackState
    {
        AimedLaserAttack,
        RockThrow,
        FlyStomp,
        CreateTimeRifts
    }

    private AntagBossAnimationHandler animationHandler;
    internal AntagBossState currentAntagBossAttackState;

    [Header("Antag Boss Settings:")]
    [SerializeField] private Transform LeftStartingPosition;
    [SerializeField] private Transform RightStartingPosition;

    [Header("Idle Settings:")]
    [SerializeField] private float maxIdleDuration;
    [SerializeField] private float minIdleDuration;
    private float idleTimer;

    #region Aimed Laser Wind Up Settings
    [Header("Aimed Laser Attack Settings:")]
    [SerializeField] private int laserDamage;
    [SerializeField] private Vector2 knockbackForce = Vector2.zero;
    [SerializeField] private float knockbackTime = 0f;

    [Space]
    [SerializeField] private float maxLaserDistance;
    [SerializeField] private int minAimedLaserAttackAmount;
    [SerializeField] private int maxAimedLaserAttackAmount;
    private int aimedLaserAttackAmount;

    // Laser Shooting Utilities:
    [Space]
    [SerializeField] private Laser attackLaser;
    [SerializeField] private Laser telegraphLaser;

    [Space]
    [SerializeField] private Transform aimingLaserControl;
    private Transform LaserShootTarget;
    
    private bool hasInitiatedAimedLaserAttackWindUp;
    [SerializeField] private float aimedLaserAttackWindUpDuration;
    public float aimedLaserAttackWindUpTimer { get; private set; }

    // Aimed Laser Aiming:
    private bool hasInitiatedAimedLaserAttackAiming;
    [SerializeField] private float aimedLaserAttackAimingDuration;
    public float aimedLaserAttackAimingTimer { get; private set; }

    // Aimed Laser Ease In:
    private bool hasInitiatedAimedLaserAttackEaseIn;
    public float aimedLaserAttackEaseInTimer { get; private set; }


    // Aimed Laser Shooting:
    private bool hasInitiatedAimedLaserAttackShooting;
    [SerializeField] private float aimedLaserAttackShootingDuration;
    public float aimedLaserAttackShootingTimer { get; private set; }

    // Aimed Laser Shooting:
    private bool hasInitiatedAimedLaserAttackEaseOut;
    public float aimedLaserAttackEaseOutTimer { get; private set; }
    #endregion

    #region Rock Throw Settings
    [Header("Rock Throw Settings:")]
    [SerializeField] private float rockThrow_WindUpDuration;
    [SerializeField] private float rockSpawnDelay = 0.17f;
    private bool hasSpawnedRocks;

    //Rock throw Windup
    private bool hasInitiatedRockThrow_WindUp;
    public float rockThrow_WindUpTimer { get; private set; }

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject rocks;

    //Rock Throwing
    private bool hasInitiatedRockThrowing;
    [SerializeField] private float rockThrowingDuration;
    public float rockThrowingTimer { get; private set; }

    [Header("Create Time Rift Settings:")]
    [SerializeField] private GameObject createTimeRiftsSpawnPortal;
    [Space]
    [SerializeField] private float createTimeRiftsMinXSpawnPosition = -20f;
    [SerializeField] private float createTimeRiftsMaxXSpawnPosition = 20f;

    #endregion

    #region Flying Stomp Attack Settings
    [Header("Flying Stomp Settings:")]

    [SerializeField] private int minFlyingStompAmount;
    [SerializeField] private int maxFlyingStompAmount;
    private int FlyingStompAmount;

    //Flying Stomp Windup
    private bool hasInitiatedFlyingStompWindUp;
    public float FlyingStompWindUpTimer { get; private set; }

    //Flying Stomp Fly Up
    private bool hasInitiatedFlyingStompFlyUp;
    public bool flyingStompFlyUpComplete { get; private set; }
    [SerializeField] private Transform FlyingStompFlyUpTransform;
    [SerializeField] private float FlyUpDuration = 2f;
    private float FlyingStompFlyUpStartingYPosition;

    //Flying Stomp Tracking
    private bool hasInitiatedFlyingStompTracking;
    public float FlyingStompTrackingTimer { get; private set; }
    [SerializeField] private float FlyingStompTrackingDuration = 4f;
    [SerializeField] private float FlyingStompTrackingSpeed = 1f;
    [Space]
    [SerializeField] private GameObject FlyingStompTrackingShadow;
    [SerializeField] private float FlyingStompTrackingShadowDelay = 1f;
    private bool FlyingStompTrackingShadowEnabled  = false;
    [Space]

    private Vector2 trackingGroundPoint;
    [SerializeField] private LayerMaskObject trackingGroundLayerMask;
    [SerializeField] private float trackingRaycastlength = 100f;
    private Tween flyingStompTrackingTween;


    //Flying Stomp Fly Down
    private bool hasInitiatedFlyingStompFlyDown;
    public bool flyingStompFlyDownComplete { get; private set; }
    [SerializeField] private float FlyDownDuration = 2f;
    [SerializeField] private float flyingStompFlyDownStoppingDistance = 0f;

    //Flying Stomp Landing
    private bool hasInitiatedFlyingStompLanding;
    public float FlyingStompLandingTimer { get; private set; }

    //Flying Stomp CoolDown
    private bool hasInitiatedFlyingStompCoolDown;
    public float FlyingStompCoolDownTimer { get; private set; }
    [SerializeField] private float FlyingStompCoolDownDuration;

    #endregion

    // Create Time Rifts Summoning:
    private bool hasInitiatedCreateTimeRiftsSummoning;
    [SerializeField] private float createTimeRiftsSummoningDuration;
    public float createTimeRiftsSummoningTimer { get; private set; }
    private bool hasSummonedCreateTimeRiftsEnemies;
    private float createTimeRiftsSummonDelay = 0f;

    #region Teleport Utilities

    private bool isTeleporting = false;

    #endregion

    #region Attack Cycle Utilities

    private bool HasSetupAttackState = false;


    #endregion


    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();

        // Retrieve Animation Handler:
        animationHandler = GetComponentInChildren<AntagBossAnimationHandler>();

        setUpLasers();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Idle State Cycle

    protected override void IdleStateEntry()
    {
        idleTimer = UnityEngine.Random.Range(minIdleDuration, maxIdleDuration + 1);
    }

    protected override void RunIdleState()
    {
        base.RunIdleState();

        idleTimer -= Time.deltaTime;

        if (idleTimer < 0)
        {
            SwitchState(State.Attack);
            return;
        }
    }

    protected override void IdleStateExit()
    {
        
    }

    #endregion

    #region Antag Attack Cycle

    protected void AntagAttackStateEntry()
    {
        HasSetupAttackState = true;

        if (currentAntagBossAttackState.state == AttackState.AimedLaserAttack) 
        {
            SetupAimedLaserAttack();
        }
        else if (currentAntagBossAttackState.state == AttackState.RockThrow)
        {
            SetupRockThrow();
        }
        else if (currentAntagBossAttackState.state == AttackState.FlyStomp)
        {
            SetupFlyingStomp();
        }
        else if (currentAntagBossAttackState.state == AttackState.CreateTimeRifts)
        {
            SetupCreateTimeRiftsAttack();
        }
    }

    protected bool IsValidateAntagAttackState(BossAttackState bossAttackState)
    {
        if (bossAttackState == null)
        {
            return false;
        }

        if (!(bossAttackState is AntagBossState))
        {
            return false;
        }

        return true;
    }

    protected override void RunCurrentAttackState(BossAttackState bossAttackState)
    {
        if (!HasSetupAttackState)
        {
            // Validate Antag Boss State:
            if (!IsValidateAntagAttackState(bossAttackState))
            {
                Debug.LogError("Invalid Antag Boss State " + bossAttackState?.ToString());
                AntagAttackStateExit();
                return;
            }

            // Convert State to Antag State:
            currentAntagBossAttackState = (AntagBossState) bossAttackState;

            AntagAttackStateEntry();
            return;
        }

        // Run Attack State Update:
        AntagAttackStateUpdate();
    }

    protected void AntagAttackStateUpdate()
    {
        if (currentAntagBossAttackState.state == AttackState.AimedLaserAttack) 
        {
            AimedLaserAttackUpdate();
        }
        else if (currentAntagBossAttackState.state == AttackState.RockThrow)
        {
            RockThrowUpdate();
        }
        else if (currentAntagBossAttackState.state == AttackState.FlyStomp)
        {
            FlyingStompUpdate();
        }
        else if (currentAntagBossAttackState.state == AttackState.CreateTimeRifts)
        {
            CreateTimeRiftsAttackUpdate();
        }
    }

    protected void AntagAttackStateExit()
    {
        if (currentAntagBossAttackState.state == AttackState.AimedLaserAttack) 
        {
            CleanUpAimedLaserAttack();
        }
        else if (currentAntagBossAttackState.state == AttackState.RockThrow)
        {
            CleanUpRockThrow();
        }
        else if (currentAntagBossAttackState.state == AttackState.FlyStomp)
        {
            CleanUpFlyingStomp();
        }
        else if (currentAntagBossAttackState.state == AttackState.CreateTimeRifts)
        {
            CleanUpCreateTimeRiftsAttack();
        }

        HasSetupAttackState = false;

        // Call to Cycle Enemy Complete:
        CurrentStateComplete();

        // Switch to Idle After Any Attack State is Complete:
        SwitchState(State.Idle);
    }

    #endregion

    #region Boss Attack State

    protected override void AttackStateEntry()
    {
        
    }

    protected override void AttackStateExit()
    {
        
    }

    #endregion

    #region Aimed Laser Attack

    private void SetupAimedLaserAttack()
    {
        // Generate Random Attack Amount:
        aimedLaserAttackAmount = UnityEngine.Random.Range(minAimedLaserAttackAmount, maxAimedLaserAttackAmount + 1);

        // Reset Aimed Laser Attack Variables:
        ResetAimedLaserAttack();

        if (!IsFacingPlayer())
        {
            // Teleport Antag to Starting Position: (If Needed)
            Vector3 newStartingPosition = GetFurthestStartingPositionFromPlayer();
            StartTeleport(newStartingPosition, () => AimedLaserAttackMidTeleportAction());
        }
        else
        {
            // Continue to Attack without Teleporting
        }
    }

    private void AimedLaserAttackMidTeleportAction()
    {
        // Orientate Antag to Look at Player:
        orientateToPlayer();
    }

    private void AimedLaserAttackUpdate()
    {
        // If Teleporting, Wait Till Complete:
        if (isTeleporting)
        {
            return;
        }

        // Initiate Windup:
        if (!hasInitiatedAimedLaserAttackWindUp)
        {
            hasInitiatedAimedLaserAttackWindUp = true;
            initiateAimedLaserAttackWindup();
            return;
        }
        // Windup Cycle:
        if (aimedLaserAttackWindUpTimer >= 0)
        {
            aimedLaserAttackWindUpTimer -= Time.deltaTime;
            aimedLaserAttackWindupCycle();
            return;
        }

        // Initiate Aiming:
        if (!hasInitiatedAimedLaserAttackAiming)
        {
            hasInitiatedAimedLaserAttackAiming = true;
            initiateAimedLaserAttackAiming();
            return;
        }
        // Aiming Cycle:
        if (aimedLaserAttackAimingTimer >= 0)
        {
            aimedLaserAttackAimingTimer -= Time.deltaTime;
            aimedLaserAttackAimingCycle();
            return;
        }

        // Initiate Ease In:
        if (!hasInitiatedAimedLaserAttackEaseIn)
        {
            hasInitiatedAimedLaserAttackEaseIn = true;
            initiateAimedLaserAttackEaseIn();
            return;
        }
        // Ease In Cycle:
        if (aimedLaserAttackEaseInTimer >= 0)
        {
            aimedLaserAttackEaseInTimer -= Time.deltaTime;
            aimedLaserAttackEaseInCycle();
            return;
        }

        // Initiate Shooting:
        if (!hasInitiatedAimedLaserAttackShooting)
        {
            hasInitiatedAimedLaserAttackShooting = true;
            initiateAimedLaserAttackShooting();
            return;
        }
        // Shooting Cycle:
        if (aimedLaserAttackShootingTimer >= 0)
        {
            aimedLaserAttackShootingTimer -= Time.deltaTime;
            aimedLaserAttackShootingCycle();
            return;
        }

        // Initiate Ease Out:
        if (!hasInitiatedAimedLaserAttackEaseOut)
        {
            hasInitiatedAimedLaserAttackEaseOut = true;
            initiateAimedLaserAttackEaseOut();
            return;
        }
        // Ease Out Cycle:
        if (aimedLaserAttackEaseOutTimer >= 0)
        {
            aimedLaserAttackEaseOutTimer -= Time.deltaTime;
            aimedLaserAttackEaseOutCycle();
            return;
        }

        aimedLaserAttackAmount--;
        
        if (aimedLaserAttackAmount > 0)
        {
            ResetAimedLaserAttack();
            return;
        }
        
        AntagAttackStateExit();
    }

    // Wind Up:
    private void initiateAimedLaserAttackWindup()
    {
        //Animation
        animationHandler.playAimedLaserAttackWindupAnimation();
    }

    private void aimedLaserAttackWindupCycle()
    {
        
    }

    private float GetAimedLaserAttackWindupAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.AimedLaserAttackWindupAnimationString);
    }

    // Aiming:
    private void initiateAimedLaserAttackAiming()
    {
        InitiateLaserShootCycle(PlayerInfo.Instance.transform);
        animationHandler.playAimedLaserAttackAimingAnimation();
    }

    private void aimedLaserAttackAimingCycle()
    {
        aimLaserArm();
        telegraphLaser.gameObject.SetActive(true);
    }

    private float GetAimedLaserAttackAimingAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.AimedLaserAttackAimingAnimationString);
    }

    // Ease In:
    private void initiateAimedLaserAttackEaseIn()
    {
        telegraphLaser.gameObject.SetActive(false);
        animationHandler.playAimedLaserAttackEaseInAnimation();
    }

    private void aimedLaserAttackEaseInCycle()
    {

    }

    private float GetAimedLaserAttackEaseInAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.AimedLaserAttackEaseInAnimationString);
    }
    
    // Shooting:
    private void initiateAimedLaserAttackShooting()
    {
        attackLaser.gameObject.SetActive(true);
        animationHandler.playAimedLaserAttackShootingAnimation();
    }

    private void aimedLaserAttackShootingCycle()
    {
        return;
    }

    private float GetAimedLaserAttackShootingAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.AimedLaserAttackShootingAnimationString);
    }

    // Ease Out:
    private void initiateAimedLaserAttackEaseOut()
    {
        attackLaser.gameObject.SetActive(false);
    }

    private void aimedLaserAttackEaseOutCycle()
    {
        return;
    }

    private float GetAimedLaserAttackEaseOutAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.AimedLaserAttackEaseOutAnimationString);
    }

    private void ResetAimedLaserAttack()
    {
        // Reset Windup:
        hasInitiatedAimedLaserAttackWindUp = false;
        aimedLaserAttackWindUpTimer = Mathf.Max(aimedLaserAttackWindUpDuration, GetAimedLaserAttackWindupAnimationDuration());
        telegraphLaser.gameObject.SetActive(false);
        
        // Reset Aiming:
        hasInitiatedAimedLaserAttackAiming = false;
        aimedLaserAttackAimingTimer = Mathf.Max(aimedLaserAttackAimingDuration, GetAimedLaserAttackAimingAnimationDuration());

        // Reset Ease In:
        hasInitiatedAimedLaserAttackEaseIn = false;
        aimedLaserAttackEaseInTimer = GetAimedLaserAttackEaseInAnimationDuration();

        // Reset Shooting:
        hasInitiatedAimedLaserAttackShooting = false;
        aimedLaserAttackShootingTimer = Mathf.Max(aimedLaserAttackShootingDuration, GetAimedLaserAttackShootingAnimationDuration());
        attackLaser.gameObject.SetActive(false);

        // Reset Ease Out:
        hasInitiatedAimedLaserAttackEaseOut = false;
        aimedLaserAttackEaseOutTimer = GetAimedLaserAttackEaseOutAnimationDuration();
    }

    private void CleanUpAimedLaserAttack()
    {
        print("CleanUp!");
        
        // Terminate Laser Fire:
        turnOffLaser();
    }

    #region Helper Functions

    private void setUpLasers()
    {
        //Setup Attack laser
        attackLaser.damage = laserDamage;
        attackLaser.owner = bossName;
        attackLaser.maxLaserDistance = maxLaserDistance;
        attackLaser.knockbackForce = knockbackForce;
        attackLaser.knockbackTime = knockbackTime;

        //Setup Telegraph laser
        telegraphLaser.damage = 0;
        telegraphLaser.owner = bossName;
        telegraphLaser.maxLaserDistance = maxLaserDistance;
        telegraphLaser.knockbackForce = Vector2.zero;
        telegraphLaser.knockbackTime = 0f;

        turnOffLaser();
    }

    public void aimLaserArm()
    {
        aimingLaserControl.position = LaserShootTarget.position;
    }
 
    public void InitiateLaserShootCycle(Transform target)
    {
        setLaserCannonGraphicState(AntagBossAnimationHandler.LaserCannonGraphicState.Aiming);
        aimingLaserControl.position = Vector2.zero;
        LaserShootTarget = target;
    }
 
    public void turnOffLaser()
    {
        setLaserCannonGraphicState(AntagBossAnimationHandler.LaserCannonGraphicState.Normal);
        aimingLaserControl.position = Vector2.zero;
        telegraphLaser.gameObject.SetActive(false);
        attackLaser.gameObject.SetActive(false);
    }

    private void setLaserCannonGraphicState(AntagBossAnimationHandler.LaserCannonGraphicState state)
    {
        animationHandler.setLaserCannonGraphicState(state);
    }

    #endregion

    #endregion

    #region Rock Throw Attack

    private void SetupRockThrow()
    {
        // Reset Rock Throw Attack Variables:
        ResetRockThrow();

        if (!IsFacingPlayer())
        {
            // Teleport Antag to Starting Position: (If Needed)
            Vector3 newStartingPosition = GetFurthestStartingPositionFromPlayer();
            StartTeleport(newStartingPosition, () => RockThrowMidTeleportAction());
        }
        else
        {
            // Continue to Attack without Teleporting
        }
    }

    private void RockThrowMidTeleportAction()
    {
        // Orientate Antag to Look at Player:
        orientateToPlayer();
    }

    private void RockThrowUpdate()
    {
        // If Teleporting, Wait Till Complete:
        if (isTeleporting)
        {
            return;
        }

        // Initiate Windup:
        if (!hasInitiatedRockThrow_WindUp)
        {
           // print("RockThrow: Windup Initiate");
            hasInitiatedRockThrow_WindUp = true;
            initiateRockThrow_Windup();
            return;
        }
        // Windup Cycle:
        if (rockThrow_WindUpTimer >= 0)
        {
           // print("RockThrow: Windup Cycle");
            rockThrow_WindUpTimer -= Time.deltaTime;
            RockThrow_WindupCycle();
            return;
        }

        // Initiate RockThrowing:
        if (!hasInitiatedRockThrowing)
        {
            
            hasInitiatedRockThrowing = true;
            initiateRockThrowing();
            return;
        }

        // Windup Cycle:
        if (rockThrowingTimer >= 0)
        {
            //print("RockThrow: RockThrowing Cycle");
            rockThrowingTimer -= Time.deltaTime;
            RockThrowingCycle();
            return;
        }

        AntagAttackStateExit();
    }

    // Wind Up:
    private void initiateRockThrow_Windup()
    {
        //Animation
        animationHandler.playRockThrow_WindupAnimation();
    }

    private void RockThrow_WindupCycle()
    {
        
    }
    private float GetRockThrow_WindupAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.RockThrow_WindupAnimationString);
    }


    // Rock Throwing:
    private void initiateRockThrowing()
    {
        //Start rock Throw Attack at player
        
        //Animation
        animationHandler.playRockThrowingAnimation();
    }

    private void RockThrowingCycle()
    {
        float  calculatedRockSpawnDelay = Mathf.Max(rockThrowingDuration, GetRockThrowingAnimationDuration()) - rockSpawnDelay;

        if(rockThrowingTimer <= calculatedRockSpawnDelay && !hasSpawnedRocks)
        {
            //We have thrown the rocks
            hasSpawnedRocks = true;

            spawnRocks();
        }

        //Wait for animation to Finish
    }

    private void spawnRocks()
    {
        //Calculate throw direction 
        Vector3 difference = getPlayerDirection();
        difference.Normalize();

        float rotationZ = math.atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        Quaternion realRotation = Quaternion.Euler(0f, 0f, rotationZ);

        //If we are not facing  the player do a default throw
        if (!IsFacingPlayer())
        {
            if (lookDirection == LookDirection.Left)
            {
                realRotation = Quaternion.Euler(0f, 0f, 180f);
            }
            else
            {
                realRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        //instantiate Rocks
        Instantiate(rocks, firePoint.position, realRotation);
    }
    private void ResetRockThrow()
    {
        // Reset Windup:
        hasInitiatedRockThrow_WindUp = false;
        rockThrow_WindUpTimer = Mathf.Max(rockThrow_WindUpDuration, GetRockThrow_WindupAnimationDuration());

        // Reset Rock Throwing:
        hasInitiatedRockThrowing = false;
        rockThrowingTimer = Mathf.Max(rockThrowingDuration, GetRockThrowingAnimationDuration());
        hasSpawnedRocks = false;
    }
    private float GetRockThrowingAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.RockThrowingAnimationString);
    }

    private void CleanUpRockThrow()
    {

    }

    #region Helper function

    #endregion


    #endregion

    #region Flying Stomp Attack

    private void SetupFlyingStomp()
    {
        // Generate Random Attack Amount:
        FlyingStompAmount = UnityEngine.Random.Range(minFlyingStompAmount, maxFlyingStompAmount + 1);

        ResetFlyingStomp();
    }

    private void FlyingStompUpdate()
    {
        // Initiate Windup:
        if (!hasInitiatedFlyingStompWindUp)
        {
            hasInitiatedFlyingStompWindUp = true;
            initiateFlyingStompWindup();
            return;
        }
        // Windup Cycle:
        if (FlyingStompWindUpTimer >= 0)
        {
            FlyingStompWindUpTimer -= Time.deltaTime;
            flyingStompWindupCycle();
            return;
        }

        // Initiate Fly Up:
        if (!hasInitiatedFlyingStompFlyUp)
        {
            hasInitiatedFlyingStompFlyUp = true;
            initiateFlyingStompFlyUp();
            return;
        }
        // Fly Up Cycle:
        if (!flyingStompFlyUpComplete)
        {
            flyingStompFlyUpCycle();
            return;
        }

        // Initiate Tracking:
        if (!hasInitiatedFlyingStompTracking)
        {
            hasInitiatedFlyingStompTracking = true;
            initiateFlyingStompTracking();
            return;
        }
        // Tracking Cycle:
        if (FlyingStompTrackingTimer >= 0)
        {
            FlyingStompTrackingTimer -= Time.deltaTime;
            flyingStompTrackingCycle();
            return;
        }

        // Initiate Fly Down:
        if (!hasInitiatedFlyingStompFlyDown)
        {
            hasInitiatedFlyingStompFlyDown = true;
            initiateFlyingStompFlyDown();
            return;
        }
        // Fly Down Cycle:
        if (!flyingStompFlyDownComplete)
        {
            flyingStompFlyDownCycle();
            return;
        }

        // Initiate Landing:
        if (!hasInitiatedFlyingStompLanding)
        {
            hasInitiatedFlyingStompLanding = true;
            initiateFlyingStompLanding();
            return;
        }
        // Landing Cycle:
        if (FlyingStompLandingTimer >= 0)
        {
            FlyingStompLandingTimer -= Time.deltaTime;
            flyingStompLandingCycle();
            return;
        }

        // Initiate Cool Down:
        if (!hasInitiatedFlyingStompCoolDown)
        {
            hasInitiatedFlyingStompCoolDown = true;
            initiateFlyingStompCoolDown();
            return;
        }
        // Cool Down Cycle:
        if (FlyingStompCoolDownTimer >= 0)
        {
            FlyingStompCoolDownTimer -= Time.deltaTime;
            flyingStompCoolDownCycle();
            return;
        }

        FlyingStompAmount--;

        if (FlyingStompAmount > 0)
        {
            ResetFlyingStomp();
            return;
        }

        AntagAttackStateExit();
    }

    #region Wind Up

    private void initiateFlyingStompWindup()
    {
        animationHandler.playFlyingStompWindUpAnimation();
    }

    private void flyingStompWindupCycle()
    {

    }

    private float GetFlyingStompWindupAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.FlyingStompWindUpAnimationString);
    }

    #endregion 

    #region Fly Up

    private void initiateFlyingStompFlyUp()
    {
        animationHandler.playFlyingStompFlyUpAnimation();

        FlyingStompFlyUpStartingYPosition = transform.position.y;

        // Fly Up Until Off-Screen
        transform.DOMoveY(FlyingStompFlyUpTransform.position.y, FlyUpDuration).OnComplete(() =>
        {
            flyingStompFlyUpComplete = true;
        });
    }

    private void flyingStompFlyUpCycle()
    {

    }

    #endregion

    #region Tracking
    private void initiateFlyingStompTracking()
    {
        animationHandler.playFlyingStompTrackingAnimation();
    }

    private void flyingStompTrackingCycle()
    {
        float playerX = PlayerInfo.Instance.transform.position.x; 

        transform.DOMoveX(playerX, FlyingStompTrackingSpeed);

        var raycast = Physics2D.Raycast(transform.position, Vector2.down, trackingRaycastlength, trackingGroundLayerMask.layerMask);
        if(raycast.transform != null)
        {
            trackingGroundPoint = raycast.point;
            if(FlyingStompTrackingShadow != null)
            {
                FlyingStompTrackingShadow.transform.position = trackingGroundPoint;
            }
            else
            {
                Debug.LogError(name + " Does not have a Shadow Object ");
            }
        }

        if (FlyingStompTrackingTimer <= (FlyingStompTrackingDuration - FlyingStompTrackingShadowDelay) && !FlyingStompTrackingShadowEnabled)
        {
            FlyingStompTrackingShadowEnabled = true;
            FlyingStompTrackingShadow.GetComponentInChildren<AntagShadowAnimationHandler>().playAppearAnimation();
        }

    }

    private float GetFlyingStompTrackingAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.FlyingStompTrackingAnimationString);
    }

    #endregion

    #region Fly Down
    private void initiateFlyingStompFlyDown()
    {
        animationHandler.playFlyingStompFlyDownAnimation();
        FlyingStompTrackingShadow.GetComponentInChildren<AntagShadowAnimationHandler>().playDisappearAnimation();
        
        float calculatedFlyingDownDistance = Mathf.Abs(FlyingStompFlyUpTransform.position.y - FlyingStompFlyUpStartingYPosition);
        float calculatedFlyingDownSpeed = calculatedFlyingDownDistance / FlyDownDuration;
        float calculatedFlyingDownDuration = ( calculatedFlyingDownDistance - flyingStompFlyDownStoppingDistance ) / calculatedFlyingDownSpeed;

        // Fly Down Until On the Ground
        transform.DOMoveY(FlyingStompFlyUpStartingYPosition + flyingStompFlyDownStoppingDistance, calculatedFlyingDownDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            flyingStompFlyDownComplete = true;
        });
    }

    private void flyingStompFlyDownCycle()
    {
        transform.DOMoveX(FlyingStompTrackingShadow.transform.position.x, 0f);
    }

    #endregion

    #region Landing
    private void initiateFlyingStompLanding()
    {
        animationHandler.playFlyingStompLandingAnimation();

        float calculatedFlyingDownDistance = Mathf.Abs(FlyingStompFlyUpTransform.position.y - FlyingStompFlyUpStartingYPosition);
        float calculatedFlyingDownSpeed = calculatedFlyingDownDistance / FlyDownDuration;
        float calculatedFlyingLandingDuration = ( calculatedFlyingDownDistance - ( calculatedFlyingDownDistance - flyingStompFlyDownStoppingDistance ) ) / calculatedFlyingDownSpeed;

        transform.DOMoveY(FlyingStompFlyUpStartingYPosition, calculatedFlyingLandingDuration).SetEase(Ease.Linear);
    }

    private void flyingStompLandingCycle()
    {
        transform.DOMoveX(FlyingStompTrackingShadow.transform.position.x, 0f);
    }

    private float GetFlyingStompLandingAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.FlyingStompLandingAnimationString);
    }
    #endregion

    #region CoolDown
    private void initiateFlyingStompCoolDown()
    {
        animationHandler.playFlyingStompCoolDownAnimation();
    }

    private void flyingStompCoolDownCycle()
    {

    }
    private float GetFlyingStompCoolDownAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.FlyingStompCoolDownAnimationString);
    }
    #endregion 

    private void ResetFlyingStomp()
    {
        // Reset Windup:
        hasInitiatedFlyingStompWindUp = false;
        FlyingStompWindUpTimer = GetFlyingStompWindupAnimationDuration();

        // Reset Fly Up:
        hasInitiatedFlyingStompFlyUp = false;
        flyingStompFlyUpComplete = false;

        // Reset Tracking:
        hasInitiatedFlyingStompTracking = false;
        FlyingStompTrackingTimer = Mathf.Max(GetFlyingStompTrackingAnimationDuration(), FlyingStompTrackingDuration);
        FlyingStompTrackingShadowEnabled = false;

        // Reset Fly Down:
        hideShadow();
        hasInitiatedFlyingStompFlyDown = false;
        flyingStompFlyDownComplete = false;

        // Reset Fly Landing:
        hasInitiatedFlyingStompLanding = false;
        FlyingStompLandingTimer = GetFlyingStompLandingAnimationDuration();

        // Reset Fly Cool Down:
        hasInitiatedFlyingStompCoolDown = false;
        FlyingStompCoolDownTimer = Mathf.Max(GetFlyingStompCoolDownAnimationDuration(), FlyingStompCoolDownDuration);
    }

    private void CleanUpFlyingStomp()
    {
        hideShadow();
    }

    #endregion
    
    #region Create Time Rifts Attack

    private void SetupCreateTimeRiftsAttack()
    {
        print("Setup Create Time Rifts");

        // Reset Aimed Laser Attack Variables:
        ResetCreateTimeRiftsAttack();

        if (!isGrounded())
        {
            // Teleport Antag to Starting Position: (If Needed)
            Vector3 newStartingPosition = GetFurthestStartingPositionFromPlayer();
            StartTeleport(newStartingPosition, () => RockThrowMidTeleportAction());
        }
    }

    private void CreateTimeRiftsAttackUpdate()
    {
        if (isTeleporting)
        {
            return;
        }

        // Initiate Summoning:
        if (!hasInitiatedCreateTimeRiftsSummoning)
        {
            print("Create Time Rifts: Summoning Initiated");

            hasInitiatedCreateTimeRiftsSummoning = true;
            initiateCreateTimeRiftsSummoning();
            return;
        }
        // Summoning Cycle:
        if (createTimeRiftsSummoningTimer >= 0)
        {
            print("Create Time Rifts: Summoning Cycle");

            createTimeRiftsSummoningTimer -= Time.deltaTime;
            CreateTimeRiftsSummoningCycle();
            return;
        }

        AntagAttackStateExit();
    }

    private void initiateCreateTimeRiftsSummoning()
    {
        //Animation
        animationHandler.playCreateTimeRiftsSummoningAnimation();
    }

    private void CreateTimeRiftsSummoningCycle()
    {
        float calculatedSummoningDelay = Mathf.Max(createTimeRiftsSummoningDuration, GetCreateTimeRiftsSummoningAnimationDuration()) - createTimeRiftsSummonDelay;

        if(createTimeRiftsSummoningTimer <= calculatedSummoningDelay && !hasSummonedCreateTimeRiftsEnemies)
        {
            //We have thrown the rocks
            hasSummonedCreateTimeRiftsEnemies = true;

            createTimeRiftsSummonEnemies();
        }
    }
    
    private float GetCreateTimeRiftsSummoningAnimationDuration()
    {
        return animationHandler.getAnimationLength(AntagBossAnimationHandler.CreateTimeRiftsSummoningAnimationString);
    }

    private void createTimeRiftsSummonEnemies()
    {
        foreach ( var enemySpawn in currentAntagBossAttackState.enemySpawnList )
        {
            Vector2 spawnPosition = getRandomEnemySpawnPosition();
            
            GameObject enemy = Instantiate(enemySpawn.enemyToSpawn, spawnPosition, Quaternion.Euler(Vector3.zero));

            if (createTimeRiftsSpawnPortal != null)
            {
                // Portal Spawn:
                GameObject portal = Instantiate(createTimeRiftsSpawnPortal, enemy.transform.position, transform.rotation);
                portal.transform.SetParent(transform);
                portal.SetActive(true);

                // Get Portal Scale:
                EnemyMovement enemyMovement = enemy.gameObject.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                {
                    portal.transform.localScale = enemyMovement.spawnPortalScale;
                }
            }
        }
    }

    private void ResetCreateTimeRiftsAttack()
    {
        // Reset Summoning:
        hasInitiatedCreateTimeRiftsSummoning = false;
        createTimeRiftsSummoningTimer = Mathf.Max(createTimeRiftsSummoningDuration, GetCreateTimeRiftsSummoningAnimationDuration());
        hasSummonedCreateTimeRiftsEnemies = false;
    }

    private void CleanUpCreateTimeRiftsAttack()
    {
        print("Clean Up Create Time Rifts");
    }

    #region Helper Functions

    private Vector2 getRandomEnemySpawnPosition()
    {
        return new Vector2(UnityEngine.Random.Range(createTimeRiftsMinXSpawnPosition, createTimeRiftsMaxXSpawnPosition), transform.position.y);
    }

    #endregion

    #endregion

    #region Helper Functions
    private void OnDrawGizmos()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y - trackingRaycastlength);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, position);

        if (FlyingStompTrackingTimer > 0)
        {
            Gizmos.DrawSphere(trackingGroundPoint, 0.5f);
        }
    }
    private Vector3 GetFurthestStartingPositionFromPlayer()
    {
        float leftPositionDistance = Vector2.Distance(PlayerInfo.Instance.transform.position, LeftStartingPosition.position);
        float rightPositionDistance = Vector2.Distance(PlayerInfo.Instance.transform.position, RightStartingPosition.position);

        if (rightPositionDistance >= leftPositionDistance)
        {
            return RightStartingPosition.position;
        }
        else
        {
            return LeftStartingPosition.position;
        }
    }

    private void hideShadow()
    {
        FlyingStompTrackingShadow.GetComponentInChildren<AntagShadowAnimationHandler>().playHideAnimation();
    }

    private void StartTeleport(Vector3 targetPosition, Action midTeleportAction = null)
    {
        isTeleporting = true;

        // Play Teleport In Animation:
        animationHandler.playTeleportInAnimation();

        // Call End Teleport Action After Animation Delay:
        StartCoroutine(CallEndTeleportAfterAnimation(targetPosition, midTeleportAction));
    }

    private float getTeleportAnimationDuration()
    {
        float teleportInAnimationDuration = animationHandler.getAnimationLength(AntagBossAnimationHandler.TeleportInAnimationString);
        float teleportOutAnimationDuration = animationHandler.getAnimationLength(AntagBossAnimationHandler.TeleportOutAnimationString);

        return teleportInAnimationDuration + teleportOutAnimationDuration;
    }

    private IEnumerator CallEndTeleportAfterAnimation(Vector3 targetPosition, Action midTeleportAction = null)
    {
        // Get Teleport Delay from Animation Length:
        float teleportDelay = Mathf.Clamp(animationHandler.getAnimationLength(AntagBossAnimationHandler.TeleportInAnimationString), 0f, float.MaxValue);

        yield return new WaitForSeconds(teleportDelay);

        // Move Antag to Target Position:
        transform.position = targetPosition;

        // Call Mid Teleport Action:
        midTeleportAction.Invoke();

        EndTeleport();
    }

    private void EndTeleport()
    {
        // Play Teleport Out Animation;
        animationHandler.playTeleportOutAnimation();

        isTeleporting = false;
    }

    #endregion
}
