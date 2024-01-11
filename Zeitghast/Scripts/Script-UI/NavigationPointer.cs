using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPointer : MonoBehaviour
{
    private const float REFERENCE_SCREEN_WIDTH = 1920f;
    private const float REFERENCE_SCREEN_HEIGHT = 1080f;

    [Header("Pointer Settings:")]
    public Transform target;

    [SerializeField] private Vector2 screenEdgeOffset = Vector2.zero;
    [SerializeField] private Vector2 hideDistanceOffset = Vector2.zero;
    
    [Header("Arrow Settings:")]
    [SerializeField] private GameObject arrowText;
    [SerializeField] private GameObject pointerArrow;

    private void Start()
    {
        target = GetNextLevelDoor();
    }
    
    private void Update()
    {
        if (IsTargetInFieldOfView())
        {
            HidePointer();
        }
        else
        {
            ShowPointer();
        }
    }

    private void HidePointer()
    {
        // Hide Pointer from Display:
        arrowText.SetActive(false);
        pointerArrow.SetActive(false);
    }

    private void ShowPointer()
    {
        // Set Pointer Position:
        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(target.position);
        transform.position = GetPointerPosition(targetScreenPosition);
        
        // Set Up Pointer Arrow:
        pointerArrow.transform.eulerAngles = GetArrowRotation(targetScreenPosition);

        // Show Pointer on Display:
        arrowText.SetActive(true);
        pointerArrow.SetActive(true);
    }

    private Transform GetNextLevelDoor()
    {
        // Get Next Level in Progression: (Can be Null or Empty)
        string nextLevel = GameManager.Instance.GetNextLevelInProgression();

        if (string.IsNullOrWhiteSpace(nextLevel))
        {
            return null;
        }

        if (!AdvancedSceneManager.GetAllLevelSceneNames().Contains(nextLevel))
        {
            return null;
        }

        // Get Available Level Entry Doors in Scene:
        var levelEntryDoors = GameObject.FindObjectsOfType<LevelEntryDoor>();

        // Find Level Entry Door for Next Level:
        LevelEntryDoor nextLevelDoor = levelEntryDoors.First( x => x.baseLevelScene == nextLevel );

        return nextLevelDoor.transform;
    }

    private Vector3 GetArrowRotation(Vector3 targetScreenPosition)
    {
        // Get Direction Vector Pointing to Target Position:
        Vector3 directionVector = targetScreenPosition - transform.position;

        // Use Direction Vector to Get Rotation Angle and Vector:
        float zRotation = Vector2.SignedAngle(Vector2.right, directionVector);
        Vector3 rotationVector = new Vector3(0f, 0f, zRotation);

        return rotationVector;
    }

    private Vector3 GetPointerPosition(Vector3 targetScreenPosition)
    {
        Vector3 result = new Vector3(GetPointerPositionX(targetScreenPosition), GetPointerPositionY(targetScreenPosition), 0f); 

        return result;
    }

    private float GetPointerPositionX(Vector3 targetScreenPosition)
    {
        float xCenter = Screen.width / 2;
        float halfScreenWidth = Screen.width / 2 - screenEdgeOffset.x * Screen.width / REFERENCE_SCREEN_WIDTH;

        float xDistanceFromCenter = targetScreenPosition.x - xCenter;
        xDistanceFromCenter = Mathf.Clamp(xDistanceFromCenter, -halfScreenWidth, halfScreenWidth);

        return xCenter + xDistanceFromCenter;
    }

    private float GetPointerPositionY(Vector3 targetScreenPosition)
    {
        float yCenter = Screen.height / 2;
        float halfScreenHeight = Screen.height / 2 - screenEdgeOffset.y * Screen.height / REFERENCE_SCREEN_HEIGHT;

        float yDistanceFromCenter = targetScreenPosition.y - yCenter;
        yDistanceFromCenter = Mathf.Clamp(yDistanceFromCenter, -halfScreenHeight, halfScreenHeight);

        return yCenter + yDistanceFromCenter;
    }

    private bool IsTargetInFieldOfView()
    {
        if (target == null) return true;

        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(target.position);
        
        Vector2 hideDistance = new Vector2(Screen.width / 2, Screen.height / 2) + new Vector2(hideDistanceOffset.x * Screen.width / REFERENCE_SCREEN_WIDTH, hideDistanceOffset.y * Screen.height / REFERENCE_SCREEN_HEIGHT);

        // Check Outside of Bounds X-Wise:
        float xDistanceFromCenter = Mathf.Abs(targetScreenPosition.x - Screen.width / 2);
        if (xDistanceFromCenter > hideDistance.x)
        {
            return false;
        }

        // Check Outside of Bounds Y-Wise:
        float yDistanceFromCenter = Mathf.Abs(targetScreenPosition.y - Screen.height / 2);
        if (yDistanceFromCenter > hideDistance.y)
        {
            return false;
        }

        return true;
    }
}
