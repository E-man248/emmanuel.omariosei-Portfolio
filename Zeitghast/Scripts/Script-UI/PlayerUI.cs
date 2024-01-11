using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance = null;
    
    public Hotbar hotbar;
    public Slider healthBarSlider;
    public Image healthBarFill;
    public Slider generalRateBarSlider;
    public Image generalRateBarFill;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }

    public void Start()
    {
        hotbar = GetComponentInChildren<Hotbar>();
        healthBarSlider = GetComponentInChildren<Slider>();
        healthBarFill = GetComponentInChildren<Image>();
        generalRateBarSlider = GetComponentInChildren<Slider>();
        generalRateBarFill = GetComponentInChildren<Image>();
    }
}
