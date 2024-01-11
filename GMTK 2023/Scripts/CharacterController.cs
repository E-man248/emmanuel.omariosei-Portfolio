using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public TileBasedMovment tileBasedMovment;
    public bool charcterIsActive { get; private set; } = false;

    [SerializeReference] private float inputDelay = 0.3f;
    protected bool InputEnabled = true;
    // Start is called before the first frame update

    [SerializeReference] private List<Action> actionList;
    [SerializeField] private LayerMask targetLayers;

    protected Action upAction;
    protected Transform upTargetTransform;

    protected Action downAction;
    protected Transform downTargetTransform;

    protected Action leftAction;
    protected Transform leftTargetTransform;

    protected Action rightAction;
    protected Transform rightTargetTransform;

    private ActionUI actionUI;


    protected virtual void Awake()
    {
        charcterIsActive = false;
    }
    protected virtual void Start()
    {
        tileBasedMovment = GetComponent<TileBasedMovment>();
        actionUI = GetComponentInChildren<ActionUI>();

        if (actionUI == null) Debug.LogWarning(name + "could not find actionUI");
        if (tileBasedMovment == null) Debug.LogError(name + "could not find tileBasedMovment");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        manageMovement();
    }

    protected virtual void manageMovement()
    {
        if (!charcterIsActive)
        {
            ClearAllActions();
            return;
        }

        if (!InputEnabled) return;
        

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(DisableInputForDelay());

            if(upAction != null)
            {
                upAction.performAction(transform,upTargetTransform);
            }
            else
            {
                tileBasedMovment.moveUp();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(DisableInputForDelay());
            if (leftAction != null)
            {
                leftAction.performAction(transform, leftTargetTransform);
            }
            else
            {
                tileBasedMovment.moveLeft();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(DisableInputForDelay());

            if (downAction != null)
            {
                downAction.performAction(transform, downTargetTransform);
            }
            else
            {
                tileBasedMovment.moveDown();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(DisableInputForDelay());

            if (rightAction != null)
            {
                rightAction.performAction(transform, rightTargetTransform);
            }
            else
            {
                tileBasedMovment.moveRight();
                GameStateManger.Instance.PlayerActionPerformedEvent.Invoke();
            }
        }

        checkPossibleActions();
    }

    protected IEnumerator DisableInputForDelay()
    {

        // Disable input
        InputEnabled = false;

        // Wait for the specified delay
        yield return new WaitForSeconds(inputDelay);

        // Enable input
        InputEnabled = true;
    }

    public void toggleCharacterActive(bool value)
    {
        charcterIsActive = value;
    }

    protected void checkPossibleActions()
    {
        if (!tileBasedMovment.hasReachedTargetPosition()) return;

        Collider2D upCollider = getCollision(TileBasedMovementManager.Instance.OneTileUp + transform.position);
        Collider2D downCollider = getCollision(TileBasedMovementManager.Instance.OneTileDown + transform.position);
        Collider2D leftCollider = getCollision(TileBasedMovementManager.Instance.OneTileLeft + transform.position);
        Collider2D rightCollider = getCollision(TileBasedMovementManager.Instance.OneTileRight + transform.position);

        ClearAllActions();
        
        //Checking posible up action
        foreach (var action in actionList)
        {
            if (upCollider == null) continue;

            if (action.ActionDoable(transform,upCollider.transform))
            {
                upAction = action;
                upTargetTransform = upCollider.transform;

                actionUI.changeUpSprite(upAction.actionSprite);
                break;
            }
        }

        //Checking posible left action
        foreach (var action in actionList)
        {
            if (leftCollider == null) continue;

            if (action.ActionDoable(transform, leftCollider.transform))
            {
                leftAction = action;
                leftTargetTransform = leftCollider.transform;

                actionUI.changeLeftSprite(leftAction.actionSprite);
                break;
            }
        }

        //Checking posible up action
        foreach (var action in actionList)
        {
            if (rightCollider == null) continue;

            if (action.ActionDoable(transform, rightCollider.transform))
            {
                rightAction = action;
                rightTargetTransform = rightCollider.transform;

                actionUI.changeRightSprite(rightAction.actionSprite);
                break;
            }
        }

        //Checking posible down action
        foreach (var action in actionList)
        {
            if (downCollider == null) continue;

            if (action.ActionDoable(transform, downCollider.transform))
            {
                downAction = action;
                downTargetTransform = downCollider.transform;

                actionUI.changeDownSprite(downAction.actionSprite);
                break;
            }
        }
    }

    protected void ClearAllActions()
    {
        //Clearing up actions
        upAction = null;
        upTargetTransform = null;
        actionUI.changeUpSprite(null);

        //Clearing left actions
        leftAction = null;
        leftTargetTransform = null;
        actionUI.changeLeftSprite(null);

        //Clearing right actions
        rightAction = null;
        rightTargetTransform = null;
        actionUI.changeRightSprite(null);

        //Clearing down actions
        downAction = null;
        downTargetTransform = null;
        actionUI.changeDownSprite(null);
    }
    private Collider2D getCollision(Vector3 position)
    {
        Collider2D hitinfo = Physics2D.OverlapCircle(position, 0.2f, targetLayers);
        return hitinfo;
    }
}
