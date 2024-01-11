using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImageChanger : MonoBehaviour
{
    public Sprite backgroundImage;
    // Start is called before the first frame update
    void Start()
    {
        if(backgroundImage == null)
        {
            Debug.LogError(name + " does not have a background image");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it's already changed don't cahnge the image
        if(BackgroundImageManager.Instance.currentBackGroundSprite() == backgroundImage)
        {
            return;
        }

        //triggering the background change
        if (collision != null && collision.tag == "Player")
        {
            BackgroundImageManager.Instance.ChangeBackgroundImage(backgroundImage);
        }
    }
}
