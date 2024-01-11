using UnityEngine;
using FMODUnity;

public class AntagSoundManager : EnemySoundHandler
{
    [Header("Antag Move Sounds")]
    [EventRef][SerializeField] private string MoveSound;
    [EventRef][SerializeField] private string AltMoveSound;

    [Header("RockThrow  Sounds")]
    [EventRef][SerializeField] private string RockThrowWindUpSound;
    [EventRef][SerializeField] private string RockThrowSound;

    [Header("Slam  Sounds")]
    private bool hasPlayedAttackWindUpSound;
    [EventRef][SerializeField] private string SlamSound;

    [Header("Land Sounds")]
    private bool hasPlayedAttackLandSound;
    [EventRef][SerializeField] private string FallingSound;
    [SerializeField] private float FallingSoundDelay;


    [EventRef][SerializeField] private string FlySound;
    private bool FallSoundPlayed = false;
    FMOD.Studio.EventInstance FlySoundEvent;
    private bool hasPlayedFlySound = false;



    protected override void Start()
    {
        base.Start();

        if (!string.IsNullOrEmpty(FlySound))
        {
            FlySoundEvent = RuntimeManager.CreateInstance(FlySound);
        }
    }

    protected override void OnDestroy()
    {
        cleanUp();
    }
    protected override void OnDisable()
    {
        cleanUp();
    }


    public void PlayRockThrowWindUpSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(RockThrowWindUpSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(RockThrowWindUpSound, transform.position);
    }

    public void PlayRockThrowSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(RockThrowSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(RockThrowSound, transform.position);
    }


    public void PlayMoveSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(MoveSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(MoveSound, transform.position);
    }

    public void PlayMoveAltSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(AltMoveSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(AltMoveSound, transform.position);
    }

    public void PlaySlamSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(SlamSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(SlamSound, transform.position);
    }

    public void PlayFallingSound()
    {
        if (FallSoundPlayed) return;
        FallSoundPlayed = true;
        Invoke("delayFallingSoundPlay", FallingSoundDelay);
    }
    private void delayFallingSoundPlay()
    {
       
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(FallingSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(FallingSound, transform.position);
    }

    public void ResetFallSound()
    {
        print("REEEESEET");
        FallSoundPlayed = false;
    }


    public void StartFlySound()
    {
        if (!string.IsNullOrEmpty(FlySound) && !hasPlayedFlySound)
        {
            FlySoundEvent = RuntimeManager.CreateInstance(FlySound);
            FlySoundEvent.start();

            
            hasPlayedFlySound = true;
            
        } 
    }

    public void EndFlySound()
    {
        FlySoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FlySoundEvent.release();

        hasPlayedFlySound = false;
    }


    void cleanUp()
    {
        FlySoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FlySoundEvent.release();
    }
    public override void onDeathEvent()
    {
        base.onDeathEvent();
        cleanUp();
    }
}
