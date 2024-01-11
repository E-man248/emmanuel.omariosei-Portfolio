using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnInteractTrigger : Interactable
{
    public UnityEvent interactEvent;
    protected ButtonPrompt buttonPrompt;

    protected override void Start()
    {
        base.Start();

        buttonPrompt = GetComponentInChildren<ButtonPrompt>();
        
        if (buttonPrompt != null) buttonPrompt.hide();
    }

    protected override void Update()
    {
        base.Update();

        if (buttonPrompt?.gameObject?.activeSelf == DisableInteraction)
        {
            buttonPrompt.gameObject.SetActive(!DisableInteraction);
        }
    }

    protected override void triggerEnteredAction(Collider2D collision)
    {
        if (buttonPrompt != null) buttonPrompt.show();
    }

    protected override void triggerExitAction(Collider2D collision)
    {
        if (buttonPrompt != null) buttonPrompt.hide();
    }

    protected override void interactAction()
    {
        interactEvent.Invoke();
    }
}
