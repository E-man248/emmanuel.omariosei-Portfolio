using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Cinemachine.CinemachineBlendDefinition;
public class BattleZone : MonoBehaviour
{
    private bool battleInProgress = false;

    [Header("Battle Zone Camera Target")]
    public Transform cameraTarget;
    [SerializeField] protected Style blendStyle;
    [SerializeField] protected float cameraMoveSpeed;

    private bool battleZoneComplete = false;

    [Header("Battle Zone Objects")]
    [SerializeField] private List<GameObject> battleZoneForceFields;
    private Dictionary<GameObject, ForceFieldAnimationHandler> forceFieldAnimationHandlerPairs;
    [SerializeField] private GameObject spawnPortal;
    [SerializeField] private float timeToSpawnEnemyFromPortal;
    private BattleWave currentWave = null;
    private int currentWaveIndex = -1;

    [Header("Battle Zone Waves")]
    [SerializeField] private List<BattleWave> battleWaves;

    [Header("Battle Zone text")]
    public Color instructionTextColor;
    [Space]
    public string zoneStartText;
    public Color zoneStartTextColor;
    [Space]
    public string zoneCompleteText;
    public Color zoneCompleteTextColor;
    [Space]
    public float textDuration;
    public float textFadeInDuration;
    public float textFadeOutDuration;
    public Vector2 textOffestWithIcons;

    [Header("Battle Zone Icon")]
    public Color battlezoneIconColor;
    public Sprite battleZoneIcon;

    [Header("Assassination Icon")]
    public Color assassinationIconColor;
    public Sprite assassinationIcon;
    public string assassinationText;

    [Header("Genocide Icon")]
    public Color genocideIconColor;
    public Sprite genocideIcon;
    public string genocideText;

    [Header("Battle Zone Events")]
    public UnityEvent battleZoneStartEvent;
    public UnityEvent battleZoneCompleteEvent;
    public UnityEvent battleZoneResetEvent;

    private void OnEnable()
    {
        PlayerInfo.PlayerDeathEvent.AddListener(disableBattleZone);
        SceneManager.sceneLoaded += SceneLoaded;
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
    }

    private void OnDisable()
    {
        PlayerInfo.PlayerDeathEvent.RemoveListener(disableBattleZone);
        SceneManager.sceneLoaded -= SceneLoaded;
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    private void OnDestroy()
    {
        PlayerInfo.PlayerDeathEvent.RemoveListener(disableBattleZone);
        SceneManager.sceneLoaded -= SceneLoaded;
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    public BattleZone()
    {
        battleZoneComplete = false;
    }

    private void Start()
    {
        currentWave = null;
        battleInProgress = false;
        currentWaveIndex = -1;

        if (cameraTarget == null)
        {
            cameraTarget = transform;
        }

        PlayerInfo.PlayerDeathEvent.AddListener(disableBattleZone);
        SceneManager.sceneLoaded += SceneLoaded;
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
        
        forceFieldAnimationHandlerPairs = new Dictionary<GameObject, ForceFieldAnimationHandler>();  

        foreach(GameObject forceField in battleZoneForceFields)
        {
            if (forceField != null)
            {
                forceField.SetActive(false);
                forceFieldAnimationHandlerPairs.Add(forceField, forceField.GetComponentInChildren<ForceFieldAnimationHandler>());
            }
        }      
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoaded();
    }
 
    private void SceneLoaded()
    {
        if (AdvancedSceneManager.Instance.battleZoneDictionary.ContainsKey(name))
        {
            battleZoneComplete = AdvancedSceneManager.Instance.battleZoneDictionary[name];
            Debug.Log("Battle Zone Complete Loaded as: " + battleZoneComplete);
        }

        if (battleZoneComplete)
        {
            battleZoneCompleteEvent.Invoke();
        }
    }

    private void LoadingScreenAction()
    {
        if (!AdvancedSceneManager.Instance.battleZoneDictionary.ContainsKey(name))
        {
            AdvancedSceneManager.Instance.battleZoneDictionary.Add(name, battleZoneComplete);
        }
        else
        {
            AdvancedSceneManager.Instance.battleZoneDictionary[name] = battleZoneComplete;
        }
    }

    void Update()
    {
        if (battleZoneComplete || currentWave == null)
        {
            return;
        }

        currentWave.updateWaveComplete();

        if (currentWave.WaveComplete)
        {
            // Wave Update (Next Wave Setup)
            resetWave(currentWave);
            setNextWave();
            if (currentWave != null) startWave(currentWave);
        }
    }

    public void setNextWave()
    {
        // All Battles Complete
        if (currentWaveIndex + 1 >= battleWaves.Count)
        {
            battleZoneComplete = true;
            battleZoneCompleteEvent.Invoke();
        
            if (!string.IsNullOrEmpty(zoneCompleteText))
            {
                LocationNameManager.Instance.changeMainTextUIColor(zoneCompleteTextColor);
                LocationNameManager.Instance.displayMainTextUI(zoneCompleteText, textDuration, textFadeInDuration, textFadeOutDuration);
            }

            disableBattleZone();

            return;
        }

        currentWaveIndex++;
        currentWave = battleWaves[currentWaveIndex].clone();
    }
    
    public void startWave(BattleWave battleWave)
    {
        battleInProgress = true;

        foreach (var forceFieldAnimationPair in forceFieldAnimationHandlerPairs)
        {
            forceFieldAnimationPair.Key.SetActive(true);
            forceFieldAnimationPair.Value.idleAnimation();
        }

        foreach(var enemy in battleWave.BattleWaveEnemies)
        {
            if (spawnPortal != null)
            {
                // Portal Spawn:
                GameObject portal = Instantiate(spawnPortal, enemy.position, transform.rotation);
                portal.transform.SetParent(transform);
                portal.SetActive(true);

                // Get Portal Scale:
                EnemyMovement enemyMovement = enemy.gameObject.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                {
                    portal.transform.localScale = enemyMovement.spawnPortalScale;
                }
                else if (enemy.gameObject.GetComponentInChildren<Boss>() != null)
                {
                    var boss = enemy.gameObject.GetComponentInChildren<Boss>();

                    portal.transform.localScale = boss.spawnPortalScale;
                }
            }

            if (enemy.gameObject != null)
            {
                GameObject enemyObject = Instantiate(enemy.gameObject, enemy.position, transform.rotation);
                enemyObject.transform.SetParent(transform);
                enemyObject.SetActive(false);
                
                battleWave.activeEnemies.Add(enemyObject);

                StartCoroutine(activateEnemy(enemyObject, timeToSpawnEnemyFromPortal));
            }
            else
            {
                Debug.LogError("Battle wave enemy from " + name + " tried to spawned but does not exist!");
            }
        }

        //Showing the assassination text and if first waiting for the begining text to fade 
        if (battleWave is AssassinationBattleWave && currentWaveIndex == 0)
        {
            Invoke("showAssinationText", textDuration + textFadeInDuration + textFadeOutDuration);
        }
        else if (battleWave is AssassinationBattleWave)
        {
            showAssinationText();
        }

        //Showing the genocide text and if first waiting for the begining text to fade 
        if (battleWave is GenocideBattleWave && currentWaveIndex == 0 &&  !string.IsNullOrEmpty (zoneStartText))
        {
            Invoke("showGenocideText", textDuration + textFadeInDuration + textFadeOutDuration);
        }
        else if(battleWave is GenocideBattleWave)
        {
            showGenocideText();
        }

        if (battleWave is AssassinationBattleWave)
        {
            // Add Glow to Assassination Target:
            AssassinationBattleWave assasinationBattleWave  = (AssassinationBattleWave) battleWave;
            if (assasinationBattleWave.assassinationTargetGlow != null)
            {
                int targetIndex = assasinationBattleWave.assassinationTargetIndex;
                GameObject glowObject = Instantiate(assasinationBattleWave.assassinationTargetGlow, Vector3.zero, Quaternion.Euler(Vector3.zero));
                glowObject.transform.SetParent(assasinationBattleWave.activeEnemies[targetIndex].gameObject.transform);
                glowObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void resetWave(BattleWave battleWave)
    {
        clearActiveEnemies();

        battleWave.WaveComplete = false;
    }

    private IEnumerator activateEnemy(GameObject enemyObject, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        enemyObject.SetActive(true);
    }

    public void clearActiveEnemies()
    {
        foreach(var enemy in currentWave.activeEnemies)
        {
            if (enemy != null)
            {
                var health = enemy.gameObject.GetComponentInChildren<Health>();
                if (health is EnemyHealth)
                {
                    EnemyHealth enemyHealth = (EnemyHealth) health;
                    enemyHealth.toggleSpawnOnDeath(false);
                }
                if (health is BossHealth)
                {
                    BossHealth bossHealth = (BossHealth) health;
                    bossHealth.toggleSpawnOnDeath(false);
                }
                health?.setHealth(0);
            }
        }

        currentWave.activeEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.tag == "Player")
        {
            // Battle Zone Start
            if (!battleInProgress && !battleZoneComplete)
            {
                CameraTargetManager.Instance.ChangeTarget(cameraTarget,blendStyle,cameraMoveSpeed);
                
                if(!string.IsNullOrEmpty(zoneStartText) && battleZoneIcon != null)
                {
                    LocationNameManager.Instance.changeMainTextUIColor(zoneStartTextColor);
                    LocationNameManager.Instance.changeIconUIColor(battlezoneIconColor);
                    LocationNameManager.Instance.displayLocationUI(battleZoneIcon, textOffestWithIcons, zoneStartText,"", textDuration, textFadeInDuration, textFadeOutDuration);
                }

                foreach (var forceFieldAnimationPair in forceFieldAnimationHandlerPairs)
                {
                    forceFieldAnimationPair.Key.SetActive(true);
                    forceFieldAnimationPair.Value.activateAnimation();
                }
                
                setNextWave();
                startWave(currentWave);
                battleZoneStartEvent.Invoke();
            }
        }
    }

    private void disableBattleZone()
    {
        if (currentWave != null) clearActiveEnemies();

        foreach (var forceFieldAnimationPair in forceFieldAnimationHandlerPairs)
        {
            if (forceFieldAnimationPair.Value.isActiveAndEnabled)
            {
                forceFieldAnimationPair.Value.deathAnimation();
                Invoke("disableForceFields", forceFieldAnimationPair.Value.getAnimationLength("ForceFieldDeath"));
            }
        }

        battleInProgress = false;
        currentWave = null;
        currentWaveIndex = -1;
        battleZoneResetEvent.Invoke();

        CameraTargetManager.Instance.resetTarget();
    }

    private void disableForceFields()
    {
        foreach (var forceFieldAnimationPair in forceFieldAnimationHandlerPairs)
        {
            forceFieldAnimationPair.Key.SetActive(false);
        }
    }

    private void showAssinationText()
    {
        //Show the graphic and instructions 
        if (!string.IsNullOrEmpty(assassinationText) && assassinationIcon != null)
        {
            LocationNameManager.Instance.changeMainTextUIColor(instructionTextColor);
            LocationNameManager.Instance.changeIconUIColor(assassinationIconColor);
            LocationNameManager.Instance.displayLocationUI(assassinationIcon, textOffestWithIcons, assassinationText, "", textDuration, textFadeInDuration, textFadeOutDuration);
        }
    }

    private void showGenocideText()
    {
        //Show the graphic and instructions 
        if (!string.IsNullOrEmpty(genocideText) && genocideIcon != null)
        {
            LocationNameManager.Instance.changeMainTextUIColor(instructionTextColor);
            LocationNameManager.Instance.changeIconUIColor(genocideIconColor);
            LocationNameManager.Instance.displayLocationUI(genocideIcon, textOffestWithIcons, genocideText,"", textDuration, textFadeInDuration, textFadeOutDuration);
        }
    }
}
