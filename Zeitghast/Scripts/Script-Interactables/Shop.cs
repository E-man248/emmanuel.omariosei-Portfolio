using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct WeaponCostPair
{
    public GameObject weapon;
    public int cost;
}


public class Shop : Interactable
{
    public List<WeaponCostPair> weaponsList;
    public bool shopIsEmpty;
    private Animator animator;
    private List<int> checkoutIndexList;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        checkoutIndexList = new List<int>();

        //Check to Upadate Weapon list from save?
        CheckIfShopIsEmpty();
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    protected virtual void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoaded();
    }
 
    private void SceneLoaded()
    {
        if (AdvancedSceneManager.Instance.shopDictionary.ContainsKey(name))
        {
            weaponsList = AdvancedSceneManager.Instance.shopDictionary[name];
        }
    }

    private void LoadingScreenAction()
    {
        addShopInevetoryToSceneManagerDictionary();
    }

    private void addShopInevetoryToSceneManagerDictionary()
    {
        if (!AdvancedSceneManager.Instance.shopDictionary.ContainsKey(name))
        {
            AdvancedSceneManager.Instance.shopDictionary.Add(name, weaponsList);
        }
        else
        {
            AdvancedSceneManager.Instance.shopDictionary[name] = weaponsList;
        }
    }

    protected override void Update()
    {
        base.Update();
        animator.SetBool("playerIsInRangeOfShop", enteredCollider);
        animator.SetBool("ShopIsOpened" , ShopUI.Instance.shopOpened);
       
        CheckIfShopIsEmpty();
        animator.SetBool("ShopIsEmpty", shopIsEmpty);
    }

    public void dispenseCheckOut()
    {
        List<WeaponCostPair> listOfWeaponCostPairToDestroy = new List<WeaponCostPair>();
        // Dispense Weapons Needed and Get Weapons to Remove:
        for (int i = 0; i < checkoutIndexList.Count; i++)
        {
            int index = checkoutIndexList[i];
            dispenseWeapon(index);
            listOfWeaponCostPairToDestroy.Add(weaponsList[index]);
        }
        
        // Remove Weapons:
        foreach (WeaponCostPair weaponCostPair in listOfWeaponCostPairToDestroy)
        {
            weaponsList.Remove(weaponCostPair);
        }
        checkoutIndexList.Clear();
    }

    public void addToCheckout(int weaponIndex)
    {
        checkoutIndexList.Add(weaponIndex);
    }

    private void dispenseWeapon(int weaponIndex)
    {
        if(weaponsList.Count == 0)
        {
            return;
        }
        GameObject weapon = Instantiate(weaponsList[weaponIndex].weapon, transform.position, transform.rotation);
        weapon.name = weaponsList[weaponIndex].weapon.name;
    }

    private void openShop()
    {
        if (!ShopUI.Instance.shopOpened)
        {
            ShopUI.Instance.openShop(this);
        }
    }

    protected override void interactAction()
    {
        if (!shopIsEmpty)
        {
            openShop();
        }
    }

    public void CheckIfShopIsEmpty()
    { 
        if(weaponsList.Count <= 0)
        {
            shopIsEmpty = true;
        }
        else
        {
            shopIsEmpty = false;
        }
    }
}
