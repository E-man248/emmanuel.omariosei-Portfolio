using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ClearTimeChallenge")]
public class ClearTimeLevelChallenge : LevelChallenge
{
    [Header("Challenge Settings")]
    public bool inHardMode;
    public ComparisonOperation comparisonOperation = ComparisonOperation.GreaterThanOrEqual;
    public float value = 0;

    [Header("Display Settings")]
    [SerializeField] private string sentenceHeaderText = "Beat the stage in ";
    [SerializeField] private string sentenceMiddleText = "";
    [SerializeField] private string sentenceFooterText = "";

    public override string GetDescription()
    {
        string valueDisplayText = getTimeValueDisplayText(value);

        return sentenceHeaderText + MagicBookOfTricks.GetComparisonOperationDisplayText(comparisonOperation) + " " + valueDisplayText + sentenceMiddleText + " in " + getModeDisplayText() + sentenceFooterText;
    }

    private string getTimeValueDisplayText(float timeValue)
    {
        var timeSpan = TimeSpan.FromSeconds(timeValue);

        string minutesText = timeSpan.ToString("m' minute'") + (Mathf.FloorToInt(timeValue / 60) > 1 ? "s" : "");
        string secondsText = timeSpan.ToString("s' second'") + (Mathf.FloorToInt(timeValue) > 1 ? "s" : "");

        string conjunction = !string.IsNullOrWhiteSpace(minutesText) && !string.IsNullOrWhiteSpace(secondsText) ? " and " : "";

        return minutesText + conjunction + secondsText;
    }

    private string getModeDisplayText()
    {
        return inHardMode ? "Hard Mode" : "Normal Mode";
    }
}
