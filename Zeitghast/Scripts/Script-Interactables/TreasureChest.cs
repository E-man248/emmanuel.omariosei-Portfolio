using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class TreasureChest : Interactable
{
    [Header("Treasure Chest Contents")]
    public GameObject chestContents;
    [SerializeField] private Vector2 contentSpawnOffset = Vector3.zero; 
    private Animator animator;
    private bool opened = false;
    protected ButtonPrompt buttonPrompt;

    [Header("Sound")]
    [EventRef][SerializeField] private string openSound;

    protected TreasureChest()
    {
        opened = false;
    }

    protected override void Start()
    {
        base.Start();
        glowEffect.SetActive(!opened);

        animator = GetComponent<Animator>();
        
        SceneManager.sceneLoaded += SceneLoaded;
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;

        buttonPrompt = GetComponentInChildren<ButtonPrompt>();
        buttonPrompt.hide();
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    protected virtual void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoaded();
    }
 
    private void SceneLoaded()
    {
        animator = GetComponent<Animator>();

        if (AdvancedSceneManager.Instance.treasureChestDictionary.ContainsKey(name))
        {
            opened = AdvancedSceneManager.Instance.treasureChestDictionary[name];
            
            if (opened)
            {
                animator.Play("TreasureChestClosed");
            }
            else
            {
                animator.Play("TreasureChestOpen");
            }
        }
    }

    private void LoadingScreenAction()
    {
        if (!AdvancedSceneManager.Instance.treasureChestDictionary.ContainsKey(name))
        {
            AdvancedSceneManager.Instance.treasureChestDictionary.Add(name, opened);
        }
        else
        {
            AdvancedSceneManager.Instance.treasureChestDictionary[name] = opened;
        }
    }

    protected override void Update()
    {
        if (Timer.gamePaused) return;
        checkForPlayerInput();

        if (!opened)
        {
            glowing();
        }

        interactionCooldownTimer -= Time.deltaTime;
    }

    protected override void triggerEnteredAction(Collider2D collision)
    {
        if (!opened) buttonPrompt.show();
    }

    protected override void triggerExitAction(Collider2D collision)
    {
        if (!opened) buttonPrompt.hide();
    }


    protected override void interactAction()
    {
        if (opened) return;

        if (!string.IsNullOrEmpty(openSound))
        {
            RuntimeManager.PlayOneShot(openSound, transform.position);
        }

        buttonPrompt.hide();
        animator.Play("TreasureChestOpen");

        opened = true;
        glowEffect.SetActive(false);
        
        if (chestContents != null)
        {
            Instantiate(chestContents, transform.position + new Vector3(contentSpawnOffset.x,  contentSpawnOffset.y, 0f), transform.rotation);
        }
    }
}
