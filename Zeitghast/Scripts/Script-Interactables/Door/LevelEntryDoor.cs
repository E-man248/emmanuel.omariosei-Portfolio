using System;
using Tymski;
using UnityEngine;

public class LevelEntryDoor : PortalDoor
{
    public SceneReference baseLevelScene;

    private LevelInfo levelInfo;

    protected void Awake()
    {
        active = false;
    }

    protected override void Start()
    {
        base.Start();

        levelInfo = GameManager.Instance.GetLevelInfo(baseLevelScene);

        // Setup Any Present Level Complete Indicators:
        LevelCompleteIndicator levelCompleteIndicator = GetComponentInChildren<LevelCompleteIndicator>();
        if (levelCompleteIndicator != null)
        {
            levelCompleteIndicator.LevelScene = levelInfo.baseLevelScene;
            levelCompleteIndicator.UpdateLevelComplete();
        }

        // Setup Any Present Location Name Triggers:
        LocationNameTrigger locationNameTrigger = GetComponentInChildren<LocationNameTrigger>();
        if (locationNameTrigger != null)
        {
            locationNameTrigger.LocationNameText = levelInfo.DisplayName;
        }

        // Update Level Active:
        UpdateActiveStatus();
    }

    public void UpdateActiveStatus()
    {
        if (levelInfo == null)
        {
            return;
        }

        UpdateActiveStatus(levelInfo.IsUnlocked());
    }

    public void UpdateActiveStatus(bool levelUnlocked)
    {
        if (levelUnlocked)
        {
            activate();
        }    
        else
        {
            deactivate();
        }
    }

    protected override void interactAction()
    {
        base.interactAction();

        string baseLevelSceneName = LevelInfo.GetBaseLevelSceneName(baseLevelScene);

        LevelSelectionManager.Instance.SelectLevel(baseLevelSceneName);
    }
}