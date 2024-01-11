using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    public enum State
    { 
        Attack,
        Idle,
        Dead,
    }

    public enum LookDirection
    {
        Left = -1,
        Right = 1
    }
    
    public string bossName;

    public State currentState;

    [Header("Spawn Portal Settings:")]
    public Vector2 spawnPortalScale = Vector2.one;

    internal BossHealth bossHealth { get; private set;}

    internal List<BossAttackPhase> bossAttackPhases;
    protected BossAttackPhase currentBossAttackPhase;
    protected int currentBossPhaseIndex;

    protected BossAttackState currentBossAttackState;
    protected bool CanMoveToNextAttackState = false;

    internal LookDirection lookDirection;

    // Utility Variables:
    protected GroundedCheck groundedCheck;

    #region Unity Functions

    protected virtual void Awake()
    {
        currentState = State.Idle;
        SwitchState(State.Idle);
    }

    protected virtual void Start()
    {
        // Get Utilites:
        RetrieveUtilityComponents();
        
        // Subscribe to Event:
        subscribeToEvents();

        // Setup Boss Phases:
        SetupBossPhases();

        // Setup Boss State Cycle:
        SetupStateCycle();

        orientateToPlayer();
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case State.Attack:
            {
                RunAttackStateCycle();
                break;
            }
                
            case State.Idle:
            {
                RunIdleState();
                break;
            }
                
            case State.Dead:
            {
                RunDeadState();
                break;
            }
                
            default:
            {
                // Code to execute if none of the cases match
                Debug.LogError(currentState.ToString() + " state is not implemented");
                break;
            } 
        }
    }


    protected virtual void OnEnable()
    {
        subscribeToEvents();
    }

    protected virtual void OnDisable()
    {
        unsubscribeToEvents();
    }

    protected virtual void OnDestroy()
    {
        unsubscribeToEvents();
        CleanUpBoss();
    }

    #endregion

    #region Event Functions 
    protected virtual void subscribeToEvents()
    {
        if (bossHealth != null) bossHealth.onDeathEvent.AddListener(CleanUpBoss);
    }

    protected virtual void unsubscribeToEvents()
    {
        if (bossHealth != null) bossHealth.onDeathEvent.RemoveListener(CleanUpBoss);
    }

    #endregion

    #region State management
    public void SwitchState(State incomingState)
    {
        invokefunction(currentState.ToString() + "StateExit");

        currentState = incomingState;

        invokefunction(currentState.ToString() + "StateEntry");
    }

    private void invokefunction(string functionName)
    {
        try
        {
            gameObject.SendMessage(functionName);
        }
        catch (Exception)
        {
            Debug.LogError(functionName + " could not be found");
        }
    }


    #endregion

    #region Boss Attack State
    protected virtual void AttackStateEntry()
    {
        //print("AttackStateEntry");
    }

    protected virtual void AttackStateExit()
    {
        //print("AttackStateExit");
    }

    protected virtual void RunAttackStateCycle()
    {
        if (CanMoveToNextPhase())
        {
            // Switch the Current Boss Phase to the Next in Order:
            currentBossAttackPhase = GetNextPhase();
            currentBossPhaseIndex++;

            // Activate the New Current Boss Phase:
            currentBossAttackState = currentBossAttackPhase.GetNextBossState();
        }
        else
        {
            if (CanMoveToNextAttackState) // Condition Set True When State Complete
            {
                CanMoveToNextAttackState = false;

                // Ask Current Boss Phase for the Next Boss State:
                currentBossAttackState = currentBossAttackPhase.GetNextBossState();
            }
            else
            {
                // Unpack and Run Current Boss State:
                RunCurrentAttackState(currentBossAttackState);
            }
        }
    }

    protected virtual void PopulateBossPhases()
    {
        // Find and Store Boss Phases:
        bossAttackPhases = GetComponentsInChildren<BossAttackPhase>().ToList();
    }

    private bool CanMoveToNextPhase()
    {
        BossAttackPhase nextPhase = GetNextPhase();

        if (nextPhase == null) return false;

        // Check the Health Threshold of the Next Phase:
        if (bossHealth.health <= nextPhase.HealthThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void CurrentStateComplete()
    {
        CanMoveToNextAttackState = true;
    }

    private BossAttackPhase GetNextPhase()
    {
        if (currentBossPhaseIndex+1 >= bossAttackPhases.Count)
        {
            return null;
        }

        return bossAttackPhases[currentBossPhaseIndex+1];
    }

    private void SetupBossPhases()
    { 
        // Get Boss Phases:
        PopulateBossPhases();

        // Set and Activate Initial Boss Phase:
        currentBossAttackPhase = bossAttackPhases.First();
    }

    protected virtual void SetupStateCycle()
    { 
        // Set to True to Get Initial State:
        CanMoveToNextAttackState = true;
    }

    protected abstract void RunCurrentAttackState(BossAttackState bossAttackState);

    protected virtual void CleanUpBoss()
    {

    }

    #endregion

    #region  Idle State
    protected virtual void IdleStateEntry()
    {
        //print("IdleStateEntry");
    }

    protected virtual void IdleStateExit()
    {
        //print("IdleStateExit");
    }

    protected virtual void RunIdleState()
    {
        //SwitchState(State.Attack);
    }
    #endregion

    #region  Dead State
    protected virtual void DeadStateEntry()
    {
        //print("DeadStateEntry");
    }

    protected virtual void DeadStateExit()
    {
        //print("DeadStateExit");
    }


    protected virtual void RunDeadState()
    {
        
    }
    #endregion

    #region Helper Functions

    protected void RetrieveUtilityComponents()
    {
        bossHealth = GetComponent<BossHealth>();
        groundedCheck = GetComponentInChildren<GroundedCheck>();
    }

    protected void orientateToPlayer()
    {
        Vector2 playerDirection = getPlayerDirection();

        orientate(playerDirection);
    }

    protected Vector2 getPlayerDirection()
    {
        return PlayerInfo.Instance.transform.position - transform.position;
    }

    public bool IsFacingPlayer()
    {
        Vector2 playerDirection = PlayerInfo.Instance.transform.position - transform.position;

        if (Mathf.Sign(playerDirection.x) != (int) lookDirection)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected void orientate(Vector2 direction)
    {
        lookDirection = (LookDirection)Mathf.Sign(direction.x);
    }

    public bool isGrounded()
    {
        return groundedCheck.isGrounded();
    }

    #endregion



}
