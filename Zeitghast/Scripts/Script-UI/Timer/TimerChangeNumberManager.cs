using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerChangeNumberManager : MonoBehaviour
{
    [SerializeField] private GameObject timerNumberText;
    [SerializeField] private RectTransform timerPosition;
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negetiveColor;
    
    // Start is called before the first frame update
    void Start()
    {
        subscribeToEvent();
    }

    private void OnEnable()
    {
        subscribeToEvent();
    }

    private void OnDisable()
    {
        unSubscribeToEvent();
    }
    private void OnDestroy()
    {
        unSubscribeToEvent();
    }

    private void subscribeToEvent()
    {
        if (Timer.Instance?.onTimeChangeEvent == null) return;
        Timer.Instance.onTimeChangeEvent.AddListener(instantiatesTimerNumberText);
    }

    private void unSubscribeToEvent()
    {
        if (Timer.Instance.onTimeChangeEvent == null) return;
        Timer.Instance.onTimeChangeEvent.RemoveListener(instantiatesTimerNumberText);
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void instantiatesTimerNumberText(float number)
    {
        if (timerNumberText == null) return;
        GameObject newTimerNumberText = Instantiate(timerNumberText, timerPosition);

        TextMeshProUGUI text  = newTimerNumberText.GetComponentInChildren<TextMeshProUGUI>();

        if (text == null) return;

        if(number > 0)
        {
            text.text = "+" + number;
            text.color = positiveColor;
        }
        else
        {
            text.text = "" + number;
            text.color = negetiveColor;
        }
    }
}
