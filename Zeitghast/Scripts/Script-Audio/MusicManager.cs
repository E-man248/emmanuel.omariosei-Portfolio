using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    private StudioEventEmitter musicEmitter;
    // Start is called before the first frame update
    void Start()
    {
        musicEmitter = GetComponent<StudioEventEmitter>();
        Timer.gamePausedEvent += onGamePaused;
        Timer.gameUnpausedEvent += onGameUnPaused;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            musicEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
        

    private void onGamePaused()
    {
        
    }

    private void onGameUnPaused()
    {
        
    }
}
