using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostPlaybackState
{
    Recording,
    Replay
}

[System.Serializable]
public struct GhostState
{
    public Vector3 position;
    public Sprite sprite;
    public bool spriteFlip;
    public float timeStamp;
}

[CreateAssetMenu]
public class Ghost : ScriptableObject
{
    public GhostPlaybackState ghostPlaybackState;
    [Space]
    public float recordingFrequancy;
    [Space]
    public List<GhostState> ghostRecording;
    
    //Clears the list 
    public void ResetData()
    {
        ghostRecording.Clear();
    }
}
