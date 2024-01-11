using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePage : EntityAnimationHandler
{
    #region Animation Variables
    public const string CutsceneInactive = "Cutscene_Page_Inactive";
    public const string CutscenePlaying = "Cutscene_Page_Playing";
    public const string CutsceneFinished = "Cutscene_Page_Finished";
    #endregion
    [field: SerializeField] public List<CutscenePanel> PanelList { get; protected set; }
    public CutscenePanel currentPanel { get; protected set; }
    [SerializeField] protected int currentPanelIndex = -1;

    public bool playing { get; protected set; } = false;
    public bool finished { get; protected set; } = false;

    protected override void Awake()
    {
        base.Awake();

        nextAnimation = CutsceneInactive;
        changeAnimation(CutsceneInactive);
    }

    protected override void Update()
    {
        base.Update();

        // Update "currentPanel" Object for other scripts:
        if (currentPanelIndex > PanelList.Count - 1 || currentPanelIndex < 0)
        {
            currentPanel = null;
        }
        else
        {
            currentPanel = PanelList[currentPanelIndex];
        }
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

    public void startNextPanel()
    {
        // If index is greater than the end of list then stop
        if (currentPanelIndex == PanelList.Count - 1 && currentPanel.finished)
        {
            skipAllPanels();
            currentPanelIndex++;
            return;
        }

        if (currentPanelIndex > PanelList.Count - 1)
        {
            skipAllPanels();
            return;
        }

        // If index is less than the start of list set to start
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = -1;
        }

        // If animation playing is not the first animation,
        // then skip the current panel so the next can start
        if (currentPanelIndex > 0)
        {
            PanelList[currentPanelIndex].skipCutscene();
        }

        // Increment to next panel
        currentPanelIndex++;
        syncCurrentPanelWithIndex();

        // Play new panel
        if (currentPanel != null)
        {
            Invoke("beginNextPanel", getAnimationLength("Cutscene_Page_Playing"));
        }

        playing = true;
        finished = false;
    }

    private void beginNextPanel()
    {
        currentPanel.startCutscene();
    }

    public void skipCurrentPanel()
    {
        // If index is greater than the end of list then stop
        if (currentPanelIndex > PanelList.Count - 1)
        {
            return;
        }

        PanelList[currentPanelIndex].skipCutscene();
    }

    public void skipAllPanels()
    {
        currentPanelIndex = PanelList.Count;

        foreach(CutscenePanel panel in PanelList)
        {
            panel.skipCutscene();
        }

        playing = false;

        playAnimationOnceFull(CutsceneFinished);
        Invoke("setPageFinished", getAnimationLength(CutsceneFinished));
    }

    public void resetPage()
    {
        currentPanelIndex = -1;
        currentPanel = null;

        resetPanels();
        
        playing = false;
        finished = false;
    }

    private void syncCurrentPanelWithIndex()
    {
        if (currentPanelIndex > PanelList.Count - 1 || currentPanelIndex < 0)
        {
            currentPanel = null;
        }
        else
        {
            currentPanel = PanelList[currentPanelIndex];
        }
    }

    private void resetPanels()
    {
        foreach(CutscenePanel panel in PanelList)
        {
            panel.resetPanel();
        }
    }

    private void setPageFinished()
    {
        finished = true;
    }
}
