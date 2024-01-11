using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Princess : CharacterController
{
    public bool caged;

    // Update is called once per frame
    protected override void Update()
    {
        if (caged) return;

        base.Update();
    }


    private void toggleCage(bool value )
    {
        caged = value;
    }
}
