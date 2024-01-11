using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    Transform mainCamera;

    [SerializeField] private string MainCameraTag = "MainCamera";
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag(MainCameraTag).transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, transform.position.z);
        transform.position = newPosition;
    }
}
