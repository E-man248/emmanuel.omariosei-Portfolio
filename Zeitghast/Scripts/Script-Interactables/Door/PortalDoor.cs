using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PortalDoor : OnInteractTrigger
{
    public bool active { get; protected set; } = true;

    public bool IsActive()
    {
        return active;
    }

    public void activate()
    {
        active = true;
        if (buttonPrompt != null) 
        {
            buttonPrompt.gameObject.SetActive(true);
        }
    }

    public void deactivate()
    {
        active = false;
        if (buttonPrompt != null) 
        {
            buttonPrompt.hide();
            buttonPrompt.gameObject.SetActive(false);
        }
    }

    protected override void glowing()
    {
        if (enteredCollider && interactionCooldownTimer < 0f && active)
        {
            glowEffect.SetActive(true);
        }
        else
        {
            glowEffect.SetActive(false);
        }
    }

    public override void checkForPlayerInput()
    {
        if (!active)
        {
            return;
        }

        base.checkForPlayerInput();
    }

    protected override void triggerEnteredAction(Collider2D collision)
    {
        if (!active)
        {
            return;
        }

        if (buttonPrompt != null) buttonPrompt.show();
    }
}
