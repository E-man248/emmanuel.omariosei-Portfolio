using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HatInfo")]
public class HatInfo : ScriptableObject
{
    public string hatId;
    public GameObject HatObject;

    [Header("Display Info")]
    public string hatDisplayName;
    public string hatDescription;
}
