using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomUITextGenerator : MonoBehaviour
{

    [Header("Display Settings")]
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private bool displayOnStart = false;
    [SerializeField] private bool displayOnEnable = false;
    
    [Header("Random Text Message")]
    [SerializeField] private string message = "Random Text!";
    [Space]
    [SerializeField] private List<string> messagePool;

    public void Start()
    {
        AttachTextDisplay();

        if (displayOnStart)
        {
            GenerateRandomDisplayText();
        }
    }

    public void OnEnable()
    {
        AttachTextDisplay();
        
        if (displayOnEnable)
        {
            GenerateRandomDisplayText();
        }
    }

    public void AttachTextDisplay()
    {
        if (textDisplay != null) return;

        textDisplay = GetComponent<TextMeshProUGUI>();
            
        // If no textDisplay is found, log error:
        if (textDisplay == null) Debug.LogError("There is no TextMeshPro attached to this Generator!");
    }

    public string GenerateRandomDisplayText()
    {
        if (messagePool.Count > 0)
        {
            message = messagePool[Random.Range(0, messagePool.Count)];
        }

        textDisplay.text = message;

        return message;
    }
}
