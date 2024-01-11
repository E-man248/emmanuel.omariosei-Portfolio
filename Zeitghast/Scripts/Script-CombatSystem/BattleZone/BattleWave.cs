using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BattleWaveEnemy
{
    public GameObject gameObject;
    public Vector3 position;
}

public abstract class BattleWave : ScriptableObject
{
    [HideInInspector] public bool WaveComplete;
    public List<BattleWaveEnemy> BattleWaveEnemies;
    /*[HideInInspector]*/ public List<GameObject> activeEnemies;

    public virtual void updateWaveComplete()
    {
        
    }

    public virtual BattleWave clone()
    {
        BattleWave clone = (BattleWave)BattleWave.CreateInstance("BattleWave");
        clone.name = "Battle Wave Clone";
        
        clone.BattleWaveEnemies = BattleWaveEnemies;
        clone.activeEnemies = new List<GameObject>();
        clone.WaveComplete = false;

        return clone;
    }
}
