using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class WeaponWallCollisionDetection : MonoBehaviour
{
    [HideInInspector]
    public bool isInWall;
    public TagList wallCollisions;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo != null && (wallCollisions.list.Contains(hitInfo.tag)))
        {
            isInWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo != null && (wallCollisions.list.Contains(hitInfo.tag)))
        {
            isInWall = false;
        }
    }
}
