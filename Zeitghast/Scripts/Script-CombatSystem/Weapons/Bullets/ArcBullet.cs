using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcBullet : Bullet
{
    [Header("Arc Bullet Settings")]
    public float gravity = 0f;
    public float linearDrag = 0f;

    [Header("Bounce Settings")]
    public bool bouncy;
    public float bounciness = 0f;
    [EventRef]
    public string BounceSound = null;
    public float bounceSoundInterval;
    private float bounceSoundIntervalTimer;

    [Space]
    [SerializeField] protected GameObject gameobjectToSpawnOnBounce;
    [SerializeField] private float gameobjectToSpawnOverlapCheckRadius = 0.01f;
    [SerializeField] private LayerMask gameobjectToSpawnTargetLayers;

    private Rigidbody2D bulletRigidBody;
    public Collider2D bounceCollider;

    [Header("Aim Assist")]
    [SerializeField] protected bool aimAssist = false;
    [SerializeField] [Range(0,1)] protected float aimAssistStrength = 0.2f;
    [SerializeField] protected float aimAssistTargetRange = 1f;
    protected Transform currentAssistTarget = null;
    protected Vector3 aimAssistDirection = Vector3.zero;
    protected Vector3 initialDirectionX = Vector3.zero;
    protected Vector3 initialDirectionY = Vector3.zero;

    /*
        Start method of 'ArcBullet'. Creates an instance of rigidBody which is specific to
        the arc bullet in comparison to the other bullets.
        If no rigidBody is provided on the object, a default rigidBody will be created.
    */
    protected override void Start()
    {
        base.Start();
        bulletRigidBody = GetComponent<Rigidbody2D>();

        // Arcing Bullet RigidBody
        if (bulletRigidBody == null)
        {
            bulletRigidBody = gameObject.AddComponent<Rigidbody2D>();
        }

        PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D();
        if(bounceCollider == null && bouncy)
        {
            Debug.LogError(name + "has no bounce Collider");
        }
        else if (bouncy)
        {
            //Bouncy settings 
            physicsMaterial.bounciness = bounciness;
            bounceCollider.sharedMaterial = physicsMaterial;

            //Layer settings
            bounceCollider.gameObject.layer = 19;
        }

        if (aimAssist)
        {
            initialDirectionX = transform.right.normalized;
            initialDirectionY = transform.up.normalized;
        }
        else
        {
            bulletRigidBody.velocity = transform.right * speed.x + transform.up * speed.y;
        }
    }

    /*
        Update method of 'ArcBullet'. This continuously rotates the bullet object in relation to
        its velocity, giving the effect of rising or falling.
        This 'gravity' and 'linearDrag' are changed live using this Update() method.
        The acceleration component inherited from the bullet does not actually affect the velocity
        as that is overridden by the acceloration of gravity.
    */
    protected override void FixedUpdate()
    {
        automaticDamage();

        if (aimAssist && gravity == 0f)
        {
            setAssistTarget();
            Vector3 calculatedDirectionX = (initialDirectionX + aimAssistDirection.normalized * aimAssistStrength).normalized;
            bulletRigidBody.velocity = calculatedDirectionX * speed.x + initialDirectionY * speed.y;
        }

        float angle = Mathf.Atan2(bulletRigidBody.velocity.x, bulletRigidBody.velocity.y) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, -Vector3.forward);

        if (gravity != bulletRigidBody.gravityScale)
        {
            bulletRigidBody.gravityScale = gravity;
        }

        if (linearDrag != bulletRigidBody.drag)
        {
            bulletRigidBody.drag = linearDrag;
        }

        if (bouncy && bounciness != bounceCollider.sharedMaterial.bounciness)
        {
            bounceCollider.sharedMaterial.bounciness = bounciness;
        }
    }

    protected override void performCollisionProperty(RaycastHit2D hitInfo)
    {
        if (bouncy && bounceCollider.isTrigger)
        {
            bounceCollider.isTrigger = false;
            return;
        }
        else if (!bouncy && bounceCollider != null && !bounceCollider.isTrigger)
        {
            bounceCollider.isTrigger = true;
        }

        if ((canPierce && hitInfo.collider.tag != "Ground" ) && bouncy)
        {
            //Bouncing Sound
            if (!string.IsNullOrEmpty(BounceSound))
            {
                RuntimeManager.PlayOneShot(BounceSound, transform.position);
            }
            return;
        }
        else if(canPierce && hitInfo.collider.tag == "Ground" && !bouncy)
        {
            destroyBullet(hitInfo);
            return;
        }
        else if (canPierce)//Do nothing and pass through if it can pirece but is not bouncy
        {
            return;
        }

        if(bouncy)
        {
            if(hitInfo.collider.tag != "Ground")
            {
                destroyBullet(hitInfo);
            }
            else
            {
               
            }
            return;
        }
        else if (firingType == WeaponFireType.Melee)
        {
            if (hitInfo.collider.tag != "Ground")
            {
                damage = 0;
            }
        }
        else
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red, 5f);
            destroyBullet(hitInfo);
        }
    }

    //Spawn a the gameobject on each bounce/ collison
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (!bouncy)
        {
            return;
        }

        if (hitInfo.transform.tag != "Ground")
        {
            return;
        }
        //Spawns a game object on each bounce if there is no Spawned game object in the same space
        if (gameobjectToSpawnOnBounce != null && !gameObjectToSpawnOverlaped())
        {
            Instantiate(gameobjectToSpawnOnBounce, transform.position, Quaternion.identity);
        }
    }

    //Checks if the bullet overlaps with any object in the target layer 
    public bool gameObjectToSpawnOverlaped()
    {
        return Physics2D.OverlapCircle(transform.position, gameobjectToSpawnOverlapCheckRadius, gameobjectToSpawnTargetLayers);
    }

    protected void setAssistTarget()
    {
        RaycastHit2D[] allHitInfo = Physics2D.CircleCastAll(bulletCollider.bounds.center, aimAssistTargetRange, transform.right, 0f, destroyCollisions.layerMask);

        if (currentAssistTarget == null && allHitInfo.Length > 0)
        {
            foreach (RaycastHit2D hitInfo in allHitInfo)
            {
                if (hitInfo.collider.tag == "Ground") continue;

                Health targetHealth = hitInfo.collider.GetComponentInParent<Health>();
                if (targetHealth != null && targetHealth.attackers.list.Contains(tag))
                {
                    currentAssistTarget = hitInfo.collider.transform;
                    break;
                }
            }
        }

        if (currentAssistTarget != null)
        {
            if (Vector3.Distance(transform.position, currentAssistTarget.position) > aimAssistTargetRange)
            {
                currentAssistTarget = null;
                return;
            }

            aimAssistDirection = currentAssistTarget.position - transform.position;
            aimAssistDirection.Normalize();
        }
        else aimAssistDirection = Vector3.zero;
    }

    protected override void Update()
    {
        base.Update();
        bounceSoundIntervalTimer -= Time.deltaTime;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!string.IsNullOrEmpty(BounceSound) && bounceSoundIntervalTimer <= 0)
        {
            bounceSoundIntervalTimer = bounceSoundInterval;
            RuntimeManager.PlayOneShot(BounceSound, transform.position);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (aimAssist)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, aimAssistTargetRange);
        }
    }
}
