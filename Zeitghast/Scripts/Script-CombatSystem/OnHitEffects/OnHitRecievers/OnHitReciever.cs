using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class OnHitReciever : MonoBehaviour
{
    public TagList attackers;
    public List<OnHitEffector> currentEffects;

    public List<string> immunityList;
    public bool EffectImunity;

    void Start()
    {
        currentEffects = new List<OnHitEffector>();
        immunityList = new List<string>();
    }

    void Update()
    {
        if (currentEffects.Count <= 0) return;

        foreach (OnHitEffector effect in currentEffects.ToArray())
        {
            applyEffect(effect);
        }
    }
    
    public void addEffect(OnHitEffector effect)
    {
        OnHitEffector existingEffect = findEffectInCurrentEffects(effect.effectName);

        if (existingEffect == null)
        {
            currentEffects.Add(effect.clone());
        }
        else
        {
            if(effect.refreshOnHit)
            {
                currentEffects.Add(effect.clone());
                removeCurrentEffect(existingEffect);
            }
        }
    }

    #region Apply Effect

    public void applyEffect(OnHitEffector effect)
    {
        if (EffectImunity)
        {
            removeCurrentEffect(effect);
            return;
        }

        effect.effectTimer -= Time.deltaTime;

        if (immunityList.Contains(effect.name))
        {
            return;
        }

        if (effect is Slow)
        {
            applySlowEffect((Slow) effect);
        }
        else if (effect is Stuned)
        {
            applyStunnedEffect((Stuned) effect);
        }
        else if (effect is DamageOverTime)
        {
            applyDamageOverTimeEffect((DamageOverTime) effect);
        }

        if (effect.effectTimer <= 0f)
        {
            removeCurrentEffect(effect);
            return;
        }
    }

    public virtual void applySlowEffect(Slow effect) 
    {
        
    }
    
    public virtual void applyStunnedEffect(Stuned effect) 
    {
        if (effect.stunSound != null && !effect.stunSoundPlayed)
        {
            RuntimeManager.PlayOneShot(effect.stunSound, transform.position);
            effect.stunSoundPlayed = true;
        }
    }
    public virtual void applyDamageOverTimeEffect(DamageOverTime effect) 
    {
    
    }
    #endregion
  
    #region Remove Effect
    protected virtual void removeCurrentEffect(OnHitEffector effect)
    {
        if (effect is Slow)
        {
            removeSlowEffect((Slow) effect);
        }
        else if (effect is Stuned)
        {
            removeStunnedEffect((Stuned) effect);
        }
        else if (effect is DamageOverTime)
        {
            removeDamageOverTimeEffect((DamageOverTime) effect);
        }        

        currentEffects.Remove(effect);
    }

    public virtual void removeSlowEffect(Slow effect) 
    {

    }
    
    public virtual void removeStunnedEffect(Stuned effect) 
    {
        
    }

    public virtual void removeDamageOverTimeEffect(DamageOverTime effect) 
    {
        
    }
    
    public virtual void removeAllEffects()
    {
        foreach (OnHitEffector effect in currentEffects.ToArray())
        {
            removeCurrentEffect(effect);
        }
        currentEffects.Clear();
    }

    #endregion

    #region Helper Functions
    private OnHitEffector findEffectInCurrentEffects(string EffectName)
    {
        foreach (OnHitEffector effect in currentEffects.ToArray())
        {
            if (effect.effectName.Equals(EffectName))
            {
                return effect;
            }
        }
        return null;
    }

    #endregion

}
