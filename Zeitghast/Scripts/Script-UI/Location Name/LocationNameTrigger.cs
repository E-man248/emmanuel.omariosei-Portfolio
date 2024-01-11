using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationNameTrigger : MonoBehaviour
{
    public string LocationNameText;
    
    public Color textColor;

    public float textDuration;
    public float textFadeInDuration;
    public float textFadeOutDuration;

    public bool isOneUse;
    private bool visted;

    // Start is called before the first frame update
    void Start()
    {
        visted = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //checking if you have visted this trigger once
        if(isOneUse && visted)
        {
            return;
        }

        //triggering the location name animation
        if (collision != null && collision.tag == "Player")
        {
            visted = true;
            LocationNameManager.Instance.changeMainTextUIColor(textColor);
            LocationNameManager.Instance.displayMainTextUI(LocationNameText, textDuration, textFadeInDuration, textFadeOutDuration);
        }
    }
}
