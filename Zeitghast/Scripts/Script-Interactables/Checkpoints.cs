using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Checkpoints : MonoBehaviour
{
    private Animator animator;
    private bool hasAnimator;

    [SerializeField][EventRef] private string CheckPointSound = null;
    [SerializeField] private float SoundDelay;

    [Header("Checkpoint UI")]
    [SerializeField] private Color checkpointTextColor;
    [SerializeField][TextArea] private string checkpointText;
    [SerializeField]private float checkpointTextDuration;
    [SerializeField]private float checkpointTextFadeInDuration;
    [SerializeField]private float checkpointTextFadeOutDuration;

    public void Start()
    {
        animator = GetComponent<Animator>();

        if(animator == null)
        {
            hasAnimator = false;
        }
        else
        {
            hasAnimator = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo == null)
        {
            return;
        }

        if(hitInfo.tag != "Player")
        {
            return;
        }

        PlayerHealth playerHealth = hitInfo.transform.GetComponent<PlayerHealth>();
        if (playerHealth.lastCheckPointPosition.Equals(transform) && playerHealth.isDead)
        {
            return;
        }

        if (playerHealth.lastCheckPointPosition.checkpointScript != null)
        {
            Checkpoints lastCheckpoint = playerHealth.lastCheckPointPosition.checkpointScript;
            if (lastCheckpoint != null)
            {
                lastCheckpoint.removeCheckpoint();
            }
        }

        //if we dont have this saved as our current check point 
        if (playerHealth.lastCheckPointPosition.checkpointScript != this)
        {
            //Play the checkpoint sound
            Invoke("playCheckPointSound", SoundDelay);

            //Show the checkpoint UI
            displayCheckpointUI();
        }

        

        playerHealth.lastCheckPointPosition.position = transform.position;
        playerHealth.lastCheckPointPosition.lastScene = AdvancedSceneManager.Instance.getCurrentScene();
        playerHealth.lastCheckPointPosition.checkpointScript = this;


        if (hasAnimator)
        {
            animator.SetBool("CheckPointReached", true);
        }
    }
    public void removeCheckpoint()
    {
        if (hasAnimator)
        {
            animator.SetBool("CheckPointReached", false);
        }
    }

    private void playCheckPointSound()
    {
        if (!string.IsNullOrEmpty(CheckPointSound))
        {
            RuntimeManager.PlayOneShot(CheckPointSound, transform.position);
        }
    }

    private void displayCheckpointUI()
    {
        LocationNameManager.Instance.changeMainTextUIColor(checkpointTextColor);
        LocationNameManager.Instance.displayMainTextUI(checkpointText, checkpointTextDuration, checkpointTextFadeInDuration, checkpointTextFadeOutDuration);
    }
}
