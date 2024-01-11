using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssistPointerUI : MonoBehaviour
{
    [Header("Graphics Settings")]
    [SerializeField] private GameObject GraphicsObject;

    // Utilities:
    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = PlayerInfo.Instance?.GetComponentInChildren<WeaponManager>();

        // Disable UI if no Weapon Manager is Found:
        if (weaponManager == null)
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

        Transform aimTarget = GetCurrentAimTarget();

        if (IsAimAssistActive() && aimTarget != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(aimTarget.position);
            Show();
        }
        else
        {
            Hide();
        }
    }

    private bool IsAimAssistActive()
    {
        return DataPersistanceManager.Instance.GetOptionsData().aimAssistStrengthValue > 0f;
    }

    private Transform GetCurrentAimTarget()
    {
        return weaponManager.currentAimAssistTarget?.GetComponentInParent<Health>()?.transform;
    }

    public void Hide()
    {
        GraphicsObject.SetActive(false);
    }

    public void Show()
    {
        GraphicsObject.SetActive(true);
    }
}
