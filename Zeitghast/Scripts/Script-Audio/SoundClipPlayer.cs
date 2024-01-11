using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundClipPlayer : MonoBehaviour
{
    [EventRef] public string SoundClip;

    public void playSoundClip()
    {
        RuntimeManager.PlayOneShot(SoundClip, transform.position);
    }
}
