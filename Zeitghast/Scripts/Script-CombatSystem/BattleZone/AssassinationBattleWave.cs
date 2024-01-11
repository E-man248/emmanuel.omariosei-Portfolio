using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assassination Battle Wave")]
public class AssassinationBattleWave : BattleWave
{
    [Header("Assassination Target")]
    public int assassinationTargetIndex = 0;
    public GameObject assassinationTargetGlow;

    public override void updateWaveComplete()
    {
        if (activeEnemies[assassinationTargetIndex] == null)
        {
            WaveComplete = true;
        }
    }

    public override BattleWave clone()
    {
        AssassinationBattleWave clone = (AssassinationBattleWave) AssassinationBattleWave.CreateInstance("AssassinationBattleWave");
        clone.name = "Assassination Battle Wave Clone";
        clone.assassinationTargetIndex = assassinationTargetIndex;
        clone.assassinationTargetGlow = assassinationTargetGlow;
        
        clone.BattleWaveEnemies = BattleWaveEnemies;
        clone.activeEnemies = new List<GameObject>();
        clone.WaveComplete = false;

        return clone;
    }
}
