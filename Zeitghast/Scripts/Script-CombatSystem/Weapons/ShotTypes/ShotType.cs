using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotType : ScriptableObject
{
    public virtual ShotType clone()
    {
        ShotType clone = (ShotType)ShotType.CreateInstance("ShotType");
        return clone;
    }
}
