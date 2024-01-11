using System;
using System.Collections.Generic;
using UnityEngine;

public class ConcurrentAnimationHandler : AnimationHandler
{
    [SerializeField] private bool LiveTestingEnabled = false;
    
    [Space]
    [SerializeField] private List<ConcurrentAnimationGroup> concurrentAnimationGroups;
    private Dictionary<string, float> allConcurrentAnimationGroupLengths;

    private void Start()
    {
        if (allConcurrentAnimationGroupLengths == null)
        {
            storeConcurrentAnimationGroupLengths();
        }
    }

    public override void playAnimationOnceFull(string animationGroupName)
    {
        AnimationGroup animationGroup = concurrentAnimationGroups.Find(x => x.AnimationGroupName.Equals(animationGroupName));

        if (animationGroup == null) return;

        currentAnimation = animationGroupName;

        foreach (var animation in animationGroup.animations)
        {
            animation.animationHandler.playAnimationOnceFull(animation.animationName);
        }
    }

    /// <summary>
    /// Retrieves the run length (in seconds) of the concurrent animation group based on the animation group name provided.
    /// </summary>
    public override float getAnimationLength(string animationGroupName)
    {
        // When in Live Testing, we retrieve animation group lengths dynamically to support "in-play" changes to animation groups.
        // (Should Be Disabled For Static Play)
        if (LiveTestingEnabled)
        {
            AnimationGroup animationGroup = concurrentAnimationGroups.Find(x => x.AnimationGroupName.Equals(animationGroupName));

            float animationLength = animationGroup.getAnimationLength();

            return animationLength;
        }
        else
        {
            if (allConcurrentAnimationGroupLengths == null) storeConcurrentAnimationGroupLengths();

            if (allConcurrentAnimationGroupLengths.Count == 0) return -1;

            if (!String.IsNullOrEmpty(animationGroupName) && !allConcurrentAnimationGroupLengths.ContainsKey(animationGroupName))
            {
                Debug.LogWarning("[EntityAnimationHandler] Animation \"" + animationGroupName + "\" does not exist in " + name + " Concurrent Animation Handler" + "!");
                return -1f;
            }

            return allConcurrentAnimationGroupLengths[animationGroupName];
        }
    }
    
    /// <summary>
    /// Stores the lengths of all the concurrent animation groups to improve animation length retrieval time.
    /// </summary>
    private void storeConcurrentAnimationGroupLengths()
    {
        allConcurrentAnimationGroupLengths = new Dictionary<string, float>();

        foreach(var animationGroup in concurrentAnimationGroups)
        {
            allConcurrentAnimationGroupLengths.Add(animationGroup.AnimationGroupName, animationGroup.getAnimationLength());
        }
    }
}
