using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Numbers : MonoBehaviour
{
    private TextMeshPro text;
    public float lifeTime = 1.5f;
    public float verticalMoveSpeed =5f;
    public float scaleFactor =0.001f;
    private float lifetimeCounter;

    private Color textColor;


    void Start()
    {
        text = GetComponent<TextMeshPro>();
        Invoke("destruct", lifeTime);
        lifetimeCounter = lifeTime;
    }

    void Update()
    {
        if(!Timer.gamePaused)
        {
            transform.position += new Vector3(0, verticalMoveSpeed) * Time.deltaTime;
            if (lifetimeCounter > 0)
            {
                textColor = text.color;
                textColor.a = lifetimeCounter / lifeTime;
                text.color = textColor;
            }

            if (lifetimeCounter > lifeTime / 4)
            {
                transform.localScale += Vector3.one * scaleFactor;
            }
            else
            {
                transform.localScale -= Vector3.one * scaleFactor;
            }
            lifetimeCounter -= Time.deltaTime;
        }
    }

    public void setValue(int number)
    {
        text.SetText(number.ToString());
    }
    public void destruct()
    {
        Destroy(gameObject);
    }
}
