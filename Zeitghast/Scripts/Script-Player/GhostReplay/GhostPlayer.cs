using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public Ghost ghost;
    private float timeValue;
    private int index1;
    private int index2;
    private SpriteRenderer spriteRenderer;
    private Material material;
    private bool hasAlpha;

    [Header("Replay Settings")]
    public float fadeDuration = 1;
    private float fadeTimer;
    private Coroutine resetReplayCoroutine;
    private float defaultAlphavalue;

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

        //Getting sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        if (material.HasProperty("_Alpha"))
        {
            defaultAlphavalue = spriteRenderer.material.GetFloat("_Alpha");
            hasAlpha = true;
        }
        else
        {
            hasAlpha = false;
        }

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
        timeValue += Time.unscaledDeltaTime;

        //Checking if we are in a replay State
        if (ghost.ghostPlaybackState != GhostPlaybackState.Replay)
        {
            return;
        }

        //Finding the correct index based on the time value
        getIndex();

        //Setting the position and sprite of the ghost player
        setPosition();
        setSprite();
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
            //checking if there is already a coroutine underway and stoping it if so.
            if (resetReplayCoroutine == null)
            {
                resetReplayCoroutine = StartCoroutine(resetReplay());
            }
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
    private IEnumerator resetReplay()
    {
        //Fading out the sprite
        fadeTimer = 0;
        while(fadeTimer < fadeDuration)
        {
            //Checking if we have an alpha value
            if (hasAlpha)
            {
                fadeTimer += Time.deltaTime;
                float currentAlpha = material.GetFloat("_Alpha");
                float lerpedAlpha = Mathf.Lerp(currentAlpha, 0f, (fadeTimer / fadeDuration));
                spriteRenderer.material.SetFloat("_Alpha", lerpedAlpha);
            }
            else
            {
                fadeTimer = fadeDuration;
            }
            

            yield return new WaitForEndOfFrame();
        }

        //Reseting the ghost player position to the begining of the recording
        timeValue = ghost.ghostRecording[0].timeStamp;

        //Fading in the sprite
        fadeTimer = 0;
        while (fadeTimer < fadeDuration)
        {
            if (hasAlpha)
            {
                fadeTimer += Time.deltaTime;
                float currentAlpha = material.GetFloat("_Alpha");
                float lerpedAlpha = Mathf.Lerp(currentAlpha, defaultAlphavalue, (fadeTimer / fadeDuration));
                spriteRenderer.material.SetFloat("_Alpha", lerpedAlpha);
            }
            else
            {
                fadeTimer = fadeDuration;
            }
            

            yield return new WaitForEndOfFrame();
        }

        resetReplayCoroutine = null;
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

    private void setSprite()
    {
        //Setting the sprite and it's flip
        spriteRenderer.sprite = ghost.ghostRecording[index1].sprite;
        spriteRenderer.flipX = ghost.ghostRecording[index1].spriteFlip;
    }
}
