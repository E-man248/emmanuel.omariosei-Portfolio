using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGhostRecorder : MonoBehaviour
{
    public NewGhost ghost;
    private float timer;
    private float timeValue;

    private playerAnimationHandler playerAnimationHandler;

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
        if (ghost.ghostPlaybackState == NewGhostPlaybackState.Recording)
        {
            ghost.ResetData();
            timer = 0;
            timeValue = 0;
        }
    }

    private void Start()
    {
        //getting the playerAnimationHandler renderer
        playerAnimationHandler = GetComponentInChildren<playerAnimationHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        //Updateing time values 
        timer += Time.deltaTime;
        timeValue += Time.deltaTime;

        //If we are in the recoding state and the correct time. record the ghost data
        if (ghost.ghostPlaybackState == NewGhostPlaybackState.Recording && timer >= 1/ ghost.recordingFrequency)
        {
            NewGhostState currentGhostState;
            currentGhostState.position = transform.position;
            currentGhostState.graphicsRotation = playerAnimationHandler.transform.eulerAngles;
            currentGhostState.animation = playerAnimationHandler.getCurrentAnimation();
            currentGhostState.timeStamp = timeValue;
            ghost.ghostRecording.Add(currentGhostState);

            timer = 0;
        }
    }
}
