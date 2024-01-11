using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIWeaponSprite : MonoBehaviour
{
    public float maxScale = 2f;
    public float minScale = 1f;
    public float scaleDistance = 2f;
    public int listIndex = -1;
    [HideInInspector] public Transform listCenter;
    [HideInInspector] public Weapon weaponScript;
    private Image weaponSprite;
    /*[HideInInspector]*/ public bool sold = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponSprite = GetComponentInChildren<Image>();
    }

    void Update()
    {
        if (listIndex == -1 || !ShopUI.Instance.shopOpened) return;

        // Animation:
        animator.SetBool("Sold", sold);

        // Math for Dynamic Scaling:
        
        float xDistToSelectedWeapon = listCenter.position.x - transform.position.x;
        float scaleValue = Mathf.Max(maxScale - Mathf.Abs(xDistToSelectedWeapon / scaleDistance), minScale);

        transform.localScale = new Vector3(scaleValue, scaleValue, transform.localScale.z);

        if (weaponScript != null)
        { 
            weaponSprite.sprite = weaponScript.getSprite();

            RectTransform graphicsTransform = (RectTransform) weaponSprite.gameObject.transform;
            graphicsTransform.localPosition = weaponScript.shopUI.Position;
            graphicsTransform.localRotation = Quaternion.Euler(weaponScript.shopUI.Rotation);
            graphicsTransform.localScale = weaponScript.shopUI.Scale;
        }
    }

    public void scrollToIndex()
    {
        if (ShopUI.Instance.shopOpened)
        {
            ShopUI.Instance.scrollToIndex(listIndex);
        }
    }
}
