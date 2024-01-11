using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BossUI : MonoBehaviour
{
    public static BossUI instance = null;
    
    public GameObject bossBar;
    public Slider healthBarSlider;
    public Image healthBarFill;
    public GameObject phaseIconGroup;
    private List<GameObject> phaseIconList;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;

        bossBar = transform.Find("Boss Bar").gameObject;
        healthBarSlider = GetComponentInChildren<Slider>(true);
        healthBarFill = GetComponentInChildren<Image>(true);

        phaseIconList = phaseIconGroup.GetComponentsInChildren<Transform>()
                            .Where( (x) => x.parent == phaseIconGroup.transform )
                            .Select( (x) => x.gameObject )
                            .ToList();
    }

    public void SetPhaseNumber(int phaseNumber)
    {
        if (phaseIconList.Count <= 0) return;

        // Disable All Phase Icons:
        phaseIconList.ForEach( (icon) => icon.SetActive(false) );

        // Identify How Many Phase Icons to Activate:
        int phaseNumberIndex = Mathf.Clamp(phaseNumber, 1, phaseIconList.Count) - 1;
        int phaseIconsActiveCount = phaseIconList.Count - phaseNumberIndex;

        // Active Number of Phase Icons Associated to Phase Number
        for (int i = 0; i < phaseIconsActiveCount; i++)
        {
            phaseIconList[i].SetActive(true);
        }
    }
}
