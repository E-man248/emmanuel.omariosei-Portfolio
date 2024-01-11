using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeSafeZoneUI : UI_SpriteFadeTween
{
    [Header("ChromaticAberration Settings")]
    [SerializeField] private float chromaticAberationIntensity = 0.45f;
    [SerializeField] private float transitionInSpeed = 1f;
    [SerializeField] private float transitionOutSpeed = 0.5f;

    private bool faddedIn;
    private bool faddedOut;

    protected override void Awake()
    {
        base.Awake();
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(Timer.Instance.inTimeSafeZone)
        {
            if(faddedIn) return;

            //Activate Chromatic Aberation
            PostProcessingManager.Instance.setChromaticAberationffect(chromaticAberationIntensity, transitionInSpeed);

            faddedOut = false;
            fadeInUIGroup();
        }
        else
        {
            if (faddedOut) return;

            //Deactivate Chromatic Aberation
            PostProcessingManager.Instance.setChromaticAberationffect(0f, transitionOutSpeed);
            faddedIn = false;
            fadeOutUIGroup();  
        }
    }

    protected override void fadeInUIGroupStart()
    {
        faddedIn = true;
    }

    protected override void fadeOutUIGroupStart()
    {
        faddedOut = true;
    }

}
