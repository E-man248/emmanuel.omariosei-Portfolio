using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OnTouchTriggerObject : TriggerObject
{
    private void OnEnable()
    {
        PlayerInfo.PlayerDeathEvent.AddListener(resetTrigger);
    }

    private void OnDisable()
    {
        PlayerInfo.PlayerDeathEvent.RemoveListener(resetTrigger);
    }

    private void OnDestroy()
    {
        PlayerInfo.PlayerDeathEvent.RemoveListener(resetTrigger);
    }

    [SerializeField] protected bool oneTimeEvent;
    protected bool hasPlayedEvent = false;
    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (!hasPlayedEvent && collider.tag == "Player")
        {
            runEvent();
            if (oneTimeEvent) hasPlayedEvent = true;
        }
    }

    protected void resetTrigger()
    {
        //hasPlayedEvent = false;
    }
}
