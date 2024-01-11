using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RealTimeTrackerUI : MonoBehaviour
{
    private float currentRealTimeValue = 0;
    public bool displayActive {get; private set;} = false;

    [Header("Graphics  Utilities")]
    [SerializeField] private TextMeshProUGUI realTimeValueText;
    [field: SerializeField] public RealTimeTrackerUIAnimationHandler animationHandler {get; private set;}

    #region Unity Functions
    private void Awake()
    {
        animationHandler = GetComponentInChildren<RealTimeTrackerUIAnimationHandler>();
    }

    private void Update()
    {
        // Fetch Real Time Tracker Data:

        currentRealTimeValue = RealTimeTracker.Instance.realtime;

        // Update Graphical Elements:

        UpdateRealTimeValueGraphics();

        // Check For Player Input:
        
        if (!Timer.gamePaused)
        {
            ButtonInputUpdate();
        }
    }

    private void Start()
    {
        subscribeToEvents();

        // If Game Is In Play At Start and Game Not Paused, Show Display:
        if (IsShowInPlay())
        {
            ShowDisplay();
        }
    }

    private void OnEnable()
    {
        subscribeToEvents();
    }

    private void OnDisable()
    {
        unsubscribeToEvents();
    }

    private void OnDestroy()
    {
        unsubscribeToEvents();
    }
    #endregion

    #region Event Functions
    private void subscribeToEvents()
    {
        PauseMenuUI.Instance?.PlayerPauseEvent.AddListener(OnPlayerPause);
        PauseMenuUI.Instance?.PlayerUnpauseEvent.AddListener(OnPlayerUnpause);
    }

    private void unsubscribeToEvents()
    {
        PauseMenuUI.Instance?.PlayerPauseEvent.AddListener(OnPlayerPause);
        PauseMenuUI.Instance?.PlayerUnpauseEvent.AddListener(OnPlayerUnpause);
    }

    private void OnPlayerPause()
    {
        ShowDisplay();
    }

    private void OnPlayerUnpause()
    {
        if (IsShowInPlay()) return;

        HideDisplay();
    }
    #endregion

    private void UpdateRealTimeValueGraphics()
    {
        realTimeValueText.text = LevelData.FormatClearTimeString(currentRealTimeValue);
    }

    private void ButtonInputUpdate()
    {
        if (Input.GetKeyDown("9"))
        {
            if (displayActive)
            {
                RealTimeTracker.Instance.setAndStoreShowInPlay(false);
                HideDisplay();
            }
            else
            {
                RealTimeTracker.Instance.setAndStoreShowInPlay(true);
                ShowDisplay();
            }
        }
    }

    public void ToggleShowInPlay()
    {
        if (IsShowInPlay())
        {
            RealTimeTracker.Instance.setAndStoreShowInPlay(false);
        }
        else
        {
            RealTimeTracker.Instance.setAndStoreShowInPlay(true);
        }
    }

    private bool IsShowInPlay()
    {
        return RealTimeTracker.Instance.showInPlay;
    }
    
    private void ShowDisplay()
    {
        animationHandler?.PlayAppearAnimation();
        displayActive = true;
    }

    private void HideDisplay()
    {
        animationHandler?.PlayDisappearAnimation();
        displayActive = false;
    }
}
