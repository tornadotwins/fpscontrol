using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.States.Weapon;

namespace FPSControl
{
    public abstract class FPSControlWeaponComponent : MonoBehaviour
    {
        //[HideInInspector]
        public FPSControlWeapon Weapon { get; protected set; }
        //[HideInInspector]
        public WeaponState currentState { get { return (WeaponState) Weapon.currentState; } }

        public void Link(FPSControlWeapon parent)
        {
            Weapon = parent;
        }

        public virtual void Initialize(FPSControlWeapon parent)
        {
            Debug.Log("Initializing: " + this + " at " + name);
            Link(parent);
        }

    }
}
