using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PushBoxAction", menuName = "PushBoxAction")]
public class PushBoxAction : Action
{
    [SerializeField] private string targetTag =  "Interactable";

    public override bool ActionDoable(Transform transform, Transform targetTransform)
    {
        if (targetTransform.tag != targetTag) return false;

        if (timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return false;
        }

        TileBasedMovment boxMovement = targetTransform.GetComponentInChildren<TileBasedMovment>();
        if(boxMovement == null) boxMovement = targetTransform.GetComponentInParent<TileBasedMovment>();
        if (boxMovement == null) return false;

        return true;
    }

    public override void performAction(Transform transform, Transform targetTransform)
    {
        TileBasedMovment boxMovement = targetTransform.GetComponentInChildren<TileBasedMovment>();
        if (boxMovement == null) boxMovement = targetTransform.GetComponentInParent<TileBasedMovment>();

        if (boxMovement == null) return;
            
        if(timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return;
        }

        Vector3 direction = (targetTransform.position - transform.position).normalized;

        if (boxMovement.hasNotReachedTargetPosition()) return;

        if(actionInProgress) return;

        actionInProgress = true;

        SuspicionMeter.instance.addSuspicionAmount(suspionCost);

        // Get the absolute values of the vector's components
        float x = Mathf.Abs(direction.x);
        float y = Mathf.Abs(direction.y);

        // Determine the dominant direction
        if (x > y)
        {
            // If x is dominant, set y to 0 and normalize x
            direction = new Vector3(Mathf.Sign(direction.x), 0f);
        }
        else
        {
            // If y is dominant, set x to 0 and normalize y
            direction = new Vector3(0f, Mathf.Sign(direction.y),0f);
        }

        boxMovement.moveAnyDirection(direction);

        actionInProgress =  false;

        GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
    }
}
