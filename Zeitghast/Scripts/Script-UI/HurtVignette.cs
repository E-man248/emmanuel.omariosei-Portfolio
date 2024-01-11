using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtVignette : MonoBehaviour
{
    private PlayerKnockback playerKnockback;
    private Animator animator;
    private PlayerHealth playerHealth;
    [SerializeField] private float hurtVignetteCoolDown = 0.1f;
    private float hurtVignetteCoolDownTimer;
    // Start is called before the first frame update
    void Start()
    {
        playerKnockback = PlayerInfo.Instance.GetComponent<PlayerKnockback>();
        playerHealth = PlayerInfo.Instance.GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
        playerHealth.onHurt += triggerHurtVignetteAnimation;
    }

    private void OnEnable()
    {
        if (playerHealth == null) return;
        playerHealth.onHurt += triggerHurtVignetteAnimation;
    }
    private void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.onHurt -= triggerHurtVignetteAnimation;
    }
    private void OnDestroy()
    {
        if (playerHealth == null) return;
        playerHealth.onHurt -= triggerHurtVignetteAnimation;
    }


    // Update is called once per frame
    void Update()
    {
        hurtVignetteCoolDownTimer -= Time.deltaTime;

        if (hurtVignetteCoolDownTimer > 0f)
        {
            animator.SetBool("IsHurt",true);
        }
        else
        {
            animator.SetBool("IsHurt", false);
        }

        if(playerHealth.isDead)
        {
            animator.SetBool("IsDead", true);
        }
        else
        {
            animator.SetBool("IsDead", false);
        }
    }

    public bool isHit()
    {
        return playerKnockback.knockbackTimer > 0f;
    }

    private void triggerHurtVignetteAnimation()
    {
        hurtVignetteCoolDownTimer = hurtVignetteCoolDown;
    }
}
