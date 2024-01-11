using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Pathfinding;

public class FollowerBullet : Bullet
{
    [Header("Movement")]
    public float movementSpeed;
    public float rotationOffest;
    private Rigidbody2D bulletRigidbody;

    [Header("Pathfinding")]
    public float stopDistanceFromMouse = 0.1f;
    public LayerMask wallLayer;
    public float wallDetectionDistance = 0.7f;
    public Vector2 wallBumbRange;
    protected Vector2 moveDirection;
    public float nextWayPointDistance;
    public float pathUpdateRate;
    private float pathUpdateRateTimer;

    protected Path path;
    public int currentWaypoint = 0;
    protected Seeker seeker;

    // Input:
    private PlayerInput playerInput;
    // Mouse:
    protected Vector3 mousePosition;
    // Buttons:
    private Vector2 lastButtonAimDirection = Vector2.zero;

    protected override void Start()
    {
        base.Start();
        bulletRigidbody = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        bulletRigidbody.AddForce(transform.forward);
        lastButtonAimDirection = Vector2.zero;

        playerInput = PlayerInfo.Instance.GetComponent<PlayerInput>();
    }

    protected override void FixedUpdate()
    {
        if (playerInput.currentAimInputDevice == PlayerInput.AimInput.Mouse)
        {
            mousePosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
            float distanceFromMouse = Vector3.Distance(mousePosition, transform.position);

            if (distanceFromMouse > stopDistanceFromMouse)
            {
                followMouse();
                pointToMouse();
            }
            else 
            {
                moveXY(-transform.right.x, -transform.right.y);
            }
        }
        else
        {
            followButtons();
        }

        automaticDamage();
    }

    protected void followMouse()
    { 
        if (pathUpdateRateTimer <= 0)
        {
            pathUpdateRateTimer = pathUpdateRate;
            UpdatePath();
        }
        pathUpdateRateTimer -= Time.deltaTime;
    }

    protected void followButtons()
    { 
        Vector3 aimDirection = new Vector3(Input.GetAxisRaw("Horizontal Aim"), Input.GetAxisRaw("Vertical Aim"));
        aimDirection.Normalize();

        if (!aimDirection.Equals(Vector2.zero))
        {
            lastButtonAimDirection = aimDirection;
            moveXY(aimDirection);
        }
        else
        {
            moveXY(lastButtonAimDirection);
        }
    }

    protected void pointToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 difference = mousePosition - transform.position;
        difference.Normalize();

        float rotationZ = math.atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + rotationOffest);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(bulletRigidbody.position, mousePosition, OnPathComplete);
        }
        pathFollow();

    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void pathFollow()
    {
        if (path == null)
        {
            // moveXY(0,0);
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //moveXY(0,0);
            return;
        }

        if (currentWaypoint + 1 >= path.vectorPath.Count)
        {
            //moveXY(0,0);
            return;
        }

        moveDirection = ((Vector2)path.vectorPath[currentWaypoint + 1] - bulletRigidbody.position).normalized;
        if (isGoingToHitWallX())
        {
           moveXY(-moveDirection.x, moveDirection.y);
        }
        if (isGoingToHitWallY())
        {
            moveXY(-moveDirection.x, -moveDirection.y);
        }
        else
        {
            moveXY(moveDirection.x, moveDirection.y);
        }

        float distance = Vector2.Distance(bulletRigidbody.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }
    }


    public void moveXY(float xDirection, float yDirection)
    {
        bulletRigidbody.velocity = new Vector2(xDirection * Time.deltaTime * movementSpeed * 100, yDirection * Time.deltaTime * movementSpeed * 100);
    }

    public void moveXY(Vector2 direction)
    {
        bulletRigidbody.velocity = new Vector2(direction.x * Time.deltaTime * movementSpeed * 100, direction.y * Time.deltaTime * movementSpeed * 100);
    }

    protected bool isGoingToHitWallX()
    {
        //BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask);

        RaycastHit2D Xhit = Physics2D.BoxCast(bulletCollider.bounds.center, new Vector2(bulletCollider.bounds.size.x + wallBumbRange.x, bulletCollider.bounds.size.y), 0, moveDirection * Vector2.right, wallDetectionDistance, wallLayer);

        //Debugging Box
        Vector2 top = new Vector2(bulletCollider.bounds.center.x, bulletCollider.bounds.min.y);
        Vector2 bottom = new Vector2(bulletCollider.bounds.center.x, bulletCollider.bounds.max.y);

        Debug.DrawRay(top, moveDirection * Vector2.right * (bulletCollider.bounds.extents.x + wallBumbRange.x / 2), Color.blue);
        Debug.DrawRay(bulletCollider.bounds.center, moveDirection * Vector2.right * (bulletCollider.bounds.extents.x + wallBumbRange.x / 2), Color.blue);
        Debug.DrawRay(bottom, moveDirection * Vector2.right * (bulletCollider.bounds.extents.x + wallBumbRange.x / 2), Color.blue);

        return Xhit.collider != null;
    }

    protected bool isGoingToHitWallY()
    {
        //BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask);

        RaycastHit2D Yhit = Physics2D.BoxCast(bulletCollider.bounds.center, new Vector2(bulletCollider.bounds.size.x, bulletCollider.bounds.size.y + wallBumbRange.y), 0, moveDirection * Vector2.up, wallDetectionDistance, wallLayer);

        //Debugging box
        Vector2 left = new Vector2(bulletCollider.bounds.min.x, bulletCollider.bounds.center.y);
        Vector2 right = new Vector2(bulletCollider.bounds.max.x, bulletCollider.bounds.center.y);

        Debug.DrawRay(left, moveDirection * Vector2.up * (bulletCollider.bounds.size.y + wallBumbRange.y / 2), Color.blue);
        Debug.DrawRay(bulletCollider.bounds.center, moveDirection * Vector2.up * (bulletCollider.bounds.size.y + wallBumbRange.y / 2), Color.blue);
        Debug.DrawRay(right, moveDirection * Vector2.up * (bulletCollider.bounds.size.y + wallBumbRange.y / 2), Color.blue);

        return Yhit.collider != null;
    }
}
