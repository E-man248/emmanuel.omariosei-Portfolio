
using UnityEngine;
using Tymski;
using UnityEngine.SceneManagement;
using System;

public class GameOverUI : MonoBehaviour
{
    public Animator animator;

    [Header("Restart Button Settings")]
    [SerializeField] private TransitionType restartTransitionIn;
    [SerializeField] private TransitionType restartTransitionOut;

    [Header("Exit Button Settings")]
    [SerializeField] private TransitionType exitTransitionIn;
    [SerializeField] private TransitionType exitTransitionOut;

    private long playerPauseControlKey;

    private void Start()
    {
        animator = GetComponent<Animator>();

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

    private void subcribeToEvents()
    {
        LevelManager.Instance?.TimesUp.AddListener(onTimeUp);
    }

    private void unsubcribeToEvents()
    {
        LevelManager.Instance?.TimesUp.RemoveListener(onTimeUp);
    }

    private void onTimeUp()
    {
        playerPauseControlKey = PauseMenuUI.Instance.TryDisablePlayerPause();
        OpenDisplay();
    }

    private void OpenDisplay()
    {
        animator.SetBool("GameIsOver", true);
    }

    private void CloseDisplay()
    {
        animator.SetBool("GameIsOver", false);
    }

    public void RestartLevel()
    {
        LevelManager.Instance.RestartLevel(restartTransitionIn, restartTransitionOut);
        CloseDisplay();
        PauseMenuUI.Instance.TryEnablePlayerPause(playerPauseControlKey);
    }

    public void ExitLevel()
    {     
        LevelManager.Instance.ReturnToHubLevel(exitTransitionIn, exitTransitionOut);
        CloseDisplay();
        PauseMenuUI.Instance.TryEnablePlayerPause(playerPauseControlKey);
    }
}
