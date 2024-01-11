
using System;

[Serializable]
public class SequentialAnimationGroup : AnimationGroup
{
    /// <summary>
    /// Plays through the entire group of animations sequentially.
    /// </summary>
    public override void playAnimationOnceFull()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns the length of time it would take to play the entire animation group.<br/>
    /// The length of time is the sum of the time it takes to play each animation in the group.
    /// </summary>
    public override float getAnimationLength()
    {
        float fullSequentialAnimationGroupLength = 0f;

        foreach (var animation in animations)
        {
            AnimationHandler animationHandler = animation.animationHandler;
            string animationName = animation.animationName;

            float animationLength = animationHandler.getAnimationLength(animationName);

            if (animationLength > 0)
            {
                fullSequentialAnimationGroupLength += animationLength;
            }
        }

        return fullSequentialAnimationGroupLength;
    }
}
