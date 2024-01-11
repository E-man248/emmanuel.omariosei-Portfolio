using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Sprite closedSprite;

    [field: SerializeField] public bool gateOpened { get; private set; }
    private SpriteRenderer spriteRenderer;
    [SerializeField]  private GameObject gateCollider;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if(gateOpened)
        {
            openGate();
        }
        else
        {
            closegate();
        }
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public void openGate()
    {
        spriteRenderer.sprite = openedSprite;
        gateOpened = false;
        gateCollider.SetActive(false);
    }

    public void closegate()
    {
        spriteRenderer.sprite = closedSprite;
        gateOpened = true;
        gateCollider.SetActive(true);
    }
}
