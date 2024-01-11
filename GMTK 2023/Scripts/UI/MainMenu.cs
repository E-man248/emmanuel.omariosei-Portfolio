using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneReference startScene;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private GameObject levelSelectionMenu;

    // Start is called before the first frame update
    public void onStartButton()
    {
        DisableButtons();
        SceneManager.LoadSceneAsync(startScene);
    }

    public void onLevelSelectButton()
    {
        levelSelectionMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void DisableButtons()
    {
        newGameButton.interactable = false;
        levelSelectButton.interactable = false;
    }
}
