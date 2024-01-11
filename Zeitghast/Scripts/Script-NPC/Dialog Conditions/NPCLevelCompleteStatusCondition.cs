using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;

public class NPCLevelCompleteStatusCondition : NPCCondition
{
    [Header("Level Complete Condition Settings")]
    [SerializeField] private SceneReference levelToCheck;
    [Space]
    [SerializeField] private bool requiredLevelCompleteStatus = true;
    public override bool GetStatus()
    {
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(levelToCheck.name))
        {
            Debug.LogError("Given Level Scene Name is Invalid for Level Complete Check!");

            return false;
        }

        LevelData levelData = GameManager.Instance.GetLevelData(levelToCheck.name);

        return requiredLevelCompleteStatus && levelData.LevelComplete;
    }
}
