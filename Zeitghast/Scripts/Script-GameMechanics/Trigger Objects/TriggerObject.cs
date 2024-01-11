using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] protected UnityEvent triggerEvent;
    /**
        <summary>
        Triggers the triggerEvent.
        Intended to be used if associated with another script that would call this function.
        </summary>
    **/
    public virtual void runEvent()
    {
        triggerEvent.Invoke();
    }
}
