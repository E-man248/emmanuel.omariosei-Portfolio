using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;

public class SceneDoor : Door
{
    [Header("Scene Door")]
    public Vector3 targetPosition;
    [SerializeField] private SceneReference targetScene;

    public TransitionType transitionIn;
    public TransitionType transitionOut;
    private Action loadingSceneAction = null;

    [Header("Time value in the next scene")]
    public bool setTimeValue;
    public float timeValue;

    protected override void Start()
    {
        base.Start();

        loadingSceneAction += () => 
        {
            if (targetPosition != null)
            {
                PlayerInfo.Instance.SetPlayerPosition(targetPosition);
            }
            else
            {
                PlayerInfo.Instance.SetPlayerPosition(Vector3.zero);
            }

            //Changing the time in the next scene 
            if(setTimeValue)
            {
                AdvancedSceneManager.Instance.useNewTime = true;
                AdvancedSceneManager.Instance.newTime = timeValue;
            }
        };

        SceneManager.sceneLoaded += SceneLoaded;
        SceneLoaded();
    }

    protected void SceneLoaded()
    {
        interactionCooldownTimer = interactionCooldown;
    }

    protected void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoaded();
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

    protected override void Update()
    {
        base.Update();
    }

    protected override void interactAction()
    {
        if (!active)
        {
            return;
        }

        base.interactAction();

        AdvancedSceneManager.Instance.loadScene(targetScene.name, transitionIn, transitionOut, loadingSceneAction);
    }

    protected override void setLastCheckpoint()
    {
        playerHealth.setLastCheckPointPosition(targetPosition, targetScene.name);
    }

    protected override void resetPlayerWeaponManager()
    {
        loadingSceneAction += () =>
        {
            base.resetPlayerWeaponManager();
        };
    }
}
