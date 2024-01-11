using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINumberManager : MonoBehaviour
{
    public static UINumberManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public GameObject healingNumber; 
    public GameObject damageNumber; 
    public GameObject hurtNumber; 
    // Start is called before the first frame update
    void Start()
    {
        if(healingNumber == null)
        {
            Debug.LogError("[UINumberManager] has no healingNumber Object");
        }

        if (damageNumber == null)
        {
            Debug.LogError("[UINumberManager] has no damageNumber Object");
        }

        if (hurtNumber == null)
        {
            Debug.LogError("[UINumberManager] has no hurtNumber Object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void createHealNumber(int value , Transform entityposition)
    {
        Transform healNumberClone = Instantiate(healingNumber.transform, entityposition.position, Quaternion.identity);
        TextMeshPro text = healNumberClone.GetComponent<TextMeshPro>();
        text.SetText(value.ToString());
    }
    public void createDamageNumber(int value, Transform entityposition)
    {
        Transform damageNumberClone = Instantiate(damageNumber.transform, entityposition.position, Quaternion.identity);
        TextMeshPro text = damageNumberClone.GetComponent<TextMeshPro>();
        text.SetText(Mathf.Abs(value).ToString());
    }

    public void createHurtNumber(int value, Transform entityposition)
    {
        Transform hurtNumberClone = Instantiate(hurtNumber.transform, entityposition.position, Quaternion.identity);
        TextMeshPro text = hurtNumberClone.GetComponent<TextMeshPro>();
        text.SetText(Mathf.Abs(value).ToString());
    }
}
