using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
    private string cameraTag = "MainCamera";
    private Transform cameraTransform;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag(cameraTag).transform;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    protected virtual void Update()
    {
        if (isInRenderDistance())
        {
            setActive();
        }
        else
        {
            setInactive();
        }
    }

    protected virtual void setInactive()
    {
        
    }

    protected virtual void setActive()
    {
        
    }

    protected bool isInRenderDistance()
    {
        float distance = Vector3.Distance(transform.position, cameraTransform.position);
        return distance < PlayerInfo.Instance.enemy_AI_Update_Distance;
    } 


    
}
