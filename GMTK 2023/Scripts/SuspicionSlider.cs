using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuspicionSlider : MonoBehaviour
{
    [SerializeField] private Scrollbar suspiciousScrollbar;
    public void Start()
    {
        SuspicionMeter.instance.suspicionMeterChangeEvent.AddListener(setSliderValue);
    }

    private void Update()
    {
        
    }

    private void OnDisable()
    {
        SuspicionMeter.instance.suspicionMeterChangeEvent.RemoveListener(setSliderValue);
    }

    private void OnDestroy()
    {
        SuspicionMeter.instance.suspicionMeterChangeEvent.RemoveListener(setSliderValue);
    }

    IEnumerator checkforSuspicionMeter()
    {
        //Debug.LogWarning("waiting");
        yield return new WaitUntil(() => SuspicionMeter.instance != null);
        //Debug.LogWarning("finished waiting");
        SuspicionMeter.instance.suspicionMeterChangeEvent.AddListener(setSliderValue);
    }

    public void setSliderValue(int x)
    {
        
        suspiciousScrollbar.size = ((float) x / SuspicionMeter.instance.suspicionCap);

        if (suspiciousScrollbar.size == 1)
        {
            GameStateManger.Instance.GameOver.Invoke();
        }

        //Debug.Log(suspiciousScrollbar.size);
    }
}
