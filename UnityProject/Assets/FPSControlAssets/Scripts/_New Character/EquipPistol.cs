using UnityEngine;
using System.Collections;
using FPSControl;
[RequireComponent(typeof(FPSControlPlayerWeaponManager))]
public class EquipPistol : MonoBehaviour 
{
	// Use this for initialization
	void Start ()
    {
        if(enabled)
        {
            FPSControlPlayerWeaponManager weapons = GetComponent<FPSControlPlayerWeaponManager>();
            weapons.Add("Beretta", false);
            FPSControlRangedWeapon beretta = weapons.Get<FPSControlRangedWeapon>("Beretta");
            beretta.SetAmmo(10, 3);
            
            weapons.Add("Melee", false);
			
			weapons.Add("Test", true);
        }

	}
}
