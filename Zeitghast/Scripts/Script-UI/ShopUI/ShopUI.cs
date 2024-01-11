using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    [HideInInspector] public Shop currentShop;
    public List<WeaponCostPair> weaponsList;
    public bool shopOpened;

    [Header("Weapon Information")]
    [SerializeField] TMPro.TextMeshProUGUI WeaponNameHolder = null;
    [SerializeField] TMPro.TextMeshProUGUI WeaponDiscriptionHolder = null;
    [SerializeField] TMPro.TextMeshProUGUI WeaponCostHolder = null;

    [Header("Scroll Section")]
    public float spacing;
    public float scrollSpeed = 1f;
    public float keyPressScrollSpeed = 0.3f;
    private float rightKeyPressTimer;
    private float leftKeyPressTimer;
    public Transform listContainer;
    private List<GameObject> containerPool;

    [Space]
    [SerializeField] private int selectedWeaponIndex;
    private float boxLength;

    [Header("Scroll Section Scaling")]
    public float maxScale = 2f;
    public float minScale = 1f;
    public float scaleDistance = 2f;

    [Header("Purchase Text Message")]
    public Color purchaseMessageColor;
    [TextArea] public List<string> purchaseMessagePool;
    private string purchaseMessage = "Thank You for Your Purchase!"; // Default Message if Pool Empty!
    private bool purchaseMessageDisplaying = false;

    [Header("Sold Out Text Message")]
    public Color soldOutMessageColor;
    public List<string> soldOutMessagePool;
    private string soldOutMessage = "Sold Out!"; // Default Message if Pool Empty!

    [Header("Error Text Message")]
    public float errorMessageDuration = 2f;
    public Color errorMessageColor;
    public List<string> errorMessagePool;
    private string errorMessage = "Invalid Transaction!"; // Default Message if Pool Empty!
    private bool errorMessageDisplaying = false;
    private float errorMessageTimer;

    [Header("Sound")]
    [EventRef] public string openSound = null;
    [EventRef] public string scrollSound = null;
    [EventRef] public string closeButtonSound = null;
    [EventRef] public string buySound = null;
    [EventRef] public string cantAffordSoundSound = null;

    // Animation:
    private Animator animator;

    [Header("Depth Of Field")]
    public float DepthOfFieldSpeed = 5f;

    // Utilities:
    private long playerPauseDisableKey = 0L;

    void Awake()
    {
        containerPool = new List<GameObject>();

        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        errorMessageTimer = 0f;
        errorMessageDisplaying = false;
        purchaseMessageDisplaying = false;
    }

    private void Update()
    {
        if (shopOpened)
        {
            if (weaponsList.Count <= 0) return;

            // Weapon Scroll:
            float xPos = -1f * (boxLength*spacing) * selectedWeaponIndex;
            Vector3 scrollPosition = new Vector3(xPos, 0f, 0f);
            listContainer.localPosition = Vector3.MoveTowards(listContainer.localPosition, scrollPosition, scrollSpeed * 1000f * Time.unscaledDeltaTime);

            playerInput();

            if (!purchaseMessageDisplaying)
            {   
                if (errorMessageTimer > 0f)
                {
                    errorMessageTimer -= Time.unscaledDeltaTime;
                }
                else if (errorMessageDisplaying)
                {
                    updateWeaponInformation();
                }
            }
        }

        animationCheck();
    }

    public void openShop(Shop shop)
    {
        shopOpened = true;
        playerPauseDisableKey = PauseMenuUI.Instance.disablePlayerPause();
        Timer.PauseGame();

        weaponsList = shop.weaponsList;
        currentShop = shop;
        selectedWeaponIndex = 0;

        //Shop Open Sound
        if (!string.IsNullOrEmpty(openSound))
        {
            RuntimeManager.PlayOneShot(openSound, transform.position);
        }

        for (int i = 0; i < weaponsList.Count; i++)
        {
            // GameObject Set Up:
            GameObject weaponSprite = ShopUIContainerPool.instance.GetFromPool();
            weaponSprite.transform.localScale = Vector3.one;
            weaponSprite.transform.SetParent(listContainer);
            weaponSprite.name = "Weapon Sprite " + i;

            // Initial Position Math:
            RectTransform weaponSpriteRectTransform = (RectTransform)weaponSprite.transform;

            int numberOfBoxes = weaponsList.Count;

            boxLength = weaponSpriteRectTransform.rect.width;

            float xPos = (i * boxLength * spacing);
            Vector3 positionOfBox = new Vector3(xPos, 0f, 0f);

            weaponSpriteRectTransform.localPosition = positionOfBox;

            // ShopUIWeaponSprite Set Up:
            ShopUIWeaponSprite weaponSpriteScript = weaponSprite.GetComponent<ShopUIWeaponSprite>();
            if (weaponSpriteScript != null)
            {
                weaponSpriteScript.listIndex = i;
                weaponSpriteScript.listCenter = transform;
                
                weaponSpriteScript.maxScale = maxScale;
                weaponSpriteScript.minScale = minScale;
                weaponSpriteScript.scaleDistance = scaleDistance;

                weaponSpriteScript.sold = false;

                Weapon weaponScript = weaponsList[i].weapon.GetComponent<Weapon>();
                if (weaponScript != null)
                {
                    weaponSpriteScript.weaponScript = weaponScript;
                }
            }

            containerPool.Add(weaponSprite);
        }

        //Update the text of the UI
        updateWeaponInformation();
    }
 
    public void addSelectedWeaponCheckout()
    {
        ShopUIWeaponSprite weaponSpriteScript = containerPool[selectedWeaponIndex].GetComponent<ShopUIWeaponSprite>();
        if (weaponSpriteScript.sold == true) return;

        float currentTime = Timer.Instance.GetCurrentTime();

        // Verification of Purchase:
        if (currentTime >= weaponsList[selectedWeaponIndex].cost)
        {
            // Purchase:
            weaponSpriteScript.sold = true;
            Timer.Instance.ChangeTime(-weaponsList[selectedWeaponIndex].cost);
            currentShop.addToCheckout(selectedWeaponIndex);
            displayPurchaseMessage();

            //Buying Sound
            if (!string.IsNullOrEmpty(buySound))
            {
                RuntimeManager.PlayOneShot(buySound, transform.position);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(cantAffordSoundSound))
            {
                RuntimeManager.PlayOneShot(cantAffordSoundSound, transform.position);
            }
            // Error:
            displayErrorMessage();
        }
    }

    public void closeShop()
    {
        PauseMenuUI.Instance.enablePlayerPause(playerPauseDisableKey);
        Timer.UnpauseGame();

        if (!string.IsNullOrEmpty(closeButtonSound))
        {
            RuntimeManager.PlayOneShot(closeButtonSound, transform.position);
        }

        foreach (GameObject element in containerPool)
        {
            ShopUIContainerPool.instance.AddToPool(element);
        }
        containerPool.Clear();

        currentShop.dispenseCheckOut();

        shopOpened = false;
    }

    public void scrollLeft()
    {
        if (!shopOpened) return;

        if (!string.IsNullOrEmpty(scrollSound))
        {
            RuntimeManager.PlayOneShot(scrollSound, transform.position);
        }

        if (selectedWeaponIndex <= 0)
        {
            selectedWeaponIndex = weaponsList.Count-1;
        }
        else
        {
            selectedWeaponIndex--;
        }

        //Update the text of the UI
        updateWeaponInformation();
    }

    public void scrollRight()
    {
        if (!shopOpened) return;

        if (!string.IsNullOrEmpty(scrollSound))
        {
            RuntimeManager.PlayOneShot(scrollSound, transform.position);
        }

        if (selectedWeaponIndex >= weaponsList.Count-1)
        {
            selectedWeaponIndex = 0;
        }
        else
        {
            selectedWeaponIndex++;
        }

        //Update the text of the UI
        updateWeaponInformation();
    }
    public void scrollToIndex(int index)
    {
        if (!shopOpened || index > weaponsList.Count-1 || index < 0) return;

        selectedWeaponIndex = index;

        //Update the text of the UI
        updateWeaponInformation();
    }

    private void playerInput()
    {
        //Buying 
        if (Input.GetButtonDown("Submit"))
        {
            addSelectedWeaponCheckout();
        }

        if (Input.GetButtonDown("Cancel") && shopOpened)
        {
            ShopUI.Instance.closeShop();
        }
        //Left and right Scrolling
        if (Input.GetAxisRaw("Horizontal") > 0 && rightKeyPressTimer >= keyPressScrollSpeed)
        {
            scrollRight();
            rightKeyPressTimer = 0;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && leftKeyPressTimer >= keyPressScrollSpeed)
        {
            scrollLeft();
            leftKeyPressTimer = 0;
        }

        leftKeyPressTimer += Time.unscaledDeltaTime;
        rightKeyPressTimer += Time.unscaledDeltaTime;
    }

    public void updateWeaponInformation()
    {
        errorMessageDisplaying = false;
        purchaseMessageDisplaying = false;

        // Check if Sold Out:
        ShopUIWeaponSprite weaponSpriteScript = containerPool[selectedWeaponIndex].GetComponent<ShopUIWeaponSprite>();
        if (weaponSpriteScript.sold == true)
        {
            if (soldOutMessagePool.Count > 0)
            {
                soldOutMessage = soldOutMessagePool[Random.Range(0, soldOutMessagePool.Count)];
            }

            // Font:
            WeaponDiscriptionHolder.fontStyle = TMPro.FontStyles.Bold;
            WeaponDiscriptionHolder.color = soldOutMessageColor;

            // Update the text of the UI:
            WeaponNameHolder.text = weaponSpriteScript.weaponScript.weaponDisplayName;
            WeaponDiscriptionHolder.text = soldOutMessage;
            WeaponCostHolder.text = "(Sold Out)";
        }
        else
        {
            // Font:
            WeaponDiscriptionHolder.fontStyle = TMPro.FontStyles.Normal;
            WeaponDiscriptionHolder.color = Color.white;

            // Update the text of the UI:
            WeaponNameHolder.text = weaponsList[selectedWeaponIndex].weapon.GetComponent<Weapon>().weaponDisplayName;
            WeaponDiscriptionHolder.text = "" + weaponsList[selectedWeaponIndex].weapon.GetComponent<Weapon>().weaponDescription;
            WeaponCostHolder.text = "(-" + weaponsList[selectedWeaponIndex].cost + " Sec)";
        }
    }
    
    public void displayPurchaseMessage()
    {
        if (purchaseMessageDisplaying) return;
        
        purchaseMessageDisplaying = true;
        
        // Font:
        WeaponDiscriptionHolder.fontStyle = TMPro.FontStyles.Bold;
        WeaponDiscriptionHolder.color = purchaseMessageColor;

        // Randomize Purchase Message:
        if (purchaseMessagePool.Count > 0) purchaseMessage = purchaseMessagePool[Random.Range(0, purchaseMessagePool.Count)];

        // Display Purchase Message:
        WeaponNameHolder.text = "";
        WeaponDiscriptionHolder.text = purchaseMessage;
    }

    public void displayErrorMessage()
    {
        if (errorMessageDisplaying) return;
        
        errorMessageDisplaying = true;
        
        // Font:
        WeaponDiscriptionHolder.fontStyle = TMPro.FontStyles.Bold;
        WeaponDiscriptionHolder.color = errorMessageColor;

        // Randomize Error Message:
        if (errorMessagePool.Count > 0) errorMessage = errorMessagePool[Random.Range(0, errorMessagePool.Count)];

        // Display Error Message:
        WeaponNameHolder.text = "";
        WeaponDiscriptionHolder.text = errorMessage;
        errorMessageTimer = errorMessageDuration;
    }

    private void animationCheck()
    {
        animator.SetBool("shopOpened", shopOpened);
    }

}
