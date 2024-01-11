using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSoundManger : MonoBehaviour
{
    private MovingPlatforms movingPlatform;
    private StudioEventEmitter movingSound;


    [Header("Sound")]
    [EventRef][SerializeField] private string randomAmbientSound = "event:/GameObject/MineShaftGas";

    [SerializeField]private int minRandomSoundTime = 5;
    [SerializeField] private int maxRandomSoundTime = 7;
    private float currentSoundTime;
    private float currentSoundTimer;

    // Start is called before the first frame update
    void Start()
    {
        movingPlatform = GetComponent<MovingPlatforms>();
        movingSound = GetComponent<StudioEventEmitter>();


        setRandomSoundTime();
    }

    // Update is called once per frame
    void Update()
    {
        if(movingPlatform.isMoving())
        {
            checkToPlayRandomSound();

            if (!movingSound.IsPlaying()) movingSound.Play();
        }
        else
        {
            movingSound.Stop();
        }
    }


    private void setRandomSoundTime()
    {
        currentSoundTime = Random.Range(minRandomSoundTime, maxRandomSoundTime);
        currentSoundTimer = currentSoundTime;
    }
    public void checkToPlayRandomSound()
    {
        //Tick down the timer
        currentSoundTimer -= Time.deltaTime;

        //retrun if the time is not up
        if (currentSoundTimer > 0f)
        {
            return;
        }

        //Play Sound
        if (!string.IsNullOrEmpty(randomAmbientSound))
        {
            RuntimeManager.PlayOneShot(randomAmbientSound, transform.position);
        }

        //Calculate a new random Time
        setRandomSoundTime();
    }
}
