using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndClickWeapon : Weapon
{
    /*
     * Does standard shooting code but the firing point has been set to the location of the mouse 
     */
    public override void shoot()
    {
        FiringPoint.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
        base.shoot();
    }
}

