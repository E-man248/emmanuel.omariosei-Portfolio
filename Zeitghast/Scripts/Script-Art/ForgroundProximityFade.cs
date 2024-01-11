using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ForgroundProximityFade : MonoBehaviour
{
    
    [SerializeField] private float fadeTriggerRadius = 5;
    [SerializeField] private float maxfadeAmount = 0.2f;

    private Transform playerPosition;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = PlayerInfo.Instance.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        fadeOnProximity();
    }

    void fadeOnProximity()
    {
        float distance = Vector3.Distance(playerPosition.position, transform.position);
       
        if (distance > fadeTriggerRadius)
        {
            resetAlpha();
            return;
        }


        float distanceNormalized = distance / fadeTriggerRadius;
        float alphaValue = Mathf.Clamp(distanceNormalized, maxfadeAmount, 1f);

        spriteRenderer.color = setAlpha(spriteRenderer.color, alphaValue);
    }

    void resetAlpha()
    {
        if (spriteRenderer.color.a == 1) return;

        spriteRenderer.color = setAlpha(spriteRenderer.color,1f);
    } 
    
    /// <summary>
    /// Returns a color with the set alpha
    /// </summary>
    private Color setAlpha(Color originalColor,float newColor)
    {
        return new Color(originalColor.r, originalColor.g, originalColor.b, newColor);
    }



    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, fadeTriggerRadius);
    }

    
}
