using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int Maxhealth;
    public int currentHealth { get; private set; }

    public UnityEvent deathEvent;
    public float dealthLagTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = Maxhealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {
            death();
        }
    }

    public void changeHealth(int value)
    {
        currentHealth += value;
    }

    private void death()
    {
        deathEvent.Invoke();
        Destroy(gameObject, dealthLagTime);
    }

}
