using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StunEffectAnimationHandler : MonoBehaviour
{
    [HideInInspector] public string effectName; 
    private Animator effectAnimator;
    public string currentAnimation;
    private int currentAnimationIndex;
    public PlayerOnHitReciever playerOnHitReciever;

    public List<float> animationClickThreshold;

    void Start()
    {
        effectAnimator = GetComponent<Animator>();
        currentAnimationIndex = -1;
    }

    private void Update()
    {
        if (playerOnHitReciever == null || animationClickThreshold.Count == 0)
        {
            return;
        }

        if (currentAnimationIndex < animationClickThreshold.Count-1)
        {
            if (playerOnHitReciever.currentStunMashAmount >= animationClickThreshold[currentAnimationIndex+1])
            {
                currentAnimationIndex++;
                currentAnimation = effectName + currentAnimationIndex;
                effectAnimator.Play(currentAnimation);
            }
        }
    }
    
}
