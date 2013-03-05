using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.States.Weapon;

namespace FPSControl
{
    public abstract class FPSControlWeaponComponent : MonoBehaviour
    {
        public FPSControlWeapon Weapon { get; protected set; }
        public WeaponState currentState { get { return (WeaponState) Weapon.currentState; } }

        public virtual void Initialize(FPSControlWeapon parent)
        {
            Weapon = parent;
        }

    }
}
