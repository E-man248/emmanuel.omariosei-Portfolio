using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class A_StarUpdater : MonoBehaviour
{
    [SerializeField]private float scanRate = 0.2f;
    private float scanTimer;
    private AstarPath astarPath;
    // Start is called before the first frame update
    void Start()
    {
        astarPath = GetComponent<AstarPath>();
        if (astarPath == null) Debug.LogError(name + " Could not find a path finder");

        scanTimer = scanRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (scanRate <= 0) return;
        if(astarPath == null) return;

        scanTimer -= Time.deltaTime; 
        if (scanTimer <= 0)
        {
            scanTimer = scanRate;
            AstarPath.active.Scan();
        }
    }
}
