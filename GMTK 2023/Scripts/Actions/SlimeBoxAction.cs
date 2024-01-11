using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeBoxAction", menuName = "SlimeBoxAction")]
public class SlimeBoxAction : Action
{
    [SerializeField] private string targetTag =  "Character";

    public override bool ActionDoable(Transform transform, Transform targetTransform)
    {
        if (targetTransform.tag != targetTag) return false;

        if (timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return false;
        }

        TileBasedMovment slimeMovement = targetTransform.GetComponentInChildren<TileBasedMovment>();
        if(slimeMovement == null) slimeMovement = targetTransform.GetComponentInParent<TileBasedMovment>();
        if (slimeMovement == null) return false;

        SlimeController slimeController = targetTransform.GetComponentInChildren<SlimeController>();
        if(slimeController == null) slimeController = targetTransform.GetComponentInParent<SlimeController>();
        if (slimeController == null) return false;

        //Check time of day 
        return true;
    }

    public override void performAction(Transform transform, Transform targetTransform)
    {
        TileBasedMovment slimeMovement = targetTransform.GetComponentInChildren<TileBasedMovment>();
        if (slimeMovement == null) slimeMovement = targetTransform.GetComponentInParent<TileBasedMovment>();

        if (slimeMovement == null) return;
            
        if(timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return;
        }

        Vector3 direction = (targetTransform.position - transform.position).normalized;

        if (slimeMovement.hasNotReachedTargetPosition()) return;

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

        slimeMovement.moveAnyDirection(direction);

        actionInProgress =  false;

        GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
    }
}
