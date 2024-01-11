using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tymski;
using UnityEngine;

public class NPCLevelTimeLeftCondition : NPCCondition
{
    [Header("Level Time Left Condition Settings")]
    [SerializeField] private SceneReference levelToCheck;
    [Space]
    [SerializeField] private ComparisonOperation comparisonOperation = ComparisonOperation.GreaterThanOrEqual;
    [SerializeField] private float value = 0;
    
    public override bool GetStatus()
    {
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(levelToCheck.name))
        {
            Debug.LogError("Given Level Scene Name is Invalid for Level Time Left Check!");

            return false;
        }

        LevelData levelData = GameManager.Instance.GetLevelData(levelToCheck.name);

        float bestRunTimeLeft = levelData.BestRunTimeLeft;

        return MagicBookOfTricks.CompareValues(bestRunTimeLeft, comparisonOperation, value);
    }
}
