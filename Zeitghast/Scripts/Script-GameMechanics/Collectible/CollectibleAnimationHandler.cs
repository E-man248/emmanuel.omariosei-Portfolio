using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleAnimationHandler : EntityAnimationHandler
{
    [Header("Collectible Animation Handler Settings")]
    [SerializeField] private float inactiveModeTransparency = 0.5f;

    private Collectible collectible;
    private SpriteRenderer spriteRenderer;
    
    private Color activeColor;
    private Color inactiveColor;
    
    private const string idleAnimationString = "CollectibleIdle";
    private const string collectedAnimationString = "CollectibleCollect";

    private void Start()
    {
        collectible = GetComponentInParent<Collectible>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            activeColor = spriteRenderer.color;
            inactiveColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, inactiveModeTransparency);
        }
    }

    protected override void animate()
    {
        idleAnimation();
        collectedAnimation();

        base.animate();
    }

    protected override void Update()
    {
        base.Update();

        // Non-Mutually Exclusive Animations:
        activityAnimation();
    }

    #region Animation Booleans

    public bool isCollected()
    {
        return collectible.CollectedInLevelRun;
    }

    public bool isActive()
    {
        return !collectible.CollectedInGameSave;
    }

    #endregion

    #region Mutually Exclusive Animations

    private void idleAnimation()
    {
        nextAnimation = idleAnimationString;
    }

    private void collectedAnimation()
    {
        if (isCollected())
        {
            nextAnimation = collectedAnimationString;
        }
    }

    #endregion

    #region Non-Mutually Exclusive Animations

    private void activityAnimation()
    {
        if (spriteRenderer == null) return;
        
        if (!isActive())
        {
            spriteRenderer.color = inactiveColor;
        }
        else
        {
            spriteRenderer.color = activeColor;
        }
    }

    #endregion
}
