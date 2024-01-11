using System;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    public float startingTimeValue;
    [SerializeField] protected bool GlobalTimeSafeZone = false;
    [field:SerializeField] public float CurrentTime {get; private set;}
    public bool timePause;
    public static bool gamePaused = false;
    public TimerAnimationHandler timerAnimationHandler;


    [HideInInspector] public bool inTimeSafeZone;

    [SerializeField] TMPro.TextMeshProUGUI CountdownText = null;

    [Header("Timer Paused")]
    public Color timePausedColor;
    private Color defaultColor;

    [Header("Timer Incrementation  Settings")]
    [SerializeField] private float defaultTimeScale = 1f;
    [SerializeField] private float LastSecTimeScaleThreshold = 5f;
    [SerializeField] private float LastSecTimeScale = 0.5f;
    private float currentTimeScale = 1f;

    [Header("Time Color")]
    [SerializeField] private Color timeAddedColor;
    [SerializeField] private Color timeRemovedColor;
    [SerializeField] private float timeChangeColorDuration;
    private float defaultColorResotrationTimer;
    private float timeChangeColorTimer;
    private bool timeIncremented;


    [Header("Flashing Settings")]
    public float flashSpeed;
    [Range(0, 1)] public float flashRange;
    private float flashTarget;
    private bool flashTimer;

    [Header("Sound")]
    public float pauseLPFSpeed;
    private float GlobalGamePausedValue; // <-- Name this better!!

    [Header("Depth Of Field")]
    public float DepthOfFieldSpeed;

    //Events
    public static event Action gamePausedEvent;
    public static event Action gameUnpausedEvent;
    public UnityEvent<float> onTimeChangeEvent;
    public Timer()
    {
        CurrentTime = startingTimeValue;
        onTimeChangeEvent = new UnityEvent<float>();
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentTime = startingTimeValue;

        subcribeToEvents();
        timerAnimationHandler = GetComponentInChildren<TimerAnimationHandler>();
    }

    void Start()
    {
        CountdownText = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        //setting the default text color 
        if (CountdownText != null)
        {
            defaultColor = CountdownText.color;
        }

        //Making sure the timer condition to change the color is false on start 
        timeChangeColorTimer = timeChangeColorDuration + 1f;
    }

    void Update()
    {
        updateTimer();

        if (GlobalTimeSafeZone)
        {
            //Starts flashing the timer and sets timer color to the pause color
            activateTimerPausedVisual();
        }

        manageLowpassFilter();

        timerVisualUpdate();
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

    private void subcribeToEvents()
    {
        gamePausedEvent += onGamePaused;
        gameUnpausedEvent += onGameUnpaused;
    }

    private void unsubcribeToEvents()
    {
        gamePausedEvent -= onGamePaused;
        gameUnpausedEvent -= onGameUnpaused;
    }

    public void ChangeTime(float TimeIncrements)
    {
       
        //Checking if  we are adding time or losing time
        if (CurrentTime + TimeIncrements > CurrentTime)
        {
            timeIncremented = true;
        }
        else
        {
            timeIncremented = false;
        }

        //Starting the time change color via the timer
        timeChangeColorTimer = 0f;
        //Setting up  the timer to restore the timer color back to defalut
        defaultColorResotrationTimer = 0f;

        onTimeChangeEvent.Invoke(TimeIncrements);

        SetTime(CurrentTime + TimeIncrements);
    }
    private void updateTimer()
    {
        //Setting time scale
        if (CurrentTime <= LastSecTimeScaleThreshold)
        {
            currentTimeScale = LastSecTimeScale;
        }
        else
        {
            currentTimeScale = defaultTimeScale;
        }


        if (!timePause && CurrentTime > 0 && !GlobalTimeSafeZone)
        {
            SetTime(CurrentTime - (Time.deltaTime * currentTimeScale));
        }

        if (CountdownText != null)
        {
            CountdownText.text = GetCurrentTimeFormattedString();
        }
    }

    public string GetCurrentTimeFormattedString()
    {
        return CurrentTime.ToString("0.00");
    }

    public void SetTime(float time)
    {
        if ((CurrentTime <= 0f) && (time <= 0f))
        {
            return;
        }

        CurrentTime = time;
        CurrentTime = Mathf.Clamp(CurrentTime, 0, float.MaxValue);
    }


    public void ResetTime()
    {
        SetTime(startingTimeValue);
    }

    public float GetCurrentTime()
    {
        return CurrentTime;
    }
    public void stopTimer()
    {
        if (inTimeSafeZone)
        {
            return;
        }

        timePause = true;

        //Starts flashing the timer and sets timer color to the pause color
        activateTimerPausedVisual();
    }
    public void continueTimer()
    {
        if (inTimeSafeZone)
        {
            return;
        }

        timePause = false;

        //Stop flashing the timer and sets timer color to the default color
        deactivateTimerPausedVisual();
    }

    public static void PauseGame()
    {
        Time.timeScale = 0f;
        gamePaused = true;

        if (gamePausedEvent != null)
        {
            //Invoking the pause Event
            gamePausedEvent();
        }
    }


    public static void UnpauseGame()
    {
        gamePaused = false;
        Time.timeScale = 1f;

        if (gameUnpausedEvent != null)
        {
            //Invoking the unpause Event
            gameUnpausedEvent();
        }
    }

    private void onGameUnpaused()
    {
        //Deactivate Depth of Field  
        PostProcessingManager.Instance.setDepthOfFieldEffect(1f, Instance.DepthOfFieldSpeed);

        //Unpause Reqiured Sounds
        FMODUnity.RuntimeManager.GetBus("bus:/WeaponSFX").setPaused(false);
        FMODUnity.RuntimeManager.GetBus("bus:/PlayerSFX").setPaused(false);
        FMODUnity.RuntimeManager.GetBus("bus:/GameObjects").setPaused(false);

        Instance.continueTimer();
    }

    private void onGamePaused()
    {
        Time.timeScale = 0f;
        gamePaused = true;

        //Activate Depth of Field  
        PostProcessingManager.Instance.setDepthOfFieldEffect(300f, Instance.DepthOfFieldSpeed);

        //Pause Reqiured Sounds
        FMODUnity.RuntimeManager.GetBus("bus:/WeaponSFX").setPaused(true);
        FMODUnity.RuntimeManager.GetBus("bus:/PlayerSFX").setPaused(true);
        FMODUnity.RuntimeManager.GetBus("bus:/GameObjects").setPaused(true);

        Instance.stopTimer();
    }

    //Starts flashing
    public void activateTimerPausedVisual()
    {
        //Flash the timer
        Instance.flashTimer = true;
    }


    //Stop flashing the timer 
    public void deactivateTimerPausedVisual()
    {
        //Stop Flashing the timer
        Instance.flashTimer = false;
    }

    //manages and vhanges the timer color
    private void timerTextColormanagment()
    {
        timeChangeColorTimer += Time.unscaledDeltaTime;

        //If the timer is changed we set to the  correct changed color else we set to eiter default or  paused color
        if (timeChangeColorTimer <= timeChangeColorDuration)
        {
            float timeChangelerpValue = timeChangeColorTimer / timeChangeColorDuration;

            //Change the color based on whether  we add or subtract time
            if (timeIncremented)
            {
                //lerping the colors 
                float time_Added_Lerped_R_Value = Mathf.Lerp(CountdownText.color.r, Instance.timeAddedColor.r, timeChangelerpValue);
                float time_Added_Lerped_G_Value = Mathf.Lerp(CountdownText.color.g, Instance.timeAddedColor.g, timeChangelerpValue);
                float time_Added_Lerped_B_Value = Mathf.Lerp(CountdownText.color.b, Instance.timeAddedColor.b, timeChangelerpValue);
                //Set timer color to the  lerped timeAddedColor color while keeping the same alpha value
                Instance.CountdownText.color = new Color(time_Added_Lerped_R_Value, time_Added_Lerped_G_Value, time_Added_Lerped_B_Value, Instance.CountdownText.color.a);
            }
            else
            {
                //lerping the colors 
                float time_Removed_Lerped_R_Value = Mathf.Lerp(CountdownText.color.r, Instance.timeRemovedColor.r, timeChangelerpValue);
                float time_Removed_Lerped_G_Value = Mathf.Lerp(CountdownText.color.g, Instance.timeRemovedColor.g, timeChangelerpValue);
                float time_Removed_Lerped_B_Value = Mathf.Lerp(CountdownText.color.b, Instance.timeRemovedColor.b, timeChangelerpValue);
                //Set timer color  to the  lerped  timeRemovedColor color while keeping the same alpha value
                Instance.CountdownText.color = new Color(time_Removed_Lerped_R_Value, time_Removed_Lerped_G_Value, time_Removed_Lerped_B_Value, Instance.CountdownText.color.a);
            }
        }
        else
        {
            defaultColorResotrationTimer += Time.unscaledDeltaTime;
            float defaulTimelerpValue = defaultColorResotrationTimer / timeChangeColorDuration;

            //Change the color based on whether  time is paused or not
            if (!timePause && !GlobalTimeSafeZone)
            {
                //lerping the colors 
                float default_Color_Lerped_R_Value = Mathf.Lerp(CountdownText.color.r, Instance.defaultColor.r, defaulTimelerpValue);
                float default_Color_Lerped_G_Value = Mathf.Lerp(CountdownText.color.g, Instance.defaultColor.g, defaulTimelerpValue);
                float default_Color_Lerped_B_Value = Mathf.Lerp(CountdownText.color.b, Instance.defaultColor.b, defaulTimelerpValue);
                //Set timer color  to the  lerped  default color while keeping the same alpha value
                Instance.CountdownText.color = new Color(default_Color_Lerped_R_Value, default_Color_Lerped_G_Value, default_Color_Lerped_B_Value, Instance.CountdownText.color.a);
            }
            else
            {
                //lerping the colors 
                float time_Paused_Lerped_R_Value = Mathf.Lerp(CountdownText.color.r, Instance.timePausedColor.r, defaulTimelerpValue);
                float time_Paused_Lerped_G_Value = Mathf.Lerp(CountdownText.color.g, Instance.timePausedColor.g, defaulTimelerpValue);
                float time_Paused_Lerped_B_Value = Mathf.Lerp(CountdownText.color.b, Instance.timePausedColor.b, defaulTimelerpValue);
                //Set timer color to the  lerped  pause color while keeping the same alpha value
                Instance.CountdownText.color = new Color(time_Paused_Lerped_R_Value, time_Paused_Lerped_G_Value, time_Paused_Lerped_B_Value, Instance.CountdownText.color.a);
            }
        }

    }
    private void timerVisualUpdate()
    {
        if (Instance.CountdownText == null) return;

        timerTextColormanagment();

        //Text Alpha and flashing management 
        if (!flashTimer)
        {
            //Lerping the alpha value back  to 1f which is fully visible 
            float defaultLerpedColor = Mathf.Lerp(CountdownText.color.a, defaultColor.a, Time.unscaledDeltaTime * flashSpeed);
            CountdownText.color = new Color(CountdownText.color.r, CountdownText.color.g, CountdownText.color.b, defaultLerpedColor);
        }
        else
        {
            //Lerping the alpha value between 1f and our target
            float lerpedColor = Mathf.Lerp(CountdownText.color.a, flashTarget, Time.unscaledDeltaTime * flashSpeed);
            CountdownText.color = new Color(CountdownText.color.r, CountdownText.color.g, CountdownText.color.b, lerpedColor);

            //Flip floping the alpha target
            if (CountdownText.color.a <= 0.01f + flashRange)
            {
                flashTarget = 1f;
            }
            else if (CountdownText.color.a >= 0.99f - flashRange)
            {
                flashTarget = 0f;
            }
        }
    }

    //Check if the game is paused or unpaused to  activate or deactive the lowpass filter 
    private void manageLowpassFilter()
    {
        if (gamePaused)
        {
            //Activate the lowpass filter on all sound
            GlobalGamePausedValue = Mathf.Lerp(GlobalGamePausedValue, 1f, pauseLPFSpeed);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalGamePaused", GlobalGamePausedValue);
        }
        else
        {
            //Deactivate the lowpass filter on all sound
            GlobalGamePausedValue = Mathf.Lerp(GlobalGamePausedValue, 0f, pauseLPFSpeed);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalGamePaused", GlobalGamePausedValue);
        }
    }
}
