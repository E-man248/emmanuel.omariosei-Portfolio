using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionsData
{
    public float MasterVolumeValue = 1f;
    public float MusicVolumeValue = 1f;
    public float SFXVolumeValue = 1f;
    public float aimAssistStrengthValue = OptionsManager.DEFAULT_AIM_ASSIST_VALUE;
    public bool showRealTimeTrackerInPlay = false;

    public void setMasterVolumeValue (float newMasterVolumeValue)
    {
        MasterVolumeValue = newMasterVolumeValue;
    }

    public void setMusicVolumeValue(float newMusicVolumeValue)
    {
        MusicVolumeValue = newMusicVolumeValue;
    }

    public void setSFXVolumeValue(float newSFXVolumeValue)
    {
        SFXVolumeValue = newSFXVolumeValue;
    }

    public OptionsData clone()
    {
        OptionsData clone = new OptionsData();
        clone.setMasterVolumeValue(MasterVolumeValue);
        clone.setMusicVolumeValue(MusicVolumeValue);
        clone.setSFXVolumeValue (SFXVolumeValue);
        clone.aimAssistStrengthValue = aimAssistStrengthValue;
        clone.showRealTimeTrackerInPlay = showRealTimeTrackerInPlay;

        return clone;
    }

    public override string ToString()
    {
        return "SFX = " + SFXVolumeValue + "\nMusic = " + MusicVolumeValue + "\nMaster = " + MasterVolumeValue
               + "\nAim Assist Strength = " + aimAssistStrengthValue + "\nShow Real Time Tracker In Play = " + showRealTimeTrackerInPlay;
    }
}
