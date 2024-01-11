using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EnemySoundHandler : MonoBehaviour
{
    [Header("Damaged Sound")]
    [EventRef][SerializeField] private string damagedSound;

    [Header("Death Sound")]
    [EventRef][SerializeField] private string deathSound;

    [Header("Movement Sounds")]
    [EventRef][SerializeField] private string walkingSound;
    [EventRef][SerializeField] private string jumpingSound;
    [EventRef][SerializeField] private string landingSound;

    [Header("Attack Sounds")]
    [EventRef][SerializeField] private string attackWindUpSound;
    [EventRef][SerializeField] private string attackSound;
    private bool hasPlayedAttackWindUpSound;

    [Header("Ambient Sounds")]
    [EventRef][SerializeField] private string ambientSound;
    [EventRef][SerializeField] private string constantAmbientSound;
    FMOD.Studio.EventInstance constantAmbientSoundEvent;
    public int minRandomAmbientSoundTime;
    public int maxRandomAmbientSoundTime;
    private float currentAmbientSoundTime;
    private float currentAmbientSoundTimer;

    protected enemyAnimationHandler enemyAnimationHandler;

    protected Health health;

    private float abientSoundPositionUpdateTimer;
    private float abientSoundPositionUpdateRate = 0.3f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = GetComponentInParent<Health>();

        if(health == null)
        {
            Debug.LogError(name + " Can't find a health script");
        }
        
        //Getting a grounded Enemy AnimationHandler
        enemyAnimationHandler = GetComponent<enemyAnimationHandler>();

        //Checking if the grounded Enemy AnimationHandler exists
        if (enemyAnimationHandler == null)
        {
            Debug.LogWarning(name + " Can't find a EnemyAnimationHandler");
        }

        //Picking a random time for the ambient sound to start playing
        setAmbientSoundTime();

        attachEvents();

        //Playing constant Ambient Sound
        if(!string.IsNullOrEmpty(constantAmbientSound)) constantAmbientSoundEvent = RuntimeManager.CreateInstance(constantAmbientSound); 
        
        constantAmbientSoundEvent.start();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        checkAndPlayAmbientSoundSound();

        checkToRestAttackWindUpSoundBool();

        updateConstantAbmientSound();
    }
    protected virtual void OnEnable()
    {
        attachEvents();
    }
    protected virtual void OnDisable()
    {
        dettachEvents();
    }
    protected virtual void OnDestroy()
    {
        dettachEvents();
        constantAmbientSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        constantAmbientSoundEvent.release();
    }
    private void attachEvents()
    {
        if (health == null) return;
        health.onDamageTaken.AddListener(PlayDamagedSound);
        health.onDeathEvent.AddListener(onDeathEvent);
    }

    private void dettachEvents()
    {
        if (health == null) return;
        health.onDamageTaken.RemoveListener(PlayDamagedSound);
        health.onDeathEvent.RemoveListener(onDeathEvent);
    }


    //Picks a random time for the ambient sound to start playing
    private void setAmbientSoundTime()
    {
        currentAmbientSoundTime = Random.Range(minRandomAmbientSoundTime, maxRandomAmbientSoundTime);
        currentAmbientSoundTimer = currentAmbientSoundTime;
    }
    private void updateConstantAbmientSound()
    {
        if (string.IsNullOrEmpty(constantAmbientSound)) return;

        abientSoundPositionUpdateTimer += Time.deltaTime;

        //Pause check
        constantAmbientSoundEvent.setPaused(Timer.gamePaused);

        if (abientSoundPositionUpdateTimer >= abientSoundPositionUpdateRate)
        {
            // You can set the position based on your game object's position
            FMOD.ATTRIBUTES_3D attributes = RuntimeUtils.To3DAttributes(transform.position);
            constantAmbientSoundEvent.set3DAttributes(attributes);

            abientSoundPositionUpdateTimer = 0;
        }
    }

    //Checks the current timer to see if we should play the ambient sound. then pick a new random time
    public void checkAndPlayAmbientSoundSound()
    {

        if (string.IsNullOrEmpty(ambientSound)) return;

        if(enemyAnimationHandler == null) return;

        //Tick down the timer
        currentAmbientSoundTimer -= Time.deltaTime;

        //return if the time is not up
        if (currentAmbientSoundTimer > 0f)
        {
            return;
        }
        //Play Sound
        RuntimeManager.PlayOneShot(ambientSound, transform.position);    

        //Calculate a new random Time
        setAmbientSoundTime();
    }


    #region Sound functions called by an animation event
    public void PlayWalkSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(walkingSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(walkingSound, transform.position);
    }

    public void PlayJumpingSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(jumpingSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(jumpingSound, transform.position);
    }

    public void PlayLandingSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(landingSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(landingSound, transform.position);
    }

    //Called by an animation event for an animation
    public void PlayAttackWindUpSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(attackWindUpSound))
        {
            return;
        }

        //if the windup sound has been played once return
        if (hasPlayedAttackWindUpSound)
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(attackWindUpSound, transform.position);

        hasPlayedAttackWindUpSound = true;
    }

    public void PlayAttackSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(attackSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(attackSound, transform.position);

    }
    #endregion

    #region Sounds called  by other events
    public void PlayDamagedSound()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(damagedSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(damagedSound, transform.position);
    }

    public virtual void onDeathEvent()
    {
        //if the event ref string is not valid(null or empty) return
        if (string.IsNullOrEmpty(deathSound))
        {
            return;
        }

        //Play Sound
        RuntimeManager.PlayOneShot(deathSound, transform.position);
    }
    #endregion

    #region Sound functions to restore boolean
    public void checkToRestAttackWindUpSoundBool()
    {
        if (enemyAnimationHandler == null) return;
        //if the grounded enemy is still winding up return
        if (enemyAnimationHandler.isWindingUpAttack())
        {
            return;
        }

        hasPlayedAttackWindUpSound = false;
    }
    #endregion
}
