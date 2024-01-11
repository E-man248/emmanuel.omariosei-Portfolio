using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreviewUI : MonoBehaviour
{
    public static LevelPreviewUI Instance { get; private set; }

    private string levelNameOnDisplay;
    private bool hardModeAvailable = false;
    
    private Animator animator;
    private long playerPauseDisableKey;
    internal bool isDisplaying = false;

    [Header("Preview Display Assets")]
    [SerializeField] private TextMeshProUGUI previewTitleText;
    [SerializeField] private Image previewImage;

    [Space]
    [SerializeField] private GameObject collectiblesUnavailableLabel;
    [SerializeField] private GameObject collectiblePreviewGroup;
    [Range(0,1)]
    [SerializeField] private float collectibleNotFoundTransparency = 0.5f;
    [SerializeField] private TextMeshProUGUI collectibleTotalText;

    [Space]
    [SerializeField] private TextMeshProUGUI normalModeBestRunTimeLeft;
    [SerializeField] private TextMeshProUGUI normalModeBestClearTime;
    [SerializeField] private TextMeshProUGUI hardModeBestRunTimeLeft;
    [SerializeField] private TextMeshProUGUI hardModeBestClearTime;

    [Space]
    [SerializeField] private GameObject challengeUnavailableLabel;
    [SerializeField] private GameObject challengePreviewGroup;
    [SerializeField] private TextMeshProUGUI challengeDescriptionText;
    [SerializeField] private GameObject challengeCompleteIcon;

    [Space]
    [SerializeField] private GameObject hardModeLockedButton;
    [SerializeField] private GameObject hardModeLockedButtonLabel;
    [SerializeField] private TextMeshProUGUI hardModeLockedButtonLabelText;
    [SerializeField] private GameObject hardModeUnlockedButtonLabel;

    [Header("Level Open Transition")]
    [SerializeField] private TransitionType levelOpenTransitionIn = TransitionType.None;
    [SerializeField] private TransitionType levelOpenTransitionOut = TransitionType.None;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        animator = GetComponent<Animator>();
    }

    public void ShowLevelPreview(LevelInfo levelInfo, LevelData normalModeLevelData, LevelData hardModeLevelData)
    {
        // Set Up Level Display:
        levelNameOnDisplay = levelInfo.baseLevelScene.name;
        hardModeAvailable = levelInfo.HasHardUnlocked();

        previewTitleText.text = levelInfo.DisplayName;
        previewImage.sprite = levelInfo.PreviewImage;

        SetUpCollectiblePreview(levelInfo, normalModeLevelData.CollectiblesCollected);

        SetUpHardModeButtonLabel(levelInfo, levelInfo.HasHardUnlocked(), normalModeLevelData.CollectiblesCollected);

        SetUpHighScoreDisplay(normalModeLevelData, hardModeLevelData);

        SetUpChallengePreview(levelInfo, normalModeLevelData.LevelChallengeComplete);

        // Open Level Display:
        OpenDisplay();
    }

    private void SetUpCollectiblePreview(LevelInfo levelInfo, Dictionary<string, bool> collectiblesCollected)
    {
        if (!levelInfo.HasCollectibles)
        {
            collectibleTotalText.text = "";

            collectiblesUnavailableLabel.SetActive(true);
            collectiblePreviewGroup.SetActive(false);
        }
        else
        {      
            // Set Up Collectible Icons:
            var collectibleIcons = collectiblePreviewGroup.GetComponentsInChildren<Image>();

            foreach (var collectibleIcon in collectibleIcons)
            {
                // We use the name on the icon to relate it with a collectible in the save data:
                bool collectibleFound = collectiblesCollected[collectibleIcon.name];

                // Change collectible transparency based on whether it was found:
                if (collectibleFound)
                {
                    collectibleIcon.color = new Color(collectibleIcon.color.r, collectibleIcon.color.g, collectibleIcon.color.b, 1f);
                }
                else
                {
                    collectibleIcon.color = new Color(collectibleIcon.color.r, collectibleIcon.color.g, collectibleIcon.color.b, collectibleNotFoundTransparency);
                }
            }

            // Set up Collectible Total:
            int numberOfCollectiblesCollected = collectiblesCollected.Count( x => x.Value == true );
            collectibleTotalText.text = "(" + numberOfCollectiblesCollected + "/" + collectiblesCollected.Count + ")";

            // Display Collectible Group:
            collectiblesUnavailableLabel.SetActive(false);
            collectiblePreviewGroup.SetActive(true);
        }
    }

    private void SetUpChallengePreview(LevelInfo levelInfo, bool challengeCompleted)
    {
        LevelChallenge challenge = levelInfo.Challenge;

        if (challenge != null)
        {
            challengeUnavailableLabel.SetActive(false);
            challengePreviewGroup.SetActive(true);

            challengeDescriptionText.text = challenge.GetDescription();
            challengeCompleteIcon.SetActive(challengeCompleted);
        }
        else
        {
            challengeUnavailableLabel.SetActive(true);
            challengePreviewGroup.SetActive(false);

            challengeDescriptionText.text = "";
        }
    }

    private void SetUpHardModeButtonLabel(LevelInfo levelInfo, bool hardModeUnlocked, Dictionary<string, bool> collectiblesCollected)
    {
        if (!levelInfo.HasHardMode())
        {
            hardModeLockedButton.SetActive(false);
            return;
        }

        hardModeLockedButton.SetActive(true);

        if (hardModeUnlocked)
        {
            // Show Only Unlocked Hard Mode Button Label:
            hardModeUnlockedButtonLabel.SetActive(true);
            hardModeLockedButtonLabel.SetActive(false);
        }
        else
        {
            // Calculate Collectibles Needed:
            int numberOfCollectiblesCollected = collectiblesCollected.Count( x => x.Value == true );
            int collectiblesNeeded = Mathf.Max(0, levelInfo.HardModeUnlockCollectibleQuota - numberOfCollectiblesCollected);

            // Format Locked Hard Mode Button Label Text:
            string formattedLabelText = hardModeLockedButtonLabelText.text;
            formattedLabelText = Regex.Replace(formattedLabelText, @"\d+", "" + collectiblesNeeded);

            hardModeLockedButtonLabelText.text = formattedLabelText;
            
            // Show Only Locked Hard Mode Button Label:
            hardModeUnlockedButtonLabel.SetActive(false);
            hardModeLockedButtonLabel.SetActive(true);
        }
    }

    private void SetUpHighScoreDisplay(LevelData normalModeLevelData, LevelData hardModeLevelData = null)
    {
        normalModeBestRunTimeLeft.text = LevelData.FormatTimeLeftString(normalModeLevelData.BestRunTimeLeft);
        normalModeBestClearTime.text = LevelData.FormatClearTimeString(normalModeLevelData.BestClearTime);
        
        if (hardModeLevelData != null)
        {
            hardModeBestRunTimeLeft.text = LevelData.FormatTimeLeftString(hardModeLevelData.BestRunTimeLeft);
            hardModeBestRunTimeLeft.gameObject.SetActive(true);
            hardModeBestClearTime.text = LevelData.FormatClearTimeString(hardModeLevelData.BestClearTime);
            hardModeBestClearTime.gameObject.SetActive(true);
        }
        else
        {
            hardModeBestRunTimeLeft.text = "";
            hardModeBestRunTimeLeft.gameObject.SetActive(false);
            hardModeBestClearTime.text = "";
            hardModeBestClearTime.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        handlePlayerInput();
    }

    private void handlePlayerInput()
    {
        if (Input.GetButtonDown("Cancel") && isDisplaying)
        {
            CloseDisplay();
        }
    }

    public void OpenDisplay()
    {
        // Set Display State to Active:
        animator.SetBool("DisplayActive", true);

        // Disable Player Pause:
        playerPauseDisableKey = PauseMenuUI.Instance.disablePlayerPause();

        // Unpause Game:
        Timer.PauseGame();

        isDisplaying = true;
    }

    public void CloseDisplay()
    {
        // Set Display State to Inactive:
        animator.SetBool("DisplayActive", false);

        // Enable Player Pause:
        PauseMenuUI.Instance.enablePlayerPause(playerPauseDisableKey);
        
        // Unpause Game:
        Timer.UnpauseGame();

        isDisplaying = false;
    }

    public void OpenLevelOnDisplay(bool hardModeEnabled)
    {
        // Check Request is Valid:
        if (hardModeEnabled && !hardModeAvailable)
        {
            return;
        }

        // Close Level Display:
        CloseDisplay();

        // Open Level:
        LevelSelectionManager.Instance.OpenLevel(levelNameOnDisplay, hardModeEnabled, levelOpenTransitionIn, levelOpenTransitionOut);
    }
}
