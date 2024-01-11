using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Playables;

public class FileDataHandler
{
    private string dataDirectoryPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirectoryPath, string dataFileName)
    {
        this.dataDirectoryPath = dataDirectoryPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
        GameData loadedGameData = null;

        if(!File.Exists(fullPath)) 
        {
            Debug.LogError("'" + fullPath + "' Could not find file");
            return null;
        }

        try
        {
            //Get Data from file  
            string dataToLoad;
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            //deserialize file data 
            loadedGameData = JsonConvert.DeserializeObject<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError("An error happened when trying to Load save file: [" + fullPath + "] \nException[" + e + "]");
        }
        
        return loadedGameData;
    }

    public void Save(GameData gameData)
    {
        string fullPath =  Path.Combine(dataDirectoryPath, dataFileName);

        try
        {
            //creating directory path if it doesn't exist.
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            if(gameData?.all_Level_Data == null)
            {
                Debug.LogError("Game data is null!!!");
            }

            Debug.Log("Saving!");
            Debug.Log("Game data: " + gameData.ToString());

            //serialize file data
            string data = JsonConvert.SerializeObject(gameData);

            Debug.Log("Serialized game data: " + data);

            //write to File
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(" An error happened when trying to save file: [" + fullPath + "] \nException[" + e +"]");
        }
    }

}
