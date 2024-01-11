using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToken : PickUp
{
    [Space]
    [Header("Time Token")]
    public float value;
    public GameObject particleEffect;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        itemType = ItemType.TimeToken;
    }

    override protected void pickedUp()
    {
        base.pickedUp();
        Instantiate(particleEffect, transform.position, Quaternion.identity);
        Timer.Instance.ChangeTime(value);
    }
}
