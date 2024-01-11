using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIContainerPool : GameObjectPool
{
    public static ShopUIContainerPool instance { get; private set; }

    protected override void Awake()
    {
        instance = this;
    }

}
