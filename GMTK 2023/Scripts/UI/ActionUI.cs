using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer northSprite;
    [SerializeField] private SpriteRenderer eastSprite;
    [SerializeField] private SpriteRenderer southSprite;
    [SerializeField] private SpriteRenderer westSprite;
    [SerializeField] private Sprite testSprite;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void changeUpSprite(Sprite changeSprite)
    {
        northSprite.sprite = changeSprite;
    }

    public void changeDownSprite(Sprite changeSprite)
    {
        southSprite.sprite = changeSprite;
    }

    public void changeLeftSprite(Sprite changeSprite)
    {
        westSprite.sprite = changeSprite;
    }

    public void changeRightSprite(Sprite changeSprite)
    {
        eastSprite.sprite = changeSprite;
    }

}
