using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;
using System;
using System.Collections;
using UnityEngine.Events;


public class PauseMenuUI : MonoBehaviour
{
    public const float DEFAULT_PLAYER_PAUSE_ENABLE_DELAY = 0.01f;
    public static PauseMenuUI Instance;
    public Animator animator;
    public GameObject pauseButton;

    [Header("Restart Button Settings")]
    [SerializeField] private TransitionType restartTransitionIn;
    [SerializeField] private TransitionType restartTransitionOut;
    private Vector3 restartTargetPosition = Vector3.zero;

    [Header("Exit Button Settings")]
    [SerializeField] private SceneReference exitScene;
    [SerializeField] private TransitionType exitTransitionIn;
    [SerializeField] private TransitionType exitTransitionOut;
    [SerializeField] private Vector3 exitTargetPosition = Vector3.zero;

    //Variables for disabling the player ability to press the escape key to pause
    private bool playerPauseEnabled = true;
    private long currentPlayerPauseEnabledKey = -1L;
    private Coroutine enablePlayerPauseWithDelayCoroutine;
    [Header("Events")]
    public UnityEvent PlayerPauseEvent;
    public UnityEvent PlayerUnpauseEvent;

    private void Awake()
    {
        playerPauseEnabled = true;

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        enablePlayerPauseWithDelayCoroutine = null;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        restartTargetPosition = PlayerInfo.Instance.transform.position;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && playerPauseEnabled)
        {
            if (!Timer.gamePaused)
            {
                pauseGame();
            }
            else
            {
                unpauseGame();
            }
        }
    }
    protected void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    protected void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    protected void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public void OnPauseButtonUIPressed()
    {
        if (playerPauseEnabled)
        {
            pauseGame();
        }
    }

    public void pauseGame()
    {
        showPauseMenu();
        Timer.PauseGame();
        PlayerPauseEvent.Invoke();
    }

    public void unpauseGame()
    {
        hidePauseMenu();
        Timer.UnpauseGame();
        PlayerUnpauseEvent.Invoke();
    }

    /// <summary>
    /// Try to disable player pause. If this process fails, it will not throw an error, but rather return 0L.
    /// </summary>
    public long TryDisablePlayerPause()
    {
        try
        {
            long key = disablePlayerPause();
            return key;
        }
        catch (Exception)
        {
            return 0L;
        }
    }


    /// <summary>
    /// disabling the player ability to press the escape key to pause and retruning a key
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public long disablePlayerPause()
    {
        //Check if PlayerPause is already disable by checking if a timestampkey exist
        if (currentPlayerPauseEnabledKey <= 0)
        {
            // If Enable Procedure was already taking place, cancel it.
            if (enablePlayerPauseWithDelayCoroutine != null)
            {
                StopCoroutine(enablePlayerPauseWithDelayCoroutine);
            }

            playerPauseEnabled = false;

            pauseButton.SetActive(false);

            currentPlayerPauseEnabledKey = DateTime.Now.Ticks;

            //Debug.Log("Player Pause Disabled! | Key To Enable: " + currentPlayerPauseEnabledKey);

            //Generate and return time stamp
            return currentPlayerPauseEnabledKey;
        }
        else
        {
            throw new InvalidOperationException("PlayerPauseDisable is already in use!!!");
        }
    }

    /// <summary>
    /// Try to disable player pause. If this process fails, it will not throw an error, but rather return false.
    /// </summary>
    public bool TryEnablePlayerPause(long key, float delay = DEFAULT_PLAYER_PAUSE_ENABLE_DELAY)
    {
        try
        {
            enablePlayerPause(key, delay);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Enables Player Pause after given delay if given the correct unlock key.<br/>
    /// (This process will be cancelled if Player Pause is disabled again during delay)
    /// </summary>
    /// <returns></returns>
    public void enablePlayerPause(long key, float delay = DEFAULT_PLAYER_PAUSE_ENABLE_DELAY)
    {
        //if it not the right key
        if (!currentPlayerPauseEnabledKey.Equals(key))
        {
            throw new InvalidOperationException("Key provided is incorrect!!!");
        }

        // enable controls back and reset key
        enablePlayerPauseWithDelayCoroutine = StartCoroutine(enablePlayerPauseAfterDelay(delay));
    }

    private IEnumerator enablePlayerPauseAfterDelay(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        playerPauseEnabled = true;
        pauseButton.SetActive(true);
        currentPlayerPauseEnabledKey = -1L;

        //Debug.Log("Player Pause Enabled!");
    }

    protected void SceneLoaded()
    {
        if(gameObject == null)
        {
            Destroy(this);
        }
        animator = GetComponent<Animator>();
        animator.SetBool("Paused", false);

        SceneManager.sceneLoaded += SceneLoaded;
    }

    protected void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoaded();
    }

    private void showPauseMenu()
    {
        if (pauseButton != null) pauseButton.SetActive(false);

        if (!ShopUI.Instance.shopOpened)
        {
            if(animator != null)
            {
                animator.SetBool("Paused", true);
            }
            else
            {
                Debug.Log("Animator in  " + name + "is null");
            }
        }
    }

    private void hidePauseMenu()
    {
        if (pauseButton != null) pauseButton.SetActive(true);

        if (!ShopUI.Instance.shopOpened)
        {
            if (animator != null)
            {
                animator.SetBool("Paused", false);
                animator.SetBool("WindowOpened", false);
            }
            else
            {
                Debug.Log("Animator in  " + name + "is null");
            }
        }        
    }

    public void showWindowButton()
    {
        if (animator != null)
        {
            animator.SetBool("WindowOpened", true);
        }
        else
        {
            Debug.Log("Animator in  " + name + "is null");
        }
    }

    public void hideWindowButton()
    {
        if (animator != null)
        {
            animator.SetBool("WindowOpened", false);
        }
        else
        {
            Debug.Log("Animator in  " + name + "is null");
        }
    }

    public void restartLevelButtonAction()
    {
        LevelManager.Instance.RestartLevel(restartTransitionIn, restartTransitionOut);
        hidePauseMenu();
    }

    public void exitLevelButtonAction()
    {
        LevelManager.Instance.ReturnToHubLevel(exitTransitionIn, exitTransitionOut);
        hidePauseMenu();
    }
}
