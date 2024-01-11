using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class FlyingEnemy : EnemyMovement
{
    [Header("Flying Enemy Patrol Settings")]
    public Vector2 wallBumbRange;
    protected Vector2 moveDirection;
    [SerializeField] protected float patrolSpeedMultiplier = 1f;

    [Header("Pathfinding")]
    public float nextWayPointDistance;
    public float pathUpdateRate;
    protected float pathUpdateRateTimer;
    protected Vector3 currentPatrolTarget = Vector3.zero;
    protected bool resetPatrolTarget = true;

    protected Path path;
    protected int currentWaypoint = 0;
    protected Seeker seeker;


    protected override void secondarySetup()
    {
        base.secondarySetup();
        
        seeker = GetComponent<Seeker>();
        //InvokeRepeating("UpdatePath", 0f, pathUpdateRate);
    }

    protected override void idling()
    {
        base.idling();
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
            moveXY(0,0);
            resetPatrolTarget = true;
        }
        base.patrolUpdate();
    }

    protected override void patroling()
    {
        if (inHostileRange())
        {
            moveXY(0,0);
            state = State.Pursuit;
            return;
        }

        // Patrol Pattern:
        if (resetPatrolTarget)
        {
            currentPatrolTarget = patrolPoint + new Vector3(Random.Range(-patrolRange.x, patrolRange.x), Random.Range(-patrolRange.y, patrolRange.y), 0f);

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

    protected override void pursue()
    {
        if (canOrientate) orientateLook();

        if (pathUpdateRateTimer <= 0)
        {
            pathUpdateRateTimer = pathUpdateRate;
            UpdatePath();
        }
        pathUpdateRateTimer -= Time.deltaTime;

        base.pursue();
    }

    protected override void combat()
    {
        combatMovementCycle();

        base.combat();
    }

    protected virtual void combatMovementCycle()
    {
        lookAtTarget();

        if (pathUpdateRateTimer <= 0)
        {
            pathUpdateRateTimer = pathUpdateRate;
            UpdatePath();
        }
        pathUpdateRateTimer -= Time.deltaTime;
    }

    protected override void stunned()
    {
        entityRigidbody.gravityScale = stunGravity;
        if (stunTimer <= 0f)
        {
            entityRigidbody.gravityScale = 0f;
            state = State.Patroling;
        }
        stunTimer -= Time.deltaTime;
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

    private void OnPathComplete(Path p)
    {
        if (!p.error)
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

    protected virtual void pathFollow(float speedMultiplier = 1f)
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        if (currentWaypoint + 1 >= path.vectorPath.Count)
        {
            return;
        }

        moveDirection = ((Vector2)path.vectorPath[currentWaypoint + 1] - entityRigidbody.position).normalized;

        moveDirection *= speedMultiplier;

        if (isGoingToHitWallX())
        {
            moveXY(-moveDirection.x, moveDirection.y);
        }
        if (isGoingToHitWallY())
        {
            moveXY(-moveDirection.x, -moveDirection.y);
        }
        else
        {
            moveXY(moveDirection.x, moveDirection.y);
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

    protected bool isGoingToHitWallX()
    {
        //BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask);

        RaycastHit2D Xhit = Physics2D.BoxCast(entityCollider.bounds.center, new Vector2(entityCollider.bounds.size.x + wallBumbRange.x, entityCollider.bounds.size.y), 0, moveDirection * Vector2.right, wallDetectionDistance, wallLayer);

        //Debugging Box
        Vector2 top = new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.min.y);
        Vector2 bottom = new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.max.y);

        Debug.DrawRay(top, moveDirection * Vector2.right * (entityCollider.bounds.extents.x + wallBumbRange.x / 2), Color.blue);
        Debug.DrawRay(entityCollider.bounds.center, moveDirection * Vector2.right * (entityCollider.bounds.extents.x + wallBumbRange.x / 2), Color.blue);
        Debug.DrawRay(bottom, moveDirection * Vector2.right * (entityCollider.bounds.extents.x + wallBumbRange.x / 2), Color.blue);

        return Xhit.collider!= null;
    }

    protected bool isGoingToHitWallY()
    {
        //BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask);

        RaycastHit2D Yhit = Physics2D.BoxCast(entityCollider.bounds.center, new Vector2(entityCollider.bounds.size.x, entityCollider.bounds.size.y + wallBumbRange.y), 0, moveDirection * Vector2.up, wallDetectionDistance, wallLayer);

        //Debugging box
        Vector2 left = new Vector2(entityCollider.bounds.min.x, entityCollider.bounds.center.y);
        Vector2 right = new Vector2(entityCollider.bounds.max.x, entityCollider.bounds.center.y);

        Debug.DrawRay(left, moveDirection * Vector2.up * (entityCollider.bounds.size.y + wallBumbRange.y / 2), Color.blue);
        Debug.DrawRay(entityCollider.bounds.center, moveDirection * Vector2.up * (entityCollider.bounds.size.y + wallBumbRange.y / 2), Color.blue);
        Debug.DrawRay(right, moveDirection * Vector2.up * (entityCollider.bounds.size.y + wallBumbRange.y / 2), Color.blue);

        return Yhit.collider != null;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
