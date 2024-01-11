using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField] private bool canHideCursor = true;
    [SerializeField] private Texture2D cursorTexture;

    internal string targetObjectName;
    internal string targetObjectTag;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (AdvancedSceneManager.SceneIsMenu(AdvancedSceneManager.Instance.getCurrentScene()))
        {
            ShowCursor();
            return;
        }

        if (Timer.gamePaused)
        {
            ShowCursor();
            return;
        }

        if (IsCursorOffScreen())
        {
            ShowCursor();
            return;
        }

        HideCursor();
    }

    private bool IsCursorOffScreen()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Vector2 diff = new Vector2(Mathf.Abs(mousePosition.x - screenCenter.x), Mathf.Abs(mousePosition.y - screenCenter.y));
        
        if (diff.x >= Screen.width / 2 || diff.y >= Screen.height / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
    }

    private void HideCursor()
    {
        if (!canHideCursor) return;
        
        Cursor.visible = false;
    }
}
