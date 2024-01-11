using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable] [DisallowMultipleComponent]
public class BossAttackPhase:MonoBehaviour
{
    [SerializeField] public int HealthThreshold = 0;
    private BossAttackState HealthThresholdState;
    private bool HasPerformedHealthThreshold = false;

    private List<BossAttackState> RandomStates;
    private BossAttackState lastRandomState = null;

    #region Unity Functions

    protected virtual void Start()
    {
        // Get All States:
        var BossStates = GetComponents<BossAttackState>();

        // Filter Random States:
        RandomStates = BossStates.Where( state => state.type == BossAttackState.Type.Random ).ToList();

        // Filter Health Threshold State:
        try
        {
            HealthThresholdState = BossStates.SingleOrDefault( state => state.type == BossAttackState.Type.Health );
        }
        catch (Exception)
        {
            Debug.LogError("There needs to be ONE (and only one) Health Threshold State in " + this);
            HealthThresholdState = null;
        }

        HasPerformedHealthThreshold = false;
    }

    #endregion

    #region Boss State Cycle

    public BossAttackState GetNextBossState()
    {
        BossAttackState nextState;

        if (HealthThresholdState == null)
        {
            HasPerformedHealthThreshold = true;
        }

        if (!HasPerformedHealthThreshold)
        {
            HasPerformedHealthThreshold = true;

            nextState = HealthThresholdState;
        }
        else
        {
            nextState = GetRandomState();
        }

        return nextState;
    }

    private BossAttackState GetRandomState()
    {
        if (RandomStates.Count <= 0)
        {
            Debug.LogWarning("Boss Phase: " + this + " has No States!");
            return null;
        }

        if (RandomStates.Count == 1)
        {
            lastRandomState = RandomStates.First();
            return RandomStates.First();
        }

        List<BossAttackState> tempList = new List<BossAttackState>(RandomStates);

        //Removes the last State for the  possible pool
        if (lastRandomState != null) tempList.Remove(lastRandomState);
        
        lastRandomState = tempList.GetRandomElement();
        return lastRandomState;
    }

    #endregion

    public override string ToString()
    {
        return gameObject.name;
    }
}
