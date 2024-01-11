using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrailController : MonoBehaviour
{
    [HideInInspector]
    private ParticleSystem particle;
    private TrailRenderer trailRenderer;
    [HideInInspector]
    public bool selfDestruct = false;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(selfDestruct)
        {
            // When trail is done, indicate this by making it null
            if (trailRenderer != null)
            {
                Invoke("trailIsDone", trailRenderer.time);
            }

            // When particles are done, indicate this by making it null
            if (particle != null)
            {
                particle.Stop();
                if(!particle.IsAlive())
                {
                    particle = null;
                }
            }

            // If selfDestruct is true, when both trail and particles are done, destroy gameObject
            if (trailRenderer == null && particle == null)
            {
                Destroy(gameObject);
            }
        }
    }

    // When trails are done, indicate this by making them null
    void trailIsDone()
    {
        trailRenderer = null;
    }
}
