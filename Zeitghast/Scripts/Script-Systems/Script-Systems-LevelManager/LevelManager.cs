using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tymski;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{   
    public static LevelManager Instance { get; private set; }
    private long disablePlayerPauseKey;

    [Header("In-Level Settings")]
    [SerializeField] public bool LevelStartSequenceEnabled = true;
    [SerializeField] public bool LevelEndSequenceEnabled = true;
    private bool LevelSceneIsHardMode = false;
    private bool HasPlayedTimesUpSequence = false;
    
    [Header("Player")]
    public int maxPlayerHealth = 100;
    public float timePenaltyForRespawn = -10f;

    [Space]
    public GameObject defaultPlayerArsenalSlot1;
    public GameObject defaultPlayerArsenalSlot2;

    [Space]
    [SerializeField] private GameObject PlayerStartPortal;

    [Space]
    [SerializeField] private bool StartPlayerAtStartPosition = true;

    [Header("Level Navigation")]

    [Space]

    public SceneReference NextLevelScene;

    [Space]

    public SceneReference ExitLevelScene;

    [Space]

    public SceneReference HubLevelScene;

    // Level Info:
    internal LevelInfo CurrentLevelInfo;

    // Saved Level Data: (From Past Runs)
    private LevelData CurrentLevelData;

    // Recent Run Results: (From Present Instance Run)

    public float RecentRunTimeLeft { get; private set; } = 0f;
    public bool NewRecordTimeLeftOnRecentRun { get; private set; } = false;

    public float RecentClearTime { get; private set; } = 0f;
    public bool NewRecordClearTimeOnRecentRun { get; private set; } = false;
    
    public int RecentRunCollectibleCount { get; private set; } = 0;

    public bool RecentChallengeStatus { get; private set; } = false;
    public bool NewHatUnlockedOnRecentRun { get; private set; } = false;

    [Header("Level Events")]
    public UnityEvent LevelStart;
    public UnityEvent LevelComplete;
    public UnityEvent<CollectibleInfo> NewCollectibleFound;
    public UnityEvent LevelChallengeCompleted;
    public UnityEvent TimesUp;

    #region Unity Functions
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        ResetRecentRunResults();
    }
    
    private void Start()
    {
        HasPlayedTimesUpSequence = false;
    }

    private void Update()
    {
        if (IsTimeUp())
        {
            PlayTimesUpSequence();
        }
    }
    #endregion

    #region Start of Level Sequence
    public void PlayLevelStartSequence()
    {
        // Load Level Info:
        LoadLevelInfo();

        // Load Level Data:
        LoadLevelData();
        
        // Set Up Player:
        SetUpPlayer();

        // Cut Camera to Player:
        CameraTargetManager.Instance.ChangeTarget(PlayerInfo.Instance.transform, CinemachineBlendDefinition.Style.Cut, 0f); 

        // Disable Player Unpause Control:
        disablePlayerPauseKey = PauseMenuUI.Instance.disablePlayerPause();

        // Pause Game and Reset Game Time:
        Timer.PauseGame();
        Timer.Instance.ResetTime();

        float startOfLevelAnimationLength = 0f;
        
        if (LevelStartSequenceEnabled)
        {
            // Play Start of Level Animations:
            startOfLevelAnimationLength = Timer.Instance.timerAnimationHandler.getAnimationLength(TimerAnimationHandler.LevelStartAnimationString);
            Timer.Instance.activateTimerPausedVisual();
            Timer.Instance.timerAnimationHandler.playLevelStartAnimation();

            // Initiate Coroutine to Complete Sequence when Animations are Complete:
            StartCoroutine(CompleteLevelStartSequenceCoroutine(startOfLevelAnimationLength));
        }
        else
        {
            StartCoroutine(CompleteLevelStartSequenceCoroutine(startOfLevelAnimationLength));
        }
    }

    private void SetUpPlayer()
    {
        Vector3 playerStartPosition = GetPlayerStartPosition();

        if (StartPlayerAtStartPosition)
        {
            PlayerInfo.Instance.ResetPlayerAndPosition.Invoke(playerStartPosition);
        }
        else
        {
            PlayerInfo.Instance.ResetPlayer(playerStartPosition);
        }

        SpawnPlayerStartPortal(playerStartPosition);

        Health playerHealth = PlayerInfo.Instance.GetComponent<Health>();
        playerHealth.SetMaxHealth(maxPlayerHealth);
        playerHealth.ResetHealth();

        PlayerHealthBar playerHealthBar = PlayerInfo.Instance.GetComponentInChildren<PlayerHealthBar>();
        playerHealthBar.SetUpBar();
    }

    protected virtual void SpawnPlayerStartPortal(Vector3 spawnPosition)
    {
        if (PlayerStartPortal != null)
        {
            Instantiate(PlayerStartPortal, spawnPosition, Quaternion.identity, null);
        }
    }

    public virtual Vector3 GetPlayerStartPosition()
    {
        Vector3 playerStartPosition;
        if (CurrentLevelInfo != null)
        {
            playerStartPosition = CurrentLevelInfo.GetPlayerStartPosition();
        }
        else
        {
            playerStartPosition = PlayerInfo.Instance.transform.position;
        }

        return playerStartPosition;
    }

    IEnumerator CompleteLevelStartSequenceCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        // Code to execute after the specified delay
        CompleteLevelStartSequence();
    }

    public void CompleteLevelStartSequence()
    {
        if (LevelStartSequenceEnabled)
        {
            // Disable Player Unpause Control:
            Timer.Instance.deactivateTimerPausedVisual();
        }

        // Reset Camera Blend Settings:
        CameraTargetManager.Instance.resetTarget(); 

        // Unpause Game:
        Timer.UnpauseGame();

        // Enable Player Unpause Control:
        PauseMenuUI.Instance.enablePlayerPause(disablePlayerPauseKey);

        // Uncomment if you want to ensure timer level start animation cancels (ex: If you want to play it early)
        // Timer.Instance.timerAnimationHandler.playIdleAnimation();

        // Start Real Time Tracker:
        RealTimeTracker.Instance.startTimer();

        LevelStart.Invoke();
    }

    #endregion

    #region End of Level Sequence

    public void PlayLevelEndSequence()
    {
        // Disable Player Unpause Control:
        disablePlayerPauseKey = PauseMenuUI.Instance.disablePlayerPause();

        // Pause Game and Reset Game Time:
        Timer.PauseGame();
        Timer.Instance.stopTimer();

        // Stop and Save the Real Time
        RealTimeTracker.Instance.stopTimer();
        
        // Open Level End Results Menu:
        Timer.Instance.activateTimerPausedVisual();

        // Store Recent Run Results:

        CurrentLevelData.LevelComplete = true;
        
        RecentRunTimeLeft = Timer.Instance.CurrentTime;
        NewRecordTimeLeftOnRecentRun = IsNewTimeLeftRecord(RecentRunTimeLeft, CurrentLevelData.BestRunTimeLeft);
        if (NewRecordTimeLeftOnRecentRun) CurrentLevelData.BestRunTimeLeft = RecentRunTimeLeft;

        RecentClearTime = RealTimeTracker.Instance.realtime;
        NewRecordClearTimeOnRecentRun = IsNewClearTimeRecord(RecentClearTime, CurrentLevelData.BestClearTime);
        if (NewRecordClearTimeOnRecentRun) CurrentLevelData.BestClearTime = RecentClearTime;

        RecentRunCollectibleCount = GetCurrentCollectibleCount();

        // Update Level Challenge Status if Never Completed : (Unchanged if the Result is Undecided (Null))
        if (!CurrentLevelData.LevelChallengeComplete)
        {
            RecentChallengeStatus = GetLevelChallengeComplete(CurrentLevelInfo?.Challenge) ?? CurrentLevelData.LevelChallengeComplete;
            CurrentLevelData.LevelChallengeComplete = RecentChallengeStatus;
            NewHatUnlockedOnRecentRun = RecentChallengeStatus;
            LevelChallengeCompleted.Invoke();
        }
        else
        {
            RecentChallengeStatus = true;
        }

        // Enable Player Unpause Control:
        PauseMenuUI.Instance.enablePlayerPause(disablePlayerPauseKey, 0);

        // Save Level Results: 
        SaveLevelData();

        // Unlock Unlockable Content:
        if (CurrentLevelInfo?.UnlockableHat != null && LevelHatUnlockConditionMet())
        {
            HatManager.Instance.UnlockHat(CurrentLevelInfo?.UnlockableHat.hatId);
        }
        
        LevelComplete.Invoke(); // Opens Level Results UI + More

        // Automatically Exit Level if End Sequence is Disabled:
        if (!LevelEndSequenceEnabled)
        {
            Timer.UnpauseGame();
            ExitLevel();
        }
    }

    #endregion

    #region Player Respawn Sequence

    public void PlayPlayerRespawnSequence()
    {
        // If Time's Up, Cancel Respawn and Play Time Up Sequence:
        if (IsTimeUp(true))
        {
            Timer.Instance.ChangeTime(timePenaltyForRespawn);
            AdvancedSceneManager.Instance.inSceneTransition(TransitionType.LoadingScreen, TransitionType.CircleCutOut);
            PlayTimesUpSequence();
            return;
        }

        // Disable Player Pause:
        disablePlayerPauseKey = PauseMenuUI.Instance.disablePlayerPause();

        // Reset Player and Set Player Position:
        Vector3 playerRespawnPosition = PlayerInfo.Instance.GetComponent<PlayerHealth>().getLastCheckPointPosition();

        PlayerInfo.Instance.ResetPlayer(playerRespawnPosition, false);
        PlayerInfo.Instance.SetPlayerPosition(playerRespawnPosition);

        // Pause Game:
        Timer.PauseGame();

        if (PlayerStartPortal != null)
        {
            Instantiate(PlayerStartPortal, playerRespawnPosition, Quaternion.identity, null);
        }

        // Snap Camera to New Player Position:
        CameraTargetManager.Instance.ChangeTarget(PlayerInfo.Instance.transform,CinemachineBlendDefinition.Style.Cut,0f);

        // Play Transition Out of Black with Trigger to Complete Respawn Sequence:

        Action completeRespawnSequence = () => StartCoroutine(CompletePlayerRespawnSequence());

        AdvancedSceneManager.Instance.inSceneTransition(TransitionType.LoadingScreen, TransitionType.CircleCutOut, completeRespawnSequence);
    }

    IEnumerator CompletePlayerRespawnSequence()
    {
        // Play Respawn Sequence Animations:
        float respawnAnimationLength = Timer.Instance.timerAnimationHandler.getAnimationLength(TimerAnimationHandler.PlayerRespawnAnimationString);
        Timer.Instance.activateTimerPausedVisual();
        Timer.Instance.timerAnimationHandler.playPlayerRespawnAnimation();

        // Reset Camera Blend Settings:
        CameraTargetManager.Instance.resetTarget();

        yield return new WaitForSecondsRealtime(respawnAnimationLength/2f);
        Timer.Instance.ChangeTime(timePenaltyForRespawn);

        yield return new WaitForSecondsRealtime(respawnAnimationLength/2f);

        Timer.UnpauseGame();

        // End Respawn Sequence Animations:
        Timer.Instance.deactivateTimerPausedVisual();

        // Resume Timer:
        Timer.Instance.continueTimer();

        // Enable Player Pause:
        PauseMenuUI.Instance.enablePlayerPause(disablePlayerPauseKey);
    }

    #endregion

    #region Time Up Sequence

    public void PlayTimesUpSequence()
    {
        if (HasPlayedTimesUpSequence) return;

        HasPlayedTimesUpSequence = true;

        // Pause Game:
        Timer.PauseGame();

        // Play Times Up Event:
        TimesUp.Invoke();
    }
    
    public bool IsTimeUp(bool includeRespawnPenalty = false)
    {
        float calculatedCurrentTime = Timer.Instance.CurrentTime;

        if (includeRespawnPenalty)
        {
            calculatedCurrentTime += timePenaltyForRespawn;
        }

        return calculatedCurrentTime <= 0;
    }

    #endregion

    #region Time Result

    private bool IsNewTimeLeftRecord(float currentTime, float currentBestTime)
    {
        return currentTime > currentBestTime;
    }

    private bool IsNewClearTimeRecord(float currentTime, float currentBestTime)
    {
        return currentTime < currentBestTime;
    }

    #endregion

    #region Collectibles
    
    public void StoreCollectibleFound(CollectibleInfo collectibleInfo)
    {
        try
        {
            CurrentLevelData.CollectiblesCollected[collectibleInfo.CollectibleNameId] = true;
        }
        catch (Exception e)
        {
            Debug.LogError("A collectible named " + collectibleInfo.CollectibleNameId + "does not exist in the Level Collectible List\n" + e.ToString());
        }

        NewCollectibleFound.Invoke(collectibleInfo);
    }

    public bool GetCollectibleStatus(CollectibleInfo collectibleInfo)
    {
        try
        {
            return CurrentLevelData.CollectiblesCollected[collectibleInfo.CollectibleNameId];
        }
        catch (Exception)
        {
            Debug.LogError("A collectible named " + collectibleInfo.CollectibleNameId + " does not exist in the Level Collectible List!");
            return false;
        }
    }

    public int GetCurrentCollectibleCount()
    {
        int result = 0;

        // Count the number of collectibles in the dictionary that have been collected.
        // This means the element's their collectible status is collected. (true)

        result = CurrentLevelData?.CollectiblesCollected?.Values?.Count(collectibleCollected => collectibleCollected == true) ?? 0;

        return result;
    }

    public bool HardModeCollectibleQuotaReached(int collectibleCount)
    {
        int currentCollectibleCount = collectibleCount;

        return currentCollectibleCount >= CurrentLevelInfo.HardModeUnlockCollectibleQuota;
    }

    #endregion

    #region Level Challenge

    private bool? GetLevelChallengeComplete(LevelChallenge challenge)
    {
        if (challenge == null) return null; // Challenges Can Be Null (Means No Challenge)

        if (challenge is ClearTimeLevelChallenge)
        {
            ClearTimeLevelChallenge clearTimeChallenge = (ClearTimeLevelChallenge) challenge;
            
            // If Difficulty Mode Matches Return Result, Otherwise Return Null (Undecided)
            if (clearTimeChallenge.inHardMode == LevelSceneIsHardMode)
            {
                // Compare Current Level Collectible Count to Challenge Value

                return MagicBookOfTricks.CompareValues(RecentClearTime, clearTimeChallenge.comparisonOperation, clearTimeChallenge.value);
            }
            else return null;
        }

        if (challenge is TimeLeftLevelChallenge)
        {
            TimeLeftLevelChallenge timeLeftChallenge = (TimeLeftLevelChallenge) challenge;
            
            // If Difficulty Mode Matches Return Result, Otherwise Return Null (Undecided)
            if (timeLeftChallenge.inHardMode == LevelSceneIsHardMode)
            {
                // Compare Current Level Collectible Count to Challenge Value

                return MagicBookOfTricks.CompareValues(RecentRunTimeLeft, timeLeftChallenge.comparisonOperation, timeLeftChallenge.value);
            }
            else return null;
        }

        if (challenge is CollectibleCountLevelChallenge)
        {
            CollectibleCountLevelChallenge collectibleCountChallenge = (CollectibleCountLevelChallenge) challenge;

            // Compare Current Level Collectible Count to Challenge Value

            return MagicBookOfTricks.CompareValues(GetCurrentCollectibleCount(), collectibleCountChallenge.comparisonOperation, collectibleCountChallenge.value);
        }

        return false;
    }

    #endregion

    #region Level Hat

    private bool LevelHatUnlockConditionMet()
    {
        return GetLevelChallengeComplete(CurrentLevelInfo?.Challenge) ?? false;
    }

    #endregion

    #region Level Achievement Management

    public void UnlockAchievement(string achievementId)
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

    #endregion

    #region Level Navigation
    
    public void ContinueToNextLevel(TransitionType nextLevelTransitionIn = TransitionType.CircleCutIn, TransitionType nextLevelTransitionOut = TransitionType.CircleCutOut)
    {
        // Open Next Level Scene:
        AdvancedSceneManager.Instance.loadScene(NextLevelScene.name, nextLevelTransitionIn, nextLevelTransitionOut);
    }

    public void RestartLevel(TransitionType restartTransitionIn = TransitionType.CircleCutIn, TransitionType restartTransitionOut = TransitionType.CircleCutOut)
    {
        // Reload Level Scene:
        AdvancedSceneManager.Instance.reloadScene(restartTransitionIn, restartTransitionOut);
    }

    public void ExitLevel(TransitionType exitTransitionIn = TransitionType.CircleCutIn, TransitionType exitTransitionOut = TransitionType.CircleCutOut)
    {
        // Open Exit Level Scene:
        AdvancedSceneManager.Instance.loadScene(ExitLevelScene.name, exitTransitionIn, exitTransitionOut);
    }

    public void ReturnToHubLevel(TransitionType exitTransitionIn = TransitionType.CircleCutIn, TransitionType exitTransitionOut = TransitionType.CircleCutOut)
    {
        // Return to Hub Level Scene:
        AdvancedSceneManager.Instance.loadScene(HubLevelScene.name, exitTransitionIn, exitTransitionOut);
    }

    #endregion

    #region Helper Functions

    public void ResetRecentRunResults()
    {
        // Time Left:
        RecentRunTimeLeft = 0f;
        NewRecordTimeLeftOnRecentRun = false;

        // Clear Time:
        RecentClearTime = 0f;
        NewRecordClearTimeOnRecentRun = false;
    
        // Collectible Count:
        RecentRunCollectibleCount = 0;

        // Challenge:
        NewHatUnlockedOnRecentRun = false;
    }

    #endregion

    #region Level Data Management

    public void LoadLevelInfo()
    {
        string sceneName = AdvancedSceneManager.Instance.getCurrentScene();

        LevelSceneIsHardMode = LevelInfo.LevelSceneIsHardMode(sceneName);

        string baseLevelSceneName = LevelInfo.GetBaseLevelSceneName(sceneName);
        CurrentLevelInfo = GameManager.Instance.GetLevelInfo(baseLevelSceneName);
    }

    /// <summary>
    /// This method retrieves save data and loads it into the level manager for use in the level.
    /// This method is expected to be called early after the creation of the level manager to ensure the
    /// level has the level is set up appropriately for the run.
    /// </summary>
    public void LoadLevelData()
    {
        // Get Save Data Key Using Level Info:
        string levelSaveDataKey;
        if (CurrentLevelInfo != null)
        {
            levelSaveDataKey = CurrentLevelInfo.GetSaveDataKey(LevelSceneIsHardMode);
        }
        else
        {
            levelSaveDataKey = AdvancedSceneManager.Instance.getCurrentScene();
        }

        // Get Collectible Save Data Key: (Retrieved From Normal Mode Data)
        string normalModeSaveDataKey;
        if (CurrentLevelInfo != null)
        {
            normalModeSaveDataKey = CurrentLevelInfo?.GetSaveDataKey(false);
        }
        else
        {
            normalModeSaveDataKey = AdvancedSceneManager.Instance.getCurrentScene();
        }

        // Retrieve Game Data from Game Save Data:
        GameData GameData = DataPersistanceManager.Instance.getGameData();
        
        // Check for Normal Mode of Current Level:
        bool normalModeDataExists = GameData.all_Level_Data.TryGetValue(normalModeSaveDataKey, out LevelData NormalModeLevelData);

        // Retrieve Level Data from Game Save Data:
        LevelData LevelData = DataPersistanceManager.Instance.GetLevelData(levelSaveDataKey);

        // Retrieve Collectible Data from Game Save Data:
        Dictionary<string,bool> CollectibleData;
        if (normalModeDataExists)
        {
            CollectibleData = NormalModeLevelData?.CollectiblesCollected;
        }
        else
        {
            CollectibleData = LevelData.GenerateBlankCollectibleDictionary();
        }

        // Retrieve Challenge Data from Game Save Data:
        bool challengeCompleted = NormalModeLevelData?.LevelChallengeComplete ?? false;

        // Store Level Data:
        CurrentLevelData = new LevelData(LevelData); // Deep Copy of Level Data

        // Store Collectible Data:
        CurrentLevelData.CollectiblesCollected = new (CollectibleData);

        // Store Challenge Data:
        CurrentLevelData.LevelChallengeComplete = challengeCompleted;

        // Log Level Data:
        Debug.Log(CurrentLevelData.ToString());
    }

    /// <summary>
    /// This method saves the level data from the level manager into the overall game save.
    /// </summary>
    public void SaveLevelData()
    {
        // Get Level Save Data Key Using Level Info:
        string levelSaveDataKey;
        if (CurrentLevelInfo != null)
        {
            levelSaveDataKey = CurrentLevelInfo.GetSaveDataKey(LevelSceneIsHardMode);
        }
        else
        {
            levelSaveDataKey = AdvancedSceneManager.Instance.getCurrentScene();
        }

        // Get Normal Mode Save Data Key:
        string normalModeSaveDataKey;
        if (CurrentLevelInfo != null)
        {
            normalModeSaveDataKey = CurrentLevelInfo?.GetSaveDataKey(false);
        }
        else
        {
            normalModeSaveDataKey = AdvancedSceneManager.Instance.getCurrentScene();
        }

        // Save Level Data:
        DataPersistanceManager.Instance.UpdateLevelData(levelSaveDataKey, CurrentLevelData);

        // Save Normal Mode Data:
        var updatedNormalModeSaveData = DataPersistanceManager.Instance.GetLevelData(normalModeSaveDataKey);
        updatedNormalModeSaveData.CollectiblesCollected = CurrentLevelData.CollectiblesCollected;
        updatedNormalModeSaveData.LevelComplete = CurrentLevelData.LevelComplete;
        updatedNormalModeSaveData.LevelChallengeComplete = CurrentLevelData.LevelChallengeComplete;

        DataPersistanceManager.Instance.UpdateLevelData(normalModeSaveDataKey, updatedNormalModeSaveData);

        DataPersistanceManager.Instance.UpdateLevelData(normalModeSaveDataKey, updatedNormalModeSaveData);

        // Save To Game Data Files:
        DataPersistanceManager.Instance.SaveGameData();
    }

    #endregion
}

[Serializable]
public class LevelData
{
    public LevelData(string levelName)
    {
        LevelName = levelName;
        CollectiblesCollected = GenerateBlankCollectibleDictionary();
        LevelComplete = false;
        BestClearTime = float.PositiveInfinity;
        BestRunTimeLeft = 0f;
        LevelChallengeComplete = false;
    }

    public LevelData(LevelData srcLevelSaveData)
    {
        LevelName = srcLevelSaveData.LevelName;
        CollectiblesCollected = new Dictionary<string, bool>(srcLevelSaveData.CollectiblesCollected);
        LevelComplete = srcLevelSaveData.LevelComplete;
        BestClearTime = srcLevelSaveData.BestClearTime;
        BestRunTimeLeft = srcLevelSaveData.BestRunTimeLeft;
        LevelChallengeComplete = srcLevelSaveData.LevelChallengeComplete;
    }

    public static Dictionary<string, bool> GenerateBlankCollectibleDictionary()
    {
        return Collectible.collectibleNames.ToDictionary(collectibleName => collectibleName, collectibleStatus => false);
    }

    public LevelData()
    {
        LevelName = "Unknown Level";
        CollectiblesCollected = null;
        LevelComplete = false;
        BestRunTimeLeft = float.PositiveInfinity;
        BestClearTime = 0f;
        LevelChallengeComplete = false;
    }

    public string LevelName;
    public Dictionary<string, bool> CollectiblesCollected;
    public bool LevelComplete;
    public float BestClearTime;
    public float BestRunTimeLeft;
    public bool LevelChallengeComplete;

    public override string ToString()
    {
        string output = LevelName + " | Collectible Count: " + CollectiblesCollected.Count + "\n\n";

        foreach (var collectible in CollectiblesCollected)
        {
            output += collectible.Key + " | Collected = " + collectible.Value + "\n";
        }

        output += "\nChallenge Status: " + ( LevelChallengeComplete ? "Complete" : "Incomplete" ) + "\n";

        return output;
    }

    public static string FormatTimeLeftString(float time)
    {
        if (time <= 0)
        {
            return "--.--";
        }
        else
        {
            return time.ToString("0.00");
        }
    }

    public static string FormatClearTimeString(float time)
    {
        if (time >= float.MaxValue)
        {
            return "--:--";
        }
        else
        {
            var timeSpan = TimeSpan.FromSeconds(time);
            return timeSpan.ToString("mm':'ss");
        }
    }
}
