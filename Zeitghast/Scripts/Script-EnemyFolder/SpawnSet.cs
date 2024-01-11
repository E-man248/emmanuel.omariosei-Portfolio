using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSet : MonoBehaviour
{
    [field:SerializeField] public List<GameObject> Enemies { get; private set;}
}
