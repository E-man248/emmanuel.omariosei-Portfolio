using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatHolder : HatHolder
{
    protected override void Start()
    {
        HatInfo recentPlayerHatInfo = HatManager.Instance.GetRecentPlayerHat();

        base.Start();

        Equip(recentPlayerHatInfo);
    }
}
