using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleCountChallenge")]
public class CollectibleCountLevelChallenge : LevelChallenge
{
    [Header("Challenge Settings")]
    public ComparisonOperation comparisonOperation = ComparisonOperation.GreaterThanOrEqual;
    public int value;
    
    [Header("Display Settings")]
    [SerializeField] private string sentenceHeaderText = "Collect ";
    [SerializeField] private string sentenceFooterText = "";

    public override string GetDescription()
    {
        string comparisonOperationDisplayText;
        if (value == Collectible.collectibleNames.Count)
        {
            comparisonOperationDisplayText = "all";
        }
        else
        {
            comparisonOperationDisplayText = MagicBookOfTricks.GetComparisonOperationDisplayText(comparisonOperation);
        }

        return sentenceHeaderText + comparisonOperationDisplayText + " " + value + " time artifacts" + sentenceFooterText;
    }
}