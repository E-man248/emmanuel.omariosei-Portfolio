using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NewGhostPlayer : MonoBehaviour
{
    public NewGhost ghost;
    private float timeValue;
    private int index1;
    private int index2;
    private GhostAnimationHandler ghostAnimationHandler;
    private List<SpriteRenderer> spriteRenderers;

    [Header("Replay Settings")]
    public float fadeDuration = 1;
    private float fadeTimer;

    private bool hasCollider;
    private bool playReplay;

    private void Awake()
    {
        //Checking for a ghost object
        if (ghost == null)
        {
            Debug.LogError(name + " has no Ghost Object to record to");
            return;
        }

        //Starting from the begining
        timeValue = ghost.ghostRecording[0].timeStamp;

        //Get Ghost Animation Handler:
        ghostAnimationHandler = GetComponentInChildren<GhostAnimationHandler>();

        //Getting sprite renderer
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();

        //Setting up and checking if the ghost has a collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            hasCollider = true;
        }
        else
        {
            hasCollider = false;
        }
        playReplay = false;

    }

    // Update is called once per frame
    void Update()
    {
        //Play if the ghost doesn't have a collider or told to play
        if(!hasCollider || playReplay)
        {
            //The ghost replaying update
            replayGhost();

            //Checking if the replay has finished 
            replayCompletionCheck();
        }
    }

    private void replayGhost()
    {
        //Updating time
        timeValue += Time.deltaTime;

        //Checking if we are in a replay State
        if (ghost.ghostPlaybackState != NewGhostPlaybackState.Replay)
        {
            return;
        }

        //Finding the correct index based on the time value
        getIndex();

        //Setting the position and sprite of the ghost player
        setPosition();
        setGraphicsRotation();
        setAnimation();
    }

    private void replayCompletionCheck()
    {
        //Checking if we have reached the end of the recording based either index 1 or 2 reaching the end of the ;ist
        if (index1 != ghost.ghostRecording.Count - 1 && index2 != ghost.ghostRecording.Count - 1)
        {
            return;
        }

        //getting the last index
        int lastIndex = ghost.ghostRecording.Count - 1;


        //if the time value is not 0 or at the very lat timevalue. we have reached the-
        //end of the replay so restart. 
        if (timeValue != ghost.ghostRecording[0].timeStamp && timeValue >= ghost.ghostRecording[lastIndex].timeStamp)
        {
            resetReplay();
        }
    }

    //Start playing if the player enters the trigger collider 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            return;
        }

        //checking if the replay has been played for the first time 
        if(!playReplay)
        {
            //Starting from the begining
            timeValue = ghost.ghostRecording[0].timeStamp;
        }
        playReplay = true;
        
    }
    private void resetReplay()
    {
        //Reseting the ghost player position to the begining of the recording
        timeValue = ghost.ghostRecording[0].timeStamp;
    }

    private void getIndex()
    {
        //Finding the index that matchs with our currnet time value
        for(int i = 0; i < ghost.ghostRecording.Count - 2; i++)
        {
            if (ghost.ghostRecording[i].timeStamp == timeValue)
            {
                index1 = i;
                index2 = i;
                return;
            }
            else if(ghost.ghostRecording[i].timeStamp < timeValue && timeValue < ghost.ghostRecording[i + 1].timeStamp)
            {
                index1 = i;
                index2 = i + 1;
                return;
            }
        }

        //If we can't find anything we have reached the end
        index1 = ghost.ghostRecording.Count - 1;
        index2 = ghost.ghostRecording.Count - 1;
    }

    private void setPosition()
    {
        //if the recorded position is the same as next recorded position, set the position. else lerp to next recorded position
        if (index1 == index2)
        {
            transform.position = ghost.ghostRecording[index1].position;
        }
        else
        {
            float interpolationFactor = (timeValue - ghost.ghostRecording[index1].timeStamp) / (ghost.ghostRecording[index2].timeStamp - ghost.ghostRecording[index1].timeStamp);
            transform.position = Vector3.Lerp(ghost.ghostRecording[index1].position, ghost.ghostRecording[index2].position, interpolationFactor);
        }
    }

    private void setGraphicsRotation()
    {
        //Setting the animation
        ghostAnimationHandler.transform.eulerAngles = ghost.ghostRecording[index1].graphicsRotation;
    }

    private void setAnimation()
    {
        //Setting the animation
        ghostAnimationHandler.animationToPlayNext = ghost.ghostRecording[index1].animation;
    }
}
