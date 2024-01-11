using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tymski;
using UnityEngine;
using UnityEngine.AI;

public class LevelSelectionManager : MonoBehaviour
{
    public static LevelSelectionManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    public void SelectLevel(string baseLevelName)
    {
        // Pause Game:
        Timer.PauseGame();

        // Retrieve Level Info:
        LevelInfo levelInfo;
        try
        {
            levelInfo = GameManager.Instance.GetLevelInfo(baseLevelName);
        }
        catch (Exception)
        {
            throw new ArgumentException("The level info for the given level name , '" + baseLevelName + "', could not be found!");
        }

        // Retrieve Level Save Data:
        LevelData normalModeLevelData;
        LevelData hardModeLevelData = null;
        try
        {
            normalModeLevelData = GameManager.Instance.GetLevelData(levelInfo.GetSaveDataKey());
            
            if (levelInfo.HasHardMode())
            {
                hardModeLevelData = GameManager.Instance.GetLevelData(levelInfo.GetSaveDataKey(true));
            }
        }
        catch (Exception e)
        {
            throw new ArgumentException("Some of the level data for the given level name , '" + baseLevelName + "', could not be found!\n" + e.ToString());
        }

        // Show Level Preview:
        LevelPreviewUI.Instance.ShowLevelPreview(levelInfo, normalModeLevelData, hardModeLevelData);
    }

    public void OpenLevel(string baseLevelName, bool hardModeEnabled = false, TransitionType levelOpenTransitionIn = TransitionType.None, TransitionType levelOpenTransitionOut = TransitionType.None)
    {
        // Pause Game:
        Timer.PauseGame();

        // Retrieve Level Info:
        LevelInfo levelInfo;
        try
        {
            levelInfo = GameManager.Instance.GetLevelInfo(baseLevelName);
        }
        catch (Exception)
        {
            throw new ArgumentException("The level info for the given level name , '" + baseLevelName + "', could not be found!");
        }
        
        // Set Level Start Name and Position for Selected Mode:
        string levelStartSceneName;
        Vector3 levelStartPosition;

        levelStartSceneName = levelInfo.GetLevelStartScene(hardModeEnabled);
        levelStartPosition = levelInfo.GetPlayerStartPosition(hardModeEnabled);

        // Load Level Scene:
        AdvancedSceneManager.Instance.loadScene(levelStartSceneName, levelOpenTransitionIn, levelOpenTransitionOut, () => PlayerInfo.Instance.ResetPlayerAndPosition(levelStartPosition));
    }
}