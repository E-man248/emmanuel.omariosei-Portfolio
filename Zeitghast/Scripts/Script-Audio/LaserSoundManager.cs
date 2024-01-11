using FMODUnity;
using System;
using UnityEngine;

public class LaserSoundManager : MonoBehaviour
{
    public FMOD.Studio.EventInstance laserSoundEvent; 
    [SerializeField]private float MaxChargedPitchValue = 3f;
    [SerializeField]private float ChargedPitchRate = 0.5f;
    private float currentChargedPitchValue;

    [EventRef]
    public string laserSound = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateChargedPitchValue();
    }

    private void updateChargedPitchValue()
    {
        currentChargedPitchValue += Time.deltaTime * ChargedPitchRate;

        currentChargedPitchValue = Mathf.Clamp(currentChargedPitchValue, 0f, MaxChargedPitchValue);

        laserSoundEvent.setParameterByName("ChargeValue", currentChargedPitchValue);
    }

    private void OnEnable()
    {
        startLaserSound();
    }
    private void OnDisable()
    {
        stopLaserSound();
    }
    private void OnDestroy()
    {
        stopLaserSound();
    }

    void startLaserSound()
    {
        if (string.IsNullOrEmpty(laserSound)) return;

        laserSoundEvent = RuntimeManager.CreateInstance(laserSound);

        resetChargedPitchValue();
        

        laserSoundEvent.start();
    }

    void stopLaserSound()
    {
        resetChargedPitchValue();
        laserSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        laserSoundEvent.release();
    }

    void resetChargedPitchValue()
    {
        currentChargedPitchValue = 0f;
    }

}
