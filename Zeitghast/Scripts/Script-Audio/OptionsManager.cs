using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager: MonoBehaviour
{
    
    [Header("Aim Assist Options Settings")]
    [SerializeField] private float aimAssistLowValue = 2f;
    [SerializeField] private float aimAssistHighValue = 3f;
    public static float DEFAULT_AIM_ASSIST_VALUE { get; private set; } = 3f;
    internal bool HasSetUpToggle = true;

    private FMOD.Studio.VCA MasterVCA;
    private FMOD.Studio.VCA SFX_VCA;
    private FMOD.Studio.VCA MusicVCA;
    private OptionsData optionsData;

    // Start is called before the first frame update
    void Awake()
    {
        MasterVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        SFX_VCA = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");
        MusicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
    }
    private void Start()
    {
        HasSetUpToggle = true;

        if (optionsData == null)
        {
            optionsData = DataPersistanceManager.Instance.getGameData().options_Data;
        }

        SetMasterVolume(optionsData.MasterVolumeValue);
        SetMusicVolume(optionsData.MusicVolumeValue);
        SetSFX_Volume(optionsData.SFXVolumeValue);
        SetAimAssistValue(optionsData.aimAssistStrengthValue);
    }


    public void SetMasterVolume(float volume)
    {
        // Ensure the volume stays within the valid range (0 to 1)
        volume = Mathf.Clamp01(volume);

        // Set the master VCA volume
        MasterVCA.setVolume(volume);

        // Get Up to Date Game Data:
        optionsData = DataPersistanceManager.Instance.GetOptionsData();

        // Update Game Data:
        optionsData.setMasterVolumeValue(volume);
        DataPersistanceManager.Instance.UpdateOptionsData(optionsData);
        DataPersistanceManager.Instance.SaveGameData();
    }

    public float GetMasterVolume()
    {
        float currentVolume;
        MasterVCA.getVolume(out currentVolume);
        return currentVolume;
    }

    public void SetSFX_Volume(float volume)
    {
        // Ensure the volume stays within the valid range (0 to 1)
        volume = Mathf.Clamp01(volume);

        // Set the master VCA volume
        SFX_VCA.setVolume(volume);

        // Get Up to Date Game Data:
        optionsData = DataPersistanceManager.Instance.GetOptionsData();

        // Update Game Data:
        optionsData.setSFXVolumeValue(volume);
        DataPersistanceManager.Instance.UpdateOptionsData(optionsData);
        DataPersistanceManager.Instance.SaveGameData();
    }
    public float GetSFXV_Volume()
    {
        float currentVolume;
        SFX_VCA.getVolume(out currentVolume);
        return currentVolume;
    }

    public void SetMusicVolume(float volume)
    {
        // Ensure the volume stays within the valid range (0 to 1)
        volume = Mathf.Clamp01(volume);

        // Set the master VCA volume
        MusicVCA.setVolume(volume);

        // Get Up to Date Game Data:
        optionsData = DataPersistanceManager.Instance.GetOptionsData();

        // Update Game Data:
        optionsData.setMusicVolumeValue(volume);
        DataPersistanceManager.Instance.UpdateOptionsData(optionsData);
        DataPersistanceManager.Instance.SaveGameData();
    }
    public float GetMusicVolume()
    {
        float currentVolume;
        MusicVCA.getVolume(out currentVolume);
        return currentVolume;
    }


    public void SetAimAssistMode(AimAssistMode mode)
    {
        // Get Aim Assist Mode Value:
        float aimAssistStrengthValue = GetAimAssistModeValue(mode);

        // Set Aim Assist Value:
        SetAimAssistValue(aimAssistStrengthValue);
    }

    public void SetAimAssistValue(float aimAssistStrengthValue)
    {
        // Get Up to Date Game Data:
        optionsData = DataPersistanceManager.Instance.GetOptionsData();

        // Update Game Data:
        optionsData.aimAssistStrengthValue = aimAssistStrengthValue;

        DataPersistanceManager.Instance.UpdateOptionsData(optionsData);
        DataPersistanceManager.Instance.SaveGameData();
    }

    public void ToggleShowRealTimeTrackerValue()
    {
        print("function call");
        if (!HasSetUpToggle)
        {
            print("Return!");
            return;
        }

        // Get Up to Date Game Data:
        optionsData = DataPersistanceManager.Instance.GetOptionsData();

        bool newValue = !optionsData.showRealTimeTrackerInPlay;

        RealTimeTracker.Instance?.setShowInPlay(newValue);

        // Update Game Save Options Data:

        optionsData.showRealTimeTrackerInPlay = newValue;

        DataPersistanceManager.Instance.UpdateOptionsData(optionsData);

        Debug.Log("About to Save!");
        DataPersistanceManager.Instance.SaveGameData();
    }

    private float GetAimAssistModeValue(AimAssistMode mode)
    {
        switch (mode)
        {
            case AimAssistMode.Off:
                return 0;
                
            case AimAssistMode.Low:
                return aimAssistLowValue;

            case AimAssistMode.High:
                return aimAssistHighValue;
        }

        throw new ArgumentException("Invalid Aim Assist Mode Entered!");
    }

    public AimAssistMode GetAimAssistMode()
    {
        float aimAssistValue = GetAimAssistValue();

        if (aimAssistValue <= 0f)
        {
            return AimAssistMode.Off;
        }
        else if (aimAssistValue > 0f && aimAssistValue <= aimAssistLowValue)
        {
            return AimAssistMode.Low;
        }
        else
        {
            return AimAssistMode.High;
        }
    }

    public float GetAimAssistValue()
    {
        // Get Up to Date Game Data:
        optionsData = DataPersistanceManager.Instance.GetOptionsData();

        return optionsData.aimAssistStrengthValue;
    }

    public bool GetShowRealTimeTrackerValue()
    {
        // Get Up to Date Game Data:
        optionsData = DataPersistanceManager.Instance.GetOptionsData();

        print("Options Show Timer in Play: " + optionsData.showRealTimeTrackerInPlay);

        return optionsData.showRealTimeTrackerInPlay;
    }
}

public enum AimAssistMode
{
    Off = 0,
    Low = 1,
    High = 2,
}
