using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tymski;

public class NPCLevelClearTimeCondition : NPCCondition
{
    [Header("Level Clear Time Condition Settings")]
    [SerializeField] private SceneReference levelToCheck;
    [Space]
    [SerializeField] private ComparisonOperation comparisonOperation = ComparisonOperation.GreaterThanOrEqual;
    [SerializeField] private float value = 0;

    public override bool GetStatus()
    {
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(levelToCheck.name))
        {
            Debug.LogError("Given Level Scene Name is Invalid for Level Clear Time Check!");

            return false;
        }

        LevelData levelData = GameManager.Instance.GetLevelData(levelToCheck.name);

        float bestClearTime = levelData.BestClearTime;

        return MagicBookOfTricks.CompareValues(bestClearTime, comparisonOperation, value);
    }
}
