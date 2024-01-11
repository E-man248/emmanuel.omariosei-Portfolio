using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOptimizer : Optimizer
{
    private Rigidbody2D entityRigidbody;
    private SpriteRenderer[] spriteRender;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        entityRigidbody = GetComponentInChildren<Rigidbody2D>();
        spriteRender = GetComponentsInChildren<SpriteRenderer>();

        if (entityRigidbody == null) Debug.LogError(name + "could not find a rigidbody");
        if (spriteRender == null) Debug.LogError(name + "could not find a sprite Render");
    }

    protected override void setInactive()
    {
        disableSprite();
        disablePhysics();
    }

    protected override void setActive()
    {
        enablePhysics();
        enableSprite();
    }

    void disablePhysics()
    {
        if (entityRigidbody != null)
        {
            entityRigidbody.bodyType = RigidbodyType2D.Static;
        }
    }

    void enablePhysics()
    {
        if (entityRigidbody != null)
        {
            entityRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void disableSprite()
    {
        foreach (var sprite in spriteRender)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    void enableSprite()
    {
        foreach (var sprite in spriteRender)
        {
            sprite.gameObject.SetActive(true);
        }
    }

}
