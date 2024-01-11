using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : CharacterController
{
    protected override void manageMovement()
    {
        if (!charcterIsActive)
        {
            ClearAllActions();
            return;
        }

        if (!InputEnabled) return;

        if (GameStateManger.Instance.currentTime == GameStateManger.DayNightState.Night)
        {
            return;
        }
        
        if (Input.GetKey(KeyCode.W))
        {   
            StartCoroutine(DisableInputForDelay());

            if(upAction != null)
            {
                upAction.performAction(transform,upTargetTransform);
            }
            else
            {
                tileBasedMovment.moveUp();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
            
        }

        if (Input.GetKey(KeyCode.A))
        {
            StartCoroutine(DisableInputForDelay());
            if (leftAction != null)
            {
                leftAction.performAction(transform, leftTargetTransform);
            }
            else
            {
                tileBasedMovment.moveLeft();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(DisableInputForDelay());

            if (downAction != null)
            {
                downAction.performAction(transform, downTargetTransform);
            }
            else
            {
                tileBasedMovment.moveDown();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            StartCoroutine(DisableInputForDelay());

            if (rightAction != null)
            {
                rightAction.performAction(transform, rightTargetTransform);
            }
            else
            {
                tileBasedMovment.moveRight();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
        }

        checkPossibleActions();
    }
}