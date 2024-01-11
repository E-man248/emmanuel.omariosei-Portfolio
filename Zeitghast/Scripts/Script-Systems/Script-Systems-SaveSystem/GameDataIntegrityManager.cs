using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataIntegrityManager : MonoBehaviour
{
    /// <summary>
    /// Validates given Game Data and returns a Working Game Data copy (Repaired if needed)
    /// </summary>
    public static GameData ValidateAndRepairGameData(GameData gameData)
    {
        if (gameData == null)
        {
            Debug.Log("Game Data Wiped!");
            return new GameData();
        }
        
        GameData validGameData = new GameData(gameData);

        validGameData.hat_Data = ValidateAndRepairHatData(gameData.hat_Data);

        validGameData.all_Level_Data = ValidateAndRepairLevelDataDictionary(gameData.all_Level_Data);

        validGameData.options_Data = ValidateAndRepairOptionsData(gameData.options_Data);

        return validGameData;
    }

    private static Dictionary<string, LevelData> ValidateAndRepairLevelDataDictionary(Dictionary<string, LevelData> currentLevelDataDictionary)
    {
        Dictionary<string, LevelData> validLevelDataDictionary = new Dictionary<string, LevelData>(currentLevelDataDictionary);

        var LevelNameMasterList = AdvancedSceneManager.GetAllLevelSceneNames();

        // - If Level Name Key is Invalid (Compare with Master List) => Delete Invalid
        foreach (var levelName in validLevelDataDictionary.Keys.ToList())
        {
            if (!LevelNameMasterList.Contains(levelName))
            {
                Debug.LogError("Invalid Level: " + levelName);
                Debug.Log("Removing Invalid Level from Level Data Dictionary");
                validLevelDataDictionary.Remove(levelName);
            }
        }

        // - If Level Names Missing from Master => JSON Handles Addition Missing Levels

        // - If Level Count Does Not Match Master => Create New All Level Data Dictionary
        if (validLevelDataDictionary.Count != LevelNameMasterList.Count)
        {
            Debug.LogError("Level Count Invalid");
            Debug.Log("Creating New Level Data Dictionary.");
            validLevelDataDictionary = GameData.GenerateBlankLevelDataDictionary();
        }

        // - Level.LevelName is not the same as the Key => Change Level.LevelName to Key
        foreach (var level in validLevelDataDictionary)
        {
            if (!level.Key.Equals(level.Value.LevelName))
            {
                Debug.LogError("Level Data Name does not match Key -> Renaming Level Data Name to match.");
                Debug.Log("Removing Invalid Collectible from Collectible Data Dictionary.");
                level.Value.LevelName = level.Key;
            }
        }

        // - Validate and Repair Collectible Data:
        foreach (var level in validLevelDataDictionary)
        {
            level.Value.CollectiblesCollected = ValidateAndRepairCollectibleDataDictionary(level.Value.CollectiblesCollected);
        }

        return validLevelDataDictionary;
    }
    
    private static Dictionary<string, bool> ValidateAndRepairCollectibleDataDictionary(Dictionary<string, bool> currentCollectibleDataDictionary)
    {
        Dictionary<string, bool> validCollectibleDataDictionary = new Dictionary<string, bool>(currentCollectibleDataDictionary);

        var CollectibleNameMasterList = Collectible.collectibleNames;

        // - If Collectible Name Key is Invalid (Compare with Master List) => Delete Invalid
        foreach (var collectibleName in validCollectibleDataDictionary.Keys.ToList())
        {
            if (!CollectibleNameMasterList.Contains(collectibleName))
            {
                Debug.LogError("Invalid Collectible: " + collectibleName);
                Debug.Log("Removing Invalid Collectible from Collectible Data Dictionary.");
                validCollectibleDataDictionary.Remove(collectibleName);
            }
        }

        // - If Collectibles Names Missing from Master => JSON Handles Addition Missing Collectible

        // - If Collectible Count Does Not Match Master => Create New All Collectible Data Dictionary
        if (validCollectibleDataDictionary.Count != CollectibleNameMasterList.Count)
        {
            Debug.LogError("Collectible Count Invalid");
            Debug.Log("Creating New Collectible Data Dictionary.");
            validCollectibleDataDictionary = Collectible.GenerateBlankCollectibleDictionary();
        }

        return validCollectibleDataDictionary;
    }

    private static OptionsData ValidateAndRepairOptionsData(OptionsData currentOptionsData)
    {
        OptionsData validOptionsData = currentOptionsData.clone();

        // Options Validation:
        // - Audio Values Must Fall Between Range (0 to 1)

        if (validOptionsData.MasterVolumeValue < 0f || validOptionsData.MasterVolumeValue > 1f)
        {
            Debug.LogError("MasterVolumeValue has Invalid value of :" + validOptionsData.MasterVolumeValue);
            Debug.Log(" setting MasterVolumeValue to default value of 1f");
            validOptionsData.setMasterVolumeValue(1f);
        }

        if (validOptionsData.SFXVolumeValue < 0f || validOptionsData.SFXVolumeValue > 1f)
        {
            Debug.LogError("SFXVolumeValue has Invalid value of :" + validOptionsData.SFXVolumeValue);
            Debug.Log(" setting SFXVolumeValue to default value of 1f");
            validOptionsData.setSFXVolumeValue(1f);
        }

        if (validOptionsData.MusicVolumeValue < 0f || validOptionsData.MusicVolumeValue > 1f)
        {
            Debug.LogError("MusicVolumeValue has Invalid value of :" + validOptionsData.MusicVolumeValue);
            Debug.Log(" setting MusicVolumeValue to default value of 1f");
            validOptionsData.setMusicVolumeValue(1f);
        }

        return validOptionsData;
    }

    private static HatData ValidateAndRepairHatData(HatData currentHatData)
    {
        HatData validHatData = new HatData(currentHatData);

        var HatMasterList = MasterHatIdList.HatIds;

        // - If Hat Id Key is Invalid (Compare with Master List) => Delete Invalid
        foreach (var hatId in validHatData.HatUnlockStatus.Keys.ToList())
        {
            if (!HatMasterList.Contains(hatId))
            {
                Debug.LogError("Invalid Hat: " + hatId);
                Debug.Log("Removing Invalid Hat from Hat Data Status.");
                validHatData.HatUnlockStatus.Remove(hatId);
            }
        }

        // - If Hat Ids Missing from Master => JSON Handles Addition Missing Hat Ids

        // - If Hat Count Does Not Match Master => Create New All Hat Data Dictionary
        if (validHatData.HatUnlockStatus.Count() != HatMasterList.Count())
        {
            Debug.LogError("Hat Count Invalid");
            Debug.Log("Creating New Hat Status Data.");
            validHatData.HatUnlockStatus = HatData.GenerateBlankHatDataDictionary();
        }

        return validHatData;
    }
}
