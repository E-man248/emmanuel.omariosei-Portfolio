using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPC : Interactable
{
    [Space]
    [Space]
    [SerializeField] public string NPCName = "NPC";
    [Space]

    [Header("NPC Settings")]
    [SerializeField] private GameObject DialogOptionsGroup;
    
    private List<NPCDialog> Dialogs;
    private List<NPCDialog> PossibleDialogs;
    private List<NPCDialog> CurrentDialogPool;
    internal NPCDialog CurrentDialog;
    internal string CurrentEmote;
    
    #region NPC Utility Assets
    [SerializeField] public GameObject TextBox;
    [SerializeField] private TextMeshPro TextDisplay;
    protected ButtonPrompt buttonPrompt;

    #endregion

    [Header("NPC Events")]
    [Space]

    public UnityEvent OnDialogShow;
    public UnityEvent OnDialogHide;

    protected void Awake()
    {
        // Retrieve Available Dialog Options:

        Dialogs = DialogOptionsGroup.GetComponentsInChildren<NPCDialog>().ToList();

        // Retrieve NPC Utitlity Assets:

        buttonPrompt = GetComponentInChildren<ButtonPrompt>();
    }

    protected override void Start()
    {
        base.Start();

        // Set Up NPC Utitlity Assets:

        buttonPrompt.hide();
        buttonPrompt.gameObject.SetActive(false);

        // Set Up Dialog Cycle:

        UpdatePossibleDialogs();
        HideDialog();
        CurrentEmote = null;

        // Determine if NPC should Show:
        var NPCShowConditions = GetComponents<NPCCondition>().ToList();

        if (NPCCondition.ConditionsMet(NPCShowConditions))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    protected void UpdatePossibleDialogs()
    {
        PossibleDialogs = new List<NPCDialog>();

        foreach(var dialog in Dialogs)
        {
            if (dialog.CanAdd())
            {
                PossibleDialogs.Add(dialog);
            }
        }
    }

    protected override void triggerEnteredAction(Collider2D collision)
    {
        if (!IsTalking())
        {
            buttonPrompt.gameObject.SetActive(true);
            buttonPrompt.show();
        }
    }

    protected override void triggerExitAction(Collider2D collision)
    {
        if (IsTalking())
        {
            HideDialog();
        }
        else
        {
            buttonPrompt.hide();
        }
    }

    protected override void interactAction()
    {
        if (IsTalking())
        {
            CurrentDialog.OnHide.Invoke();
        }

        // Select Next Dialog:
        NPCDialog nextDialog = GetNextDialog();

        // Show Dialog:
        ShowDialog(nextDialog);

        // Hide Talk Prompt:
        buttonPrompt.hide();
        buttonPrompt.gameObject.SetActive(false);
    }

    private NPCDialog GetNextDialog()
    {
        if (PossibleDialogs.Count <= 0)
        {
            return null;
        }
        
        if (CurrentDialogPool == null || CurrentDialogPool.Count == 0)
        {
            CurrentDialogPool = new List<NPCDialog>(PossibleDialogs);
        }

        NPCDialog nextDialog = CurrentDialogPool.GetRandomElement();

        CurrentDialogPool.Remove(nextDialog);

        return nextDialog;
    }

    private void ShowDialog(NPCDialog dialog)
    {
        CurrentDialog = dialog;
        TextDisplay.text = dialog.Text;
        CurrentEmote = dialog.Emote.Trim();

        OnDialogShow.Invoke();
        CurrentDialog.OnShow.Invoke();
    }

    private void HideDialog()
    {
        TextDisplay.text = "";

        OnDialogHide.Invoke();
        CurrentDialog?.OnHide.Invoke();

        CurrentDialog = null;
    }

    public bool IsTalking()
    {
        return CurrentDialog != null;
    }
}
