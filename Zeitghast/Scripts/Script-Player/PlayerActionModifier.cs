using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAction
{
    WallJump
}
public enum playerActionModification
{
    Enable,
    Disable
}
public class PlayerActionModifier : MonoBehaviour
{
    private PlayerInput playerInput;
    public playerActionModification playerActionModification;
    public PlayerAction playerAction;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = PlayerInfo.Instance.GetComponent<PlayerInput>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        if(collision.tag != "Player")
        {
            return;
        }

        switch (playerAction)
        {
            case PlayerAction.WallJump:
                if(playerActionModification == playerActionModification.Enable)
                {
                    playerInput.WallJumpEnabled = true;
                }
                else if(playerActionModification == playerActionModification.Disable)
                {
                    playerInput.WallJumpEnabled = false;
                }
                break;
        }
    }
}
