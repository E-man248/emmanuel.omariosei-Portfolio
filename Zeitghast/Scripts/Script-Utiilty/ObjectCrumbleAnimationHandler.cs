using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ObjectCrumbleAnimationHandler : MonoBehaviour
{
    public string objectName; 
    private Animator objectAnimator;
    public string currentAnimation;
    private int currentAnimationIndex;
    public Health objectHealth;

    public List<float> nextAnimationThresholds;

    void Start()
    {
        objectAnimator = GetComponentInChildren<Animator>();
        currentAnimationIndex = -1;
    }

    protected void OnEnable()
    {
        if (objectAnimator != null) objectAnimator.Play(currentAnimation);
    }

    private void Update()
    {
        animate();
    }

    public void animate()
    {
        if (objectHealth == null || nextAnimationThresholds.Count == 0)
        {
            return;
        }

        if (currentAnimationIndex < nextAnimationThresholds.Count-1)
        {
            if (objectHealth.health <= nextAnimationThresholds[currentAnimationIndex+1])
            {
                currentAnimationIndex++;
                currentAnimation = objectName + "Crumble" + currentAnimationIndex;
                objectAnimator.Play(currentAnimation);
            }
        }
    }  
}
