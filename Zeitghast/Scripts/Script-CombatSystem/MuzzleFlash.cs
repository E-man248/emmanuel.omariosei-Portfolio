using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public float lifeTime;
    [SerializeField]private GameObject extraObjects;
    [SerializeField]private Vector2 extraObjectOffset;

    void Start()
    {
        if(extraObjects != null)
        {
            Vector2 normalizedPerpendicular = Vector2.Perpendicular(transform.position).normalized;
            Vector2 transposedVector = new Vector2(normalizedPerpendicular.x * extraObjectOffset.x, normalizedPerpendicular.y * extraObjectOffset.y);
            
            Vector2 instantiationLocation = new Vector2(transform.position.x + transposedVector.x, transform.position.y + transposedVector.y);

            Instantiate(extraObjects, instantiationLocation, Quaternion.identity, null);
        }

        Invoke("destroyMuzzleFlash", lifeTime);
    }

    public virtual void destroyMuzzleFlash()
    {
        Destroy(gameObject);
    }
}
