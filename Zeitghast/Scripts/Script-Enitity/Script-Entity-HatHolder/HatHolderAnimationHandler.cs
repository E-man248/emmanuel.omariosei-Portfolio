using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HatHolderAnimationHandler : EntityAnimationHandler
{
    [Header("Hat Holder Animation Handler Settings")]
    [SerializeField] private HatHolder hatHolder;
    private Dictionary<string, GameObject> hatGraphicsCache;
    private GameObject currentHatGraphics;

    protected override void Awake()
    {
        base.Awake();

        if (hatHolder == null)
        {
            hatHolder = GetComponentInParent<HatHolder>();
        }

        if (hatHolder == null)
        {
            gameObject.SetActive(false);
        }

        hatGraphicsCache = new Dictionary<string, GameObject>();
    }

    
    #region Unity Functions
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
        hatHolder?.hatEquipped.AddListener(OnHatEquipped);
        hatHolder?.hatRemoved.AddListener(OnHatRemoved);
    }

    private void unsubscribeToEvents()
    {
        hatHolder?.hatEquipped.RemoveListener(OnHatEquipped);
        hatHolder?.hatRemoved.RemoveListener(OnHatRemoved);
    }

    private void OnHatEquipped(HatInfo equippedHat)
    {
        // Hide Current Hat Graphics:
        currentHatGraphics?.SetActive(false);

        if (equippedHat == null)
        {
            currentHatGraphics = null;
            return;
        }

        // Store New Equipped Hat Graphics:
        if (hatGraphicsCache.ContainsKey(equippedHat.hatId))
        {
            // If Hat Graphics are Cached, Load from Cached:
            currentHatGraphics = hatGraphicsCache[equippedHat.hatId];
        }
        else
        {
            // If Hat Graphics Not Cached, Create and Add to Cache:
        
            currentHatGraphics = Instantiate(equippedHat.HatObject, transform);

            hatGraphicsCache.Add(equippedHat.hatId, currentHatGraphics);
        }

        // Show New Current Hat Graphics:
        currentHatGraphics.SetActive(true);
    }

    private void OnHatRemoved(HatInfo removedHat)
    {
        // Hide Current Hat Graphics:
        currentHatGraphics?.SetActive(false);

        currentHatGraphics = null;
    }

    #endregion

}
