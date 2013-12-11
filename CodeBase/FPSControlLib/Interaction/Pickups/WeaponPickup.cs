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
        public bool ThisIsAWeapon = true;
        public bool ThisIsAClip = false;
        public int clips = 1;
        //public FPSControlRangedWeaponType weaponType;

        public override void Interact()
        {
            if (ThisIsAWeapon)
            {
                if (weaponManager.CanAddWeapon(weaponName))
                {   
                    weaponManager.AddToInventory(weaponName, equip);
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Debug.LogWarning("Weapon \"" + weaponName + "\" could not be added.");
                }
            }

            if (ThisIsAClip)
            {
                FPSControlRangedWeapon weapon = (FPSControlRangedWeapon)FPSControlPlayerData.GetWeapon(weaponName);
                weapon.AddAmmo(clips);
                //Debug.Log("------- Destroying " + transform.parent.gameObject.name);
                Destroy(transform.parent.gameObject);
            }
        } 
    }
}
