using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBasedMovment : MonoBehaviour
{
    [SerializeField]private float moveSpeed = 3f;
    [SerializeField]private LayerMask obstaclesLayer;
    internal Vector2 lastMoveDirection = Vector2.zero;

    public Transform movePoint;

    private void Awake()
    {
        movePoint = new GameObject(name + " Move Point").transform;
        movePoint.position = transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    public void moveUp()
    {
        if (hasNotReachedTargetPosition()) return;

        Vector3 newPosition = movePoint.position + TileBasedMovementManager.Instance.OneTileUp;

        if (cantwalk(newPosition)) return;

        movePoint.position = newPosition;
        lastMoveDirection = TileBasedMovementManager.Instance.OneTileUp;

    }

    public void moveDown()
    {
        if (hasNotReachedTargetPosition()) return;

        Vector3 newPosition = movePoint.position + TileBasedMovementManager.Instance.OneTileDown;

        if (cantwalk(newPosition)) return;

        movePoint.position = newPosition;
        lastMoveDirection = TileBasedMovementManager.Instance.OneTileDown;
    }

    public void moveLeft()
    {
        if (hasNotReachedTargetPosition()) return;
        
        Vector3 newPosition = movePoint.position +  TileBasedMovementManager.Instance.OneTileLeft;


        if (cantwalk(newPosition)) return;

        movePoint.position = newPosition;
        lastMoveDirection = TileBasedMovementManager.Instance.OneTileLeft;
    }

    public void moveRight()
    {
        if (hasNotReachedTargetPosition()) return;

        Vector3 newPosition = movePoint.position + TileBasedMovementManager.Instance.OneTileRight;


        if(cantwalk(newPosition)) return;

        movePoint.position = newPosition;
        lastMoveDirection = TileBasedMovementManager.Instance.OneTileRight;
    }

    public void moveAnyDirection(Vector3 driection)
    {
        if (hasNotReachedTargetPosition()) return;

        Vector3 newPosition = movePoint.position + driection;


        if (cantwalk(newPosition)) return;

        movePoint.position = newPosition;
        lastMoveDirection = driection;
    }

    public bool hasNotReachedTargetPosition()
    {
        return Vector3.Distance(transform.position, movePoint.position) > 0.001f;
    }

    public bool hasReachedTargetPosition()
    {
        return transform.position == movePoint.position;
    }

    private bool cantwalk(Vector3 position)
    {
        Collider2D hitinfo = Physics2D.OverlapCircle(position, 0.2f, obstaclesLayer);
        return hitinfo;
    }

}
