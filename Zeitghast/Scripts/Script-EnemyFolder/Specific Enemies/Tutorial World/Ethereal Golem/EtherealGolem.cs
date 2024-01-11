using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealGolem : GroundedEnemy
{
    [Header("Ethereal Golem Settings")]
    [SerializeField] private bool canParryDashAttack = true;
    [SerializeField] protected Vector2 dashAttackParryKnockbackForce;
    [SerializeField] protected float dashAttackParryKnockbackTime;
    protected Dictionary<EnviromentalHarm, knockbackForceCacheData> knockbackForceCache;
    protected EtherealGolemAnimationHandler etherealGolemAnimationHandler;
    

    public bool dashAttackParryAnimationIsPlaying { get; protected set; } = false;

    protected struct knockbackForceCacheData
    {
        public Vector2 knockbackForce;
        public float knockbackTime;
    }


    protected override void Start()
    {
        base.Start();

        try
        {
            etherealGolemAnimationHandler = (EtherealGolemAnimationHandler)animationHandler;
        }
        catch (System.Exception)
        {
            //Debug.LogWarning("InvalidCastException: Specified cast is not valid");
        }

        // Store all original knockback force values so that they can be reverted when parrying:
        knockbackForceCache = new Dictionary<EnviromentalHarm, knockbackForceCacheData>();
        foreach (EnviromentalHarm harmScript in GetComponentsInChildren<EnviromentalHarm>())
        {
            knockbackForceCacheData data;
            data.knockbackForce = harmScript.knockbackForce;
            data.knockbackTime = harmScript.knockbackTime;
            knockbackForceCache.Add(harmScript, data);
        }
    }

    protected override void patroling()
    {
        //if (canOrientate) orientateLook();
        base.patroling();
    }

    protected override void combat()
    {
        dashAttackBlock();
        moveX(0);
        base.combat();
    }

    protected bool playerIsDashAttacking()
    {
        return PlayerInfo.Instance.GetComponentInChildren<WeaponManager>().dashingWithDashAttackWeapon();
    }

    protected void turnOffDashAttackParryAnimation()
    {
        dashAttackParryAnimationIsPlaying = false;
        disableDashAttackParryKnockback();
    }


    protected virtual void dashAttackBlock()
    {
        if (!canParryDashAttack) return;
        if (etherealGolemAnimationHandler != null && playerIsDashAttacking() && !dashAttackParryAnimationIsPlaying)
        {
            etherealGolemAnimationHandler.dashAttackParryAnimation();
            enableDashAttackParryKnockback();

            dashAttackParryAnimationIsPlaying = true;
            Invoke("turnOffDashAttackParryAnimation", etherealGolemAnimationHandler.getAnimationLength("EtherealGolemDashAttackParry"));
        }

        if (dashAttackParryAnimationIsPlaying) return;
    }

    /**
        <summary>
        Sets the knockback force of all the Environmental Harm scripts on the enemy to
        have the dashAttackParryKnockbackForce value. The knockbackForceCache Dictionary is
        used which has a list of all Environmental Harm scripts on the enemy as well as their
        associated original knockback value for resetting later.
        </summary>
    **/
    protected void enableDashAttackParryKnockback()
    {
        foreach (var harmScriptPair in knockbackForceCache)
        {
            harmScriptPair.Key.knockbackForce = dashAttackParryKnockbackForce;
            harmScriptPair.Key.knockbackTime = dashAttackParryKnockbackTime;
        }
    }

    /**
        <summary>
        Sets the knockback force of all the Environmental Harm scripts on the enemy back
        to their original value. This is done through the knockbackForceCache Dictionary which
        has a list of all Environmental Harm scripts on the enemy as well as their
        associated original knockback value.
        </summary>
    **/
    protected void disableDashAttackParryKnockback()
    {
        foreach (var harmScriptPair in knockbackForceCache)
        {
            harmScriptPair.Key.knockbackForce = harmScriptPair.Value.knockbackForce;
            harmScriptPair.Key.knockbackTime = harmScriptPair.Value.knockbackTime;
        }
    }
}
