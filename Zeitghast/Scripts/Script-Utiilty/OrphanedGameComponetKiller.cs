using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrphanedGameComponetKiller : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(transform.parent == null)
        {
            Destroy(gameObject);
        }
    }
}
