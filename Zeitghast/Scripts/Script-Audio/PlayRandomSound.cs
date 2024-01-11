using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    [Header("Sound")]
    [EventRef][SerializeField] private string randomAmbientSound;

    [SerializeField] private int minRandomSoundTime = 3;
    [SerializeField] private int maxRandomSoundTime = 5;
    private float currentSoundTimer;

    // Start is called before the first frame update
    void Start()
    {
        setRandomSoundTime();
    }

    // Update is called once per frame
    void Update()
    {
        checkToPlayRandomSound();
    }


    private void setRandomSoundTime()
    {
        currentSoundTimer = Random.Range(minRandomSoundTime, maxRandomSoundTime + 1);
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
