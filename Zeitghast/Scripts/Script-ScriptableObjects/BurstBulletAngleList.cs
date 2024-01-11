using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BurstBulletAngleListObject")]
public class BurstBulletAngleList : ScriptableObject
{
    public List<BurstBulletAngle> list;

    public BurstBulletAngleList clone()
    {
        BurstBulletAngleList clone = CreateInstance<BurstBulletAngleList>();
        clone.list = new List<BurstBulletAngle>();

        foreach (BurstBulletAngle angle in list)
        {
            BurstBulletAngle angleCopy = new BurstBulletAngle();
            angleCopy.bullet = angle.bullet;
            angleCopy.shotAngle = angle.shotAngle;
            angleCopy.randomAngleDeviation = angle.randomAngleDeviation;
            clone.list.Add(angleCopy);
        }

        return clone;
    }
}
