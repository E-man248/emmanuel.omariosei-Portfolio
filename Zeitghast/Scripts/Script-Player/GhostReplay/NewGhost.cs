using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NewGhostPlaybackState
{
    Recording,
    Replay
}

[System.Serializable]
public struct NewGhostState
{
    public Vector3 position;
    public Vector3 graphicsRotation;
    public string animation;
    public float timeStamp;
}

[CreateAssetMenu]
public class NewGhost : ScriptableObject
{
    public NewGhostPlaybackState ghostPlaybackState;
    [Space]
    public float recordingFrequency;
    [Space]
    public List<NewGhostState> ghostRecording;
    
    //Clears the list 
    public void ResetData()
    {
        ghostRecording.Clear();
    }
}
