using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakBoxAction", menuName = "BreakBoxAction")]
public class BreakBoxAction : Action
{
    [SerializeField] private string targetTag = "Obstacle";
    [SerializeField] private int damage = 1;

    public override bool ActionDoable(Transform transform, Transform targetTransform)
    {
        if (targetTransform.tag != targetTag) return false;
        if (timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return false;
        }


        Health health = targetTransform.GetComponentInChildren<Health>();
        if (health == null) health = targetTransform.GetComponentInParent<Health>();
        if (health == null) return false;

        return true;
    }

    public override void performAction(Transform transform, Transform targetTransform)
    {
        if (targetTransform.tag != targetTag) return;

        if (timeSpecific)
        {
            if (GameStateManger.Instance.currentTime != TimeForAction) return;
        }

        Health health = targetTransform.GetComponentInChildren<Health>();
        if (health == null) health = targetTransform.GetComponentInParent<Health>();
        if (health == null) return;

        health.changeHealth(-damage);

        SuspicionMeter.instance.addSuspicionAmount(suspionCost);
        GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
    }
}
