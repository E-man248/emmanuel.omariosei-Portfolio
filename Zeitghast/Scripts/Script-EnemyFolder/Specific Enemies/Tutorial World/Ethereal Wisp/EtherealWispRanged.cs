using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EtherealWispRanged : FlyingEnemy
{
    [Header("Ethereal Wisp")]
    [SerializeField] protected float retreatRadius;
    [SerializeField] protected float retreatSpeedMultiplier = 1f;

    [Header("Ethereal Line of Sight")]
    [SerializeField] protected float lineOfSightWidth;
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

    protected override void combatMovementCycle()
    {
        if (inRetreatRange() && hasLineOfSight(target.position))
        {
            Vector3 retreatPosition = transform.position + (transform.position - targetPosition);
            Debug.DrawLine(transform.position, retreatPosition, Color.magenta);

            GraphNode endNode = AstarPath.active.GetNearest(retreatPosition).node;
            if (endNode.Walkable)
            {
                if (pathUpdateRateTimer <= 0)
                {
                    pathUpdateRateTimer = pathUpdateRate;
                    UpdatePath(retreatPosition, retreatSpeedMultiplier);
                }
                pathUpdateRateTimer -= Time.deltaTime;
            }
            else
            {
                moveXY(retreatSpeedMultiplier * (retreatPosition - transform.position).normalized);
            }
        }
        else if (hasLineOfSight(target.position))
        {
            moveXY(Vector2.zero);
        }
        else
        {
            if (pathUpdateRateTimer <= 0)
            {
                pathUpdateRateTimer = pathUpdateRate;
                UpdatePath();
            }
            pathUpdateRateTimer -= Time.deltaTime;
            
        }
    }

    public override bool hasLineOfSight(Vector3 point)
    {
        Vector2 boxCastSize = new Vector2(lineOfSightWidth, lineOfSightWidth);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, point - transform.position, Vector2.Distance(point, transform.position), lineOfSightTargets.layerMask);

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

    private bool inRetreatRange()
    {
        return Vector3.Distance(target.position, transform.position) < retreatRadius;
    }

    private void drawLineOfSight()
    {

        if(target != null )
        {
            Vector3 space = lineOfSightWidth / 2 * Vector2.Perpendicular(transform.position - target.position).normalized;
            Gizmos.DrawLine(transform.position + space, target.position + space);
            Gizmos.DrawLine(transform.position - space, target.position - space);
        }
        
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //Debuging Retreat Zone
        Gizmos.color = Color.magenta;

        if(entityCollider != null)
        {
            Gizmos.DrawWireSphere(entityCollider.bounds.center, retreatRadius);
        }

        drawLineOfSight();
    }
}
