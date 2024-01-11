using UnityEngine;
using UnityEngine.Events;

public class HatDispenser : MonoBehaviour
{
    [Header("Hat Dispenser Settings")]
    [SerializeField] private bool UnlockHatOnInteract = false;

    [Header("Hat Dispenser Utilities")]
    [SerializeField] private HatHolder hatHolder;
    private OnInteractTrigger interactTrigger;
    
    [Header("Hat Holder Events")]
    public UnityEvent<HatInfo> hatDispensed;

    // Utilities:
    private GameObject hatHolderGraphics;

    #region Unity Functions
    private void Awake()
    {
        if (hatHolder == null)
        {
            hatHolder = GetComponentInChildren<HatHolder>();

            if (hatHolder == null)
            {
                hatHolder = gameObject.AddComponent<HatHolder>();
            }
        }

        if (hatHolder != null)
        {
            hatHolderGraphics = GetComponentInChildren<HatHolderAnimationHandler>()?.gameObject;
        }

        if (interactTrigger == null)
        {
            interactTrigger = GetComponentInChildren<OnInteractTrigger>();
        }
    }

    private void Start()
    {
        subscribeToEvents();
    }

    private void Update()
    {
        if (interactTrigger != null)
        {
            // Update Interaction Trigger Availability based on Hat Unlock:
            if (hatHolder.RequireEquippedHatToBeUnlocked)
            {
                interactTrigger.DisableInteraction = !IsDispensableHatUnlocked();
                hatHolderGraphics.SetActive(IsDispensableHatUnlocked());
            }
        }
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
        interactTrigger?.interactEvent.AddListener(OnInteract);
    }

    private void unsubscribeToEvents()
    {
        interactTrigger?.interactEvent.RemoveListener(OnInteract);
    }

    private void OnInteract()
    {
        // On Interact, Dispense Equipped Hat to Player:

        HatHolder playerHatHolder = PlayerInfo.Instance.GetComponentInChildren<HatHolder>();

        if (UnlockHatOnInteract)
        {
            HatManager.Instance.UnlockHat(hatHolder.EquippedHat.hatId);
        }

        DispenseHat(playerHatHolder);
    }

    #endregion

    private void DispenseHat(HatHolder hatHolderToEquip)
    {
        hatHolderToEquip.Equip(hatHolder.EquippedHat);

        hatDispensed.Invoke(hatHolder.EquippedHat);
    }

    private bool IsDispensableHatUnlocked()
    {
        if (string.IsNullOrWhiteSpace(hatHolder?.EquippedHat?.hatId)) return true;

        return HatManager.Instance.GetHatUnlockStatus(hatHolder.EquippedHat.hatId);
    }
}
