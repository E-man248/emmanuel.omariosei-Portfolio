using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletSpawner : Bullet
{
    [Header("Bullet Spawner Stats")]
    public float spawnRate;
    public bool pointWithPlayerInput = true;
    public float rotationOffest;
    private float spawnTimer;

    [Header("Required Game Objects")]
    public GameObject bullet;
    public LayerMaskObject spawnedBulletDestroyCollisions;
    public Transform firingPoint;

    // Button Aim:
    private PlayerInput playerInput;
    private Vector2 lastButtonAimDirection = Vector2.zero;

    protected override void Start()
    {
        base.Start();
        
        playerInput = PlayerInfo.Instance.GetComponent<PlayerInput>();
    }
    
    /** <summary>
        A similar shoot function to that of Weapon. Shoots a 'bullet' of the specified prefab,
        at the rate of 'spawnDuration'.
        The 'damage' and 'canPierce' values of the bullet are overridden by 'damage' and 'bulletsCanPierce'
        respectively.
        </summary>
    **/
    public void shoot()
    {
        if (spawnTimer <= 0)
        {
            // Set Bullet Stats:
            Bullet currentBullet = Instantiate(bullet, firingPoint.position, firingPoint.rotation).GetComponent<Bullet>();
            currentBullet.tag = tag;
            currentBullet.owner = owner;

            // Set Destroy Collisions for Spawned Bullet:
            if (spawnedBulletDestroyCollisions != null)
            {
                currentBullet.destroyCollisions = spawnedBulletDestroyCollisions;
            }
            else if (destroyCollisions != null)
            {
                currentBullet.destroyCollisions = destroyCollisions;
            }

            spawnTimer = spawnRate;
        }
    }

    private void pointToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 difference = mousePosition - transform.position;
        difference.Normalize();

        float rotationZ = math.atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        firingPoint.rotation = Quaternion.Euler(0f, 0f, rotationZ + rotationOffest);
    }

    private void pointWithButtonInput()
    {
        Vector3 aimDirection = new Vector3(Input.GetAxisRaw("Horizontal Aim"), Input.GetAxisRaw("Vertical Aim"));
        aimDirection.Normalize();

        float rotationZ = 0f;
        if (!aimDirection.Equals(Vector2.zero))
        {
            lastButtonAimDirection = aimDirection;
            rotationZ = math.atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        }
        else
        {
            rotationZ = math.atan2(lastButtonAimDirection.y, lastButtonAimDirection.x) * Mathf.Rad2Deg;
        }

        firingPoint.rotation = Quaternion.Euler(0f, 0f, rotationZ + rotationOffest);
    }

    /**  <summary>
        The inherited Update() method of the parent is overridden.
        This update only increments the 'spawnTimer' to indicate the
        next time for a the bullet spawner to shoot, as well as move the
        bullet as the inherited method does.
        </summary>
    **/
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        spawnTimer -= Time.deltaTime;
        if (pointWithPlayerInput)
        {
            if (playerInput.currentAimInputDevice == PlayerInput.AimInput.Mouse)
            {
                pointToMouse();
            }
            else
            {
                pointWithButtonInput();
            }
        }
        shoot();
    }
}
