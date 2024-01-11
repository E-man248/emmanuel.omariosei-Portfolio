using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImageManager : MonoBehaviour
{
    public static BackgroundImageManager Instance = null;
    private SpriteRenderer BackgroundImageSpriteRenderer;

    private void Awake()
    {
        //Singelton Checking
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        BackgroundImageSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBackgroundImage(Sprite newSprite)
    {
        BackgroundImageSpriteRenderer.sprite = newSprite;
    }

    public Sprite currentBackGroundSprite()
    {
        return BackgroundImageSpriteRenderer.sprite;
    }
}
