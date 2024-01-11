using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationHandler : MonoBehaviour
{
    [SerializeField] protected string currentAnimation;

    public abstract void playAnimationOnceFull(string animationName);

    public abstract float getAnimationLength(string animationName);

    public string getCurrentAnimation()
    {
        return currentAnimation;
    }
}
