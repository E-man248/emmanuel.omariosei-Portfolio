using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLasersightAnimationHandler : EntityAnimationHandler
{
    private PlayerLasersight playerLasersight;

    [Header("Lasersight Animation Handler Settings")]
    [SerializeField] private GameObject SpriteGroup;

    private void Start()
    {
        playerLasersight = GetComponentInParent<PlayerLasersight>();
    }

    protected override void animate()
    {
        if (playerLasersight.displayActive)
        {
            SpriteGroup.SetActive(true);
        }
        else
        {
            SpriteGroup.SetActive(false);
        }
    }
}
