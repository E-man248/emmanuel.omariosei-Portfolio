using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HealthTextmeshProDisplay : MonoBehaviour
{
    public Health health;
    public TextMeshPro textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponentInChildren<Health>();
        textMeshPro = GetComponentInChildren<TextMeshPro>();
    }


    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = "" + health.health;
    }
}
