using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class JumpPad : MonoBehaviour
{
    public float jumpforce;
    [EventRef]
    public string bounceSound = null;
    //private bool additiveJump = false;
    private PlayerInput playerInput;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if(playerInput != null && playerInput.isGrounded())
        {
            playerInput.setControlsDisable(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerInput = collision.GetComponent<PlayerInput>();
            animator.StopPlayback();
            animator.Play("MushRoomJump");
            if(bounceSound != null)
            {
                RuntimeManager.PlayOneShot(bounceSound, transform.position);
            }
            playerInput.dashReset();
            playerInput.jump(jumpforce);
        }
        else
        {
            animator.Play("MushRoomJumpIdle");
        }

        playerInput = null;
    }
}
