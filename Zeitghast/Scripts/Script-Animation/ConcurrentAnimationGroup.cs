
using System;

[Serializable]
public class ConcurrentAnimationGroup : AnimationGroup
{
    /// <summary>
    /// Plays the entire group of animations concurrently.
    /// </summary>
    public override void playAnimationOnceFull()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns the length of time it would take to play the entire animation group.<br/>
    /// The length of time depends on the amount of time the longest animation takes to finish.
    /// </summary>
    public override float getAnimationLength()
    {
        float greatestAnimationPairLength = 0f;

        foreach (var animation in animations)
        {
            AnimationHandler animationHandler = animation.animationHandler;
            string animationName = animation.animationName;

            float animationLength = animationHandler.getAnimationLength(animationName);

            if (animationLength > greatestAnimationPairLength)
            {
                greatestAnimationPairLength = animationLength;
            }
        }

        return greatestAnimationPairLength;
    }
}
