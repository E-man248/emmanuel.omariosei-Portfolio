using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    [SerializeField] private bool caged = true;
    [SerializeField] private int suspicionAmount = 1;
    private Princess princess;

    [SerializeField] private GameObject doll;

    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Sprite closedSprite;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
          spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            unCagePrincess();
        }
    }

    private void Start()
    {
        princess = FindObjectOfType<Princess>();

        if(princess == null) Debug.LogError(name + " Could not be found");

        cagePrincess();
    }

    public void cagePrincess()
    {
        caged = true;
        princess.caged = true;
        princess.transform.GetComponent<TileBasedMovment>().movePoint.position = transform.position;
        princess.transform.GetComponent<Collider2D>().enabled = false;
        princess.transform.position = transform.position;
        spriteRenderer.sprite = closedSprite;

        doll.SetActive(false);
    }

    public void unCagePrincess()    
    {
        caged = false;
        princess.caged = false;
        princess.transform.GetComponent<Collider2D>().enabled = true;

        SuspicionMeter.instance.addSuspicionAmount(suspicionAmount);
        spriteRenderer.sprite = openedSprite;

        doll.SetActive(true);
    }
}
