using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteFlasher : MonoBehaviour
{
    [Range(0,1)]public float FlashIntensity;
    public float flashDuration = 1f;
    private EnemyHealth enemyHealth;
    private SpriteRenderer[] sprites;

    private bool isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        subscribeToEvents();
    }

    private void OnEnable()
    {
        subscribeToEvents();
    }

    private void OnDisable()
    {
        unSubscribeToEvents();
    }

    private void OnDestroy()
    {
        unSubscribeToEvents();
    }

    private void subscribeToEvents()
    {
        if (enemyHealth == null) return;

        enemyHealth.onDamageTaken.AddListener(setUpAndStartFlashing);
    }

    private void unSubscribeToEvents()
    {
        if (enemyHealth == null) return;
        enemyHealth.onDamageTaken.RemoveListener(setUpAndStartFlashing);
    }

    IEnumerator startFlashing(float duration)
    {
        float elapsedTime = 0f;
        isFlashing = true;

        while (elapsedTime < duration)
        {
            float colorValue = Mathf.Lerp(0f, 1f, elapsedTime/duration);

            for (int i = 0; i < sprites.Length; i++)
            {
                Color newColor = new Color(sprites[i].color.r, colorValue, colorValue);
                sprites[i].color = newColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        resetSprites();
    }

    private void setUpAndStartFlashing()
    {
        if (isFlashing) return;

        StartCoroutine(startFlashing(flashDuration));
    }

    private void resetSprites()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            Color newColor = new Color(sprites[i].color.r, 1f, 1f);
            sprites[i].color = newColor;
        }

        isFlashing = false;
    }
}
