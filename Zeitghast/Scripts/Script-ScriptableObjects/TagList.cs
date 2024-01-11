using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListObject")]
public class TagList : ScriptableObject
{
    public List<string> list;

    public TagList()
    {
        if (list == null)
        {
            list = new List<string>();
        }
    }
}
