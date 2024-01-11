using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Dactyl : FlyingEnemy
{
    [Header("Dactyl Settings")]
    public float divingSpeed;
    private bool isRetreating = false;
    private bool divePathFound = false;
    private Vector3 lastNeutralPosition;
    protected override void secondarySetup()
    {
        base.secondarySetup();
        isRetreating = false;
        divePathFound = false;
    }

    protected override void patroling()
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        distanceFromPatrol = currentPosition - new Vector2(patrolPoint.x, patrolPoint.y);

        // Patrol Pattern:
        if (distanceFromPatrol.x > patrolRange.x)
        {
            moveDirection.x = -1;
        }
        else if (distanceFromPatrol.x < -patrolRange.x)
        {
            moveDirection.x = 1;
        }
        else if (distanceFromPatrol.x == 0)
        {
            if (UnityEngine.Random.Range(-1, 0) == 0)
            {
                moveDirection.x = -1;
            }
            else
            {
                moveDirection.x = 1;
            }
        }

        if (isGoingToHitWallX())
        {
            moveDirection.x *= -1;
        }

        if (isGoingToHitWallY())
        {
            moveDirection.y *= -1;
        }

        if (inHostileRange())
        {
            moveXY(0, 0);
            state = State.Pursuit;
            return;
        }

        moveXY(moveDirection.x, 0);
    }

    protected void SetUpDivePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(entityRigidbody.position, target.position, DivePathComplete);
        }
    }

    private void DivePathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            divePathFound = true;
        }
    }

    protected override void pursue()
    {
        //lastNeutralPosition = transform;
        currentSpeed = divingSpeed;

        if (!isRetreating)
        {
            target = PlayerInfo.Instance.GetComponent<Transform>();
            if (!divePathFound)
            {
                SetUpDivePath();
            }
            else
            {
                pathFollow();
            }
        }
        else
        {
            divePathFound = false;
        }

        base.pursue();
    }

    protected override void patrolUpdate()
    {
        base.patrolUpdate();
    }

    protected override void pursuitUpdate()
    {
        orientateLook();

        if (!isRetreating && inAttackRange())
        {
            state = State.Combat;
            Debug.DrawLine(transform.position, target.position, Color.magenta);
            if (attackAngleArcs != null) inAttackAngleArcs(target.position);
        }

        if (isRetreating && !inHostileRange())
        {
            state = State.Patroling;
        }

    }

    protected override void combat()
    {
        isRetreating = true;
    }
}
