using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndGate : Gate
{
    [SerializeField]private List<PressurePlate> pressurePlates;
    // Update is called once per frame
    protected override void Start()
    {

    }

    protected override void Update()
    {
        if (allplatesActive())
        {
            openGate();
        }
        else
        {
            closegate();
        }
    }

    private bool allplatesActive()
    {
        foreach (var plate in pressurePlates)
        {
            if (!plate.plateState) return false;
        }

        return true;
    }
}
