using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.AI;

public class HealthStation : Interactable
{
    public int healingCost;

    //Private variables
    private Animator animator;
    private PlayerHealth playerHealth;
    private PlayerOnHitReciever playerOnHitReciever;
    private bool healedMessageIsDisplaying = false;
    private bool errorMessageIsDisplaying = false;
    private bool healthFullMessageIsDisplaying = false;
    
    [Space]
    [Header("Text Settings")]
    public string defaultMessage;
    
    public float healedMessageDuration = 2f;
    private float healedMessageTimer;
    public Color healedMessageColor;
    [TextArea] public List<string> healedMessagePool;

    [Header("Error Full Message")]
    public float errorMessageDuration = 1f;
    private float errorMessageTimer;
    public Color errorMessageColor;
    [TextArea] public List<string> errorMessagePool;

    [Header("Health Full Message")]
    public float healthFullMessageDuration = 1f;
    private float healthFullMessageTimer;
    public Color healthFullMessageColor;
    [TextArea] public List<string> healthFullMessagePool;

    [Space]
    [Header("Required Game Objects")]
    [SerializeField] TMPro.TextMeshPro CostText = null;
    [SerializeField] TMPro.TextMeshPro RegularText = null;

    [Header("Sound")]
    [EventRef] public string HealSound = null;
    [EventRef] public string RejectSound = null;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        CostText.text = "-" + healingCost +" Sec";
        RegularText.text = defaultMessage;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        animator.SetBool("playerIsInRangeOfHealthStation", enteredCollider);
        
        //Text Messages
        healedMessageTimer -= Time.deltaTime;
        healthFullMessageTimer -= Time.deltaTime;
        errorMessageTimer -= Time.deltaTime;
        revertMessages();
    }

    protected override void triggerEnteredAction(Collider2D collision)
    {
        if (playerHealth == null)
        {
            playerHealth = collision.transform.GetComponent<PlayerHealth>();
        }

        if (playerOnHitReciever == null)
        {
            playerOnHitReciever = collision.transform.GetComponent<PlayerOnHitReciever>();
        }
    }

    protected override void interactAction()
    {
        //Heal
        if (playerHealth != null)
        {
            animator.SetTrigger("healButtonPressed");
            healPlayer();
        }
        else
        {
            Debug.LogError("No Player Health");
        }
    }

    public void healPlayer()
    {
        float currentTime = Timer.Instance.GetCurrentTime();
        if (playerHealth.maxHealth == playerHealth.health)
        {
            displayHealthFullMessage();
            playRejectSound();
            return;
        }
        //Heal the player 
        if (currentTime >= healingCost)
        {
            playerHealth.ResetHealth();
            playerOnHitReciever.removeAllEffects();
            Timer.Instance.ChangeTime(-healingCost);
            displayHealedMessage();

            playHealSound();
        }
        else
        {
            displayCantAffordMessage();
            playRejectSound();
        }
    }

    public void displayHealthFullMessage()
    {
        if (healthFullMessageIsDisplaying) return;

        healthFullMessageIsDisplaying = true;
        healthFullMessageTimer = healthFullMessageDuration;
        // Font:
        RegularText.color = healthFullMessageColor;
        // Randomize Purchase Message:
        if (healthFullMessagePool.Count > 0)
        {
            RegularText.text = healthFullMessagePool[Random.Range(0, healedMessagePool.Count)];
            CostText.text = "";
        }
    }

    public void displayHealedMessage()
    {
        if (healedMessageIsDisplaying) return;

        healedMessageIsDisplaying = true;
        healedMessageTimer = healedMessageDuration;
        // Font:
        RegularText.color = healedMessageColor;
        // Randomize Purchase Message:
        if (healedMessagePool.Count > 0)
        {
            RegularText.text = healedMessagePool[Random.Range(0, healedMessagePool.Count)];
            CostText.text = "";
        }
    }
    public void displayCantAffordMessage()
    {
        if (errorMessageIsDisplaying) return;

        errorMessageIsDisplaying = true;
        errorMessageTimer = errorMessageDuration;

        RegularText.color = errorMessageColor;
        // Randomize Purchase Message:
        if (errorMessagePool.Count > 0)
        {
            RegularText.text = errorMessagePool[Random.Range(0, errorMessagePool.Count)];
            CostText.text = "";
        }

    }

    public void revertMessages()
    {
        if(healedMessageTimer <= 0f && healedMessageIsDisplaying)
        {
            RegularText.text = defaultMessage;
            CostText.text = "-" + healingCost + " Sec";
            healedMessageIsDisplaying = false;
            RegularText.color = healedMessageColor;
        }

        if (errorMessageTimer <= 0f && errorMessageIsDisplaying)
        {
            RegularText.text = defaultMessage;
            CostText.text = "-" + healingCost + " Sec";
            errorMessageIsDisplaying = false;
            RegularText.color = healedMessageColor;
        }

        if (healthFullMessageTimer <= 0f && healthFullMessageIsDisplaying)
        {
            RegularText.text = defaultMessage;
            CostText.text = "-" + healingCost + " Sec";
            healthFullMessageIsDisplaying = false;
            RegularText.color = healedMessageColor;
        }
    }

    private void playRejectSound()
    {
        //Play Reject Sound
        if (!string.IsNullOrEmpty(RejectSound))
        {
            RuntimeManager.PlayOneShot(RejectSound, transform.position);
        }
    } 
    
    private void playHealSound()
    {
        //Play Heal Sound
        if (!string.IsNullOrEmpty(HealSound))
        {
            RuntimeManager.PlayOneShot(HealSound, transform.position);
        }
    }
}
