using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WeaponSoundHandler : MonoBehaviour
{
    [EventRef] public string reloadSound = null;

    public void playReloadSound()
    {
        if (!string.IsNullOrEmpty(reloadSound))
        {
            RuntimeManager.PlayOneShot(reloadSound, transform.position);
        }
    }
}
