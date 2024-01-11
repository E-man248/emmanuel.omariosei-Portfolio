using System.Linq;
using UnityEngine;

public class HubLevelManager : LevelManager
{
    public override Vector3 GetPlayerStartPosition()
    {
        // Retrieve the Last Played Level:
        string lastPlayedLevel = GetLastPlayedLevelName();

        // Find a Level Entry Door for the Last Played Level:
        LevelEntryDoor lastPlayedLevelEntryDoor = FindLevelEntryDoor(lastPlayedLevel);

        // If a Level Entry Door is Found Set the Player Start Position to that Door's Position:
        if (lastPlayedLevelEntryDoor != null)
        {
            return lastPlayedLevelEntryDoor.transform.position;
        }
        else
        {
            return base.GetPlayerStartPosition();
        }
    }

    protected override void SpawnPlayerStartPortal(Vector3 spawnPosition)
    {
        string lastPlayedLevel = GetLastPlayedLevelName();

        if (string.IsNullOrWhiteSpace(lastPlayedLevel))
        {
            base.SpawnPlayerStartPortal(spawnPosition);
        }
    }

    private string GetLastPlayedLevelName()
    {
        return GameManager.Instance?.LastPlayedLevel?.baseLevelScene?.name;
    }

    private LevelEntryDoor FindLevelEntryDoor(string baseLevelName)
    {
        if (string.IsNullOrWhiteSpace(baseLevelName)) return null;

        var allLevelEntryDoorsInScene = GameObject.FindObjectsOfType<LevelEntryDoor>();

        LevelEntryDoor result = allLevelEntryDoorsInScene.FirstOrDefault( x => x.baseLevelScene == baseLevelName );

        return result;
    }
}
