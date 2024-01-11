using System.Collections;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;

public class NPCAnimationHandler : EntityAnimationHandler
{
    #region Animation Strings

    private string NPCIdleAnimationString = "NPCIdle";
    private string NPCTextBoxShowAnimationString = "NPCTextBoxShow";
    private string NPCTextBoxHideAnimationString = "NPCTextBoxHide";
    private string NPCTextBoxHiddenAnimationString = "NPCTextBoxHidden";

    #endregion

    private NPC npc;
    private AnimationHandler npcTextBoxAnimationHandler;

    private void Start()
    {
        npc = GetComponentInParent<NPC>();

        NPCIdleAnimationString = npc.NPCName + "Idle";

        npcTextBoxAnimationHandler = npc.TextBox.GetComponentInChildren<AnimationHandler>();
        npcTextBoxAnimationHandler.playAnimationOnceFull(NPCTextBoxHiddenAnimationString);
        
        subcribeToEvents();
    }

    private void OnEnable()
    {
        subcribeToEvents();
    }

    private void OnDisable()
    {
        unsubcribeToEvents();
    }

    private void OnDestroy()
    {
        unsubcribeToEvents();
    }

    private void subcribeToEvents()
    {
        npc?.OnDialogShow.AddListener(playShowDialog);
        npc?.OnDialogHide.AddListener(playHideDialog);
    }

    private void unsubcribeToEvents()
    {
        npc?.OnDialogShow.RemoveListener(playShowDialog);
        npc?.OnDialogHide.RemoveListener(playHideDialog);
    }

    protected override void animate()
    {
        nextAnimation = NPCIdleAnimationString;

        base.animate();
    }

    private void playShowDialog()
    {
        // Reset Animation Currently Playing:
        resetCurrentAnimation();

        // Play Emote Animation:
        playAnimationOnceFull(getEmoteAnimation(npc.CurrentEmote), () => !npc.IsTalking());

        // Play Dialog Textbox Pop-Up:
        npcTextBoxAnimationHandler.playAnimationOnceFull(NPCTextBoxShowAnimationString);
    }

    private void playHideDialog()
    {
        if (!npcTextBoxAnimationHandler.getCurrentAnimation().Equals(NPCTextBoxHiddenAnimationString))
        {
            npcTextBoxAnimationHandler.playAnimationOnceFull(NPCTextBoxHideAnimationString);
        }
    }

    private string getEmoteAnimation(string emoteType)
    {
        if (string.IsNullOrWhiteSpace(emoteType))
        {
            return NPCIdleAnimationString;
        }
        
        return npc.NPCName + emoteType;
    }
}