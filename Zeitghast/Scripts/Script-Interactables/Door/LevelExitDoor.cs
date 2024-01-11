using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitDoor : PortalDoor
{
    protected override void interactAction()
    {
        base.interactAction();

        LevelManager.Instance.PlayLevelEndSequence();
    }
}
