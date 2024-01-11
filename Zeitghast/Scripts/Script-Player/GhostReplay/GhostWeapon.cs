using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWeapon : EntityAnimationHandler
{
    [Header("Animation Settings")]
    public string attackAnimationString;
    protected float stillAnimationTimer;

    [Header("Bullet")]
    public Transform FiringPoint;
    public GameObject bulletObject;
    public float fireRate;
    private float fireRateTimer;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;
    public Vector2 muzzleFlashOffest;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        fireRateTimer += Time.deltaTime;

        if(fireRateTimer >= fireRate)
        {
            fireRateTimer = 0;
            shoot();
        }
    }

    private void shoot()
    {
        attackAnimation();
        if (bulletObject == null)
        {
            return;
        }

        if (FiringPoint != null)
        {
            
            Instantiate(bulletObject, FiringPoint.position, FiringPoint.rotation);
        }
        else
        {
            Instantiate(bulletObject, transform.position, transform.rotation);
        }

        spawnMuzzleFlash();
    }

    private void spawnMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            Transform muzzleFlashParent = transform;

            Vector3 newMuzzleFlashlocation = new Vector3(muzzleFlashOffest.x, muzzleFlashOffest.y, 0f);
            GameObject currentMuzzleFlash = Instantiate(muzzleFlash, muzzleFlashParent);

            currentMuzzleFlash.transform.localPosition = newMuzzleFlashlocation;
        }
    }


    public void attackAnimation()
    {
        playAnimationOnceFull(attackAnimationString);
    }
}
