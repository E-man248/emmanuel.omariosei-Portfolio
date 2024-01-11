using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected bool enteredCollider;
    [Header("Interactable Objects Settings")]
    public GameObject glowEffect;
    public float interactionCooldown = 0f;
    protected float interactionCooldownTimer;
    public bool DisableInteraction = false;
    private PlayerInput playerInput;


    protected virtual void Start()
    {
        playerInput = PlayerInfo.Instance.GetComponent<PlayerInput>();
        enteredCollider = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Timer.gamePaused) return;

        if(interactionCooldownTimer < 0f && !DisableInteraction)
        {
            checkForPlayerInput();
        }

        glowing();

        interactionCooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.tag == "Player")
        {
            triggerEnteredAction(collision);
            enteredCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.tag == "Player")
        {
            triggerExitAction(collision);
            enteredCollider = false;
        }
    }

    public virtual void checkForPlayerInput()
    {
        if (Input.GetButtonDown("Interact") && enteredCollider && !playerInput.controlsDisabled)
        {
            interactionCooldownTimer = interactionCooldown;
            interactAction();
        }
    }

    protected virtual void glowing()
    {
        if (enteredCollider && interactionCooldownTimer < 0f && !DisableInteraction)
        {
            glowEffect.SetActive(true);
        }
        else
        {
            glowEffect.SetActive(false);
        }
    }


    protected virtual void triggerEnteredAction(Collider2D collision)
    {

    }

    protected virtual void triggerExitAction(Collider2D collision)
    {

    }

    protected virtual void interactAction()
    {
        
    }
}
