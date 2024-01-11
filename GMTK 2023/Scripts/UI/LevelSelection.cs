using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Tymski;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private SceneReference[] listOfScenes;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject MainMenu;
    
    public void onBackButton()
    {
        MainMenu.SetActive(true);  
        this.gameObject.SetActive(false);
    }

    public void onSelectedLevel(int arrayElements)
    {
        SceneReference selectedScene = listOfScenes[arrayElements];
        DisableButtons();
        SceneManager.LoadSceneAsync(selectedScene);
    }


    public void DisableButtons()
    {
        for(int x = 0; x > levelButtons.Length; x++)
        {
            levelButtons[x].interactable = false;
        }
        backButton.interactable = false;
    }
}
