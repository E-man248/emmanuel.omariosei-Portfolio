using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamIntegrationManager : MonoBehaviour
{
    public static SteamIntegrationManager Instance { get; private set; }
    public bool SteamConnectionActive { get; private set; } = false;

    #region Unity Functions
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        SetupSteamClient();
    }

    private void Update()
    {
        if (!SteamConnectionActive) return;

        SteamUpdateCycle();
    }

    private void OnApplicationQuit()
    {
        ShutdownSteamConnection();
    }
    #endregion

    private void SetupSteamClient()
    {
        try
        {
            // Initialize Steam Client with Steam Game ID (2593070)
            Steamworks.SteamClient.Init(2593070);

            SteamConnectionActive = true;

            // Print Connection Success Statement:
            print(GetConnectionSuccessStatement());
        }
        catch (Exception e)
        {
            SteamConnectionActive = false;

            // Print Connection Failure Statement:
            print(GetConnectionFailureStatement(e.ToString()));
        }
    }

    public string GetUsername()
    {
        return Steamworks.SteamClient.Name;
    }

    public string GetFriendList()
    {
        string friendList = "";

        foreach (var friend in Steamworks.SteamFriends.GetFriends())
        {   
            friendList += friend + "\n";
        }

        return friendList;
    }

    public Steamworks.Data.Achievement GetAchievementData(string achievementId)
    {
        return new Steamworks.Data.Achievement(achievementId);
    }

    #region Helper Functions
    private string GetConnectionSuccessStatement()
    {
        string connectionSuccessStatement = ""
                + "Steam Connection Successful!\n"
                + "Username: " + GetUsername() + "\n"
                + "Friend List: \n" + GetFriendList();

        return connectionSuccessStatement;
    }
    private string GetConnectionFailureStatement(string errorMessage = null)
    {
        string connectionFailureStatement = ""
                + "Steam Connection Failed!\n";

        if (!String.IsNullOrWhiteSpace(errorMessage))
        {
            connectionFailureStatement += "Error Message: \n" + errorMessage;
        }

        return connectionFailureStatement;
    }

    private void SteamUpdateCycle()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    private void ShutdownSteamConnection()
    {
        if (!SteamConnectionActive) return;

        Steamworks.SteamClient.Shutdown();

        SteamConnectionActive = false;
    }
    #endregion
}
