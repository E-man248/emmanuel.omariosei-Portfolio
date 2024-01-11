using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public enum ItemType
{
    TimeToken,
    Health
}

public class PickUp : MonoBehaviour
{
    public ItemType itemType;

    [Header("Sounds")]
    [EventRef]
    public string PickUpSounds = null;

    [Header("PickUp Settings")]
    [SerializeField] private float pickUpRange = 0f;
    [SerializeField] private float rangePickUpSpeed= 1f;
    [SerializeField] private float rangePickUpAcceleration = 1.5f;

    private Transform playerTransform;
    private Rigidbody2D itemRigidbody;
    private float currentAcceleration;

    protected virtual void Start()
    {
        playerTransform = PlayerInfo.Instance.transform;
        itemRigidbody = GetComponent<Rigidbody2D>();
        currentAcceleration = 0f;
    }

    protected void Update()
    {
        flyTowardsPlayer();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            pickedUp();
            destroy();
        } 
    }

    private void destroy()
    {
        Destroy(gameObject);
    }

    virtual protected void pickedUp()
    {
        if (PickUpSounds != null)
        {
            RuntimeManager.PlayOneShot(PickUpSounds, transform.position);
        }
    }


    private void flyTowardsPlayer()
    {
        if (itemRigidbody == null) return;

        if (pickUpRange <= 0) return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        

        if (distance > pickUpRange)
        {
            currentAcceleration = 0f;
            return;
        }

        currentAcceleration += rangePickUpAcceleration * Time.deltaTime;

        Vector2 directionCalculation = (playerTransform.position - transform.position).normalized;

        itemRigidbody.velocity = directionCalculation * (rangePickUpSpeed + currentAcceleration);
    }


    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }


    
}
