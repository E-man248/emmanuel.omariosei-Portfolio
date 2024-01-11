using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HatManager : MonoBehaviour
{
    public static HatManager Instance { get; private set; }

    private HatData CurrentHatData;

    [Header("Hat Manager Settings")]
    [SerializeField] private List<HatInfo> hatInfoList;

    private PlayerHatHolder playerHatHolder;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    #region Unity Functions
    
    private void Start()
    {
        if (CurrentHatData == null) CurrentHatData = GetHatData();

        playerHatHolder = PlayerInfo.Instance.GetComponentInChildren<PlayerHatHolder>();
    
        subscribeToEvents();
    }

    private void Update()
    {
        if (playerHatHolder == null && PlayerInfo.Instance != null)
        {
            playerHatHolder = PlayerInfo.Instance.GetComponentInChildren<PlayerHatHolder>();

            subscribeToEvents();
        }
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
        playerHatHolder?.hatEquipped.AddListener(OnPlayerHatEquipped);
    }

    private void unsubscribeToEvents()
    {
        playerHatHolder?.hatEquipped.RemoveListener(OnPlayerHatEquipped);
    }

    private void OnPlayerHatEquipped(HatInfo equippedHat)
    {
        CurrentHatData = GetHatData(true);

        CurrentHatData.RecentPlayerHatId = equippedHat?.hatId;

        DataPersistanceManager.Instance.UpdateHatData(CurrentHatData);

        DataPersistanceManager.Instance.SaveGameData();
    }

    #endregion

    private void SetPlayerHatToRecentPlayerHat()
    {
        HatInfo recentPlayerHatInfo = GetHatInfo(GetHatData().RecentPlayerHatId);

        playerHatHolder.Equip(recentPlayerHatInfo);
    }

    public HatInfo GetRecentPlayerHat()
    {
        string recentPlayerHatId = GetHatData().RecentPlayerHatId;

        return GetHatInfo(recentPlayerHatId);
    }

    public HatInfo GetHatInfo(string hatId)
    {
        // Hat Id can be Null (No Hat)
        if (string.IsNullOrEmpty(hatId)) return null;

        try
        {
            return hatInfoList.Single( (hatInfo) => hatInfo.hatId == hatId );
        }
        catch (Exception)
        {
            throw new Exception("Hat Manager is missing '" + hatId + "' from its Hat Info List or Present Duplicated!");
        }
    }

    private HatData GetHatData(bool getFreshData = false)
    {
        if (CurrentHatData == null || getFreshData)
        {
            CurrentHatData = DataPersistanceManager.Instance.getGameData().hat_Data;
        }

        return CurrentHatData;
    }

    public void UnlockHat(string hatId)
    {
        SetHatUnlockStatus(hatId, true);
    }

    public void SetHatUnlockStatus(string hatId, bool status)
    {
        CurrentHatData = GetHatData(true);

        CurrentHatData.HatUnlockStatus[hatId] = status;

        DataPersistanceManager.Instance.UpdateHatData(CurrentHatData);

        DataPersistanceManager.Instance.SaveGameData();
    }

    public bool GetHatUnlockStatus(string hatId)
    {
        if (CurrentHatData == null) CurrentHatData = GetHatData();

        return CurrentHatData.HatUnlockStatus[hatId];
    }

    public int GetHatUnlockCount()
    {
        if (CurrentHatData == null) CurrentHatData = GetHatData();

        return CurrentHatData.HatUnlockStatus.Count( (hatStatus) => hatStatus.Value == true );
    }
}
