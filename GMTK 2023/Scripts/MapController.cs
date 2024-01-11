using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject dayMap;
    [SerializeField] private GameObject nightMap;
    
    private void Update()
    {
        changeMap();
    }


    public void changeMap()
    {
        switch(GameStateManger.Instance.currentTime)
        {
            case GameStateManger.DayNightState.Day:
                dayMap.SetActive(true);
                nightMap.SetActive(false);
                break;
            case GameStateManger.DayNightState.Night:
                    dayMap.SetActive(false);
                    nightMap.SetActive(true);
                break;
        }
    }
}
