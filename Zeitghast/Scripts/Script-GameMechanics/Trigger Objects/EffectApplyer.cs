using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectApplyer : MonoBehaviour
{
    [SerializeField] protected string targetTag = "Player";
    [SerializeField] protected OnHitEffector onHitEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null && collision.tag == "Player") return;

        if (collision.tag != targetTag) return;

        applyEffect(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null && collision.tag == "Player") return;

        if (collision.tag != targetTag) return;

        applyEffect(collision);
    }

    protected virtual void applyEffect(Collider2D collision)
    {
        OnHitReciever targetEffectReciever = collision.GetComponentInParent<OnHitReciever>();
        if (targetEffectReciever == null)
        {
            return;
        }

        if (onHitEffect != null)
        {
            targetEffectReciever.addEffect(onHitEffect);
        }

    }
}
