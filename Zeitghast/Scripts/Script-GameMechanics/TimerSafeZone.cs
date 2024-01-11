using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TimerSafeZone : MonoBehaviour
{
    [Header("Sounds")] [EventRef] public string EnterSound = null;
    [Header("Sounds")][EventRef] public string ExitSound = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Timer.Instance.stopTimer();
            Timer.Instance.inTimeSafeZone = true;
            playEnterSound();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Timer.Instance.inTimeSafeZone = false;
            Timer.Instance.continueTimer();
            playExitSound();
        }
    }


    private void playEnterSound()
    {
        if (!string.IsNullOrEmpty(EnterSound))
        {
            RuntimeManager.PlayOneShot(EnterSound, transform.position);
        }
    }

    private void playExitSound()
    {
        if (!string.IsNullOrEmpty(ExitSound))
        {
            RuntimeManager.PlayOneShot(ExitSound, transform.position);
        }
    }

}