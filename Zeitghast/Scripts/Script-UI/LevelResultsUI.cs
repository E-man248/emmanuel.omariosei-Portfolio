using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tymski;
using UnityEngine;

public class LevelResultsUI : MonoBehaviour
{
    private Animator animator;

    [Header("Level Result Display Assets")]
    
    [SerializeField] private TextMeshProUGUI TimeLeftTextMesh;
    [SerializeField] private TextMeshProUGUI TimeLeftRecordTextMesh;
    [SerializeField] private string TimeLeftRecordText = "New Record!";
    
    [Space]

    [SerializeField] private TextMeshProUGUI ClearTimeTextMesh;
    [SerializeField] private TextMeshProUGUI ClearTimeRecordTextMesh;
    [SerializeField] private string ClearTimeRecordText = "New Record!";

    [Space]

    [SerializeField] private GameObject CollectibleDisplayGroup;
    [SerializeField] private TextMeshProUGUI CollectibleCountTextMesh;
    [SerializeField] private TextMeshProUGUI HardModeUnlockTextMesh;
    [SerializeField] private string HardModeUnlockText = "Hard Mode Unlocked!";

    [Space]

    [SerializeField] private GameObject ChallengeDisplayGroup;
    [SerializeField] private TextMeshProUGUI ChallengeDisplayTextMesh;
    [SerializeField] private GameObject ChallengeCompleteIcon;
    [SerializeField] private TextMeshProUGUI NewHatUnlockTextMesh;
    [SerializeField] private string NewHatUnlockText = "New Hat Unlocked!";

    [Header("Restart Button Transition")]
    [SerializeField] private TransitionType restartLevelTransitionIn;
    [SerializeField] private TransitionType restartLevelTransitionOut;

    [Header("Continue Button Transition")]
    [SerializeField] private TransitionType continueTransitionIn;
    [SerializeField] private TransitionType continueTransitionOut;

    [Header("Exit Button Transition")]
    [SerializeField] private TransitionType exitTransitionIn;
    [SerializeField] private TransitionType exitTransitionOut;

    private long disablePlayerPauseKey;

    #region Unity Execution Events

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        subcribeToEvents();
    }

    private void OnEnable()
    {
        subcribeToEvents();
    }
    private void OnDisable()
    {
        unsubcribeToEvents();
    }

    private void OnDestroy()
    {
        unsubcribeToEvents();
    }

    #endregion

    private void subcribeToEvents()
    {
        LevelManager.Instance?.LevelComplete.AddListener(PlayLevelEndSequence);
    }

    private void unsubcribeToEvents()
    {
        LevelManager.Instance?.LevelComplete.RemoveListener(PlayLevelEndSequence);
    }

    public void RestartLevel()
    {
        // Enable Player Unpause Control:
        PauseMenuUI.Instance.TryEnablePlayerPause(disablePlayerPauseKey);

        // Reload Level Scene:
        LevelManager.Instance.RestartLevel(restartLevelTransitionIn, restartLevelTransitionOut);
        
        // Close Visual Display:
        CloseDisplay();
    }

    public void ContinueToNextLevel()
    {
        // Enable Player Unpause Control:
        PauseMenuUI.Instance.TryEnablePlayerPause(disablePlayerPauseKey);

        // Begin Transition to Next Level Scene:
        LevelManager.Instance.ContinueToNextLevel(continueTransitionIn, continueTransitionOut);
        
        // Close Visual Display:
        CloseDisplay();
    }

    public void ExitLevel()
    {
        // Enable Player Unpause Control:
        PauseMenuUI.Instance.TryEnablePlayerPause(disablePlayerPauseKey);

        // Begin Transition to Level Hub Scene:
        LevelManager.Instance.ExitLevel(exitTransitionIn, exitTransitionOut);
        
        // Close Visual Display:
        CloseDisplay();
    }

    public void PlayLevelEndSequence()
    {
        // Do Not Display Screen if Level End Sequence is Disabled:
        if (!LevelManager.Instance.LevelEndSequenceEnabled) return;

        // Disable Player Unpause Control:
        disablePlayerPauseKey = PauseMenuUI.Instance.TryDisablePlayerPause();

        // Store Level Results in Visual Display:
        LoadDisplay();

        // Open Visual Display:
        OpenDisplay();

        // Note: When display is open, the menu options in said display must be used to progress...
    }
    
    public void LoadDisplay()
    {
        // Time Left:
        TimeLeftTextMesh.text = LevelData.FormatTimeLeftString(LevelManager.Instance.RecentRunTimeLeft);
        if (LevelManager.Instance.NewRecordTimeLeftOnRecentRun)
        {
            TimeLeftRecordTextMesh.text = TimeLeftRecordText;
        }
        else
        {
            TimeLeftRecordTextMesh.text = "";
        }
    
        // Clear Time:
        ClearTimeTextMesh.text = LevelData.FormatClearTimeString(LevelManager.Instance.RecentClearTime);
        if (LevelManager.Instance.NewRecordClearTimeOnRecentRun)
        {
            ClearTimeRecordTextMesh.text = ClearTimeRecordText;
        }
        else
        {
            ClearTimeRecordTextMesh.text = "";
        }

        // Collectibles:
        bool showCollectibleData = CollectibleDataDisplayEnabled();
        if (showCollectibleData)
        {
            if(CollectibleDisplayGroup != null) 
            {
                CollectibleDisplayGroup.SetActive(true);

                CollectibleCountTextMesh.text = LevelManager.Instance.RecentRunCollectibleCount + "/" + Collectible.collectibleNames.Count;
                HardModeUnlockTextMesh.text = ""; // Hard Mode Unlock Notification Hidden (Removed Feature)
            }
        }
        else
        {
            if(CollectibleDisplayGroup != null) CollectibleDisplayGroup.SetActive(false);
        }

        // Challenge:
        bool showChallengeData = ChallengeDataDisplayEnabled();
        if (showChallengeData)
        {
            if (ChallengeDisplayGroup != null) 
            {
                ChallengeDisplayGroup.SetActive(true);

                ChallengeDisplayTextMesh.text = LevelManager.Instance.CurrentLevelInfo.Challenge.GetDescription();

                ChallengeCompleteIcon.SetActive(LevelManager.Instance.RecentChallengeStatus);

                NewHatUnlockTextMesh.text = LevelManager.Instance.NewHatUnlockedOnRecentRun ? NewHatUnlockText : "";
            }
        }
        else
        {
            if(ChallengeDisplayGroup != null) ChallengeDisplayGroup.SetActive(false);
        }
    }

    private bool CollectibleDataDisplayEnabled()
    {
        bool currentLevelHasCollectibles = LevelManager.Instance.CurrentLevelInfo?.HasCollectibles ?? false;

        return currentLevelHasCollectibles;
    }

    private bool ChallengeDataDisplayEnabled()
    {
        bool currentLevelHasChallenge = LevelManager.Instance.CurrentLevelInfo?.Challenge != null;

        return currentLevelHasChallenge;
    }
    
    public void OpenDisplay()
    {
        animator.SetBool("DisplayActive", true);
    }

    public void CloseDisplay()
    {
        animator.SetBool("DisplayActive", false);
    }
}
