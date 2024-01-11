using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    public Ghost ghost;
    private float timer;
    private float timeValue;

    private SpriteRenderer playerSpriteRenderer;
    private playerAnimationHandler playerA;

    // Start is called before the first frame update
    void Awake()
    {
        //Checking for a ghost object
        if(ghost == null)
        {
            Debug.LogError(name + " has no Ghost Object to record to");
            return;
        }

        //setting up for recording
        if (ghost.ghostPlaybackState == GhostPlaybackState.Recording)
        {
            ghost.ResetData();
            timer = 0;
            timeValue = 0;
        }
    }

    private void Start()
    {
        //getting the sprite renderer
        playerSpriteRenderer = GetComponentInChildren<playerAnimationHandler>().playerSpriteRenderer;
    }

    // Update is called once per frame
    void Update()
    {
        //Updateing time values 
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        //If we are in the recoding state and the correct time. record the ghost data
        if (ghost.ghostPlaybackState == GhostPlaybackState.Recording && timer >= 1/ ghost.recordingFrequancy)
        {
            GhostState currentGhostState;
            currentGhostState.position = transform.position;
            currentGhostState.sprite = playerSpriteRenderer.sprite;
            currentGhostState.timeStamp = timeValue;
            currentGhostState.spriteFlip = playerSpriteRenderer.flipX;
            ghost.ghostRecording.Add(currentGhostState);

            timer = 0;
        }
    }
}
