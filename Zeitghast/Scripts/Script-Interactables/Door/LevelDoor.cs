using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : Door
{
    public Transform targetPosition;
    public TransitionType transitionIn;
    public TransitionType transitionOut;
    private Action transitionAction = null;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void interactAction()
    {
        if (!active)
        {
            return;
        }

        base.interactAction();

        if (targetPosition != null)
        {
            //Changing player position between transitions
            transitionAction =()=> 
            { 
                PlayerInfo.Instance.SetPlayerPosition(targetPosition.position); 
            };

            AdvancedSceneManager.Instance.inSceneTransition(transitionIn,transitionOut, transitionAction);
        }
        else
        {
            Debug.LogError("[LevelDoor] No 'playerTransfrom' Transform for " + name);
        }
    }

    protected override void setLastCheckpoint()
    {
        playerHealth.setLastCheckPointPosition(targetPosition.position, AdvancedSceneManager.Instance.getCurrentScene());
    }


}
