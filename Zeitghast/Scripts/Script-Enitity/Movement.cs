using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public string entityName;
    [HideInInspector] public Rigidbody2D entityRigidbody;
    [HideInInspector] public SpriteRenderer entitySpriteRenderer;

    [Header("Movement Settings")]
    public float maxSpeed = 30f;
    public float minSpeed = 1f;
    public float speed = 1f;
    protected float variableSpeed;
    protected float currentSpeed;
    public float jumpForce = 1f;

    [Header("Graphics Settings")]
    public bool canOrientate = true;
    public bool flipOrientation = false;

    [Header("Entity Ground Check Settings")]
    public LayerMaskObject groundLayers;
    public float wallDetectionDistance = 0.1f;
    public float groundDetectionDistance = 0.1f;
    [HideInInspector] public Collider2D entityCollider;

    protected virtual void Awake()
    {
        entityCollider = GetComponent<Collider2D>();
        entityRigidbody = GetComponent<Rigidbody2D>();
        entitySpriteRenderer  = GetComponentInChildren<SpriteRenderer>();

        if(entitySpriteRenderer == null)
        {
            Debug.LogError("There is no Sprite Render in " + name);
        }
        if (entityCollider == null)
        {
            Debug.LogError("There is no Collider in " + name);
        }
        if (entityRigidbody == null)
        {
            Debug.LogError("There is no Rigidbody in " + name);
        }

        currentSpeed = speed;
        
        Timer.gamePausedEvent += onGamePaused;
        Timer.gameUnpausedEvent += onGameUnPaused;
    }

    protected virtual void Update()
    {
        //this with the childs update
        if (Timer.gamePaused)
        {
            return;
        }
        currentSpeed = speed + variableSpeed;
    }

    public virtual void moveX(float xDirection)
    {
        if (entityRigidbody.bodyType == RigidbodyType2D.Static) return;
        entityRigidbody.velocity = new Vector2(xDirection * Time.deltaTime * currentSpeed * 100, entityRigidbody.velocity.y);
    }

    public virtual void moveY(float yDirection)
    {
        if (entityRigidbody.bodyType == RigidbodyType2D.Static) return;
        entityRigidbody.velocity = new Vector2(entityRigidbody.velocity.x, yDirection * Time.deltaTime * currentSpeed * 100);
    }

    public virtual void moveXY(float xDirection, float yDirection)
    {
        if (entityRigidbody.bodyType == RigidbodyType2D.Static) return;
        entityRigidbody.velocity = new Vector2(xDirection * Time.deltaTime * currentSpeed * 100, yDirection * Time.deltaTime * currentSpeed * 100);
    }
    
    public virtual void moveXY(Vector2 moveDirection)
    {
        moveXY(moveDirection.x, moveDirection.y);
    }

    public virtual void jump(float appliedJumpForce)
    {
        Vector2 movment = new Vector2(entityRigidbody.velocity.x, appliedJumpForce);
        entityRigidbody.velocity = movment;
    }

    public virtual void impulseJump(Vector2 targetPosition, float jumpHeight)
    {
        float distanceFromTarget = targetPosition.x - transform.position.x;

        entityRigidbody.AddForce(new Vector2(distanceFromTarget * (entityRigidbody.mass/1.8f), jumpHeight * entityRigidbody.mass), ForceMode2D.Impulse);
    }

    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(entityCollider.bounds.center, entityCollider.bounds.size, 0, Vector2.down, groundDetectionDistance, groundLayers.layerMask);
        return hit.collider != null;
    }

    public virtual void setVariableSpeed(float newSpeed)
    {
        float calculatedSpeed = speed + newSpeed;

        if (calculatedSpeed > maxSpeed)
        {
            variableSpeed = maxSpeed - speed;
        }
        else if (calculatedSpeed <= minSpeed)
        {
            variableSpeed = minSpeed - speed;
        }
        else
        {
            variableSpeed = newSpeed;
        }  
    }

    public virtual float addToVariableSpeed(float speedToAdd)
    {
        setVariableSpeed(variableSpeed + speedToAdd);
    
        return variableSpeed;
    }

    protected virtual void onGamePaused()
    {

    }

    protected virtual void onGameUnPaused()
    {
        
    }
}
