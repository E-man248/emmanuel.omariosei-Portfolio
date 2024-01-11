using System;
using Tymski;
using UnityEngine;

public class LevelCompleteIndicator : MonoBehaviour
{
    public SceneReference LevelScene;
    [field:SerializeField] public bool LevelComplete {private set; get;} = false;
    [SerializeField] private bool CheckCompleteStatusOnStart = false;

    private void Start()
    {
        if (LevelScene == null)
        {
            LevelComplete = false;
            return;
        }

        if (CheckCompleteStatusOnStart)
        {
            UpdateLevelComplete();
        }
    }

    private bool LevelIsComplete(string levelSceneName)
    {
        GameData gameData = DataPersistanceManager.Instance.getGameData();

        // Throw error if level name is invalid: (not in master list)
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(levelSceneName))
        {
            throw new ArgumentException("levelScene", "The given level scene name, '" + levelSceneName + "', does not have a valid level name contained in it.");
        }

        LevelData levelData = gameData.all_Level_Data[levelSceneName];

        return levelData.LevelComplete;
    }

    public void UpdateLevelComplete()
    {
        LevelComplete = LevelIsComplete(LevelScene.name);
    }
}
