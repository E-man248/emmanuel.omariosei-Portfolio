using System.Collections.Generic;
using UnityEngine;

public class DataPersistanceManager : MonoBehaviour
{
    public static DataPersistanceManager Instance { get; private set; } = null;

    [Header("Save File Location Config")]
    [SerializeField] private string dataFileName = "ZeitghastSave";

    private GameData GameData = null;
    private FileDataHandler fileDataHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        GameData = LoadGamedata();
    }

    private void setUpFileDataHandler()
    {
        if (fileDataHandler == null)
        {
            fileDataHandler = new FileDataHandler(Application.persistentDataPath, dataFileName);
        }
    }

    public GameData getGameData()
    {
        //Check if we already have a game data file loaded
        if (GameData != null)
        {
            return GameData;
        }

        //Load from file 
        GameData = LoadGamedata();
        
        return new(GameData);
    }

    public GameData LoadGamedata()
    {
        setUpFileDataHandler();
        GameData data = fileDataHandler.Load();

        data = GameDataIntegrityManager.ValidateAndRepairGameData(data);

        return data;
    }

    public void ResetGameData(bool includeOptions = false)
    {
        GameData NewGameData = new GameData();

        // Copy over old options data if needed (done by default)
        if (!includeOptions)
        {
            NewGameData.options_Data = GameData.options_Data.clone();
        }
        
        GameData = NewGameData;
        Debug.Log("Game Data Wiped!");
    }

    public void UpdateAllLevelData(GameData gameData)
    {
        UpdateAllLevelData(gameData.all_Level_Data);
    }

    public void UpdateAllLevelData(Dictionary<string, LevelData> all_Level_Data)
    {
        GameData.all_Level_Data = all_Level_Data;
    }

    public void UpdateLevelData(string levelSaveDataKey, LevelData levelData)
    {
        GameData.all_Level_Data[levelSaveDataKey] = levelData;
    }

    public LevelData GetLevelData(string levelSaveDataKey)
    {
        return new (getGameData().all_Level_Data[levelSaveDataKey]);
    }

    public void UpdateOptionsData(GameData gameData)
    {
        UpdateOptionsData(gameData.options_Data);
    }

    public OptionsData GetOptionsData()
    {
        return getGameData().options_Data.clone();
    }

    public void UpdateOptionsData(OptionsData optionsData)
    {
        GameData.options_Data = optionsData.clone();
    }

    public void UpdateHatData(HatData hat_Data)
    {
        GameData.hat_Data = hat_Data;
    }

    public HatData GetHatData()
    {
        return new HatData(getGameData().hat_Data);
    }

    public void SaveGameData()
    {
        setUpFileDataHandler();

        fileDataHandler.Save(GameData);
    }
}
