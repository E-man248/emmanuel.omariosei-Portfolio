using UnityEngine;

public class PlayerLasersight : MonoBehaviour
{
    [Header("Lasersight Settings")]
    [SerializeField] private Vector2 firingPointOffset;
    private WeaponManager weaponManager;
    public bool displayActive {get; private set;}

    private void Awake()
    {
        // Retrieve Weapon Manager Script from Parent:
        weaponManager = GetComponentInParent<WeaponManager>();
    }

    private void Update()
    {
        // If no weapon manager is present, disable lasersight script:
        if (weaponManager == null)
        {
            Disable();
            return;
        }

        Weapon currentWeapon = weaponManager.currentWeapon;

        // Toggle lasersight visibility based on whether current weapon can shoot bullets:
        if (currentWeapon != null && WeaponCanShoot(currentWeapon))
        {
            Vector2 weaponFiringPoint = GetWeaponFiringPoint(currentWeapon);

            Show(weaponFiringPoint, firingPointOffset + currentWeapon.additionalLasersightOffset);
        }
        else
        {
            Hide();
        }
    }

    #region Laser Sight Display Functions

    private void Show(Vector2 weaponFiringPoint, Vector2 offset)
    {
        CalibratePosition(weaponFiringPoint, offset);

        displayActive = true;
    }

    private void CalibratePosition(Vector2 weaponFiringPoint, Vector2 offset)
    {
        transform.position = weaponFiringPoint;
        transform.localPosition += (Vector3) offset;
    }

    private void Hide()
    {
        displayActive = false;
    }

    /// <summary>
    /// Disable Lasersight functionality until re-enabled
    /// </summary>
    private void Disable()
    {
        gameObject.SetActive(false);
    }

    #endregion
    
    #region Weapon Data Access

    private bool WeaponCanShoot(Weapon weapon)
    {
        // Check General Weapon Shooting:
        if (!weapon.canShoot || !weapon.canInstantiateBullet())
        {
            return false;
        }

        // Check Shot Type Specific Shooting:
        switch(weapon.ShotType)
        {
            case StandardShotType:
            return standardShotWeaponCanShoot( (StandardShotType) weapon.ShotType);

            case ChargedShotType:
            return chargedShotWeaponCanShoot( (ChargedShotType) weapon.ShotType);

            default:
            return true;
        }
    }

    private bool standardShotWeaponCanShoot(StandardShotType standardShot)
    {
        return standardShot.fireRateTimer < 0f && !standardShot.isOverheating;
    }

    private bool chargedShotWeaponCanShoot(ChargedShotType chargedShot)
    {
        return chargedShot.baseFireRateTimer < 0f || chargedShot.currentChargeIndex > 0;
    }

    private Vector2 GetWeaponFiringPoint(Weapon weapon)
    {
        return weapon.FiringPoint.position;
    }

    #endregion
}
