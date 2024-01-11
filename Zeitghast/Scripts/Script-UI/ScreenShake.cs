using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVritualCamera;
    private float shakeDurationTimer;

    private void Awake()
    {
        Instance = this;
        cinemachineVritualCamera = GetComponent<CinemachineVirtualCamera>();

        CinemachineBasicMultiChannelPerlin cinemachineBasicMCP = cinemachineVritualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMCP.m_AmplitudeGain = 0;

    }

    public void ShakeScreen(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMCP = cinemachineVritualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        cinemachineBasicMCP.m_AmplitudeGain = intensity;
        shakeDurationTimer = duration;
    }


    // Update is called once per frame
    void Update()
    {
        if(shakeDurationTimer >= 0)
        {
            shakeDurationTimer -= Time.deltaTime;
        }
        else
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMCP = cinemachineVritualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMCP.m_AmplitudeGain = 0;
        }
    }
}
