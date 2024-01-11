using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : GameObjectPool
{
    public static AfterImagePool instance { get; private set; }

    protected override void Awake()
    {
        instance = this;
    }

}
