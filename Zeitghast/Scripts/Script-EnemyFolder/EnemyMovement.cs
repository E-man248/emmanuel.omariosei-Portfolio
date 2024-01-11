using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;

public class EnemyMovement : Movement
{
    public enum State
    {
        Idle,
        Pursuit,
        Patroling,
        Combat,
        Stunned,
        Inactive
    }

    [SerializeField] private bool setUpDebug = false;
    [Header("Enemy")]
    public State state;
    public LayerMask wallLayer;
    [HideInInspector] public int lookDirection;

    [Header("Projectile")]
    public GameObject Bullet;

    [Header("Combat Settings")]
    [SerializeField] protected bool hostileZoneIsACircle = false;
    public Vector2 HostileZoneSize;
    [Space]
    [SerializeField] protected bool attackZoneIsACircle = false;
    public Vector2 attackDistance;
    [Space]
    public float attackSpeed;
    protected float attackSpeedTimer;
    [Space]
    protected int attackAmount;
    public int maxAttackAmount;
    [Space]
    [SerializeField] protected float attackWindUpDuration;
    internal float attackWindUpTimer;
    [Space]
    public float coolDownDuration;
    internal float coolDownTimer;
    [Space]
    public LayerMaskObject lineOfSightTargets;
    public LayerMaskObject bulletDestroyCollisions;

    [Header("Attack Arm")]
    public Transform attackArm;
    public Transform attackSpawn;
    [SerializeField] protected GameObject muzzleFlash;
    [SerializeField] protected Vector3 muzzleFlashOffest;

    [Header("AI")]
    public Transform target = null;
    private Collider2D targetCollider;
    protected Vector3 targetPosition;
    public Vector2ListObject attackAngleArcs;

    [Header("Patrol Settings")]
    public Vector2 patrolRange;
    protected Vector2 distanceFromPatrol;
    protected Vector3 patrolPoint;
    public int maxPatrolDuration;
    public int minPatrolDuration;
    protected float patrolTimer;

    [Header("Idle Settings")]
    public float maxIdleDuration = 3f;
    public float minIdleDuration = 1f;
    protected float idleTimer;

    [Header("Stuck Settings")]
    public Transform stuckCheckPosition;
    public float stuckCheckRadius = 0.01f;

    [Header("Stun Settings")]
    public float stunDuration = 1f;
    public float stunTimer { get; protected set; }
    protected float stunGravity = 2f;
    
    [Header("Spawn Portal Settings")]
    public Vector2 spawnPortalScale = Vector2.one;

    //Animation
    protected enemyAnimationHandler animationHandler;
    protected bool attackAnimationIsPlaying = false;
    protected Quaternion currentShootingAimAngle;

    protected virtual void Start()
    {
        //Check if wall layer is valid 
        if (wallLayer == LayerMask.GetMask("Nothing"))
        {
            Debug.LogError(name + "'s Wall layer set to nothing");
        }

        target = PlayerInfo.Instance.transform;
        targetCollider = target.GetComponent<Collider2D>();

        if (attackAngleArcs != null)
        {
            if (attackAngleArcs.list.Count == 0)
            {
                Debug.LogError("No Attack Angle Arcs in Enemy Script of " + name);
                attackAngleArcs.list.Add(new Vector2(0, 0));
            }
        }

        if (attackArm == null)
        {
            GameObject backupAttackArm = new GameObject();
            backupAttackArm.transform.SetParent(transform);
            backupAttackArm.transform.localPosition = Vector3.zero;
            backupAttackArm.layer = gameObject.layer;

            attackArm = backupAttackArm.transform;
            if(setUpDebug) Debug.Log(gameObject.name + " has no attack point set!!");
        }

        if (attackSpawn == null)
        {
            GameObject backupAttackSpawn = new GameObject();
            backupAttackSpawn.transform.SetParent(attackArm.transform);
            backupAttackSpawn.transform.localPosition = Vector3.zero;
            backupAttackSpawn.layer = gameObject.layer;

            attackSpawn = backupAttackSpawn.transform;
            if (setUpDebug) Debug.Log(gameObject.name + " has no attack point set!!");
        }

        if (Bullet == null)
        {
            if (setUpDebug) Debug.Log(gameObject.name + " has no bullet!!");
        }

        //checking if there is a stuck position set if not use the enemy's transform
        if(stuckCheckPosition == null)
        {
            stuckCheckPosition = transform; 
        }

        //Animation
        animationHandler = GetComponentInChildren<enemyAnimationHandler>();

        generalSetup();
    }

    protected void generalSetup()
    {
        state = State.Patroling;
        variableSpeed = 0f;
        
        idleSetup();
        patrolSetup();
        pursuitSetup();
        combatSetup();
        stunSetup();
        secondarySetup();
    }

    protected virtual void secondarySetup()
    {

    }
    protected override void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > PlayerInfo.Instance.enemy_AI_Update_Distance )
        {
            return;
        }

        base.Update();
    }

    public override void moveXY(float xDirection, float yDirection)
    {
        if (entityRigidbody == null) return;

        base.moveXY(xDirection, yDirection);
    }
    
    public override void moveXY(Vector2 moveDirection)
    {
        if (entityRigidbody == null) return;

        base.moveXY(moveDirection.x, moveDirection.y);
    }

    public override void moveX(float xDirection)
    {
        if (entityRigidbody == null) return;

        base.moveX(xDirection);
    }

    public override void moveY(float yDirection)
    {
        if (entityRigidbody == null) return;

        base.moveY(yDirection);
    }

    protected virtual void idleSetup()
    {

    }

    protected virtual void idleUpdate()
    {
        if (inHostileRange())
        {
            // Patrol Switch from Idle
            state = State.Patroling;
        }

        if (idleTimer > 0)
        {
            // Idle Timer Tick
            idleTimer -= Time.deltaTime;
        }
        else
        {
            // Idle Timer Reset
            idleTimer = UnityEngine.Random.Range(maxIdleDuration, minIdleDuration);
            state = State.Patroling;
        }
    }

    protected virtual void idling()
    {
        
    }

    protected virtual void patrolSetup()
    {
        distanceFromPatrol = transform.position;
        resetPatrolPoint();
        patrolTimer = UnityEngine.Random.Range(minPatrolDuration, maxPatrolDuration);
        idleTimer = UnityEngine.Random.Range(maxIdleDuration, minIdleDuration);
    }

    protected virtual void patrolUpdate()
    {
        if (patrolTimer <= 0)
        {
            patrolTimer = UnityEngine.Random.Range(minPatrolDuration, maxPatrolDuration);
            state = State.Idle;
            return;
        }

        patrolTimer -= Time.deltaTime;
    }

    protected virtual void pursuitSetup()
    {

    }

    protected virtual void pursuitUpdate()
    {
        orientateLook();

        if (!inHostileRange())
        {
            state = State.Patroling;
            
        }

        if (inAttackRange())
        {
            state = State.Combat;
        }

        // Debug Drawing:
        Debug.DrawLine(transform.position, targetPosition, Color.magenta);
        if (attackAngleArcs != null) inAttackAngleArcs(targetPosition);
    }

    protected virtual void combatSetup()
    {
        coolDownTimer = coolDownDuration;
        attackAmount = maxAttackAmount;
        attackWindUpTimer = attackWindUpDuration;
        attackAnimationIsPlaying = false;
        attackSpeedTimer = attackSpeed;
    }

    protected virtual void combatUpdate()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.magenta);
        if (attackAngleArcs != null) inAttackAngleArcs(targetPosition);
    }

    protected virtual void stunSetup()
    {
        stunTimer = 0f;
    }

    protected void FixedUpdate()
    {
        // Stop the Enemy From Using its AI when Out of Enemy AI Update Distance:
        // Switch to Patrol After
        
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > PlayerInfo.Instance.enemy_AI_Update_Distance)
        {
            state = State.Inactive;
            inactive();
            return;
        }

        // Set Target Position: (Based on Target Collider)

        if (targetCollider != null) targetPosition = targetCollider.bounds.center;

        // State Management:

        if (state == State.Inactive)
        {
            inactive();
        }

        if (state == State.Patroling)
        {
            patrolUpdate();
            patroling();
        }

        else if (state == State.Combat)
        {
            combatUpdate();
            combat();
        }

        else if (state == State.Pursuit)
        {
            pursuitUpdate();
            pursue();
        }

        else if (state == State.Idle)
        {
            idleUpdate();
            idling();
        }
        
        else if (state == State.Stunned)
        {
            stunned();
        }
    }

    protected virtual void inactive()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= PlayerInfo.Instance.enemy_AI_Update_Distance)
        {
            state = State.Patroling;
        }
    }

    protected virtual void patroling()
    {
        if (inHostileRange())
        {
            moveXY(0,0);
            state = State.Pursuit;
        }
    }

    protected virtual void pursue()
    {

    }

    protected virtual void combat()
    {
        attackWindUpCycle();

        if (attackWindUpTimer > 0)
        {
            return;
        }

        attackCycle();
    }

    protected virtual void attackWindUpCycle()
    {
        if (attackAnimationIsPlaying) return;

        attackWindUpTimer -= Time.deltaTime;
        
        if (attackWindUpTimer > 0)
        {
            lookAtTarget();
            //This is sets attackSpeedTimer to 0 so that the initial attack triggers immediately
            attackSpeedTimer = 0;
            return;
        }
    }

    protected virtual void attackCycle()
    {
        if (attackAnimationIsPlaying) return;

        if (attackAmount > 0)
        {
            if (attackSpeedTimer > 0)
            {
                attackSpeedTimer -= Time.deltaTime;
                return;
            }
            else
            {
                attack();
                playAttackAnimation();
                attackAmount--;
                attackSpeedTimer = attackSpeed;

                if(animationHandler != null)
                {
                    Invoke("leaveCombatState", animationHandler.getAnimationLength(entityName + "Attack"));  
                }
                else
                {
                    leaveCombatState();
                }
                
            }
        }
        else
        {
            coolDown();
        }
    }

    protected virtual void leaveCombatState()
    {
        attackAnimationIsPlaying = false;
        if (!inAttackRange())
        {
            attackAmount = maxAttackAmount;
            attackWindUpTimer = attackWindUpDuration;
            state = State.Pursuit;
        }
    }

    protected virtual void coolDown()
    {
        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
        }
        else
        {
            attackAmount = maxAttackAmount;
            attackSpeedTimer = attackSpeed;
            attackWindUpTimer = attackWindUpDuration;
            coolDownTimer = coolDownDuration;
        }
    }

    protected virtual void attack()
    {
        if (inAttackRange())
        {
            shootTarget(targetPosition);
        }
    }

    protected virtual void stunned()
    {
        entityRigidbody.gravityScale = stunGravity;
        if (stunTimer <= 0f)
        {
            entityRigidbody.gravityScale = 1f;
            state = State.Patroling;
        }
        stunTimer -= Time.deltaTime;
    }

    public void stunEnemy(float duration, float gravityEffect)
    {
        if (state == State.Stunned) return;

        stunEnemy(duration);
        stunGravity = gravityEffect;
    }

    public void stunEnemy(float duration)
    {
        if (state == State.Stunned) return;

        stunGravity = 2f;
        stunTimer = duration;
        state = State.Stunned;

        //Debug.Log("Enemy has been Stunned for " + duration);
    }

    public void stunEnemy()
    {
        if (state == State.Stunned) return;
        
        stunEnemy(stunDuration);
    }

    protected virtual void shootTarget(Vector3 point)
    {
        aimAtTarget(point);
        
        // Bullet Shot:
        if(Bullet != null)
        {
            GameObject bullet = Instantiate(Bullet, attackSpawn.position, currentShootingAimAngle);
            bullet.GetComponent<Bullet>().destroyCollisions = bulletDestroyCollisions;
            bullet.tag = "EnemyBullet";
            bullet.layer = 14;
            bullet.GetComponent<Bullet>().owner = entityName;
        }

        // Muzzle Flash Spawn:
        spawnMuzzleFlash();
    }

    protected virtual void aimAtTarget(Vector3 point)
    {
        Vector3 difference = point - transform.position;
        difference.Normalize();

        float rotationZ = math.atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        Quaternion realRotation = Quaternion.Euler(0f, 0f, rotationZ);
        attackArm.rotation = realRotation;

        currentShootingAimAngle = realRotation;
    }

    protected void spawnMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            Transform muzzleFlashParent = attackArm;

            Vector3 newMuzzleFlashlocation = new Vector3(muzzleFlashOffest.x, muzzleFlashOffest.y, 0f);
            GameObject currentMuzzleFlash = Instantiate(muzzleFlash, muzzleFlashParent);

            currentMuzzleFlash.transform.localPosition = newMuzzleFlashlocation;
        }
    }

    protected virtual void orientateLook()
    {
        if (!canOrientate) return;

        if (entityRigidbody.velocity.x < 0)
        {
            lookDirection = -1;
        }
        else if (entityRigidbody.velocity.x > 0)
        {
            lookDirection = 1;
        }
    }

    protected virtual void lookAtTarget()
    {
        if (!canOrientate) return;

        float targetDirection = targetPosition.x - transform.position.x;

        if (targetDirection < 0)
        {
            lookDirection = -1;
        }
        else if (targetDirection > 0)
        {
            lookDirection = 1;
        }
    }

    protected virtual void lookAtTarget(Vector3 lookTarget)
    {
        if (!canOrientate) return;

        float targetDirection = lookTarget.x - transform.position.x;

        if (targetDirection < 0)
        {
            lookDirection = -1;
        }
        else if (targetDirection > 0)
        {
            lookDirection = 1;
        }
    }

    protected virtual void resetPatrolPoint()
    {
        patrolPoint = transform.position;
    }

    public virtual bool inHostileRange()
    {
        if (hostileZoneIsACircle)
        {
            if (Vector3.Distance(transform.position, target.position) < HostileZoneSize.x)
            {
                return true;
            }
            else return false;
        }
        else
        {
            if (Mathf.Abs(transform.position.x - target.position.x) < HostileZoneSize.x && Mathf.Abs(transform.position.y - target.position.y) < HostileZoneSize.y)
            {
                return true;
            }
            else return false;
        }
    }

    public virtual bool inAttackRange()
    {
        //If the enemy does not have line of sight with the target -> Do not engage in combat
        if (!hasLineOfSight(targetPosition))
        {
            return false;
        }

        //If the target is not within the attack angle  -> Do not engage in combat
        if (attackAngleArcs != null && !inAttackAngleArcs(targetPosition))
        {
            return false;
        }

        if (attackZoneIsACircle)
        {
            if (Vector3.Distance(transform.position, target.position) < attackDistance.x)
            {
                return true;
            }
            else return false;
        }
        else
        {
            if (Mathf.Abs(transform.position.x - target.position.x) < attackDistance.x && Mathf.Abs(transform.position.y - target.position.y) < attackDistance.y)
            {
                return true;
            }
            else return false;
        }
    }

    public virtual bool hasLineOfSight(Vector3 point)
    {
        Vector2 difference = point - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, difference, Vector2.Distance(point, transform.position), lineOfSightTargets.layerMask);

        bool result = true;
        if (hit.collider != null)
        {
            if (hit.collider.tag != "Player")
            {
                result = false;
            }
        }
        return result;
    }

    public bool isStuck()
    {
        if (stuckCheckRadius == 0) return false;

        return Physics2D.OverlapCircle(stuckCheckPosition.position, stuckCheckRadius, groundLayers.layerMask);
    }

    // IMPORTANT: Angle Difference Cannot Be Larger than 180 Degrees!!
    public bool inAttackAngleArcs(Vector3 point)
    {
        Vector2 pointToPlayer = (point - transform.position).normalized;

        foreach (Vector2 arc in attackAngleArcs.list)
        {
            Vector2 vectorMin = (Vector2)transform.position - (new Vector2(Mathf.Cos(arc.x * Mathf.Deg2Rad), Mathf.Sin(arc.x * Mathf.Deg2Rad)) + (Vector2)transform.position);
            Vector2 vectorMax = (Vector2)transform.position - (new Vector2(Mathf.Cos(arc.y * Mathf.Deg2Rad), Mathf.Sin(arc.y * Mathf.Deg2Rad)) + (Vector2)transform.position);

            Debug.DrawLine(transform.position, (Vector2)transform.position - vectorMin, Color.yellow);
            Debug.DrawLine(transform.position, (Vector2)transform.position - vectorMax, Color.yellow);
        }

        foreach (Vector2 arc in attackAngleArcs.list)
        {
            Vector2 vectorMin = (Vector2)transform.position - (new Vector2(Mathf.Cos(arc.x * Mathf.Deg2Rad), Mathf.Sin(arc.x * Mathf.Deg2Rad)) + (Vector2)transform.position);
            Vector2 vectorMax = (Vector2)transform.position - (new Vector2(Mathf.Cos(arc.y * Mathf.Deg2Rad), Mathf.Sin(arc.y * Mathf.Deg2Rad)) + (Vector2)transform.position);

            if (vectorMin.Equals(vectorMax))
            {
                return true;
            }

            float differenceMin = Vector2.SignedAngle(pointToPlayer, vectorMin);
            float differenceMax = Vector2.SignedAngle(vectorMax, pointToPlayer);

            if (differenceMin >= 0 && differenceMax >= 0)
            {
                return true;
            }
        }

        return false;
    }

    #region Animations
    protected virtual void playAttackAnimation()
    {
        if (animationHandler != null)
        {
            animationHandler.attackAnimation();
        }
        attackAnimationIsPlaying = true;
    }
    #endregion

    public virtual void debugPatrol()
    { 
        //Debuging Patrol Range
        Gizmos.color = new Color(0f, 255f, 0f, 0.25f);
        Gizmos.DrawCube(patrolPoint, new Vector3(patrolRange.x * 2f, patrolRange.y * 2f, 1f));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(patrolPoint, new Vector3(patrolRange.x * 2f, patrolRange.y * 2f, 1f));
    }

    public virtual void debugHostileRange()
    { 
        //Debuging Hostile Range
        if (hostileZoneIsACircle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(entityCollider.bounds.center, HostileZoneSize.x);
        }
        else
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(entityCollider.bounds.center, new Vector3(HostileZoneSize.x * 2f, HostileZoneSize.y * 2f, 1f));
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(entityCollider.bounds.center, new Vector3(HostileZoneSize.x * 2f, HostileZoneSize.y * 2f, 1f));
        }
    }

    public virtual void debugAttackRange()
    {
        //Debuging Attack Range
        if (attackZoneIsACircle)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(entityCollider.bounds.center, attackDistance.x);
        }
        else
        {
            Gizmos.color = new Color(252, 227, 3, 0.4f);
            Gizmos.DrawCube(entityCollider.bounds.center, new Vector3(attackDistance.x * 2f, attackDistance.y * 2f, 1f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(entityCollider.bounds.center, new Vector3(attackDistance.x * 2f, attackDistance.y * 2f, 1f));
        }
    }

    public void debugStuckRadius()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(stuckCheckPosition.position, stuckCheckRadius);
    }


    public virtual void OnDrawGizmos()
    {
        if (!Application.IsPlaying(this))
        {
            return;
        }

        if (entityCollider == null)
        {
            return;
        }

        debugHostileRange();

        debugAttackRange();

        debugPatrol();

        debugStuckRadius();
    }
}
