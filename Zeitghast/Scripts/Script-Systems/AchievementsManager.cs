using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager Instance { get; private set; }

    [field: SerializeField] public List<string> AchievementsIdList {get; private set;}

    public UnityEvent<string> AchievementUnlocked;

    #region Unity Functions
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    #endregion

    public string GetAchievementDisplayName(string achievementId)
    {
        var achievement = GetAchievementData(achievementId);

        return achievement.Name;
    }

    public string GetAchievementDescription(string achievementId)
    {
        var achievement = GetAchievementData(achievementId);

        return achievement.Description;
    }

    public void UnlockAchievement(string achievementId)
    {
        var achievement = GetAchievementData(achievementId);

        achievement.Trigger();

        AchievementUnlocked.Invoke(achievementId);
    }

    public void LockAchievement(string achievementId)
    {
        var achievement = GetAchievementData(achievementId);

        achievement.Clear();
    }
    
    public void ResetAllAchievements()
    {
        foreach (string achievement in AchievementsIdList)
        {
            LockAchievement(achievement);
        }
    }

    #region Helper Functions

    private Steamworks.Data.Achievement GetAchievementData(string achievementId)
    {
        if (!AchievementExists(achievementId))
        {
            throw new Exception("Achievement Id '" + achievementId + "' Does Not Exist In Master Id List!");
        }

        return SteamIntegrationManager.Instance.GetAchievementData(achievementId);
    }

    private bool AchievementExists(string achievementId)
    {
        return AchievementsIdList.Contains(achievementId);
    }

    #endregion
}
