using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] protected bool initateOnStart = false;
    public float lifeTime;

    public void Start()
    {
        if (initateOnStart) InitiateDestruct();
    }

    public void InitiateDestruct()
    {
        Invoke("Destruct", lifeTime);
    }

    public virtual void Destruct()
    {
        Destroy(gameObject);
    }
}
