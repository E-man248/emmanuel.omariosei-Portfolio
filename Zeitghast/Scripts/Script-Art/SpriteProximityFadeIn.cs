using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteProximityFadeIn : MonoBehaviour
{
    [SerializeField] private float fadeInTriggerRadius = 5;
    [SerializeField] private float fadeInMaxDistance = 2;
    [Space]
    [SerializeField] private float maxfadeOutAmount = 0f;
    [SerializeField] private float maxfadeInAmount = 1f;
    [Space]
    [SerializeField] private float fadeSpeed = 6f;

    private Transform playerPosition;
    private SpriteRenderer spriteRenderer;
    private float distance;
    private float distanceNormalized;
    private float newDistance;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        resetAlpha();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerPosition = PlayerInfo.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        fadeInOnProximity();
    }

    void fadeInOnProximity()
    {
        Vector2 fadeInMaxVector = fadeInMaxDistance * (playerPosition.position - transform.position).normalized;

        Vector2 newTransform = new Vector2 (transform.position.x + fadeInMaxVector.x, transform.position.y);
        distance = Mathf.Abs(playerPosition.position.x - newTransform.x);

        newDistance = distance + fadeInMaxVector.x;
        if (newDistance > fadeInTriggerRadius)
        {
            //Reset Back to Zero
            spriteRenderer.color = setAlpha(spriteRenderer.color, Mathf.Lerp(spriteRenderer.color.a, 0f, fadeSpeed * Time.deltaTime));
            return;
        }


        distanceNormalized = 1 - (distance / fadeInTriggerRadius);
        float alphaValue = Mathf.Clamp(distanceNormalized, maxfadeOutAmount, maxfadeInAmount);

        // Smoothly transition the alpha value
        spriteRenderer.color = setAlpha(spriteRenderer.color, Mathf.Lerp(spriteRenderer.color.a, alphaValue, fadeSpeed * Time.deltaTime));
    }

    void resetAlpha()
    {
        if (spriteRenderer.color.a == 0) return;

        spriteRenderer.color = setAlpha(spriteRenderer.color, 0f);
    }

    /// <summary>
    /// Returns a color with the set alpha
    /// </summary>
    private Color setAlpha(Color originalColor, float newColor)
    {
        return new Color(originalColor.r, originalColor.g, originalColor.b, newColor);
    }



    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, fadeInTriggerRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fadeInMaxDistance);
    }


}
