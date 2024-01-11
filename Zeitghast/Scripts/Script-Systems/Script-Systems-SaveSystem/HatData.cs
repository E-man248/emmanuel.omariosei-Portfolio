using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HatData
{
    public Dictionary<string, bool> HatUnlockStatus;
    public string RecentPlayerHatId;

    public HatData()
    {
        try 
        {
            HatUnlockStatus = GenerateBlankHatDataDictionary();
        }
        catch (Exception)
        {
            HatUnlockStatus = null;
        }

        RecentPlayerHatId = null;
    }

    public HatData(HatData srcHatData)
    {
        try 
        {
            HatUnlockStatus = new Dictionary<string, bool>(srcHatData.HatUnlockStatus);
        }
        catch (Exception)
        {
            var blankHatData = new HatData();
            HatUnlockStatus = blankHatData.HatUnlockStatus;
        }

        RecentPlayerHatId = srcHatData.RecentPlayerHatId;
    }

    public static Dictionary<string, bool> GenerateBlankHatDataDictionary()
    {
        Dictionary<string, bool> result = new Dictionary<string, bool>();

        foreach (string hatId in MasterHatIdList.HatIds)
        {
            result[hatId] = false;
        }

        return result;
    }
    public override string ToString()
    {
        string output = "";

        output += "Recent Player Hat Id: " + RecentPlayerHatId + "\n\n";

        output += "Hat Unlock Count: " + HatUnlockStatus.Count( (status) => status.Value == true ) + "\n\n";

        foreach (var hatStatus in HatUnlockStatus)
        {
            output += hatStatus.Key + " | Unlocked = " + hatStatus.Value + "\n";
        }

        return output;
    }

}
