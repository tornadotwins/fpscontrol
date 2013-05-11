using System.Collections;
using UnityEngine;
using FPSControl;

namespace FPSControl
{
    class WeaponPickup : InteractiveObject
    {
        public FPSControlPlayerWeaponManager weaponManager;
        public string weaponName;
        public bool equip = true;

        public override void Interact()
        {
            if (weaponManager.CanAddWeapon(weaponName))
            {
                weaponManager.Add(weaponName, equip);
            }
            else
            {
                Debug.LogWarning("Weapon \"" + weaponName + "\" could not be added.");
            }
        } 
    }
}
