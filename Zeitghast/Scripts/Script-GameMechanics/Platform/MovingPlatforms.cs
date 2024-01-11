using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MovingPlatforms : MonoBehaviour
{
    [Header("Platform")]
    public List<PlatformPositionPoints> PositionsToMoveTo;
    internal int currentTargetPosition { get; private set; }


    public bool touchToMove;
    public bool isTouching;

    private bool enableMovment = true;

    // Start is called before the first frame update
    void Start()
    {
        currentTargetPosition = 0;
        transform.position = PositionsToMoveTo[0].Transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enableMovment) return;

        if (Vector2.Distance(transform.position, PositionsToMoveTo[currentTargetPosition].Transform.position) < 0.02f)
        {
            currentTargetPosition++;
            if (currentTargetPosition == PositionsToMoveTo.Count)
            {
                currentTargetPosition = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (touchToMove && isTouching)
        {
            transform.position = Vector2.MoveTowards(transform.position, PositionsToMoveTo[currentTargetPosition].Transform.position, PositionsToMoveTo[currentTargetPosition].MoveSpeed * Time.deltaTime);
        }
        else if (!touchToMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, PositionsToMoveTo[currentTargetPosition].Transform.position, PositionsToMoveTo[currentTargetPosition].MoveSpeed * Time.deltaTime);
        }
    }

    public void movePlatform(bool value)
    {
        if(touchToMove) 
        {
            isTouching = value;
        }
        else 
        {
            Debug.LogWarning("touchToMove is not on " + name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            isTouching = true;
        }

        if (collision.transform.position.y > transform.position.y)
        {
            collision.transform.SetParent(transform);
        }

        Movement movement = collision.transform.GetComponent<Movement>();
        GroundedCheck groundedCheck = collision.transform.GetComponent<GroundedCheck>();
        if (movement != null)
        {
            if (!movement.isGrounded())
            {
                collision.transform.SetParent(transform);
            }
        }
        else if (groundedCheck != null)
        {
            if (!groundedCheck.isGrounded())
            {
                collision.transform.SetParent(transform);
            }
        }
        else
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null && collision.transform.parent != null)
        {
            if (collision.transform.parent.Equals(this.transform))
            {
                collision.transform.SetParent(null);
            }
        }
    }

    public void toggleMovingPlatform(bool value)
    {
        enableMovment = value;
    }

    public Vector2 getMoveDirection()
    {
        int previousTargetPosition;
        if (currentTargetPosition == 0)
        {
            previousTargetPosition = PositionsToMoveTo.Count - 1;
        }
        else
        {
            previousTargetPosition = currentTargetPosition - 1;
        }

        PlatformPositionPoints previousTargetPositionPoint = PositionsToMoveTo[previousTargetPosition];
        PlatformPositionPoints currentTargetPositionPoint = PositionsToMoveTo[currentTargetPosition];

        Vector2 moveDirection = currentTargetPositionPoint.Transform.position - previousTargetPositionPoint.Transform.position;

        return moveDirection.normalized;
    }

    public bool isMoving()
    {
        if (touchToMove && !isTouching)
        {
            return false;
        }

        return PositionsToMoveTo[currentTargetPosition].MoveSpeed > 0;
    }
}

[System.Serializable]
public struct PlatformPositionPoints
{
    public Transform Transform;
    public float MoveSpeed;
    
}