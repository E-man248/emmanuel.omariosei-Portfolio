using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstBullet : Bullet
{
    [Header("Burst Bullet Game Objects")]
    public BurstBulletAngleList burstBulletAngles;
    public Transform firingPoint;

    public float spawnRadius = 0f;
    [SerializeField]private bool pickRandomAnglesFromAngleList = false;
    [SerializeField] private float bulletSpeedDeviationRange = 0f;

    private Rigidbody2D bulletRigidbody;

    /*
        An override of the inherited Awake() method of the Bullet.
        The function shootBurst() is invoked after lifeTime instead of destroyBullet()
    */
    protected override void Awake()
    {
        Invoke("shootBurst", lifeTime);
        bulletRigidbody = GetComponent<Rigidbody2D>();

        //Burst Bullet should not be able to move so the Rigidbody is set to static
        if (bulletRigidbody != null) bulletRigidbody.bodyType = RigidbodyType2D.Static;
    }

    /// <summary>
    /// Overridden because Brust bullet should not apply any hits to avoid destroying the bullet
    /// </summary>
    protected override void applyHit(RaycastHit2D hitInfo)
    {

    }

    /// <summary>
    /// Overridden because Brust bullet should not move nor do automatic Damage to avoid destroying the bullet
    /// </summary>
    protected override void FixedUpdate()
    {

    }

    /// <summary>
    /// A similar shoot function to that of Weapon. Shoots all the bullets in 
    /// burstBulletAngles at their specified angles.
    /// After shooting all bullets, the function calls destroyBullet.
    /// </summary>
    private void shootBurst()
    {
        //Clone the list 
        BurstBulletAngleList burstBulletAnglesClone = burstBulletAngles.clone();

        //Shuffle the list 
        if (pickRandomAnglesFromAngleList)
        {
            burstBulletAnglesClone.list.Sort((a, b) => Random.Range(-1, 2));
        }

        foreach (BurstBulletAngle burstBullet in burstBulletAnglesClone.list)
        {
            Vector3 randomPosition = new Vector3(firingPoint.position.x + (Random.insideUnitCircle.x * spawnRadius), firingPoint.position.y + (Random.insideUnitCircle.y * spawnRadius), firingPoint.position.z);
            GameObject currentBullet = Instantiate(burstBullet.bullet, randomPosition, firingPoint.rotation);

            //Calculating shotAngle by adding a random deviation if any 
            float finalShotAngle = burstBullet.shotAngle + Random.Range(-burstBullet.randomAngleDeviation, burstBullet.randomAngleDeviation);
            currentBullet.transform.eulerAngles += new Vector3(0, 0, finalShotAngle);

            //Getting Bullet if Their is one
            Bullet childBullet = currentBullet.GetComponent<Bullet>();
            if (childBullet != null)
            {
                childBullet.owner = owner;
                //Adding Bullet speed deviation
                childBullet.speed.x += Random.Range(-bulletSpeedDeviationRange, bulletSpeedDeviationRange);
            }
        }

        destroyBullet();
    }
}


[System.Serializable]
public struct BurstBulletAngle
{
    public float shotAngle;
    public float randomAngleDeviation;
    public GameObject bullet;
}