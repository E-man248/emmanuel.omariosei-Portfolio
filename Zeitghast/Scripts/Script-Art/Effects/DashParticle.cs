using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashParticle : MonoBehaviour
{
    ParticleSystem particles;
    public float animationTime;
    private float animationTimer;
    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        getDashButton();
        animationTimer  -= Time.deltaTime;
    }

    void getDashButton()
    {
        if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift))
        {
            activateParticle();
            animationTimer = animationTime;
        }
        else if (animationTimer <= 0)
        {
            deactivateParticle();
        }
    }

    public void activateParticle()
    {
        particles.Play();
    }

    public void deactivateParticle()
    {
        particles.Pause();
        particles.Clear();
    }
}
