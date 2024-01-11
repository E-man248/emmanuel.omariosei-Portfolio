using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private Collider2D outOfBoundsCollder;

    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        outOfBoundsCollder = GetComponent<Collider2D>();
    }
    private void Start()
    {
        
    }

    [SerializeField]private TagList killTagList;
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
       

    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo == null) return;

        if(!killTagList.list.Contains(hitInfo.tag)) return;

        //Get Health script 
        Health health = hitInfo.transform.GetComponent<Health>();

        if (health == null) return;

        if (!outOfBoundsCollder.bounds.Contains(hitInfo.transform.position))
        {
            health.setHealth(0);
        }
    }
}
