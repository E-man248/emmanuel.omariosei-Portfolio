using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Pathfinding;

public class MachineGunWasp : FlyingEnemy
{
    [Header("Machine-Gun Wasp Settings")]
    [SerializeField] private float upMovementSpeedMultiplier = 2;
    [SerializeField] private float angularAttackSpread = 30f;

    public bool committedToAttack = false;

    private Vector3 chosenAttackPosition = Vector3.zero;
    protected override void combatMovementCycle()
    {
        if (inAttackAngleArcs(target.position) || (attackWindUpTimer <= 0 && attackAmount > 0))
        {
            moveXY(0,0);
        }
        else
        {
            moveXY(0,1 * upMovementSpeedMultiplier);
        }
    }

    protected override void attackWindUpCycle()
    {
        if (attackAnimationIsPlaying) return;

        // If player is not in attack arcs during windup, reset
        if (inAttackAngleArcs(target.position) || committedToAttack)
        {
            //Allows you to windup for shooting 
            attackWindUpTimer -= Time.deltaTime;
        }
        else
        {
            //Prevents from winding up to shoot 
            attackWindUpTimer = attackWindUpDuration;
            return;
        }

        if (attackWindUpTimer > 0)
        {
            attackSpeedTimer = 0;
            
            //Choosing slash locking attack position
            if (inAttackRange())
            {
                chosenAttackPosition = target.position;
                lookAtTarget(chosenAttackPosition);
            }
            else
            {
                committedToAttack = false;
                leaveCombatState();
            }
            return;
        }
    }


    protected override void attackCycle()
    {
        if (attackAnimationIsPlaying) return;
        
        committedToAttack = true;

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

                Invoke("attackAnimationFinished", animationHandler.getAnimationLength(entityName + "Attack"));
            }
        }
        else
        {
            coolDown();
        }
    }

    private void attackAnimationFinished()
    {
        attackAnimationIsPlaying = false;
    }



    protected override void attack()
    {
        //Just shoots at the position set 
        shootTarget(chosenAttackPosition);
    }

    protected override void shootTarget(Vector3 point)
    {
        Vector3 difference = point - transform.position;
        difference.Normalize();

        float rotationZ = math.atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        
        //Calculating random shot angle
        float angleBound = (angularAttackSpread / 2);
        float finalRandomShotAngle = UnityEngine.Random.Range(angleBound , -angleBound);

        //Adding random shot angle to the target angle
        Quaternion shotRotation = Quaternion.Euler(0f, 0f, finalRandomShotAngle + rotationZ);

        //Adding random shot angle to the target angle
        Quaternion armRotation = Quaternion.Euler(0f, 0f, rotationZ);
        attackArm.rotation = armRotation;

        // Bullet Shot:
        if (Bullet != null)
        {
            GameObject bullet = Instantiate(Bullet, attackSpawn.position, shotRotation);
            bullet.GetComponent<Bullet>().destroyCollisions = bulletDestroyCollisions;
            bullet.tag = "EnemyBullet";
            bullet.layer = 14;
            bullet.GetComponent<Bullet>().owner = entityName;
        }

        // Muzzle Flash Spawn:
        spawnMuzzleFlash();
    }


    protected override void coolDown()
    {
        committedToAttack = false;

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

            leaveCombatState();
        }
    }

    public override bool inAttackRange()
    {
        //If the enemy does not have line of sight with the target -> Do not engage in combat
        if (!hasLineOfSight(target.position))
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


}
