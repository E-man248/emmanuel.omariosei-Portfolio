using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;

public class NPCLevelChallengeCompleteStatusCondition : NPCCondition
{
    [Header("Level Complete Condition Settings")]
    [SerializeField] private SceneReference levelToCheck;
    [Space]
    [SerializeField] private bool requiredChallengeCompleteStatus = true;
    public override bool GetStatus()
    {
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(levelToCheck.name))
        {
            Debug.LogError("Given Level Scene Name is Invalid for Level Complete Check!");

            return false;
        }

        string baseLevelName = GameManager.Instance.GetLevelInfo(levelToCheck.name).baseLevelScene.name;

        LevelData levelData = GameManager.Instance.GetLevelData(baseLevelName);

        return requiredChallengeCompleteStatus && levelData.LevelChallengeComplete;
    }
}
