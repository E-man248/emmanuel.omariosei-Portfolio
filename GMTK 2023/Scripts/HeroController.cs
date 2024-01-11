using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public TileBasedMovment tileBasedMovment;
    public Transform princessTransform;

    public int sightDistance = 5;

    public LayerMask monstersLayerMask;
    private CharacterManager characterManager;

    protected virtual void Start()
    {
        tileBasedMovment = GetComponent<TileBasedMovment>();
        princessTransform = FindObjectOfType<Princess>()?.transform;

        if (tileBasedMovment == null) Debug.LogError(name + "could not find tileBasedMovment");
        if (princessTransform == null) Debug.LogError(name + "could not find princess");

        GameStateManger.Instance.PlayerActionPerformedEvent.AddListener(OnPlayerActionPerformed);
        characterManager = GameStateManger.Instance.GetComponent<CharacterManager>();
    }

    private void OnEnable()
    {
       //GameStateManger.Instance.PlayerActionPerformedEvent.AddListener(OnPlayerActionPerformed);
    }

    private void OnDisable()
    {
       GameStateManger.Instance.PlayerActionPerformedEvent.RemoveListener(OnPlayerActionPerformed);
    }

    private void OnDestroy()
    {
       GameStateManger.Instance.PlayerActionPerformedEvent.RemoveListener(OnPlayerActionPerformed);
    }

    private void OnPlayerActionPerformed()
    {
        Debug.Log("Action Performed!");

        Transform targetTransform = determineTarget();

        float distance = Vector2.Distance(targetTransform.position, transform.position);

        if (distance > 1f)
        {
            moveToward(targetTransform.position);
        }
        else
        {
            Debug.Log("Inside Here");
            if (targetIsMonster(targetTransform))
            {
                if(targetTransform.GetComponent<SlimeController>() && (GameStateManger.Instance.currentTime == GameStateManger.DayNightState.Day))
                {
                    targetTransform.GetComponent<Health>()?.changeHealth(-1);
                    characterManager.characterControllers.Remove(targetTransform.GetComponent<CharacterController>());
                    
                }
                if (GameStateManger.Instance.currentTime == GameStateManger.DayNightState.Night)
                {
                    targetTransform.GetComponent<Health>()?.changeHealth(-1);
                    characterManager.characterControllers.Remove(targetTransform.GetComponent<CharacterController>());

                }
                else
                {
                    transform.GetComponent<Health>().changeHealth(-1);
                }
            }
        }
    }

    protected Transform determineTarget()
    {
        var result = princessTransform;

        var castHitList = Physics2D.CircleCastAll(transform.position, sightDistance, Vector2.right, 0f, monstersLayerMask);

        foreach (var castHit in castHitList)
        {
            if (castHit == true && castHit.transform != princessTransform)
            {
                result = castHit.transform;
                break;
            }
        }

        return result;
    }

    protected void moveToward(Vector2 targetPosition)
    {
        Vector3 direction = (targetPosition - new Vector2(transform.position.x, transform.position.y)).normalized;

        print("Direction:" + direction);

        // Get the absolute values of the vector's components
        float x = Mathf.Abs(direction.x);
        float y = Mathf.Abs(direction.y);

        // Determine the dominant direction
        if (x > y)
        {
            // If x is dominant, set y to 0 and normalize x
            direction = new Vector3(Mathf.Sign(direction.x), 0f);
        }
        else
        {
            // If y is dominant, set x to 0 and normalize y
            direction = new Vector3(0f, Mathf.Sign(direction.y),0f);
        }

        tileBasedMovment.moveAnyDirection(direction);
    }

    public bool targetIsMonster(Transform transform)
    {
        return transform.GetComponent<CharacterController>() != null && transform.GetComponent<Princess>() == null;
    }
}
