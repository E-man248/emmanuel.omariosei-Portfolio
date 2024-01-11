using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleTrackerUI : MonoBehaviour
{
    private int currentCollectibleCount = 0;
    private CollectibleInfo recentFoundCollectible = null;
    public bool displayActive {get; private set;}

    [Header("Graphics Settings")]
    [SerializeField] private Sprite defaultRecentFoundCollectibleSprite;

    [Header("Graphics  Utilities")]
    [SerializeField] private TextMeshProUGUI collectibleCountText;
    [SerializeField] private Image iconSpriteRenderer;
    [field: SerializeField] public CollectibleTrackerUIAnimationHandler animationHandler {get; private set;}

    #region Unity Functions
    private void Awake()
    {
        animationHandler = GetComponentInChildren<CollectibleTrackerUIAnimationHandler>();
    }

    private void Update()
    {
        // Fetch Collectible Data:

        currentCollectibleCount = LevelManager.Instance.GetCurrentCollectibleCount();

        // Update Graphical Elements:

        UpdateRecentFoundCollectibleIconGraphics();

        UpdateCollectibleCountGraphics();
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
        LevelManager.Instance?.NewCollectibleFound.AddListener(OnCollectibleFound);
        PauseMenuUI.Instance?.PlayerPauseEvent.AddListener(OnPlayerPause);
        PauseMenuUI.Instance?.PlayerUnpauseEvent.AddListener(OnPlayerUnpause);
    }

    private void unsubscribeToEvents()
    {
        LevelManager.Instance?.NewCollectibleFound.RemoveListener(OnCollectibleFound);
        PauseMenuUI.Instance?.PlayerPauseEvent.AddListener(OnPlayerPause);
        PauseMenuUI.Instance?.PlayerUnpauseEvent.AddListener(OnPlayerUnpause);
    }
    private void OnCollectibleFound(CollectibleInfo collectible)
    {
        recentFoundCollectible = collectible;

        NotifyNewCollectibleFound();
    }

    private void OnPlayerPause()
    {
        ShowDisplay();
    }

    private void OnPlayerUnpause()
    {
        HideDisplay();
    }
    #endregion

    private void UpdateRecentFoundCollectibleIconGraphics()
    {
        if (recentFoundCollectible != null)
        {
            iconSpriteRenderer.sprite = recentFoundCollectible.DisplayIcon;
        }
        else
        {
            iconSpriteRenderer.sprite = defaultRecentFoundCollectibleSprite;
        }
    }

    private void UpdateCollectibleCountGraphics()
    {
        collectibleCountText.text = currentCollectibleCount + " / 20";
    }

    private void NotifyNewCollectibleFound()
    {
        animationHandler?.PlayNotifyAnimation();
    }

    private void ShowDisplay()
    {
        animationHandler?.PlayAppearAnimation();
        displayActive = true;
    }

    private void HideDisplay()
    {
        animationHandler?.PlayDisappearAnimation();
        displayActive = false;
    }
}
