using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Genocide Battle Wave")]
public class GenocideBattleWave : BattleWave
{
    public override void updateWaveComplete()
    {
        WaveComplete = false;

        foreach (var enemy in activeEnemies.ToArray())
        {
            if (enemy != null)
            {
                return;
            }
            else
            {
                activeEnemies.Remove(enemy);
            }
        }

        WaveComplete = true;
    }

    public override BattleWave clone()
    {
        GenocideBattleWave clone = (GenocideBattleWave) GenocideBattleWave.CreateInstance("GenocideBattleWave");
        clone.name = "Genocide Battle Wave Clone";
        
        clone.BattleWaveEnemies = BattleWaveEnemies;
        clone.activeEnemies = new List<GameObject>();
        clone.WaveComplete = false;

        return clone;
    }
}
