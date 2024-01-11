using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    //Volume
    private Volume volume;
    private VolumeProfile volumeProfile;
    public static PostProcessingManager Instance = null;

    //Depth Of Field
    private DepthOfField depthOfField;
    private float CurrenDOFCoroutineTransitionTime;
    private Coroutine DepthOfFieldCoroutine;

    private ChromaticAberration chromaticAberation;
    private float CurrentAberationCoroutineTransitionTime;
    private Coroutine ChromaticAberationCoroutine;

    private void Awake()
    {
        //Singelton Checking
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        //Getting the volume object and it's effects list
        volume = GetComponent<Volume>();
        if (volume == null)
        {
            Debug.LogError("There is no volume in " + name);
            return;
        }

        volumeProfile = volume.sharedProfile;


        setupDOF();
        setupChromaticAberation();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void setupDOF()
    {
        //Setting depth of field
        if (!volumeProfile.TryGet<DepthOfField>(out var depthOfFieldComponent))
        {
            //if there is no Depth of field component 
            Debug.LogError("There is no Depth of field Override in " + name);
        }
        else
        {
            depthOfField = depthOfFieldComponent;
        }

        //Default
        depthOfField.focalLength.value = 1f;
    }


    private void setupChromaticAberation()
    {
        //Setting depth of field
        if (!volumeProfile.TryGet<ChromaticAberration>(out var chromaticAberationComponent))
        {
            //if there is no Depth of field component 
            Debug.LogError("There is no Depth of field Override in " + name);
        }
        else
        {
            chromaticAberation = chromaticAberationComponent;
        }

        //Default
        chromaticAberation.intensity.value = 0f;
    }

    private void setDefault()
    {
        depthOfField.focalLength.value = 1f;
        chromaticAberation.intensity.value = 0f;
    }



    //Activates the depth of field effect given an amount and speed 
    public void setDepthOfFieldEffect(float DepthOfFieldValue, float transitionTime)
    {
        //Clamping values
        if(DepthOfFieldValue > 300f)
        {
            DepthOfFieldValue = 300f;
        }
        else if (DepthOfFieldValue < 1f)
        {
            DepthOfFieldValue = 1f;
        }
        
        //Reseting the timer for the animation
        CurrenDOFCoroutineTransitionTime = 0f;

        //Starting the animation
        if(DepthOfFieldCoroutine != null)
        {
            StopCoroutine(DepthOfFieldCoroutine);
        }
        DepthOfFieldCoroutine = StartCoroutine(depthOfFieldAnimation(DepthOfFieldValue, transitionTime));
    }

    IEnumerator depthOfFieldAnimation(float targetDepthOfFieldValue, float transitionTime)
    {
        while(CurrenDOFCoroutineTransitionTime / transitionTime < 1f)
        {
            CurrenDOFCoroutineTransitionTime += Time.unscaledDeltaTime;

            float completionRate = CurrenDOFCoroutineTransitionTime / transitionTime;
            depthOfField.focalLength.value = Mathf.Lerp(depthOfField.focalLength.value, targetDepthOfFieldValue, completionRate);
            yield return null;
        }
    }



    //Activates the ChromaticAberation effect given an amount and speed 
    public void setChromaticAberationffect(float aberationValue, float transitionTime)
    {
        //Clamping values
        if (aberationValue > 1f)
        {
            aberationValue = 1f;
        }
        else if (aberationValue < 0f)
        {
            aberationValue = 0f;
        }

        //Reseting the timer for the animation
        CurrentAberationCoroutineTransitionTime = 0f;

        //Starting the animation
        if (ChromaticAberationCoroutine != null)
        {
            StopCoroutine(ChromaticAberationCoroutine);
        }
        ChromaticAberationCoroutine = StartCoroutine(ChromaticAberationAnimation(aberationValue, transitionTime));
    }

    IEnumerator ChromaticAberationAnimation(float targetAberationValue, float transitionTime)
    {
        while (CurrentAberationCoroutineTransitionTime / transitionTime < 1f)
        {
            CurrentAberationCoroutineTransitionTime += Time.unscaledDeltaTime;

            float completionRate = CurrentAberationCoroutineTransitionTime / transitionTime;
            chromaticAberation.intensity.value = Mathf.Lerp(chromaticAberation.intensity.value, targetAberationValue, completionRate);
            yield return null;
        }
    }

    private void OnApplicationQuit()
    {
        setDefault();   
    }

}
