using UnityEngine;

[CreateAssetMenu(fileName = "TimeLeftChallenge")]
public class TimeLeftLevelChallenge : LevelChallenge
{
    [Header("Challenge Settings")]
    public bool inHardMode;
    public ComparisonOperation comparisonOperation = ComparisonOperation.GreaterThanOrEqual;
    public float value = 0;
    
    [Header("Display Settings")]
    [SerializeField] private string sentenceHeaderText = "Beat the stage with ";
    [SerializeField] private string sentenceMiddleText = " left on the clock";
    [SerializeField] private string sentenceFooterText = "";

    public override string GetDescription()
    {
        return sentenceHeaderText + MagicBookOfTricks.GetComparisonOperationDisplayText(comparisonOperation) + " " + value + " seconds" + sentenceMiddleText + "in " + getModeDisplayText() + sentenceFooterText;
    }
    
    private string getModeDisplayText()
    {
        return inHardMode ? "Hard Mode" : "Normal Mode";
    }
}
