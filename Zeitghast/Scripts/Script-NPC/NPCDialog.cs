using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPCDialog : MonoBehaviour
{
    [TextArea]
    public string Text;
    [Space]
    public string Emote = "";

    [Space]
    [Header("Dialog Events")]
    [Space]
    public UnityEvent OnShow;
    public UnityEvent OnHide;

    private List<NPCCondition> AllConditions;
    
    private void Awake()
    {
        // Retrieve Dialog Assets:

        AllConditions = GetComponents<NPCCondition>().ToList();
    }

    public bool CanAdd()
    {
        return NPCCondition.ConditionsMet(AllConditions);
    }
}
