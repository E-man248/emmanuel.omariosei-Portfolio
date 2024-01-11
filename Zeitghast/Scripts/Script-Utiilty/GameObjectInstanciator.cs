using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectInstanciator : MonoBehaviour
{
    [SerializeField] private bool instantiateOnStart = false;
    [SerializeField] private GameObject targetGameObject;
    [SerializeField] private Vector2 gameObjectOffset;
    
    void Start()
    {
        if (instantiateOnStart) instantiateObject();
    }

    public void instantiateObject()
    {
        if (gameObject != null)
        {
            Transform targetGameObjectParent = transform;

            Vector3 targetGameObjectLocation = new Vector3(gameObjectOffset.x, gameObjectOffset.y, 0f);
            GameObject currentTargetGameObject = Instantiate(targetGameObject, targetGameObjectParent);

            currentTargetGameObject.transform.localPosition = targetGameObjectLocation; 
            currentTargetGameObject.transform.parent = null;
        }
    }
}
