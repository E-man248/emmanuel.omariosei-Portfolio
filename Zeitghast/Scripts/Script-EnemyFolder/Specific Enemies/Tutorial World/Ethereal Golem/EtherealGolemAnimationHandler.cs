using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealGolemAnimationHandler : GroundedEnemyAnimationHandler
{
    #region Animations Names
    protected string DashAttackParryAnimationString;
    #endregion

    [Header("Landing Screen Shake Effect")]
    public float landingShakeIntensity = 0.5f;
    public float landingShakeDuration = 0.2f;
    private EtherealGolem etherealGolemAI;

    protected override void Awake()
    {
        base.Awake();

        #region Animation String Definitions
        DashAttackParryAnimationString = enemyName + "DashAttackParry";
        
        etherealGolemAI = (EtherealGolem) groundedEnemyAI;
        
        attackAnimationInterupt = () =>
        {
            return Mathf.Abs(enemyMovement.entityRigidbody.velocity.x) > 0 && enemyHealth.health > 0 /*&& etherealGolemAI.dashAttackParryAnimationIsPlaying*/;
        };
        #endregion
    }
    
    protected override void Update()
    {
        base.Update();
    }

    public override void landingAnimation()
    {
        base.landingAnimation();
        ScreenShake.Instance.ShakeScreen(landingShakeIntensity, landingShakeDuration);
    }

    public void dashAttackParryAnimation()
    {
        playAnimationOnceFull(DashAttackParryAnimationString);
    }
    
    protected override void orientateEnemy()
    {
        if (!enemyMovement.canOrientate)
        {
            return;
        }

        if (enemyMovement.lookDirection == 1)
        {
            transform.parent.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (enemyMovement.lookDirection == -1)
        {
            transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }
}
