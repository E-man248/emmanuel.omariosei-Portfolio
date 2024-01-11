using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GameData
{
    public Dictionary<string, LevelData> all_Level_Data;
    public OptionsData options_Data;
    public HatData hat_Data;
    public bool devModeEnabled;

    public GameData()
    {
        all_Level_Data = GenerateBlankLevelDataDictionary();

        options_Data = new OptionsData();

        hat_Data = new HatData();

        devModeEnabled = false;
    }

    public GameData(GameData gameDataToCopy)
    {
        all_Level_Data = DeepCloneLevelDictionary(gameDataToCopy.all_Level_Data);

        options_Data = gameDataToCopy.options_Data.clone();

        hat_Data = new HatData(gameDataToCopy.hat_Data);

        devModeEnabled = gameDataToCopy.devModeEnabled;
    }

    private Dictionary<string, LevelData> DeepCloneLevelDictionary (Dictionary<string, LevelData> dataToCopy)
    {
        Dictionary<string, LevelData> result = new Dictionary<string, LevelData>(dataToCopy);

        foreach (var levelData in result.ToList())
        {
            result[levelData.Key] = new LevelData(levelData.Value);
        }

        return result;
    }

    public static Dictionary<string, LevelData> GenerateBlankLevelDataDictionary()
    {
        Dictionary<string, LevelData> result = new Dictionary<string, LevelData>();

        foreach (string sceneName in AdvancedSceneManager.GetAllLevelSceneNames())
        {
            result[sceneName] = new LevelData(sceneName);
        }

        return result;
    }

    public override string ToString() 
    {
        string output = "";

        output += "\nDev Mode Enabled: " + devModeEnabled + "\n";

        output += "\nHat Data: \n" + hat_Data.ToString();

        output += "\nOptions Data: \n" + options_Data.ToString() + "\n";

        output += "\nLevel Data: \n";

        foreach (var level in all_Level_Data)
        {
            output += level.ToString();
        }

        output += "\n";

        return output;
    }
}