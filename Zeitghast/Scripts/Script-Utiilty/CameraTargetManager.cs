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
    private CinemachineConfiner cinemachineConfiner;
    private CinemachineTransposer cinemachineTransposer;

    [SerializeField]private float defaultCameraZoom = 8.5f;
    public float defaultCameraZoomSpeed;

    [Header("Camera Zoom Settings")]
    [SerializeField] private AnimationCurve curve;

    public CinemachineBrain cinemachineBrain;
    private const string MAIN_CAMERA_TAG = "MainCamera";

    private Coroutine zoomCameraCoroutine;
    private Coroutine offsetCameraCoroutine;

    public Transform defaultTarget {get; private set;}
    public Vector3 defaultCameraOffset {get; private set;}
    private Style defaultBlendStyle;
    private float defaultBlendTime;

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

        //Set up default settings
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        defaultBlendStyle = cinemachineBrain.m_DefaultBlend.m_Style;
        defaultBlendTime = cinemachineBrain.m_DefaultBlend.m_Time;
        defaultCameraOffset = cinemachineTransposer.m_FollowOffset;
    }

    void Start()
    {
        

        GameObject cameraObject = GameObject.FindGameObjectWithTag("CameraBounds");
        if(cameraObject != null)
        {
            cinemachineConfiner.m_BoundingShape2D = cameraObject.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogError(name + " can not find a gameobject tagged CameraBounds");
        }

        defaultTarget = PlayerInfo.Instance.transform;


        resetTarget();
        resetCameraZoom();
        subscribeToEvents();
    }

    private void OnEnable()
    {
        subscribeToEvents();
    }

    private void OnDisable()
    {
        unsubscribeToEvents();
    }

    private void OnDestroy()
    {
        unsubscribeToEvents();
    }

    #region Event Managment
    private void subscribeToEvents()
    {
        Timer.gamePausedEvent += setUpdateMethodToLate;
        Timer.gameUnpausedEvent += setUpdateMethodToFixed;
    }

    private void unsubscribeToEvents()
    {
        Timer.gamePausedEvent -= setUpdateMethodToLate;
        Timer.gameUnpausedEvent -= setUpdateMethodToFixed;
    }
    #endregion


    public void resetTarget()
    {
        resetTarget(defaultBlendTime);
    }

    public void resetTarget(float time)
    {
        cinemachineVirtualCamera.m_Follow = defaultTarget;
        cinemachineBrain.m_DefaultBlend.m_Style = defaultBlendStyle;
        cinemachineBrain.m_DefaultBlend.m_Time = time;
    }

    public void ChangeTarget(Transform target, Style blendStyle, float time)
    {
        cinemachineVirtualCamera.m_Follow = target;
        cinemachineBrain.m_DefaultBlend.m_Style = blendStyle;
        cinemachineBrain.m_DefaultBlend.m_Time = time;
    }

    public void ChangeCameraZoom(float zoomValue, float zoomSpeed)
    {
        if(zoomCameraCoroutine != null)
        {
            StopCoroutine(zoomCameraCoroutine);
        }
        zoomCameraCoroutine = StartCoroutine(zoomCamera(zoomValue,zoomSpeed));
    }

    public void resetCameraZoom(float zoomSpeed)
    {
        if (zoomCameraCoroutine != null)
        {
            StopCoroutine(zoomCameraCoroutine);
        }
        zoomCameraCoroutine = StartCoroutine(zoomCamera(defaultCameraZoom, zoomSpeed));
    }

    public void resetCameraZoom()
    {
        cinemachineVirtualCamera.m_Lens.OrthographicSize = defaultCameraZoom;
    }

    public IEnumerator zoomCamera(float zoomValue, float zoomSpeed )
    {
        float zoomTimer = 0;
        while (zoomTimer < zoomSpeed)
        {
            zoomTimer += Time.deltaTime;

            float lerpedZoom = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, zoomValue, curve.Evaluate(zoomTimer / zoomSpeed));
            cinemachineVirtualCamera.m_Lens.OrthographicSize = lerpedZoom;

            yield return new WaitForFixedUpdate();
        }
    }

    public void ChangeCameraOffset(Vector3 cameraOffset, float offsetSpeed)
    {
        if (offsetCameraCoroutine != null)
        {
            StopCoroutine(offsetCameraCoroutine);
        }
        offsetCameraCoroutine = StartCoroutine(offsetCamera(new Vector3(cameraOffset.x, cameraOffset.y, defaultCameraOffset.z), offsetSpeed));
    }

    public void resetCameraOffset(float offsetSpeed)
    {
        if (offsetCameraCoroutine != null)
        {
            StopCoroutine(offsetCameraCoroutine);
        }
        offsetCameraCoroutine = StartCoroutine(offsetCamera(defaultCameraOffset, offsetSpeed));
    }

    public IEnumerator offsetCamera(Vector3 offsetValue, float offsetSpeed)
    {
        float offsetTimer = 0;
        while (offsetTimer < offsetSpeed)
        {
            offsetTimer += Time.deltaTime;

            Vector3 lerpedOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, offsetValue, curve.Evaluate(offsetTimer / offsetSpeed));
            cinemachineTransposer.m_FollowOffset = lerpedOffset;

            yield return new WaitForFixedUpdate();
        }
    }


    private void setUpdateMethodToFixed()
    {
        cinemachineBrain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
    }

    private void setUpdateMethodToLate()
    {
        cinemachineBrain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.LateUpdate;
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
    }
}
