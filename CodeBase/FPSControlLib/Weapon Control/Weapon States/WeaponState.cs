using System;
using System.Collections.Generic;
using FPSControl.States;
using UnityEngine;

namespace FPSControl.States.Weapon
{
    [System.Serializable]
    public class WeaponState : State
    {
        public WeaponState(string name, FPSControlWeapon parent) : base(name, parent){}
    }
}
