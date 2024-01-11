using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageSprite : MonoBehaviour
{
    [SerializeField] private float imageDuration = 0.1f;
    private float imageTimer;
    [SerializeField] private float initialAlpha = 255f;
    [SerializeField] private float alphaMultiplier = 0.85f;
    private float currentAlpha;
    private Color currentColor;

    [SerializeField] private Transform Player;
    private SpriteRenderer playerSpriteRenderer;
    private playerAnimationHandler playerAnimationHandler;
    private SpriteRenderer spriteRenderer;
    
    // Weapon In After-Image:
    /*private SpriteRenderer currentWeaponSpriteRenderer;
    private SpriteRenderer weaponImageRenderer;
    private GameObject weaponImageObject;
    private Transform currentWeaponTransform;
    private WeaponManager playerWeaponManager;*/

    private void OnEnable()
    {
        if (Player == null)
        {
            Player = PlayerInfo.Instance.transform;
            if (Player == null) Debug.LogError("No 'Player' Transform in 'AfterImageSprite' Script for " + name);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimationHandler = Player.GetComponentInChildren<playerAnimationHandler>();
        playerSpriteRenderer = playerAnimationHandler.playerSpriteRenderer;

        // Match Player and Set Up Sprite:
        if (playerAnimationHandler.isInvincible())
        {
            currentAlpha = playerAnimationHandler.invincibilityTransparency;
        }
        else
        {
            currentAlpha = initialAlpha;
        }
        
        imageTimer = imageDuration;

        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        transform.position = playerAnimationHandler.transform.position;
        transform.rotation = playerAnimationHandler.transform.rotation;

        // Set Up Player Weapon Sprite:
        /*if (weaponImageRenderer == null && Player.GetComponentInChildren<WeaponManager>().currentWeapon != null) 
        {
            weaponImageObject = new GameObject("weaponImage");
            weaponImageRenderer = weaponImageObject.AddComponent<SpriteRenderer>();
        }
        if (weaponImageRenderer != null)
        {
            playerWeaponManager = Player.GetComponentInChildren<WeaponManager>();
            currentWeaponTransform = playerWeaponManager.currentWeapon.transform;
            currentWeaponSpriteRenderer = playerWeaponManager.currentWeapon.GetComponentInChildren<SpriteRenderer>();
            
            weaponImageRenderer.sprite = currentWeaponSpriteRenderer.sprite;
            weaponImageRenderer.flipX = currentWeaponSpriteRenderer.flipX; 
            weaponImageObject.transform.position = currentWeaponTransform.position;
            weaponImageObject.transform.rotation = currentWeaponTransform.rotation;
            weaponImageObject.transform.parent = transform;  
        }*/
    }

    private void FixedUpdate()
    {
        currentAlpha *= alphaMultiplier;
        currentColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, currentAlpha);
        spriteRenderer.color = currentColor;
        //if (weaponImageRenderer != null) weaponImageRenderer.color = currentColor;

        if (imageTimer < 0f)
        {
            AfterImagePool.instance.AddToPool(gameObject);
        }
        imageTimer -= Time.deltaTime;
    }
    
}
