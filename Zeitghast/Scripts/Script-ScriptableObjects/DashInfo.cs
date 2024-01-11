using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashInfo")]
public class DashInfo : ScriptableObject
{
    [System.Serializable]
    public struct spawnBulletInfo
    {
        public GameObject bulletToSpawn;
        public parentOfSpawnInfo parentOfSpawn;
    };
    
    [System.Serializable]
    public enum parentOfSpawnInfo
    {
        None,
        Player,
        WeaponFiringPoint,
    };

    [Header("Dash")]
    public float dashSpeedMultiplier;
    public float dashVariableSpeedMultiplier = 0.5f;
    public float startDashTime;
    public float dashCoolDown;
    public GameObject playerDashParticles;

    [Header("Dash Assists")]
    public float groundCoolDownMultiplier = 1;
    public float extraDashAimTime;
    public float afterDashControl = 1;

    [Header("Dash Attack")]
    public spawnBulletInfo spawnBulletStartOfDash;
    public spawnBulletInfo spawnBulletInDashCollision;
    public spawnBulletInfo spawnBulletEndOfDash;
}
