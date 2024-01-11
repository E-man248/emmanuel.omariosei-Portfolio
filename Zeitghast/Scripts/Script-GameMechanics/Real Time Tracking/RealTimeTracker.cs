using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeTracker : MonoBehaviour
{
    [SerializeField] public float realtime { get; private set; }
    private bool tickdownTime;
    public static RealTimeTracker Instance;
    public bool showInPlay { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // Get Data From Game Save:
        showInPlay = getShowInPlayFromGameSave();
    }


    // Start is called before the first frame update
    void Start()
    {
        resetRealTime();
        subscribeToEvents();
    }

    private void OnEnable()
    {
        subscribeToEvents();
    }

    private void OnDisable()
    {
        unSubscribeToEvents();
    }

    private void OnDestroy()
    {
        unSubscribeToEvents();
    }

    private void subscribeToEvents()
    {
        Timer.gamePausedEvent += OnTimePaused;
        Timer.gameUnpausedEvent += OnTimeUnpaused;
    }

    private void unSubscribeToEvents()
    {
        Timer.gamePausedEvent -= OnTimePaused;
        Timer.gameUnpausedEvent -= OnTimeUnpaused;
    }

    // Update is called once per frame
    void Update()
    {
        if(tickdownTime)
        {
            realtime += Time.deltaTime;
        }
    }

    public void resetRealTime()
    {
        setRealTime(0f);
    }

    public void setRealTime(float time)
    {
        realtime = Mathf.Clamp(time,0, float.MaxValue);
    }

    private bool getShowInPlayFromGameSave()
    {
        return DataPersistanceManager.Instance.GetOptionsData().showRealTimeTrackerInPlay;
    }

    public void setAndStoreShowInPlay(bool value)
    {
        showInPlay = value;

        // Update Game Save Options Data:

        OptionsData optionsData = DataPersistanceManager.Instance.GetOptionsData();
        optionsData.showRealTimeTrackerInPlay = showInPlay;

        DataPersistanceManager.Instance.UpdateOptionsData(optionsData);

        DataPersistanceManager.Instance.SaveGameData();
    }

    public void setShowInPlay(bool value)
    {
        showInPlay = value;
    }

    public void startTimer()
    {
        tickdownTime = true;
    }

    public void stopTimer() 
    {
        tickdownTime = false;
    }

    private void OnTimePaused()
    {
        stopTimer();
    }

    private void OnTimeUnpaused() 
    {
        startTimer();
    }

    internal void ToggleShowInPlay()
    {
        setAndStoreShowInPlay(!showInPlay);
    }
}
