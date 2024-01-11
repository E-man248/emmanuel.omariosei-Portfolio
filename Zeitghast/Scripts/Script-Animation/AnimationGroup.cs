using System;
using System.Collections.Generic;
public abstract class AnimationGroup
{
    public string AnimationGroupName;
    public List<AnimationPair> animations;
    
    /// <summary>
    /// Plays the entire group of animations.
    /// </summary>
    public abstract void playAnimationOnceFull();

    /// <summary>
    /// Returns the length of time it would take to play the entire animation group.
    /// </summary>
    public abstract float getAnimationLength();
}

[Serializable]
public struct AnimationPair
{
    public AnimationHandler animationHandler;
    public string animationName;
}