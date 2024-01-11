using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class Hotbar : MonoBehaviour
{
    #region Animation Variables
    [Header("Hotbar Animation")]
    private Animator animator;

    // Player Animation Sources:
    private WeaponManager weaponManager;
    public string currentAnimation;
    private Dictionary<string, float> allAnimationClipsToLength;
    [SerializeField] private bool autoAnimate;
    protected Func<bool> animationInterupt;
    #endregion
    
    #region Animations

    private string nextAnimation;
    private const string weaponOneIsSelectedString = "WeaponOneSelectedAnimation";
    private const string weaponTwoIsSelectedString = "WeaponTwoSelectedAnimation";
    private const string noWeaponIsSelectedString = "NoWeaponSelectedAnimation";
    private const string oneWeaponIsSelectedString = "OneWeaponSelectedAnimation";

    #endregion

    #region Utility Variables:
    public RectTransform weaponOneSpriteObject;
    public RectTransform weaponTwoSpriteObject;

    private Image weaponOneSprite;
    private Image weaponTwoSprite;

    #endregion

    #region UnityFunctions
    void Start()
    {
        animator = GetComponent<Animator>();
        if (weaponOneSpriteObject != null || weaponTwoSprite != null)
        {
            weaponOneSprite = weaponOneSpriteObject.GetComponent<Image>();
            weaponTwoSprite = weaponTwoSpriteObject.GetComponent<Image>();
        }
        else
        {   
            Debug.LogError("[Hotbar] One of the weapon Sprite Reference is missing in" + name);
        }

        autoAnimate = true;

        if (PlayerInfo.Instance != null)
        {
            weaponManager = PlayerInfo.Instance.transform.GetComponentInChildren<WeaponManager>();
        }
        else
        {   
            Debug.LogError("[playerAnimationHandler] No 'Player' for " + name);
        }

    }
    
    void Update()
    {
        // Hot Bar Animation Booleans:
        if (autoAnimate)
        {
            animateHotBar();
        }

        // Hot Bar Weapon Sprites:
        updateWeaponSlotSprites();
    }

    #endregion

    #region Animation Booleans:
    public bool weaponOneIsSelected()
    {
        return !weaponManager.arsenalEmpty() && weaponManager.currentWeapon == weaponManager.arsenalSlot1;
    }
    public bool weaponTwoIsSelected()
    {
        return !weaponManager.arsenalEmpty() && weaponManager.currentWeapon == weaponManager.arsenalSlot2;
    }
    public bool weaponOneEmpty()
    {
        return weaponManager.arsenalSlot1 == null;
    }
    public bool weaponTwoEmpty()
    {
        return weaponManager.arsenalSlot2 == null;
    }
    public bool oneWeaponIsSelected()
    {
        return weaponOneEmpty() ^ weaponTwoEmpty();
    }
    public bool noWeaponIsSelected()
    {
        return weaponManager.arsenalEmpty();
    }

    #endregion

    #region Animations
    void animateHotBar()
    {
        if (weaponOneIsSelected())
        {
            animator.SetBool("WeaponOneIsSelected", true);
        }
        else
        {
            animator.SetBool("WeaponOneIsSelected", false);
        }

        if (weaponTwoIsSelected())
        {
            animator.SetBool("WeaponTwoIsSelected", true);
        }
        else
        {
            animator.SetBool("WeaponTwoIsSelected", false);
        }

        if (weaponOneEmpty())
        {
            animator.SetBool("WeaponOneEmpty", true);
        }
        else
        {
            animator.SetBool("WeaponOneEmpty", false);
        }

        if (weaponTwoEmpty())
        {
            animator.SetBool("WeaponTwoEmpty", true);
        }
        else
        {
            animator.SetBool("WeaponTwoEmpty", false);
        }
        
        if(oneWeaponIsSelected())
        {
            animator.SetBool("OneWeaponIsSelected", true);
        }
        else
        {
            animator.SetBool("OneWeaponIsSelected", false);
        }

        if(noWeaponIsSelected())
        {
            animator.SetBool("NoWeaponIsSelected", true);
        }
        else
        {
            animator.SetBool("NoWeaponIsSelected", false);
        }
    }
    #endregion



    #region Animation Helper Functions

    private void updateWeaponSlotSprites()
    {
        if (!weaponOneEmpty())
        {
            weaponOneSpriteObject.anchoredPosition = weaponManager.arsenalSlot1.hotBarUI.Position;
            weaponOneSpriteObject.localScale = weaponManager.arsenalSlot1.hotBarUI.Scale;
            weaponOneSpriteObject.localRotation = Quaternion.Euler( weaponManager.arsenalSlot1.hotBarUI.Rotation);

            weaponOneSprite.sprite = weaponManager.arsenalSlot1.getSprite();
            weaponOneSprite.color = new Color(weaponOneSprite.color.r,weaponOneSprite.color.g,weaponOneSprite.color.b,255f);
        }
        else
        {
            weaponOneSprite.sprite = null;
            weaponOneSprite.color = new Color(weaponOneSprite.color.r,weaponOneSprite.color.g,weaponOneSprite.color.b,0f);
        }

        if (!weaponTwoEmpty())
        {
            weaponTwoSpriteObject.anchoredPosition = weaponManager.arsenalSlot2.hotBarUI.Position;
            weaponTwoSpriteObject.localScale = weaponManager.arsenalSlot2.hotBarUI.Scale;
            weaponTwoSpriteObject.localRotation = Quaternion.Euler(weaponManager.arsenalSlot2.hotBarUI.Rotation);

            weaponTwoSprite.sprite = weaponManager.arsenalSlot2.getSprite();
            weaponTwoSprite.color = new Color(weaponOneSprite.color.r,weaponOneSprite.color.g,weaponOneSprite.color.b,255f);
        }
        else
        {
            weaponTwoSprite.sprite = null;
            weaponTwoSprite.color = new Color(weaponOneSprite.color.r,weaponOneSprite.color.g,weaponOneSprite.color.b,0f);
        }
    }
  
    private void changeAnimation(string animation)
    {
        animator.Play(animation);
        currentAnimation = animation;
    }

    private void checkAnimationInterupt()
    {
        if (animationInterupt == null) return;

        if (animationInterupt())
        {
            CancelInvoke("turnOnAnimationSwitchOnFrame");
            turnOnAnimationSwitchOnFrame();
        }
    }

    private void storeAnimationAllLengths()
    {
        AnimationClip[] allAnimationClips = animator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in allAnimationClips)
        {
            allAnimationClipsToLength.Add(clip.name, clip.length);
        }
    }
    public float getAnimationLength(string animation)
    {
        return allAnimationClipsToLength[animation];
    }

    private void playAnimationOnceFull(string animation, Func<bool> interuption = null)
    {
        autoAnimate = false;
        animator.Play(animation);
        currentAnimation = animation;
        animationInterupt = interuption;

        Invoke("turnOnAnimationSwitchOnFrame", getAnimationLength(animation));
    }

    private void playAnimationTillInterupt(string animation, Func<bool> interuption)
    {
        if (interuption == null) return;

        autoAnimate = false;
        animator.Play(animation);
        currentAnimation = animation;
        animationInterupt = interuption;
    }

    private void turnOnAnimationSwitchOnFrame() { autoAnimate = true;}
    #endregion

}