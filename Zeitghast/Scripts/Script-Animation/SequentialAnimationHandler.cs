using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SequentialAnimationHandler : AnimationHandler
{
    [SerializeField] private bool LiveTestingEnabled = false;
    
    [Space]
    [SerializeField] private List<SequentialAnimationGroup> sequentialAnimationGroups;
    private Dictionary<string, float> allSequentialAnimationGroupLengths;

    private void Start()
    {
        if (allSequentialAnimationGroupLengths == null)
        {
            storeSequentialAnimationGroupLengths();
        }
    }

    public override void playAnimationOnceFull(string animationGroupName)
    {
        // Select the Animation Group given the animation group name:
        AnimationGroup animationGroup = sequentialAnimationGroups.Find(x => x.AnimationGroupName.Equals(animationGroupName));

        if (animationGroup == null) return;

        float animationStartTimeDelay = 0f;

        currentAnimation = animationGroupName;

        foreach (var animation in animationGroup.animations)
        {
            StartCoroutine(playDelayedAnimation(animationStartTimeDelay, animation));

            float animationLength = animation.animationHandler.getAnimationLength(animation.animationName);

            if (animationLength > 0f)
            {
                animationStartTimeDelay += animationLength;
            }
        }
    }

    private IEnumerator playDelayedAnimation(float startTimeDelay, AnimationPair animation)
    {
        yield return new WaitForSecondsRealtime(startTimeDelay);

        animation.animationHandler.playAnimationOnceFull(animation.animationName);
    }

    /// <summary>
    /// Retrieves the run length (in seconds) of the sequential animation group based on the animation group name provided.
    /// </summary>
    public override float getAnimationLength(string animationGroupName)
    {
        // When in Live Testing, we retrieve animation group lengths dynamically to support "in-play" changes to animation groups.
        // (Should Be Disabled For Static Play)
        if (LiveTestingEnabled)
        {
            AnimationGroup animationGroup = sequentialAnimationGroups.Find(x => x.AnimationGroupName.Equals(animationGroupName));

            float animationLength = animationGroup.getAnimationLength();

            return animationLength;
        }
        else
        {
            if (allSequentialAnimationGroupLengths == null) storeSequentialAnimationGroupLengths();

            if (allSequentialAnimationGroupLengths.Count == 0) return -1;

            if (!String.IsNullOrEmpty(animationGroupName) && !allSequentialAnimationGroupLengths.ContainsKey(animationGroupName))
            {
                Debug.LogWarning("[EntityAnimationHandler] Animation \"" + animationGroupName + "\" does not exist in " + name + " Sequential Animation Handler" + "!");
                return -1f;
            }

            return allSequentialAnimationGroupLengths[animationGroupName];
        }
    }
    
    /// <summary>
    /// Stores the lengths of all the sequential animation groups to improve animation length retrieval time.
    /// </summary>
    private void storeSequentialAnimationGroupLengths()
    {
        allSequentialAnimationGroupLengths = new Dictionary<string, float>();

        foreach(var animationGroup in sequentialAnimationGroups)
        {
            allSequentialAnimationGroupLengths.Add(animationGroup.AnimationGroupName, animationGroup.getAnimationLength());
        }
    }
}
