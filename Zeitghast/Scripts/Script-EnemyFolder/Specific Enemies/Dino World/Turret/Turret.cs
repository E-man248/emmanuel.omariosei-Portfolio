using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : StationaryEnemy
{
    protected override void attackCycle()
    {
        if (coolDownTimer < coolDownDuration)
        {
            aimAtTarget(targetPosition);
        }

        base.attackCycle();
    }

    protected override void pursue()
    {
        aimAtTarget(targetPosition);

        base.pursue();
    }

    protected override void patroling()
    {
        aimAtTarget(targetPosition);

        base.patroling();
    }

    protected override void idling()
    {
        aimAtTarget(targetPosition);

        base.idling();
    }

    protected override void shootTarget(Vector3 point)
    {        
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
}
