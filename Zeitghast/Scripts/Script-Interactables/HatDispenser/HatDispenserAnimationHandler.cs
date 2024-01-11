using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatDispenserAnimationHandler : EntityAnimationHandler
{
    public const string IdleAnimationString = "HatDispenserIdle";
    public const string DispenseAnimationString = "HatDispenserDispense";

    // Utilities:
    private HatDispenser hatDispenser;

    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();

        hatDispenser = GetComponentInParent<HatDispenser>();
    }
    
    private void Start()
    {
        subscribeToEvents();
    }

    private void OnEnable()
    {
        subscribeToEvents();
    }
    private void OnDisable()
    {
        unsubscribeToEvents();
    }

    private void OnDestroy()
    {
        unsubscribeToEvents();
    }

    #endregion

    #region Event Functions
    private void subscribeToEvents()
    {
        hatDispenser?.hatDispensed.AddListener(OnDispense);
    }

    private void unsubscribeToEvents()
    {
        hatDispenser?.hatDispensed.RemoveListener(OnDispense);
    }

    private void OnDispense(HatInfo hatInfo)
    {
        // On OnDispense, Play Dispense Animation:

        playHatDispenseAnimation();
    }

    #endregion

    protected override void animate()
    {
        nextAnimation = IdleAnimationString;
        
        base.animate();
    }

    public void playHatDispenseAnimation()
    {
        playAnimationOnceFull(DispenseAnimationString);
    }
}
