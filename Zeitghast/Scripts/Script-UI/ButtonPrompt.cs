using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonPrompt : EntityAnimationHandler
{
    protected Animator animator;
    protected bool hidden = true;
    protected bool active = true;

    protected void Start()
    {
        animator = GetComponent<Animator>();

        if (!hidden)
        {
            nextAnimation = "ButtonPromptShow";
        }
        else
        {
            nextAnimation = "ButtonPromptHide";
        }
    }

    public void show()
    {
        nextAnimation = "ButtonPromptShow";
        hidden = false;
    }

    public void hide()
    {
        nextAnimation = "ButtonPromptHide";
        hidden = true;
    }

    public void activate()
    {
        nextAnimation = "ButtonPromptActivate";
        active = true;
    }

    public void deactivate()
    {
        nextAnimation = "ButtonPromptDeactivate";
        active = false;
    }
}
