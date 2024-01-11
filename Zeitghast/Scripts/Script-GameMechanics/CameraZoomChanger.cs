using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineBlendDefinition;

public class CameraZoomChanger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected bool useOntrigger2D = true;
    public bool oneUse;

    [Header("Camera Target")]
    [SerializeField] protected Transform cameraTarget;
    [SerializeField] protected Style blendStyle;
    [SerializeField] protected float cameraMoveSpeed;

    [Header("Camera Offset")]
    [SerializeField] protected Vector2 cameraOffset;
    [SerializeField] protected float cameraOffsetSpeed;

    [Header("Camera Zoom")]
    public float CameraZoom;
    public float CameraZoomSpeed;
    
    private bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!useOntrigger2D)
        {
            return;
        }

        if(triggered && oneUse)
        {
            return;
        }

        if (collision == null)
        {
            return;
        }

        if(collision.tag != "Player")
        {
            return;
        }

        changeCameraPosition();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!useOntrigger2D)
        {
            return;
        }

        if (collision == null)
        {
            return;
        }

        if (collision.tag != "Player")
        {
            return;
        }

        triggered = true;
        restoreZoomCameraPosition();
    }


    public void changeCameraPosition()
    {
        if (cameraTarget != null)
        {
            CameraTargetManager.Instance.ChangeTarget(cameraTarget, blendStyle, cameraMoveSpeed);
        }
        
        if (cameraOffset != Vector2.zero)
        {
            CameraTargetManager.Instance.ChangeCameraOffset(cameraOffset, cameraOffsetSpeed);
        }

        CameraTargetManager.Instance.ChangeCameraZoom(CameraZoom, CameraZoomSpeed);
    }

    public void restoreZoomCameraPosition()
    {
        CameraTargetManager.Instance.resetCameraZoom(CameraZoomSpeed);

        CameraTargetManager.Instance.resetCameraOffset(cameraOffsetSpeed);

        CameraTargetManager.Instance.resetTarget(cameraMoveSpeed);
    }
}
