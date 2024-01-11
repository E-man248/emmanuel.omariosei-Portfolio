using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairUI : MonoBehaviour
{
    [Header("Graphics Settings")]
    [SerializeField] private GameObject GraphicsObject;
    [SerializeField] private Image GraphicsImage;
    [SerializeField] private Sprite CrossHairSpriteDefault;
    [SerializeField] private List<SpriteTargetTagPair> crossHairSpriteTargetTagPairs;

    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = PlayerInfo.Instance?.GetComponent<PlayerInput>();

        // Disable UI if no Player or PlayerInput is Found:
        if (playerInput == null)
        {
            Hide();
            gameObject.SetActive(false);
            return;
        }

        Show();
    }
    
    private void Update()
    {
        if (Timer.gamePaused)
        {
            Hide();
            return;
        }

        if (playerInput.currentAimInputDevice != PlayerInput.AimInput.Mouse)
        {
            Hide();
            return;
        }

        UpdateCrossHairSprite();

        transform.position = GetAimPosition();

        Show();
    }

    private Vector3 GetAimPosition()
    {
        return Camera.main.WorldToScreenPoint(playerInput.getMousePosition());
    }

    private void UpdateCrossHairSprite()
    {
        string crosshairTargetTag = getCurrentTargetTag();

        Sprite targetCrossHairSprite = getCrossHairSprite(crosshairTargetTag);

        GraphicsImage.sprite = targetCrossHairSprite;
    }

    private string getCurrentTargetTag()
    {
        return CursorManager.Instance.targetObjectTag;
    }

    /// <summary>
    /// Gets the Crosshair Sprite for the <b>first</b> CrossHairSpriteTargetTagPair that has a match for the target's tag.<br/>
    /// The default sprite will be returned if a defined target tag sprite does not exist.
    /// </summary>
    /// <returns></returns>
    private Sprite getCrossHairSprite(string targetTag)
    {
        if (string.IsNullOrWhiteSpace(targetTag)) return CrossHairSpriteDefault;

        foreach (var pair in crossHairSpriteTargetTagPairs)
        {
            if (pair.targetTags.Contains(targetTag))
            {
                return pair.sprite;
            }
        }

        return CrossHairSpriteDefault;
    }

    public void Hide()
    {
        GraphicsObject.SetActive(false);
    }

    public void Show()
    {
        GraphicsObject.SetActive(true);
    }

    [Serializable]
    private struct SpriteTargetTagPair
    {
        public Sprite sprite;
        public List<string> targetTags;
    }
}
