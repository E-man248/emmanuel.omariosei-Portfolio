using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class GroundedEnemy : EnemyMovement
{
    [Header("Grounded Enemy Jump Settings")]
    public bool jumpEnabled = true;
    [SerializeField] protected float jumpCooldownDuration = 3;
    [SerializeField] protected float jumpWindupDuration = 0f;
    protected float jumpCooldownTimer;
    [HideInInspector] public float jumpWindupTimer;
    protected bool hasJumpedAfterWindup = false;
    protected bool hasLandedAfterJump = false;
    protected bool hasPlayedLandingAnimation = false;

    [Header("Grounded Enemy Patrol Settings")]
    public float wallBumbRange;
    [SerializeField] protected float patrolSpeedMultiplier = 1f;
    [SerializeField] protected float pursueSpeedMultiplier = 1f;
    protected Vector2 moveDirection;
    protected Vector3 currentPatrolTarget = Vector3.zero;
    protected bool resetPatrolTarget = true;

    //Pathfinding
    [Header("Pathfinding")]
    public float nextWayPointDistance;
    public float pathUpdateRate;
    private float pathUpdateRateTimer;
    public float jumpThreshold;

    protected Path path;
    protected int currentWaypoint = 0;
    protected Seeker seeker;

    [Header("Ledge Detection")]
    [SerializeField] protected bool hasLedgeDetection = true;
    [SerializeField] protected float ledgeDetectionDistance = 1f;
    [SerializeField] protected float ledgeDetectionXOffset = 1f;
    private float ledgeDetectionTimer = 0;
    private const float LEDGE_DETECTION_TIMER_DURATION = 0.1f;

    //Animation
    protected GroundedEnemyAnimationHandler groundedEnemyAnimationHandler;


    protected override void Start()
    {
        base.Start();
        groundedEnemyAnimationHandler = (GroundedEnemyAnimationHandler) animationHandler;
    }

    protected override void secondarySetup()
    {
       seeker = GetComponent<Seeker>();
       hasJumpedAfterWindup = false;
       hasLandedAfterJump = false;
       hasPlayedLandingAnimation = false;
    }

    protected override void Update()
    {
        base.Update();
        if (jumpEnabled && jumpWindupTimer <= 0f) jumpCooldownTimer -= Time.deltaTime;
    }

    protected override void idling()
    {
        moveX(0);
    }

    protected override void patrolSetup()
    {
        resetPatrolTarget = true;
        base.patrolSetup();
    }

    protected override void patrolUpdate()
    {
        if (patrolTimer <= 0)
        {
            moveX(0);
            resetPatrolTarget = true;
        }
        base.patrolUpdate();
    }

    protected override void patroling()
    {
        if (patrolTimer <= 0)
        {
            return;
        }

        if (pursueCheck())
        {
            moveX(0);
            state = State.Pursuit;
            return;
        }

        if (isGoingToHitWall())
        {
            //moveDirection.x *= -1; <---------------------------------------------------------------wall check
        }

        //Ledge Dectection cycle
        if(ledgeDetectionTimer <= 0f)
        {
            if (!IsTouchingGroundAhead() && hasLedgeDetection && isGrounded())
            {
                resetPatrolTarget = true;
                ledgeDetectionTimer = LEDGE_DETECTION_TIMER_DURATION;
            }
        }
        ledgeDetectionTimer -= Time.deltaTime;

        // Patrol Pattern:
        if (resetPatrolTarget)
        {
            if (!IsTouchingGroundAhead() && hasLedgeDetection && isGrounded())
            {
                if (patrolPoint.x < currentPatrolTarget.x)
                {
                    moveDirection.x = -1;
                    currentPatrolTarget = patrolPoint + new Vector3(Random.Range(-patrolRange.x, 0f), 0f, 0f);
                }
                else
                {
                    moveDirection.x = 1;
                    currentPatrolTarget = patrolPoint + new Vector3(Random.Range(0f, patrolRange.x), 0f, 0f);
                }
            }
            else
            {
                currentPatrolTarget = patrolPoint + new Vector3(Random.Range(-patrolRange.x, patrolRange.x), 0f, 0f);
            }

            GraphNode endNode = AstarPath.active.GetNearest(currentPatrolTarget).node;
            if (!endNode.Walkable) return;

            UpdatePath(currentPatrolTarget, patrolSpeedMultiplier);
            resetPatrolTarget = false;
        }

        if (pathUpdateRateTimer <= 0)
        {
            pathUpdateRateTimer = pathUpdateRate;
            UpdatePath(currentPatrolTarget, patrolSpeedMultiplier);
        }
        pathUpdateRateTimer -= Time.deltaTime;

        if (canOrientate) orientateLook();
    }

    protected override void pursuitUpdate()
    {
        // Do Not Switch States If Jumping:
        if (jumpEnabled && (jumpWindupTimer > 0f || !isGrounded())) return;

        if (jumpEnabled && !hasLandedAfterJump) return;

        base.pursuitUpdate();
    }

    protected override void pursue()
    {
        pursueOrientationCheck();

        // If Jump is Enabled, this IF statement will allow the pursue cycle to
        // return when it needs to (so that the enemy does not move when its jumping)
        // If Jump Is Not Enabled Skip This!
        if (jumpEnabled)
        {
            if (!pursueJumpCycle())
            {  
                return;
            }
        }

        pursueMovementCycle();

        base.pursue();
    }

    protected virtual bool pursueJumpCycle()
    { 
        jumpWindupTimer -= Time.deltaTime;
        
        // If enemy is not ready to jump | OR | Is falling (airborne)
        if (jumpWindupTimer > 0f || !isGrounded())
        {
            if (!isGrounded())
            {
                hasLandedAfterJump = false;
                hasPlayedLandingAnimation = false;
                if (isGoingToHitWall()) moveDirection.x *= -1;
                moveX(moveDirection.x);
            }
            return false;
        }
        // If fully wound up and have not jumped, initiate jump!
        // This forces the jump to only happen once and not happen
        // again until you are fully airborne (completely not touching ground)
        else if (!hasJumpedAfterWindup)
        {
            jump(jumpForce);
            moveX(moveDirection.x);
            jumpCooldownTimer = jumpCooldownDuration;
            hasJumpedAfterWindup = true;
            hasLandedAfterJump = false;
            hasPlayedLandingAnimation = false;
            return false;
        }

        // Allow landing animation to play and reset (and only play once)
        if (!hasLandedAfterJump && isGrounded())
        {
            hasLandedAfterJump = true;
            if (groundedEnemyAnimationHandler != null)
            {
                groundedEnemyAnimationHandler.landingAnimation();
                Invoke("resetHasLanded", groundedEnemyAnimationHandler.getAnimationLength(entityName + "Landing"));
            }
            else
            {
                resetHasLanded();
            }
            return false;
        }

        // Do not move while playing landing animations
        if (!hasPlayedLandingAnimation && isGrounded())
        {
            moveX(0);
            return false;
        }

        // Continues the pursuit cycle
        // (returning false would break the cycle and re-run this function)
        return true;
    }

    protected virtual void pursueMovementCycle()
    {

        // Continue to Update Path To For Pursuit:
        if(pathUpdateRateTimer <= 0)
        {
            pathUpdateRateTimer = pathUpdateRate;
            UpdatePath(pursueSpeedMultiplier);
        }
        pathUpdateRateTimer -= Time.deltaTime;
    }

    protected virtual void pursueOrientationCheck()
    {
        if (canOrientate) orientateLook();
    }

    protected override void combat()
    {
        combatMovementCycle();

        base.combat();
    }

    protected virtual void combatMovementCycle()
    {
        moveX(0);
    }

    protected virtual void UpdatePath(float speedMultiplier = 1f)
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(entityRigidbody.position, target.position, OnPathComplete);
        }
        pathFollow(speedMultiplier);
    }

    protected virtual void UpdatePath(Vector3 targetPosition, float speedMultiplier = 1f)
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(entityRigidbody.position, targetPosition, OnPathComplete);
        }
        pathFollow(speedMultiplier);
    }

    protected void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }

        if (state == State.Patroling)
        {
            if (Vector3.Distance(transform.position, currentPatrolTarget) < 0.1f)
            {
                resetPatrolTarget = true;
            }
        }
    }

    void pathFollow(float speedMultiplier = 1f)
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        if(currentWaypoint + 1 >= path.vectorPath.Count)
        {
            return;
        }

        moveDirection = ((Vector2)path.vectorPath[currentWaypoint + 1] - entityRigidbody.position).normalized;
        
        moveDirection *= speedMultiplier;

        moveX(moveDirection.x);

        if(jumpEnabled && jumpCooldownTimer <= 0 && isGrounded())
        {
            if (moveDirection.y > jumpThreshold && moveDirection.x != 0)
            {
                jumpWindupTimer = jumpWindupDuration;
                moveX(0f);
                hasJumpedAfterWindup = false;
            }
        }
       
        float distance = Vector2.Distance(entityRigidbody.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }
    }

    protected override void orientateLook()
    {
        if (moveDirection.x < 0)
        {
            lookDirection = -1;
        }
        else if (moveDirection.x > 0)
        {
            lookDirection = 1;
        }
    }

    protected virtual void resetHasLanded()
    {
        hasPlayedLandingAnimation = true;
    }

    protected virtual bool pursueCheck()
    {
        return inHostileRange();
    }

    protected bool isGoingToHitWall()
    {
        //BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask);

        RaycastHit2D hit = Physics2D.BoxCast(entityCollider.bounds.center, new Vector2(entityCollider.bounds.size.x + wallBumbRange, entityCollider.bounds.size.y), 0, moveDirection * Vector2.right, wallDetectionDistance, wallLayer);

        //Debugging box
        Vector2 top = new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.min.y);
        Vector2 bottom = new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.max.y);

        Debug.DrawRay(top, moveDirection * Vector2.right * (entityCollider.bounds.extents.x + wallBumbRange / 2), Color.blue);
        Debug.DrawRay(entityCollider.bounds.center, moveDirection * Vector2.right * (entityCollider.bounds.extents.x + wallBumbRange / 2), Color.blue);
        Debug.DrawRay(bottom, moveDirection * Vector2.right * (entityCollider.bounds.extents.x + wallBumbRange / 2), Color.blue);

        return hit.collider != null;
    }
    
    public bool isFalling()
    {
        return !isGrounded() && entityRigidbody.velocity.y <= 0;
    }

    public bool isJumping()
    {
        return !isGrounded() && entityRigidbody.velocity.y > 0;
    }

    public bool IsTouchingGroundAhead()
    {
        Vector2 raycastPosition = new Vector2(entityCollider.bounds.center.x + ledgeDetectionXOffset * Mathf.Sign(moveDirection.x), entityCollider.bounds.center.y);
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, entityCollider.bounds.extents.y + ledgeDetectionDistance, wallLayer);
        return hit.collider != null;
    }

    public override void debugPatrol()
    { 
        //Debuging Patrol Range
        Gizmos.color = new Color(0f, 255f, 0f, 0.25f);
        Gizmos.DrawCube(patrolPoint, new Vector3(patrolRange.x * 2f, 6.9f, 1f));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(patrolPoint, new Vector3(patrolRange.x * 2f, 6.9f, 1f));
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (entityCollider == null)
        {
            return;
        }
        //Ledge Detection
        Vector2 raycastStartPosition = new Vector2(entityCollider.bounds.center.x + ledgeDetectionXOffset * Mathf.Sign(moveDirection.x), entityCollider.bounds.center.y);
        Gizmos.DrawRay(raycastStartPosition, Vector2.down *(entityCollider.bounds.extents.y + ledgeDetectionDistance));
    }
}
