using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    //Parallax
    private Vector3 startPosition;
    private GameObject mainCamera;
    [SerializeField] private Vector2 parallaxEffect;
    [SerializeField] private bool horizontalTiling = true;
    [SerializeField] private bool verticalTiling = false;
    private SpriteRenderer spriteRenderer;


    //Horizontal and Vertical Scrolling
    private Vector3 length;
    

    void Start()
    {
        //Parallax
        mainCamera = GameObject.Find("Main Camera");
        startPosition = transform.position;

        //Horizontal and Vertical Scrolling
        length = GetComponent<SpriteRenderer>().bounds.size;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;

        if (horizontalTiling)
        {
            spriteRenderer.size = new Vector2(spriteRenderer.size.x * 3f, spriteRenderer.size.y);
        }

        if (verticalTiling)
        {
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, spriteRenderer.size.y * 3f);
        }
    }

    private void FixedUpdate()
    {
        //Parallax
        float currentPositionX = (mainCamera.transform.position.x * (1 - parallaxEffect.x));
        float currentPositionY = (mainCamera.transform.position.y * (1 - parallaxEffect.y));

        //Horizontal and Vertical Scrolling

        Vector2 distance = new Vector2(mainCamera.transform.position.x * parallaxEffect.x, mainCamera.transform.position.y * parallaxEffect.y);
        transform.position = new Vector3(startPosition.x + distance.x, startPosition.y + distance.y, transform.position.z);

        if (horizontalTiling)
        {
            if (currentPositionX > startPosition.x + length.x)
            {
                startPosition.x += length.x;
            }
            else if (currentPositionX < startPosition.x - length.x)
            {
                startPosition.x -= length.x;
            }
        }

        if (verticalTiling)
        {
            if (currentPositionY > startPosition.y + length.y)
            {
                startPosition.y += length.y;
            }
            else if (currentPositionY < startPosition.y - length.y)
            {
                startPosition.y -= length.y;
            }
        }
    }
}
