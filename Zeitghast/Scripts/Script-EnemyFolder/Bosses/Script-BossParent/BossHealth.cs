using UnityEngine;

public class BossHealth : Health
{
    [Header("Death Settings")]
    public GameObject spawnOnDeath;
    [SerializeField] private bool spawnOnDeathEnabled = true;
    private bool hasSpawnedDeathSpawn = false;
    private Boss boss;
    private BossAnimationHandler animationHandler;


    protected override void Awake() 
    { 
        base.Awake();

        boss = GetComponentInChildren<Boss>();
        animationHandler = GetComponentInChildren<BossAnimationHandler>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    public override void die()
    {
        if (!invincible)
        {
            if (spawnOnDeath != null && !hasSpawnedDeathSpawn && spawnOnDeathEnabled)
            {
                Instantiate(spawnOnDeath, transform.position, transform.rotation);
                hasSpawnedDeathSpawn = true;
                print("Spawn");
            }

            boss.SwitchState(Boss.State.Dead);

            base.die();

            if (animationHandler != null)
            {
                Invoke("cleanUpObject", animationHandler.getAnimationLength(animationHandler.DeadAnimationString));
                animationHandler.playDeathAnimation();
            }
            else
            {
                cleanUpObject();
            }
        }

    }

    public void toggleSpawnOnDeath(bool value)
    {
        spawnOnDeathEnabled = value;
    }

    protected void cleanUpObject()
    {
        Destroy(gameObject);
    }
}
