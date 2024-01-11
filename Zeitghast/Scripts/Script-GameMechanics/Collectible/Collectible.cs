using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class Collectible : MonoBehaviour
{
    [field:SerializeField] public CollectibleInfo CollectibleInfo { get; private set; }
    
    /// <summary> Marked true if collectible has a stored save of being collected in the entire game, otherwise false. </summary>
    public bool CollectedInGameSave { get; private set;} = false;

    /// <summary> Marked true if collectible has been collected at least once during the current level run, otherwise false. </summary>
    public bool CollectedInLevelRun { get; private set;} = false;

    [Header("Sounds")]
    [EventRef]
    public string PickUpSounds = null;

    #region Unity Functions
    private void Start()
    {
        subcribeToEvents();
    }

    private void OnEnable()
    {
        subcribeToEvents();
    }

    private void OnDisable()
    {
        unsubcribeToEvents();
    }

    private void OnDestroy()
    {
        unsubcribeToEvents();
    }
    #endregion

    #region Event Functions
    private void subcribeToEvents()
    {
        LevelManager.Instance?.LevelStart.AddListener(LoadCollectibleStatus);
    }

    private void unsubcribeToEvents()
    {
        LevelManager.Instance?.LevelStart.RemoveListener(LoadCollectibleStatus);
    }
    #endregion

    private void LoadCollectibleStatus()
    {
        CollectedInGameSave = LevelManager.Instance.GetCollectibleStatus(CollectibleInfo);
        CollectedInLevelRun = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!CollectedInLevelRun && collider.tag == "Player")
        {
            CollectedInLevelRun = true;

            RuntimeManager.PlayOneShot(PickUpSounds, transform.position);

            if (!CollectedInGameSave)
            {
                CollectedInGameSave = true;
                LevelManager.Instance.StoreCollectibleFound(CollectibleInfo);
            }
        }
    }
    
    public static List<string> collectibleNames = new List<string> ()
    {
        "Hourglass",
        "Wall Clock",
        "Alarm Clock",
        "Sundial",
        "Cuckoo Clock",
        "Pricey Wristwatch",
        "The Dali",
        "Digital Clock",
        "Smartwatch",
        "Novelty Block Clock",
        "Radio Clock",
        "Modernist Clock",
        "Toy Clock",
        "Glass Clock",
        "Pocket Watch",
        "Hologram Clock",
        "Smartphone",
        "Sporty Stopwatch",
        "Paper Clock",
        "Egg Timer"
    };    

    public static Dictionary<string, bool> GenerateBlankCollectibleDictionary()
    {
        Dictionary<string, bool> result = new Dictionary<string, bool>();

        foreach (string collectibleName in collectibleNames)
        {
            result[collectibleName] = false;
        }

        return result;
    }
}
