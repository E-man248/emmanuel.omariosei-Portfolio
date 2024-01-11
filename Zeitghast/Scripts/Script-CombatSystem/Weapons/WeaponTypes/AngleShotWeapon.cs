using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleShotWeapon : Weapon
{
    [Header("Shot Angles")]
    public ShotAngleListObject shotAngles;

    /** <summary>
        Given a Vector2 'shotLine' value, a vector line
        from the current 'weaponManager' position to the 'mousePostion'. From there,
        it records the angle of that vector and finds an 'angleRange' in which
        the angle falls into from the list of 'shotAngles'. The 'snapToAngle' of
        matching 'ShotAngle' is then returned.
        If there is no matching 'ShotAngle' (angle does not fall into any 'angleRange'),
        then the 'float.MaxValue' is returned to indicate there is nothing to return.
        </summary>
    **/
    public float findSnapToAngle(Vector2 shotLine)
    {
        if (shotAngles == null)
        {
            return float.MaxValue;
        }

        Vector3 weaponManagerPosition = weaponManager.transform.position;
        Vector2 normalizedShotLine = shotLine.normalized;

        foreach (ShotAngle shotAngle in shotAngles.list)
        {
            Vector2 vectorMin = (Vector2) weaponManagerPosition - (new Vector2(Mathf.Cos(shotAngle.angleRange.x * Mathf.Deg2Rad), Mathf.Sin(shotAngle.angleRange.x * Mathf.Deg2Rad)) + (Vector2) weaponManagerPosition);
            Vector2 vectorMax = (Vector2) weaponManagerPosition - (new Vector2(Mathf.Cos(shotAngle.angleRange.y * Mathf.Deg2Rad), Mathf.Sin(shotAngle.angleRange.y * Mathf.Deg2Rad)) + (Vector2) weaponManagerPosition);

            // Debug Drawing:
            Debug.DrawLine(weaponManagerPosition, (Vector2)weaponManagerPosition - vectorMin, Color.yellow);
            Debug.DrawLine(weaponManagerPosition, (Vector2)weaponManagerPosition - vectorMax, Color.yellow);
        }
    
        foreach (ShotAngle shotAngle in shotAngles.list)
        {
            Vector2 vectorMin = (Vector2) weaponManagerPosition - (new Vector2(Mathf.Cos(shotAngle.angleRange.x * Mathf.Deg2Rad), Mathf.Sin(shotAngle.angleRange.x * Mathf.Deg2Rad)) + (Vector2) weaponManagerPosition);
            Vector2 vectorMax = (Vector2) weaponManagerPosition - (new Vector2(Mathf.Cos(shotAngle.angleRange.y * Mathf.Deg2Rad), Mathf.Sin(shotAngle.angleRange.y * Mathf.Deg2Rad)) + (Vector2) weaponManagerPosition);

            if (vectorMin.Equals(vectorMax))
            {
                return shotAngle.snapToAngle;
            }

            float differenceMin = Vector2.SignedAngle(normalizedShotLine, vectorMin);
            float differenceMax = Vector2.SignedAngle(vectorMax, normalizedShotLine);

            if (differenceMin > 0 && differenceMax >= 0)
            {
                return shotAngle.snapToAngle;
            }
        }

        return float.MaxValue;
    }
}

[System.Serializable]
public struct ShotAngle
{
    public Vector2 angleRange;
    public float snapToAngle;
}
