using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FreePrincessAction", menuName = "FreePrincessAction")]
public class FreePrincessAction : Action
{
    [SerializeField] private string targetTag = "Obstacle";

    public override bool ActionDoable(Transform transform, Transform targetTransform)
    {
        if (targetTransform.tag != targetTag) return false;
        if (timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return false;
        }

        Cage cage = targetTransform.GetComponentInChildren<Cage>();
        if (cage == null) cage = targetTransform.GetComponentInParent<Cage>();
        if (cage == null) return false;

        return true;
    }

    public override void performAction(Transform transform, Transform targetTransform)
    {
        if (targetTransform.tag != targetTag) return;

        if (timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return;
        }

        Cage cage = targetTransform.GetComponentInChildren<Cage>();
        if (cage == null) cage = targetTransform.GetComponentInParent<Cage>();
        if (cage == null) return;

        cage.unCagePrincess();

        SuspicionMeter.instance.addSuspicionAmount(suspionCost);

        GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
    }
}
