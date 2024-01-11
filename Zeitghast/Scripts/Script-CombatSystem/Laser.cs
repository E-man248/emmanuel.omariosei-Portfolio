using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private LineRenderer lineRenderer;
    [SerializeField] private Transform firingPoint;

    [Header("Laser Settings")]
    [SerializeField] public float maxLaserDistance = 20f;
    [SerializeField] public LayerMask HitLayerMask;
    [SerializeField] public bool flipOrientation;
    internal Vector2 hitDirection = Vector2.zero;

    [Header("Damage")]
    public int damage = 5;

    [Header("Knockback")]
    public Vector2 knockbackForce = Vector2.zero;
    public float knockbackTime = 0f;
    [SerializeField] public string owner = "";

    [Header("VFX")]
    [SerializeField] private Transform startVFX;
    [SerializeField] private Transform endVFX;

    [Header("Raycast Settings")]
    [SerializeField] public float laserRaycastThickness = 2f;

    [Header("Debugging")]
    [SerializeField] private bool showDebug;

    private Vector2 laserEndPosition;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (firingPoint == null)
        {
            firingPoint = transform;
        }
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, firingPoint.position);
        
        hitDirection = getLookDirection();


        //RaycastHit2D hits = Physics2D.RaycastAll(transform.position, transform.forward, maximumDistance, LayerMask);//List of things hit 
        Vector2 boxCastSize = new Vector2(laserRaycastThickness,laserRaycastThickness);
        RaycastHit2D hit = Physics2D.BoxCast(firingPoint.position, boxCastSize, 0f, hitDirection, maxLaserDistance, HitLayerMask);
        
        if (hit.transform != null)
        {
            float distance = Vector2.Distance(hit.point, firingPoint.position);
            laserEndPosition = firingPoint.position + ((Vector3)hitDirection * distance);

            Health health = hit.transform.GetComponentInParent<Health>();
            if(health != null)
            {
                if (health.attackers.list.Contains(tag))
                {
                    dealDamage(health);
                }
            }

            Knockback targetKnockback = hit.transform.GetComponentInParent<Knockback>();
            if (targetKnockback != null)
            {
                dealKnockback(targetKnockback);
            } 
        }
        else
        {
            laserEndPosition = firingPoint.position + ((Vector3)hitDirection * maxLaserDistance);
        }

        lineRenderer.SetPosition(1, laserEndPosition);

        //VFX
        if(startVFX != null )
        {
            startVFX.position = firingPoint.position;
        }
        if (endVFX != null)
        {
            endVFX.position = laserEndPosition;
        }

    }

    private void dealDamage(Health targetHealth)
    {
        targetHealth.changeHealth(-damage, owner);
        if (!targetHealth.invincible && damage > 0)
        {
            targetHealth.invincibilityFrames();
        }
    }

    private void dealKnockback(Knockback targetKnockback)
    {
        if (knockbackForce.Equals(Vector2.zero)) return;

        targetKnockback.applyKnockBackPoint(laserEndPosition, targetKnockback.knockbackForce + knockbackForce);
        if (!targetKnockback.knockbackDisabled)
        {
            targetKnockback.knockbackFrames(targetKnockback.knockbackDuration + knockbackTime);
        }
    }

    private Vector2 getLookDirection()
    {
        if (flipOrientation)
        {
            return new Vector2(firingPoint.right.x * (-1), firingPoint.right.y * (-1));
        }
        else
        {
            return firingPoint.right;
        }
    }


    protected virtual void OnDrawGizmos()
    {
        if (!showDebug) return;
        Gizmos.color = Color.red;
        Vector2 direction = (laserEndPosition - (Vector2)firingPoint.position).normalized;
        Vector2 transposeDirection = Vector2.Perpendicular(direction);

        Vector2 shiftDirection = ( (direction * 0f) + (transposeDirection * laserRaycastThickness) ) / 2;

        Gizmos.DrawLine(shiftDirection + (Vector2) firingPoint.position, shiftDirection + laserEndPosition);
        Gizmos.DrawLine((Vector2) firingPoint.position - shiftDirection , laserEndPosition - shiftDirection);
    }

}
