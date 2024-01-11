using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : Health
{
    [Header("Death Settings")]
    public GameObject spawnOnDeath;
    private bool hasSpawnedDeathSpawn = false;
    private bool spawnOnDeathEnabled = true;

    [Header("Save On Scene Load")]
    public bool saveOnSceneLoad = false;
    protected enemyAnimationHandler animationHandler;
    protected EnemyMovement enemyMovement;


    protected EnemyHealth()
    {
        hasSpawnedDeathSpawn = false;
    }
    protected override void Awake()
    {
        base.Awake();

        toggleSpawnOnDeath(true);
    }

    protected override void Start()
    {
        base.Start();
        animationHandler = GetComponentInChildren<enemyAnimationHandler>();
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;

        enemyMovement = GetComponent<EnemyMovement>();
    }

    protected override void Update()
    {
        base.Update();
        stuckCheck();
    }

    public void toggleSpawnOnDeath(bool value)
    {
        spawnOnDeathEnabled = value;
    }
    public override void die() 
    {
        if (!invincible)
        {
            if (spawnOnDeath != null && !hasSpawnedDeathSpawn && spawnOnDeathEnabled)
            {
                Instantiate(spawnOnDeath, transform.position, transform.rotation);
                hasSpawnedDeathSpawn = true;
            }

            if (enemyMovement != null) enemyMovement.enabled = false;
            
            base.die();

            //Disable The health bar
            if(healthBarCache != null)
            {
                healthBarCache.SetActive(false);
            }
            

            if (animationHandler != null)
            {
                Invoke("cleanUpObject", animationHandler.getAnimationLength(animationHandler.enemyName + "Death"));
                playDeathAnimation();
            }
            else
            {
                cleanUpObject();
            }
        }
    }


    protected virtual void OnEnable()
    {
        AdvancedSceneManager.loadingScreen += LoadingScreenAction;
    }

    protected virtual void OnDisable()
    {
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    protected virtual void OnDestroy()
    {
        AdvancedSceneManager.loadingScreen -= LoadingScreenAction;
    }

    protected void LoadingScreenAction()
    {
        if (saveOnSceneLoad)
        {
            if (!AdvancedSceneManager.Instance.enemyDictionary.ContainsKey(gameObject.GetInstanceID()))
            {
                GameObjectScenePair tempPair = new GameObjectScenePair(gameObject, AdvancedSceneManager.Instance.getCurrentScene());
                AdvancedSceneManager.Instance.enemyDictionary.Add(gameObject.GetInstanceID(), tempPair);
            }

            //Carry object to next scene
            transform.SetParent(AdvancedSceneManager.Instance.transform);
            gameObject.SetActive(false);
        }       
    }

    protected void cleanUpObject()
    {
        if (saveOnSceneLoad)
        {
            LoadingScreenAction();
            if (enemyMovement != null) enemyMovement.enabled = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void stuckCheck()
    {
        if (enemyMovement != null && enemyMovement.isStuck())
        {
            die();
        }
    }

    #region Animations
    protected virtual void playDeathAnimation()
    {
        if (animationHandler != null)
        {
            animationHandler.deathAnimation();
        }
    }
    #endregion
}
