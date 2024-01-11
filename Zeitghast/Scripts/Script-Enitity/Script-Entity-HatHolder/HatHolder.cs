using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HatHolder : MonoBehaviour
{
    internal HatInfo EquippedHat {get; private set;}
    [field: SerializeField] public bool RequireEquippedHatToBeUnlocked {get; private set;} = false;
    [SerializeField] private HatInfo DefaultHat;

    [Header("Hat Holder Events")]
    public UnityEvent<HatInfo> hatEquipped;
    public UnityEvent<HatInfo> hatRemoved;

    protected virtual void Start()
    {
        Equip(DefaultHat, true);
    }

    public void Equip(HatInfo hatToEquip, bool forceEquip = false)
    {
        if (hatToEquip != null)
        {
            if (!forceEquip)
            {
                if (RequireEquippedHatToBeUnlocked && !isEquippedHatUnlocked(hatToEquip))
                {
                    return;
                }
            }
        }

        if (EquippedHat != null)
        {
            RemoveEquippedHat();
        }

        EquippedHat = hatToEquip;

        hatEquipped.Invoke(hatToEquip);
    }

    public HatInfo RemoveEquippedHat()
    {
        HatInfo removedHat = EquippedHat;

        EquippedHat = null;

        hatRemoved.Invoke(removedHat);

        return removedHat;
    }

    private bool isEquippedHatUnlocked(HatInfo hat)
    {
        return HatManager.Instance?.GetHatUnlockStatus(hat.hatId) ?? false;
    }
}
