using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tymski;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    None,
    FadeIn,
    FadeOut,
    CircleCutIn,
    CircleCutOut,
    SkullCutIn,
    SkullCutOut,
    LoadingScreen,
};

[Serializable]
public struct GameObjectScenePair
{
    public GameObjectScenePair(GameObject gameObject, string scene)
    {
        this.gameObject = gameObject;
        this.scene = scene;
    }

    public GameObject gameObject;
    public string scene;
}

public class AdvancedSceneManager : MonoBehaviour
{
    public Dictionary<string, List<WeaponCostPair>> shopDictionary;
    public Dictionary<string, bool> treasureChestDictionary;
    public Dictionary<string, bool> battleZoneDictionary;
    public Dictionary<int, GameObjectScenePair> enemyDictionary;
    public Dictionary<int, GameObjectScenePair> weaponDictionary;

    public string lastScene;
    public Vector3 lastCheckpointPosition = Vector3.zero;

    public TransitionAnimationHandler transitionAnimationHandler;

    private string currentDestinationScene;
    private TransitionType currentTransitionOut = TransitionType.None;
    private Action currentLoadingSceneAction = null;
    private bool sceneLoadingInProgress = false;

    private PlayerInput playerInput;

    public static AdvancedSceneManager Instance = null;
    public static event Action loadingScreen;

    public bool currentSceneIsMenu;
    private const string MENU_IDENTIFIER = "Menu";
    private const string LEVEL_IDENTIFIER = "Level";
    private const string CUTSCENE_IDENTIFIER = "Menu - Cutscene";

    public float currentTime;
    private bool keepTime;
    [HideInInspector] public bool useNewTime = false;
    [HideInInspector] public float newTime;
    private bool resetingDictionaries = false;
    private bool isStartOfLevel = false;
    
    private AdvancedSceneManager()
    {
        sceneLoadingInProgress = false;
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        weaponDictionary = new Dictionary<int, GameObjectScenePair>();
        enemyDictionary = new Dictionary<int, GameObjectScenePair>();
        shopDictionary = new Dictionary<string, List<WeaponCostPair>>();
        treasureChestDictionary = new Dictionary<string, bool>();
        battleZoneDictionary = new Dictionary<string, bool>();

        isStartOfLevel = true;
    }

    private void Start()
    {
        if (!SceneNameIsValid(getCurrentScene()))
        {
            Debug.LogError(getCurrentScene() + " is an invaild scene name format");
            return;
        }

        currentSceneIsMenu = SceneIsMenu(getCurrentScene());

        transitionAnimationHandler = GetComponent<TransitionAnimationHandler>();
        SceneManager.sceneLoaded += SceneLoaded;

        if (!currentSceneIsMenu)
        {
            playerInput = PlayerInfo.Instance.GetComponent<PlayerInput>();
        }

        InitiateLevelStartSequence();
    }

    private bool SceneNameIsValid(string sceneName)
    {
        string[] currentSceneTemp = sceneName.Split("-");
        
        if (currentSceneTemp.Length < 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool SceneIsMenu(string scene)
    {
        return SceneMatchesIdentifier(scene, MENU_IDENTIFIER);
    }

    public static bool SceneIsLevel(string scene)
    {
        return SceneMatchesIdentifier(scene, LEVEL_IDENTIFIER);
    }

    private static bool SceneMatchesIdentifier(string scene, string sceneIdentifier)
    {
        string[] currentSceneTemp = scene.Split("-");

        if (currentSceneTemp[0].Trim().Equals(sceneIdentifier))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SceneLoadedCoroutine());
    }

    private void SceneLoaded()
    {
        
        //Deleting Player if they are in the menu
        if (currentSceneIsMenu)
        {
            if (PlayerInfo.Instance != null)
            {
                PlayerInfo.Instance.gameObject.transform.SetParent(null);
                SceneManager.MoveGameObjectToScene(PlayerInfo.Instance.gameObject, SceneManager.GetActiveScene());
                Destroy(PlayerInfo.Instance.gameObject);

                // Sometimes there is bug where player is deleted in a NON-MENU scene - Please Fix!
            }
        }
        else
        {
            
            //Setting the time
            if(keepTime)
            {
                Timer.Instance.SetTime(currentTime);
                keepTime = false;
            }
            else
            {
                Timer.Instance.SetTime(Timer.Instance.startingTimeValue);
            }
            
            if(useNewTime)
            {
                Timer.Instance.SetTime(newTime);
                useNewTime = false;
            }

            //instantaiting Gameobjects
            instantiateEnemies();
            instantiateWeapons();
        }
        
        if (playerInput != null)
        {
            playerInput.entityRigidbody.velocity = Vector3.zero;
            playerInput.dashReset();
            playerInput.setControlsDisable(false,true);
        }

        sceneLoadingInProgress = false;

        // Call Start Of Level Event
        InitiateLevelStartSequence();
        
        transitionAnimationHandler.playTransition(currentTransitionOut);
    }

    IEnumerator SceneLoadedCoroutine()
    {
        transitionAnimationHandler.playTransition(TransitionType.LoadingScreen, () => sceneLoadingInProgress == false);

        // This delay force function to start only after in game seconds start ticking.
        // (Prevents performing scene load operations too early)
        yield return new WaitForSeconds(0f);

        SceneLoaded();
    }

    public void InitiateLevelStartSequence()
    {
        // Call Start Of Level Event
        if (!currentSceneIsMenu && isStartOfLevel)
        {
            isStartOfLevel = false;
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.PlayLevelStartSequence();
            }
        }
    }

    IEnumerator LoadingSceneCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        // Code to execute after the specified delay
        LoadingScene();
    }

    private void LoadingScene()
    {
        transitionAnimationHandler.playTransition(TransitionType.LoadingScreen, () => sceneLoadingInProgress == false);

        // Before Scene Loads:
        if (currentLoadingSceneAction != null) currentLoadingSceneAction.Invoke();
        if (loadingScreen != null) loadingScreen();

        //Stoping all sounds
        FMODUnity.RuntimeManager.GetBus("bus:/WeaponSFX").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMODUnity.RuntimeManager.GetBus("bus:/PlayerSFX").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMODUnity.RuntimeManager.GetBus("bus:/GameObjects").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (resetingDictionaries)
        {
            clearDictionaries();
            resetingDictionaries = false;
        }

        SceneManager.LoadScene(currentDestinationScene);
    }

    public void loadScene(string targetScene, TransitionType transitionIn = TransitionType.None, TransitionType transitionOut = TransitionType.None, Action loadingSceneAction = null)
    {
        if (sceneLoadingInProgress) return;

        sceneLoadingInProgress = true;

        string[] targetSceneTemp = targetScene.Split("-");

        //Checking if Scene name is valid
        if(targetSceneTemp.Length < 2)
        {
            Debug.LogError(targetScene +  " is an invaild scene name format");
            return;
        }

        if (playerInput != null)
        {
            playerInput.setControlsDisable(true, true);
        }

        transitionAnimationHandler.playTransition(transitionIn);
        currentTransitionOut = transitionOut;
        currentDestinationScene = targetScene;
        currentLoadingSceneAction = loadingSceneAction;

        if(!isTheSameLevel(targetScene))
        {
            //Mark dictionaries to be cleared in a new level
            resetingDictionaries = true;

            //Forget time if we are entering a new level
            keepTime = false;

            //Mark new level start
            isStartOfLevel = true;
        }
        else
        {
            //Do not clear dictionaries in same level
            resetingDictionaries = false;

            //Storing the time if we are in the same level
            keepTime = true;
            currentTime = Timer.Instance?.CurrentTime ?? 0f;
        }

        //Check if the next scene is a menu and setting the bool if so
        if(targetSceneTemp[0].Trim().Equals("Menu"))
        {
            currentSceneIsMenu = true;
        }
        else
        {
            currentSceneIsMenu = false;
        }

        StartCoroutine(LoadingSceneCoroutine(transitionAnimationHandler.getAnimationLength(transitionIn.ToString())));
    }

    public void reloadScene(TransitionType transitionIn = TransitionType.None, TransitionType transitionOut = TransitionType.None, Action loadingSceneAction = null)
    {
        loadingSceneAction += () => 
        { 
            keepTime = false;
            resetingDictionaries = true;
            isStartOfLevel = true;
            print("New Level!");
        };

        loadScene(SceneManager.GetActiveScene().name, transitionIn, transitionOut, loadingSceneAction);
    }

    public void inSceneTransition(TransitionType transitionIn = TransitionType.None, TransitionType transitionOut = TransitionType.None, Action transitionAction = null)
    {
        //Disabling player controls
        if (playerInput != null)
        {
            playerInput.setControlsDisable(true, true);
        }

        //Doing transition in
        transitionAnimationHandler.playTransition(transitionIn);

        //Waiting for the transition in animation to finish and calling transition out
        float transitionOutLength = transitionAnimationHandler.getAnimationLength(transitionIn.ToString());

        // Play Transition Out: (Played Slightly Earlier than Animation Length to Smoothen the Transition)
        StartCoroutine(inSceneTransitionOut(transitionOut,transitionAction, transitionOutLength-0.01f)); 
    }


    private IEnumerator inSceneTransitionOut(TransitionType transitionOut,Action transitionAction,float animationLength)
    {
        //Waiting for the transition in animation to finish
        yield return new WaitForSecondsRealtime(animationLength);

        //Running transition out animation
        transitionAnimationHandler.playTransition(transitionOut);

        //running transition action
        if (transitionAction != null) transitionAction.Invoke();
        
        //Reseting Player setting 
        if (playerInput != null)
        {
            playerInput.entityRigidbody.velocity = Vector3.zero;
            playerInput.dashReset();
            playerInput.setControlsDisable(false, true);
        }
    }

    public void instantiateEnemies()
    {
        if (enemyDictionary.Count <= 0) return;

        // Kill All Pre-Existing Duplicate Enemies in the Scene:
        EnemyHealth[] prexistingEnemies = GameObject.FindObjectsOfType<EnemyHealth>();

        foreach(var enemy in prexistingEnemies)
        {
            if (enemy.saveOnSceneLoad && !enemyDictionary.ContainsKey(enemy.gameObject.GetInstanceID())) 
            {
                Destroy(enemy.gameObject);
            }
        }

        // Load Saved Enemies:
        foreach(var keyValuePair in enemyDictionary)
        {
            GameObjectScenePair pair = keyValuePair.Value;
            if (getCurrentScene().Equals(pair.scene)) 
            {
                if (pair.gameObject != null && !pair.gameObject.GetComponent<EnemyHealth>().isDead)
                {
                    pair.gameObject.transform.SetParent(null);
                    SceneManager.MoveGameObjectToScene(pair.gameObject, SceneManager.GetActiveScene());
                    pair.gameObject.SetActive(true);
                }
            }
        }
    }

    public void instantiateWeapons()
    {
        if (weaponDictionary.Count <= 0) return;
        
        // Load Saved Weapons:
        foreach(var keyValuePair in weaponDictionary)
        {
            GameObjectScenePair pair = keyValuePair.Value;
            if (getCurrentScene().Equals(pair.scene)) 
            {
                pair.gameObject.transform.SetParent(null);
                SceneManager.MoveGameObjectToScene(pair.gameObject, SceneManager.GetActiveScene());
                pair.gameObject.SetActive(true);
            }
        }
    }

    public string getCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static List<string> GetAllSceneNames()
    {
        List<string> result = new List<string>();

        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            result.Add(sceneName);
        }

        return result;
    }


    public static List<string> GetAllLevelSceneNames()
    {
        // Get All Scene Names:
        List<string> allSceneNames = GetAllSceneNames();

        // Filter Out Non-Levels:
        List<string> result = new List<string>();

        foreach (string sceneName in allSceneNames)
        {
            // Debug.Log("Level: " + sceneName + " | Is Level: " + SceneIsLevel(sceneName));

            if (SceneIsLevel(sceneName))
            {
                result.Add(sceneName);
            }
        }

        return result;
    }

    public bool isTheSameLevel(string targetScene)
    {
        string current = getCurrentScene();  

        string[] targetSceneTemp = targetScene.Split("-");
        string[] currentSceneTemp = current.Split("-");

        if (targetSceneTemp[1].Trim().Equals(currentSceneTemp[1].Trim()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isSameScene(string targetScene)
    {
        return targetScene.Equals(getCurrentScene());
    }

    public void clearDictionaries()
    {
        foreach (var entity in enemyDictionary)
        {
            entity.Value.gameObject.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(entity.Value.gameObject, SceneManager.GetActiveScene());
            Destroy(entity.Value.gameObject);

        }
        
        foreach (var entity in weaponDictionary)
        {
            entity.Value.gameObject.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(entity.Value.gameObject, SceneManager.GetActiveScene());
            Destroy(entity.Value.gameObject);
        }

        shopDictionary.Clear();
        treasureChestDictionary.Clear();
        battleZoneDictionary.Clear();
        enemyDictionary.Clear();
        weaponDictionary.Clear();
    }
}
