using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tymski;
using UnityEngine;

public class NPCLevelCollectibleCountCondition : NPCCondition
{
    [Header("Level Collectible Count Condition Settings")]
    [SerializeField] private SceneReference levelToCheck;
    [Space]
    [SerializeField] private ComparisonOperation comparisonOperation = ComparisonOperation.GreaterThanOrEqual;
    [SerializeField] private int value = 0;
    
    public override bool GetStatus()
    {
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(levelToCheck.name))
        {
            Debug.LogError("Given Level Scene Name is Invalid for Level Collectible Count Check!");

            return false;
        }

        string baseLevelName = GameManager.Instance.GetLevelInfo(levelToCheck.name).baseLevelScene.name;

        LevelData levelData = GameManager.Instance.GetLevelData(baseLevelName);

        int numberOfCollectiblesCollected = levelData.CollectiblesCollected.Count( collectibleStatus => collectibleStatus.Value );

        return MagicBookOfTricks.CompareValues(numberOfCollectiblesCollected, comparisonOperation, value);
    }
}
