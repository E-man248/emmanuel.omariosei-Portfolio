using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Battle Wave")]
public class BossBattleWave : BattleWave
{
    private Boss waveBoss;
    public override void updateWaveComplete()
    {
        if (waveBoss != null) return; 
        
        foreach (var enemy in activeEnemies.ToArray())
        {
            if (enemy.GetComponentInChildren<Boss>() != null)
            {
                return;
            }
        }
            
        WaveComplete = true;
    }

    public override BattleWave clone()
    {
        BossBattleWave clone = (BossBattleWave) BossBattleWave.CreateInstance("BossBattleWave");
        clone.name = "Boss Battle Wave Clone";
        
        clone.BattleWaveEnemies = BattleWaveEnemies;
        clone.activeEnemies = new List<GameObject>();
        clone.WaveComplete = false;

        return clone;
    }
}
