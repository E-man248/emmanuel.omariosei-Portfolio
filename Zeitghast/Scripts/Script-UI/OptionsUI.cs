using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class OptionsUI : MonoBehaviour
{
    [SerializeField]private string optionsManagerTag = "OptionsManager";
    private OptionsManager OptionsManager;

    [Header("Inputs")]
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider SFX_Slider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider AimAssistSlider;
    [SerializeField] private Toggle ShowRealTimeTrackerToggle;

    private

    // Start is called before the first frame update
    void Start()
    {
        OptionsManager = GameObject.FindGameObjectWithTag(optionsManagerTag).GetComponent<OptionsManager>();
        errorChecking();

        setUpSlider();
        setUpToggles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setUpSlider()
    {
        MasterSlider.value = OptionsManager.GetMasterVolume();
        SFX_Slider.value = OptionsManager.GetSFXV_Volume();
        MusicSlider.value = OptionsManager.GetMusicVolume();
        AimAssistSlider.value = (int) OptionsManager.GetAimAssistMode();
    }

    private void setUpToggles()
    {
        print("Here A");

        OptionsManager.HasSetUpToggle = false;
        ShowRealTimeTrackerToggle.isOn = OptionsManager.GetShowRealTimeTrackerValue();
        OptionsManager.HasSetUpToggle = true;

        print("Here B");
    }

    //public facing function to adjust volume 
    public void SetMasterVolume(float volume)
    {
        OptionsManager.SetMasterVolume(volume);
    }

    //public facing function to adjust volume 
    public void SetSFX_Volume(float volume)
    {
        OptionsManager.SetSFX_Volume(volume);
    }

    //public facing function to adjust volume 
    public void SetMusicVolume(float volume)
    {
        OptionsManager.SetMusicVolume(volume);
    }

    //public facing function to adjust aim assist mode
    public void SetAimAssistValue(float mode)
    {
        OptionsManager.SetAimAssistMode((AimAssistMode) mode);
    }

    public void ToggleShowRealTimeTrackerValue()
    {
        print("Here T");

        OptionsManager.ToggleShowRealTimeTrackerValue();
    }

    //Check to see if we have everything ready to go 
    private void errorChecking()
    {
        if (OptionsManager == null) Debug.LogError(name + " Could not find audioSettings ");
        if (MasterSlider == null) Debug.LogError(name + " Does not have a MasterSlider");
        if (SFX_Slider == null) Debug.LogError(name + " Does not have a SFX_Slider");
        if (MusicSlider == null) Debug.LogError(name + " Does not have a MusicSlider");
        if (AimAssistSlider == null) Debug.LogWarning(name + " Does not have a AimAssistSlider");
    }
}
