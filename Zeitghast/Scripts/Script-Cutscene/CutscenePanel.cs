using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CutscenePanel : EntityAnimationHandler
{
    public const string CutsceneInactive = "Cutscene_Panel_Inactive";
    public const string CutscenePlaying = "Cutscene_Panel_Playing";
    public const string CutsceneFinished = "Cutscene_Panel_Finished";

    [SerializeField][EventRef] private string panelStartSound;

    public bool playing { get; protected set; } = false;
    public bool finished { get; protected set; } = false;

    protected override void Awake()
    {
        base.Awake();

        nextAnimation = CutsceneInactive;
        changeAnimation(CutsceneInactive);
    }
    protected void Start()
    {
    }

    protected override void animate()
    {
        base.animate();

        if (playing)
        {
            nextAnimation = CutscenePlaying;
        }

        if (finished)
        {
            nextAnimation = CutsceneFinished;
        }
    }

    public void startCutscene()
    {
        //The sound playedd when a panel begins 
        playPanelStartSound();

        playing = true;
        Invoke("skipCutscene", getAnimationLength(CutscenePlaying));
    }

    public void skipCutscene()
    {
        finished = true;
        playing = false;
    }

    public void resetPanel()
    {
        nextAnimation = CutsceneInactive;
        changeAnimation(CutsceneInactive);

        finished = false;
        playing = false;
    }

    //The sound playedd when a panel begins
    public void playPanelStartSound()
    {
        if(string.IsNullOrEmpty(panelStartSound))
        {
            return;
        }

        RuntimeManager.PlayOneShot(panelStartSound, transform.position);
    }
}
