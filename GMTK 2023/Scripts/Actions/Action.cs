using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Action")]
public class Action : ScriptableObject
{
    [SerializeField] protected int suspionCost = 0;
    public Sprite actionSprite;

    [SerializeField] protected bool actionInProgress = false;

    [SerializeField] protected bool timeSpecific;
    [SerializeField] protected GameStateManger.DayNightState TimeForAction;



    public virtual bool ActionDoable(Transform transform, Transform targetTransform)
    {
        return true;
    }

    public virtual void performAction(Transform transform, Transform targetTransform)
    {
        GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
    }

}
