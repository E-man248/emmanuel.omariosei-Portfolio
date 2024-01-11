using System;
using System.Collections.Generic;
using System.Linq;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region Temporary Game State

    public LevelManager CurrentLevelManager { get; private set; }
    public LevelInfo LastPlayedLevel { get; private set; }

    #endregion

    [Header("Level Information:")]
    [SerializeField] private List<LevelInfo> levels;

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

    protected void Start()
    {
        UpdateCurrentLevelManager();

        subscribeToEvents();
    }

    protected void OnEnable()
    {
        subscribeToEvents();
    }

    protected void OnDisable()
    {
        unsubscribeToEvents();
    }

    protected void OnDestroy()
    {
        unsubscribeToEvents();
    }

    #endregion

    #region Event Functions

    private void subscribeToEvents()
    {
        LevelManager.Instance?.LevelStart.AddListener(OnLevelStart);
        LevelManager.Instance?.LevelComplete.AddListener(OnLevelComplete);
        SceneManager.sceneLoaded += OnSceneLoaded;
    } 

    private void unsubscribeToEvents()
    {
        LevelManager.Instance?.LevelStart.RemoveListener(OnLevelStart);
        LevelManager.Instance?.LevelComplete.RemoveListener(OnLevelComplete);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnLevelStart()
    {
        if (CurrentLevelManager == null) return;

        if (CurrentLevelManager.CurrentLevelInfo == null) return;

        if (!CurrentLevelManager.CurrentLevelInfo.IsHubLevel)
        {
            LastPlayedLevel = CurrentLevelManager?.CurrentLevelInfo;
        }
    }
    
    private void OnLevelComplete()
    {
        // Check for Special Hat Unlocks:
        UpdateSpecialLevelEndHatUnlocks();

        // Check for Special Achievement Unlocks:
        UpdateSpecialLevelEndAchievements();
    }
    
    protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCurrentLevelManager();
    }

    private void UpdateCurrentLevelManager()
    {
        if (CurrentLevelManager != LevelManager.Instance)
        {
            // Try to Unsubscribe from Invalid Level Manager:
            CurrentLevelManager?.LevelStart.RemoveListener(OnLevelStart);
            CurrentLevelManager?.LevelComplete.RemoveListener(OnLevelComplete);

            // Set Valid Level Manager: 
            CurrentLevelManager = LevelManager.Instance;

            // Subscribe to Valid Level Manager:
            CurrentLevelManager?.LevelStart.AddListener(OnLevelStart);
            CurrentLevelManager?.LevelComplete.AddListener(OnLevelComplete);
        }
    }

    #endregion

    private void UpdateSpecialLevelEndHatUnlocks()
    {
        // Check for All Levels Complete Challenge:
        if (IsAllLevelsComplete() && !HatManager.Instance.GetHatUnlockStatus("TimeMachine"))
        {
            HatManager.Instance.UnlockHat("TimeMachine");
        }

        // Check for Full Completion Challenge:
        if (IsAllLevelsComplete() && IsAllCollectiblesFoundInGame() && IsAllChallengesCompleteInGame() && !HatManager.Instance.GetHatUnlockStatus("Crown"))
        {
            HatManager.Instance.UnlockHat("Crown");
        }
    }

    private void UpdateSpecialLevelEndAchievements()
    {
        // Check for All Hard Mode Levels Complete Achievement:
        if (IsAllLevelsComplete(true))
        {
            UnlockAchievement("HardmodeGamer");
        }

        // Check for All Collectibles Found Achievement:
        if (IsAllCollectiblesFoundInGame())
        {
            UnlockAchievement("ThePackRat");
        }

        // Check for All Challenges Complete Achievement:
        if (IsAllChallengesCompleteInGame())
        {
            UnlockAchievement("Challenjour");
        }

        // Check for All Challenges Complete Achievement:
        if (BeatTheDevsAchievementCheck())
        {
            UnlockAchievement("BeatTheDevs");
        }
    }

    private bool BeatTheDevsAchievementCheck()
    {
        if (LevelManager.Instance.CurrentLevelInfo.baseLevelScene != "Level - Tutorial 1")
        {
            return false;
        }

        if (LevelManager.Instance.RecentClearTime > 55)
        {
            return false;
        }

        return true;
    }

    private void UnlockAchievement(string achievementId)
    {
        try
        {
            AchievementsManager.Instance.UnlockAchievement(achievementId);
        }
        catch (Exception e)
        {
            Debug.Log("Unable to Unlock Achievement '" + achievementId + "'!\n" + e.ToString());
        }
    }

    public LevelInfo GetLevelInfo(string levelSceneName)
    {
        string baseLevelName = LevelInfo.GetBaseLevelSceneName(levelSceneName);

        return levels.SingleOrDefault(x => x.baseLevelScene.name == levelSceneName);
    }

    public LevelInfo GetLevelInfo(SceneReference levelScene)
    {
        return GetLevelInfo(levelScene.name);
    }

    public LevelData GetLevelData(string levelName)
    {
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(levelName))
        {
            throw new ArgumentException("The given level name does not have an associated level scene.");
        }

        GameData gameData = DataPersistanceManager.Instance.getGameData();
        
        return gameData.all_Level_Data[levelName];
    }

    public string GetNextLevelInProgression()
    {
        LevelInfo nextLevelInProgression = levels.FirstOrDefault( level => level.CountsTowardLevelProgression && !GetLevelData(level.baseLevelScene).LevelComplete );

        if (nextLevelInProgression == null)
        {
            return null;
        }

        return nextLevelInProgression.baseLevelScene;
    }

    public bool IsAllLevelsComplete(bool inHardMode = false)
    {
        return levels.Where( x => x.CountsTowardLevelProgression == true )
                .All( x => GetLevelData(x.GetSaveDataKey(inHardMode)).LevelComplete == true );
    }

    public bool IsAllCollectiblesFound(string levelSceneName)
    {
        bool allCollectiblesFound = GetLevelData(levelSceneName).CollectiblesCollected.All( status => status.Value ==  true );

        if (!allCollectiblesFound) print("Level " + levelSceneName + " collectibles not found!");

        return allCollectiblesFound;
    }

    public bool IsAllCollectiblesFoundInGame()
    {
        return levels.Where( x => x.CountsTowardLevelProgression == true && x.HasCollectibles ).All( x => IsAllCollectiblesFound(x.GetSaveDataKey()) );
    }

    public bool IsAllChallengesCompleteInGame()
    {
        return levels.Where( x => x.CountsTowardLevelProgression == true ).All( x => GetLevelData(x.GetSaveDataKey()).LevelChallengeComplete == true );
    }
}

[Serializable]
public class LevelInfo
{
    private static string HARD_MODE_INDICATOR = " - Hard Mode";
    
    [Header("General Level Info")]
    [SerializeField] public SceneReference baseLevelScene;
    [SerializeField] public SceneReference prerequisiteLevel;
    [SerializeField] public bool CountsTowardLevelProgression;
    [SerializeField] public bool IsHubLevel;
    
    [Header("Level Display Info")]
    [SerializeField] public string DisplayName;
    [field:SerializeField] public Sprite PreviewImage {get; private set;}

    [Header("Level Settings")]
    [SerializeField] private SceneReference normalModeLevelStartScene;
    [SerializeField] private Vector3 normalModePlayerStartPosition;
    [Space]
    [SerializeField] private SceneReference hardModeLevelStartScene;
    [SerializeField] private Vector3 hardModePlayerStartPosition;

    [Header("Level Collectibles")]
    [SerializeField] public bool HasCollectibles = true;
    [SerializeField] public int HardModeUnlockCollectibleQuota = 10;

    [Header("Level Challenge")]
    [SerializeField] public LevelChallenge Challenge;

    [Header("Level Hat")]
    [SerializeField] public HatInfo UnlockableHat;

    public Vector3 GetPlayerStartPosition(bool hardMode = false)
    {
        if (hardMode)
        {
            return hardModePlayerStartPosition;
        }
        else
        {
            return normalModePlayerStartPosition;
        }
    }

    public SceneReference GetLevelStartScene(bool hardMode = false)
    {
        if (hardMode)
        {
            return hardModeLevelStartScene;
        }
        else
        {
            return normalModeLevelStartScene;
        }
    }

    #region Level Status
    public bool IsUnlocked()
    {
        // Assume Level is Unlocked if there is no Prerequisite Level:
        LevelInfo preReqLevelInfo = GameManager.Instance.GetLevelInfo(prerequisiteLevel);
        
        if (preReqLevelInfo == null) return true;

        // Get Prerequisite Level Save Data Key and Data:
        var preReqLevelSaveDataKey = preReqLevelInfo.GetSaveDataKey();

        LevelData prerequisiteLevelData = GameManager.Instance.GetLevelData(preReqLevelSaveDataKey);

        // Level is Only Unlocked if Prerequisite Level is Complete:
        return prerequisiteLevelData.LevelComplete;
    }

    public bool HasHardMode()
    {
        return !string.IsNullOrEmpty(hardModeLevelStartScene?.name);
    }

    public bool HasHardUnlocked()
    {
        return true; // Replace with condition to Lock Hard Mode
    }

    public static bool LevelSceneIsHardMode(string levelSceneName)
    {
        return levelSceneName.Contains(HARD_MODE_INDICATOR);
    }
    #endregion

    #region Save Data Keys:

    public string GetSaveDataKey(bool hardMode = false)
    {
        if (hardMode)
        {
            return GetHardModeSaveDataKey();
        }
        else
        {
            return GetNormalModeSaveDataKey();
        }
    }
    private string GetNormalModeSaveDataKey()
    {
        return baseLevelScene.name;
    }

    private string GetHardModeSaveDataKey()
    {
        string HardModeLevelSceneName = GetBaseLevelSceneName(baseLevelScene) + HARD_MODE_INDICATOR;

        return HardModeLevelSceneName;
    }
    #endregion

    #region Base Level Scene Name Conversions:
    public static string GetBaseLevelSceneName(SceneReference levelScene)
    {
        return GetBaseLevelSceneName(levelScene.name);
    }

    public static string GetBaseLevelSceneName(string levelSceneName)
    {
        string baseLevelSceneName = levelSceneName.Replace(HARD_MODE_INDICATOR, "");

        return baseLevelSceneName;
    }
    #endregion
}
