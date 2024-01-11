using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAndFade : MonoBehaviour
{
    public Vector3 finalScaleSize = Vector3.one;
    public float scaleDuration = 0.5f;
    public float fadeTransparency = 0f;
    public float fadeDuration = 0.5f;
    private bool initiated = false;

    // Shink and Fade Calculation Values:
    private float timeAfterInitiation = 0f;
    private SpriteRenderer spriteRenderer;
    private Vector3 initialScale = Vector3.one;
    private float initialTransparency = 0f;
    
    [SerializeField] protected bool initateOnStart = false;

    public void Start()
    {
        if (initateOnStart) Initiate();
    }

    public void Initiate()
    {
        timeAfterInitiation = 0f;
        
        // Shrink Setup:
        initialScale = transform.localScale;

        // Fade Setup:
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialTransparency = spriteRenderer.color.a;
        }

        initiated = true;
    }

    private void Update()
    {
        if (initiated)
        {
            // Shrink!
            Vector3 currentScale = Vector3.Lerp(initialScale, finalScaleSize, timeAfterInitiation / scaleDuration);
            transform.localScale = currentScale;

            // Fade!
            if (spriteRenderer != null)
            {
                float currentTransparency = Mathf.Lerp(initialTransparency, fadeTransparency, timeAfterInitiation / fadeDuration);
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, currentTransparency);
            }
            
            timeAfterInitiation += Time.deltaTime;
        }
    }
}
