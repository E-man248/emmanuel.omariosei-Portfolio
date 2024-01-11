using UnityEngine;

public class LaserSaurus : GroundedEnemy
{
    [Header("LaserSaurus Settings")]
    [SerializeField] private float attackDuration = 1f;
    private float attackDurationTimer = 1f;
    private Laser laser;
    [SerializeField] private float maxLaserDistance = 20f;

    // Utilities:
    private Health health;

    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<Health>();
    }

    protected override void Start()
    {
        base.Start();

        subscribeToEvents();
    }

    protected void OnEnable()
    {
        subscribeToEvents();
    }

    protected void OnDisable()
    {
        unsubscribeToEvents();
        deactivateLaser();
    }

    protected void OnDestroy()
    {
        unsubscribeToEvents();
        deactivateLaser();
    }

    private void subscribeToEvents()
    {
        health?.onDeathEvent?.AddListener(deactivateLaser);
    }

    private void unsubscribeToEvents()
    {
        health?.onDeathEvent?.RemoveListener(deactivateLaser);
    }

    protected override void leaveCombatState()
    {
        attackAnimationIsPlaying = false;
        if (!inHostileRange())
        {
            attackWindUpTimer = attackWindUpDuration;
            attackDurationTimer = attackDuration;
            laser.gameObject.SetActive(false);
            state = State.Pursuit;
        }
    }

    protected override void combatSetup()
    {
        coolDownTimer = coolDownDuration;
        attackWindUpTimer = attackWindUpDuration;
        attackAnimationIsPlaying = false;
        attackDurationTimer = attackDuration;

        // Laser Setup:

        laser = GetComponentInChildren<Laser>();
        if (laser == null) Debug.LogError("LaserSaurus Script in " + name + "is missing Laser GameObject");

        laser.gameObject.SetActive(false);
        setUpLaser();        
    }

    protected override void attackWindUpCycle()
    {
        if (attackAnimationIsPlaying) return;

        // On the first windup cycle, the enemy will look at the target, locking aim in later cycles
        if (attackWindUpTimer >= attackWindUpDuration)
        {
            lookAtTarget();
        }

        attackWindUpTimer -= Time.deltaTime;
        
        if (attackWindUpTimer > 0)
        {
            //This is sets attackSpeedTimer to 0 so that the initial attack triggers immediately
            attackSpeedTimer = 0;
            return;
        }
    }

    protected override void attack()
    {
        
    }

    protected override void attackCycle()
    {
        if (attackDurationTimer > 0)
        {
            attackDurationTimer -= Time.deltaTime;
            laser.gameObject.SetActive(true);
            attack();
        }
        else
        {
            coolDown();
            // Check To Leave the Combat State (Done in Function)
            leaveCombatState();
        }
    }

    protected override void coolDown()
    {
        laser.gameObject.SetActive(false);
        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
        }
        else
        {
            attackWindUpTimer = attackWindUpDuration;
            attackDurationTimer = attackDuration;
            coolDownTimer = coolDownDuration;
        }
    }

    private void setUpLaser()
    {
        laser.flipOrientation = flipOrientation;
        laser.owner = entityName;
        laser.maxLaserDistance = maxLaserDistance;
    }

    public bool isAttacking()
    {
        return state == State.Combat && attackWindUpTimer <= 0  && attackDurationTimer > 0f;
    }

    protected override void inactive()
    {
        deactivateLaser();
        base.inactive();
    }
    protected override void stunned()
    {
        deactivateLaser();
        base.stunned();
    }

    protected override void patroling()
    {
        deactivateLaser();
        base.patroling();
    }

    protected override void pursue()
    {
        deactivateLaser();
        base.pursue();
    }

    protected void activateLaser()
    {
        laser.gameObject.SetActive(true);
    }

    protected void deactivateLaser()
    {
        laser.gameObject.SetActive(false);
    }
}
