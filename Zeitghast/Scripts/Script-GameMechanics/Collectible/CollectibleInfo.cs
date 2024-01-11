using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CollectibleInfo
{
    [field: SerializeField]
    public string CollectibleNameId {get; private set;}
    [field: SerializeField]
    public string DisplayName {get; private set;}
    [field: SerializeField]
    public Sprite DisplayIcon {get; private set;}
    [field: TextArea, SerializeField]
    public string Description {get; private set;}
}
