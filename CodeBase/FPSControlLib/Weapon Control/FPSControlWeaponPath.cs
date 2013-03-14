using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.States.Weapon;
using FPSControl.Data;
using FPSControl.Definitions;

namespace FPSControl
{

    public class FPSControlWeaponPath : FPSControlWeaponComponent
    {
        public FPSControlWeaponPathDefinition definition = new FPSControlWeaponPathDefinition();

        public Transform origin;
        public Material material;
    }

}
