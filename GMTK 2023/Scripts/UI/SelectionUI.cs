using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUI : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField]private GameObject graphics;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (graphics == null) return;

        if(characterController.charcterIsActive)
        {
            graphics.SetActive(true);
        }
        else
        {
            graphics.SetActive(false);
        }
    }
}
