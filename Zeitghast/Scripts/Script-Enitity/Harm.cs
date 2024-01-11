using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harm : MonoBehaviour
{
    public int damage;
    public bool canPierce;
    public LayerMaskObject destroyCollisions;

    private Collider2D hitCollider;
    private float hitboxOffset = 0f;
    private bool canDamage;
    private Health enitityLastHit;
    public bool canKnockBack = true;


    protected virtual void Start()
    {
        canDamage = true;
        hitCollider = GetComponent<Collider2D>();
    }

    protected void Update()
    {
        if (canDamage == false)
        {
            return;
        }

        RaycastHit2D[] allHitInfo = Physics2D.BoxCastAll(hitCollider.bounds.center, hitCollider.bounds.size, 0f, transform.right, hitboxOffset, destroyCollisions.layerMask);
        
        if (allHitInfo.Length == 0) return;

        foreach(RaycastHit2D hitInfo in allHitInfo)
        { 
            if (hitInfo.collider != null)
            {
                Health targetHealth = hitInfo.collider.GetComponentInParent<Health>();
                if (targetHealth != null)
                {
                    if (targetHealth.attackers.list.Contains(tag))
                    {
                        if (canKnockBack) dealDamage(damage, targetHealth);
                    }
                }

                Knockback targetKnockback = hitInfo.collider.GetComponentInParent<Knockback>();
                if (targetKnockback != null)
                {
                    if (targetKnockback.attackers.list.Contains(tag))
                    {
                        dealKnockback(targetKnockback);
                    }
                } 
            }
        }        
    }

    public void dealDamage(int damage, Health targetHealth)
    {
        string nameOfKiller = gameObject.name;
        if (GetComponent<Movement>() != null)
        {
            nameOfKiller = GetComponent<Movement>().entityName;
        }

        targetHealth.changeHealth(-damage, nameOfKiller);
        if(!targetHealth.invincible && damage > 0)
        {
            targetHealth.invincibilityFrames();
        }
    }

    public void dealKnockback(Knockback targetKnockback)
    {
        targetKnockback.applyKnockBackPoint(transform.position);
        if (!targetKnockback.knockbackDisabled)
        {
            targetKnockback.knockbackFrames();
        }
    }

    public void changeCanDamage(bool toggle)
    {
        canDamage = toggle;
    }

    private void OnDrawGizmos()
    {
        if (hitCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(hitCollider.bounds.center.x + hitboxOffset, hitCollider.bounds.center.y , hitCollider.bounds.center.z), hitCollider.bounds.size);
        }
    }

}
