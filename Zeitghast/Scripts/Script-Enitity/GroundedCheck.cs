using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundedCheck : MonoBehaviour
{
    [Header("Entity Ground Check Settings")]
    public LayerMaskObject groundLayers;
    public float wallDetectionDistance = 0.1f;
    [HideInInspector] public Collider2D entityCollider;

    // Start is called before the first frame update
    void Start()
    {
        entityCollider = GetComponent<Collider2D>();
    }

    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(entityCollider.bounds.center, entityCollider.bounds.size, 0, Vector2.down, wallDetectionDistance, groundLayers.layerMask);
        return hit.collider != null;
    }
}
