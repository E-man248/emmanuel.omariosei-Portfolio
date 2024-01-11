using System;
using UnityEngine;

[Serializable]
public abstract class BossAttackState : MonoBehaviour
{
    public enum Type
    {
        Random,
        Health
    }

    public Type type;

    public override string ToString()
    {
        return gameObject.name + "'s " + type.ToString() + " State";
    }
}
