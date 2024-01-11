using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntagBossState : BossAttackState
{
    [Serializable]
    public struct EnemySpawn
    {
        public GameObject enemyToSpawn;
    }

    public AntagBoss.AttackState state;
    public List<EnemySpawn> enemySpawnList;

    public override string ToString()
    {
        return gameObject.name + "'s " + state.ToString() + " State";
    }
}
