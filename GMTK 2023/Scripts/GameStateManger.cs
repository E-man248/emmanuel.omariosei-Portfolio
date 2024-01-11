
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Tymski;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateManger : MonoBehaviour
{
    public static GameStateManger Instance = null;
    [field: SerializeField] private int suspicionCost;
    [SerializeField] SceneReference sceneReference;

    [SerializeReference] private float inputDelay = 0.3f;
    private bool InputEnabled = true;

    [SerializeReference] private bool canToggleDay = true;

    public enum DayNightState
    {
        Day,
        Night
    }

    [field: SerializeField] public DayNightState currentTime{ get; private set;}

    public UnityEvent DayTimeEvent;
    public UnityEvent NightTimeEvent;
    public UnityEvent PlayerActionPerformedEvent;
    public UnityEvent GameOver;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        PlayerActionPerformedEvent = new();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       toggleCurrentTime();
       restartCurrentScene();
        exitToMainMenu();
    }

    public void toggleCurrentTime()
    {
        if (!canToggleDay) return;

        if (!InputEnabled) return;
        if (Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(DisableInputForDelay());
            toggleCurrentTime();
            if (currentTime == DayNightState.Day)
            {
                currentTime = DayNightState.Night;
            }
            else
            {
                currentTime = DayNightState.Day;
            }
            SuspicionMeter.instance.addSuspicionAmount(suspicionCost);
        }
    }

    public void restartCurrentScene()
    {
        if (!InputEnabled) return;
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    public void exitToMainMenu()
    {
        if (!InputEnabled) return;
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync(sceneReference);
        }
    }

    public void setCurrentTime(DayNightState dayNightState)
    {
        currentTime = dayNightState;
    }

    IEnumerator DisableInputForDelay()
    {
        InputEnabled = false;

        yield return new WaitForSeconds(inputDelay);

        InputEnabled = true;
    }
}
