using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    public UnityEvent activeEvent;
    public UnityEvent inactiveEvent;

    [field: SerializeField] public bool plateState { get; private set; }

    private SpriteRenderer spriteRenderer;

    public List<string> intaractableTags;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo == null) return;
        if(!intaractableTags.Contains(hitInfo.tag)) return;

        setActive();
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo == null) return;
        if (!intaractableTags.Contains(hitInfo.tag)) return;

        setInactive();
    }
    
    private void setActive()
    {
        spriteRenderer.sprite = activeSprite;
        activeEvent.Invoke();

        plateState = true;
    }

    private void setInactive()
    {
        spriteRenderer.sprite = inactiveSprite;
        inactiveEvent.Invoke();

        plateState = false;
    }
}
