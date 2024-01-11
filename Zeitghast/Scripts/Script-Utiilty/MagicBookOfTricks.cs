using System;
using System.Collections.Generic;

public static class MagicBookOfTricks
{
    public static T GetRandomElement<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            throw new ArgumentOutOfRangeException("list","This function cannot be used on an empty list!");
        }

        int randomIndex = UnityEngine.Random.Range(0, list.Count);


        return list[randomIndex];
    }

    public static string ToStringElements<T>(this List<T> list)
    {
        string result = "List Elements:";

        for (int i = 0; i < list.Count; i++)
        {
            result += "\n[" + i + "]: " + list[i].ToString();
        }

        return result;
    }

    public static bool CompareValues(float firstValue, ComparisonOperation comparisonOperation, float secondValue)
    {
        switch (comparisonOperation)
        {
            case ComparisonOperation.GreaterThanOrEqual:
            return firstValue >= secondValue;

            case ComparisonOperation.GreaterThan:
            return firstValue > secondValue;

            case ComparisonOperation.LessThanOrEqual:
            return firstValue <= secondValue;
        
            case ComparisonOperation.LessThan:
            return firstValue < secondValue;

            default:
            return firstValue == secondValue;
        }
    }

    public static bool CompareValues(int firstValue, ComparisonOperation comparisonOperation, int secondValue)
    {
        switch (comparisonOperation)
        {
            case ComparisonOperation.GreaterThanOrEqual:
            return firstValue >= secondValue;

            case ComparisonOperation.GreaterThan:
            return firstValue > secondValue;

            case ComparisonOperation.LessThanOrEqual:
            return firstValue <= secondValue;
        
            case ComparisonOperation.LessThan:
            return firstValue < secondValue;

            default:
            return firstValue == secondValue;
        }
    }

    public static string GetComparisonOperationDisplayText(ComparisonOperation comparisonOperation)
    {
        switch (comparisonOperation)
        {
            case ComparisonOperation.GreaterThanOrEqual:
                return "at least";

            case ComparisonOperation.GreaterThan:
                return "more than";

            case ComparisonOperation.Equal:
                return "exactly";

            case ComparisonOperation.LessThanOrEqual:
                return "at most";

            case ComparisonOperation.LessThan:
                return "less than";

            default:
                return "??";
        }
    }
}

public enum ComparisonOperation
{
    GreaterThanOrEqual,
    GreaterThan,
    LessThanOrEqual,
    LessThan,
    Equal,
}
