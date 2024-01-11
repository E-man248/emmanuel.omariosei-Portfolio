using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class NPCCondition : MonoBehaviour
{
    [Header("Dialog Condition Settings")]
    public NPCDialogConditionType ConditionType;
    public bool status { get; protected set; }

    protected virtual void Start()
    {
        UpdateStatus();
    }

    public virtual void UpdateStatus()
    {
        status = GetStatus();
    }

    public abstract bool GetStatus();

    public static bool ConditionsMet(List<NPCCondition> AllConditions)
    {
        // If there are no add or remove conditions, by default the dialog should be added:
        if (AllConditions.Count <= 0)
        {
            return true;
        }

        var AddConditions = AllConditions.Where( x => x.ConditionType == NPCDialogConditionType.Add ).ToList();
        var RemoveConditions = AllConditions.Where( x => x.ConditionType == NPCDialogConditionType.Remove ).ToList();

        // If the remove conditions are met, the dialog should not be added:
        if (RemoveConditions.Count > 0 && GetStatus(RemoveConditions))
        {
            return false;
        }

        // If the add conditions are met, the dialog should be added:
        if (GetStatus(AddConditions))
        {
            return true;
        }

        // If neither the add or the remove conditions are met, the dialog should not be added:
        return false;
    }

    public static bool GetStatus(List<NPCCondition> conditions)
    {
        foreach (var condition in conditions)
        {
            condition.UpdateStatus();

            if (condition.status == false)
            {
                return false;
            }
        }

        return true;
    }
}

public enum NPCDialogConditionType
{
    Add,
    Remove
}
