using UnityEngine;

public class WeaponGlowAnimationHandler : EntityAnimationHandler
{
    [Header("Weapon Glow Animation Handler Settings")]
    [SerializeField] private GameObject PlayerGlowGraphics;
    [SerializeField] private GameObject InWorldGlowGraphics;

    // Animation Utilities:
    private WeaponAnimationHandler weaponAnimationHandler;

    protected override void Awake()
    {
        weaponAnimationHandler = transform.parent?.GetComponentInParent<WeaponAnimationHandler>();

        if (weaponAnimationHandler == null)
        {
            // Disable Object if No WeaponAnimationHandler is Found in Parent:
            Debug.LogWarning("[WeaponGlowAnimationHandler] No 'WeaponAnimationHandler' Script in Parent of " + transform.parent.name);
            gameObject.SetActive(false);
        }

        // Disable Script Controlled Animation Cycle: (Only Non-Mutually Exclusive Animations Play)
        DisableAutoAnimate = true;
    }

    protected override void Update()
    {   
        if (Timer.gamePaused) return;

        base.Update();
        
        // Non-Mutually Exclusive Animations:

        if (weaponAnimationHandler.weaponInactive())
        {
            inactiveAnimation();
        }
        else
        {
            activeAnimation();
        }
    }

    protected void inactiveAnimation()
    {
        // Deactivate Player Hand Glow:
        PlayerGlowGraphics?.SetActive(false);

        // Activate In World Weapon Glow:
        InWorldGlowGraphics?.SetActive(true);
    }

    protected void activeAnimation()
    {
        // Activate Player Hand Glow:
        PlayerGlowGraphics?.SetActive(true);

        // Deactivate In World Weapon Glow:
        InWorldGlowGraphics?.SetActive(false);
    }
}
