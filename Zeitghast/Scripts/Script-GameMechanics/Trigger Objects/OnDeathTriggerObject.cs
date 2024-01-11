using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathTriggerObject : TriggerObject
{
    protected Health health;
    protected bool hasPlayedEvent = false;

    protected void Start()
    {
        health = GetComponent<Health>();

        if (health != null)
        {
            health.onDeathEvent.AddListener(OnDeath);
        }
        else
        {
            Debug.LogError("[OnDeathTriggerObject] No 'Health' Script for " + name);
            this.enabled = false;
        }
    }

    /**
        <summary>
        Calls runEvent() which triggers the triggerEvent.
        </summary>
    **/
    protected virtual void OnDeath()
    {
        if (!hasPlayedEvent)
        {
            runEvent();
            hasPlayedEvent = true;
        }
    }

    protected void OnEnable()
    {
        if (health != null)
        {
            health.onDeathEvent.AddListener(OnDeath);
        }
    }

    protected void OnDisable()
    {
        if (health != null)
        {
            health.onDeathEvent.RemoveListener(OnDeath);
        }
    }

    protected void OnDestroy()
    {
        if (health != null)
        {
            health.onDeathEvent.RemoveListener(OnDeath);
        }
    }
}
