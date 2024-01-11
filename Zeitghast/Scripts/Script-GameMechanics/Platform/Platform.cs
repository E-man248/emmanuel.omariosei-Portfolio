using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformEffector2D platformEffector;
    public LayerMask Playerlayer;
    private BoxCollider2D playerEnterCollider;

    void Start()
    {
        playerEnterCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        checkForDownPress();
    }

    void checkForDownPress()
    {
        if(Input.GetButton("Crouch") && playerEnterCollider.IsTouchingLayers(Playerlayer))
        {
            platformEffector.rotationalOffset = 180;
        }
        else
        {
            platformEffector.rotationalOffset = 0;
        }
    }
}
