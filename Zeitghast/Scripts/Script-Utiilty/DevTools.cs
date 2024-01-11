using System.Linq;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    public bool enableDevTools = false;
    public bool devToolsActive = false;

    public PlayerHealth playerHealth;
    public PlayerKnockback playerKnockback;
    [SerializeField] private GameObject weapon;
    
    [SerializeField] private float gameDataWipeCountdownDuration = 3f;
    private float gameDataWipeCountdown;

    public static DevTools Instance = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        GameData gameData = DataPersistanceManager.Instance.getGameData();

        enableDevTools = gameData.devModeEnabled;

        devToolsActive = enableDevTools;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableDevTools && Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (devToolsActive)
            {
                devToolsActive = false;
            }
            else
            {
                devToolsActive = true;
            }
        }

        if (!enableDevTools || !devToolsActive)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Break();
        } 
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            healPlayer();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            spawnWeapon();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            togglePlayerInvincibililty();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            setPlayerHealthToOne();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            unlockAllLevels();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            unlockAllCollectiblesAndChallenges();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            unlockAllHats();
        }

        if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.M))
        {
            wipeGameData();
        }
        else
        {
            gameDataWipeCountdown = gameDataWipeCountdownDuration;
        }


        timerButtons();
    }

    void timerButtons()
    {
        if (Timer.Instance == null)
        {
            return;
        }

        if (Input.GetKeyDown("="))
        {
            Timer.Instance.ChangeTime(10f);
        }

        if (Input.GetKeyDown("-"))
        {
            Timer.Instance.ChangeTime(-10f);
        }

        if (Input.GetKeyDown("0"))
        {
            Timer.Instance.ResetTime();
        }
    }
    void healPlayer()
    {
        if(playerHealth == null)
        {
            playerHealth = PlayerInfo.Instance.GetComponent<PlayerHealth>();
        }

        if(playerHealth != null)
        {
            playerHealth.ResetHealth();
        }
    }

    void togglePlayerInvincibililty()
    {
        if (playerHealth == null)
        {
            playerHealth = PlayerInfo.Instance.GetComponent<PlayerHealth>();
        }

        if (playerKnockback == null)
        {
            playerKnockback = PlayerInfo.Instance.GetComponent<PlayerKnockback>();
        }

        if (playerHealth == null) return;

        if (playerHealth.trueInvincible)
        {
            playerHealth.trueInvincible = false;
            playerHealth.invincible = false;

            if (playerKnockback == null) return;
            playerKnockback.trueImmobility = false;
            playerKnockback.knockbackDisabled = false;
        }
        else
        {
            playerHealth.trueInvincible = true;
            playerHealth.invincible = true;

            if (playerKnockback == null) return;
            playerKnockback.trueImmobility = true;
            playerKnockback.knockbackDisabled = true;
        }
    }

    void setPlayerHealthToOne()
    {
        if (playerHealth == null)
        {
            playerHealth = PlayerInfo.Instance.GetComponent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.changeHealth(-(playerHealth.health - 1),"");
        }
    }

    void spawnWeapon()
    {
        if (weapon == null)
        {
            return;
        }

        if (playerHealth == null)
        {
            playerHealth = PlayerInfo.Instance.GetComponent<PlayerHealth>();
        }

        Instantiate(weapon, playerHealth.transform.position, playerHealth.transform.rotation);
    }

    void unlockAllLevels()
    {
        GameData gameData = DataPersistanceManager.Instance.getGameData();

        foreach (var levelData in gameData.all_Level_Data)
        {
            levelData.Value.LevelComplete = true;
        }

        var levelEntryDoors = GameObject.FindObjectsOfType<LevelEntryDoor>();

        foreach (var levelEntryDoor in levelEntryDoors)
        {
            levelEntryDoor.UpdateActiveStatus(true);
        }
    }

    void unlockAllCollectiblesAndChallenges()
    {
        unlockAllLevels();

        GameData gameData = DataPersistanceManager.Instance.getGameData();

        foreach (var levelData in gameData.all_Level_Data)
        {
            levelData.Value.LevelChallengeComplete = true;
            foreach (var collectibleStatuses in levelData.Value.CollectiblesCollected.ToList())
            {
                levelData.Value.CollectiblesCollected[collectibleStatuses.Key] = true;
            }
        }

        DataPersistanceManager.Instance.UpdateAllLevelData(gameData.all_Level_Data);

        print(gameData.ToString());

        DataPersistanceManager.Instance.SaveGameData();

        print("All Game Content Collected");
    }

    void unlockAllHats()
    {
        unlockAllLevels();

        GameData gameData = DataPersistanceManager.Instance.getGameData();

        foreach (var hatStatus in gameData.hat_Data.HatUnlockStatus.ToList())
        {
            gameData.hat_Data.HatUnlockStatus[hatStatus.Key] = true;
        }

        DataPersistanceManager.Instance.UpdateAllLevelData(gameData.all_Level_Data);

        print(gameData.ToString());

        DataPersistanceManager.Instance.SaveGameData();

        print("All Hats Unlocked");
    }

    void wipeGameData()
    {
        if (gameDataWipeCountdown >= 0f)
        {
            gameDataWipeCountdown -= Time.unscaledDeltaTime;
        }
        else
        {
            DataPersistanceManager.Instance.ResetGameData();
            gameDataWipeCountdown = gameDataWipeCountdownDuration;
        }
    }
}
