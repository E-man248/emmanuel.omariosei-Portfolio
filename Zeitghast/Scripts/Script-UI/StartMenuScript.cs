using UnityEngine;
using Tymski;


public class StartMenuScript : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueButton;

    [Header("Scene Transition")]
    public SceneReference ContinueGameScene;
    public SceneReference NewGameScene;
    public SceneReference CreditsScene;

    public TransitionType TransitionIn;
    public TransitionType TransitionOut;

    [SerializeField] private SceneReference TutorialCompleteCheck;

    private void Awake()
    {
        if (newGameButton == null) Debug.LogError("newGameButton not set");
        if (continueButton == null) Debug.LogError("continueButton not set");

        if(SaveIsNew())
        {
            ShowNewGameButton();
        }
        else
        {
            ShowContinueButton();
        }
    }


    public void loadContinueGameScene()
    {
        AdvancedSceneManager.Instance.inSceneTransition(TransitionIn, TransitionType.LoadingScreen);
        AdvancedSceneManager.Instance.loadScene(ContinueGameScene.name, TransitionIn, TransitionOut);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void loadNewGameScene()
    {
        AdvancedSceneManager.Instance.inSceneTransition(TransitionIn, TransitionType.LoadingScreen);
        AdvancedSceneManager.Instance.loadScene(NewGameScene.name, TransitionIn, TransitionOut);
    }

    public void wipeSave()
    {
        DataPersistanceManager.Instance.ResetGameData();
    }

    public void openCredits()
    {
        AdvancedSceneManager.Instance.inSceneTransition(TransitionIn, TransitionType.LoadingScreen);
        AdvancedSceneManager.Instance.loadScene(CreditsScene.name, TransitionIn, TransitionOut);
    }

    public void ShowNewGameButton()
    {
        newGameButton.SetActive(true);
        continueButton.SetActive(false);
    }

    public void ShowContinueButton()
    {
        continueButton.SetActive(true);
        newGameButton.SetActive(false);
    }

    public bool SaveIsNew()
    {
        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(TutorialCompleteCheck.name))
        {
            Debug.LogError("Given Level Scene Name is Invalid for Level Complete Check!");

            return false;
        }

        LevelData levelData = GameManager.Instance.GetLevelData(TutorialCompleteCheck.name);
        return !levelData.LevelComplete;
    }
}
