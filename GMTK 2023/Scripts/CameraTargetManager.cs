using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static Cinemachine.CinemachineBlendDefinition;

public class CameraTargetManager : MonoBehaviour
{
    //Singleton 
    public static CameraTargetManager Instance = null;

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineConfiner2D cinemachineConfiner;

    public CinemachineBrain cinemachineBrain;
    private const string MAIN_CAMERA_TAG = "MainCamera";

    public Transform defaultCameraTarget { get; private set; }


    private void Awake()
    {
        //Singelton Checking
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //Get Cinemachine Brain
        GameObject mainCameraObject = GameObject.FindGameObjectWithTag(MAIN_CAMERA_TAG);

        if (mainCameraObject == null)
        {
            Debug.LogError(name + " can't find an  object taged with " + MAIN_CAMERA_TAG);
        }
        else
        {
            cinemachineBrain = mainCameraObject.GetComponent<CinemachineBrain>();
        }

        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineConfiner = GetComponent<CinemachineConfiner2D>();
    }

    void Start()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("CameraBounds");
        if (cameraObject != null)
        {
            cinemachineConfiner.m_BoundingShape2D = cameraObject.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogError(name + " can not find a gameobject tagged CameraBounds");
        }
    }


    public void setCameraTarget()
    {
        setCameraTarget(defaultCameraTarget);
    }


    public void setCameraTarget(Transform newCameraTarget)
    {
        cinemachineVirtualCamera.m_Follow = newCameraTarget;
        cinemachineVirtualCamera.m_LookAt = newCameraTarget;
    }
}
