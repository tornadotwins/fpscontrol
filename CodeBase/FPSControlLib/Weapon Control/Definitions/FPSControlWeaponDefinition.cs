using UnityEngine;
using System.Collections;
using System;
using FPSControl;
using FPSControl.States;
using FPSControl.States.Weapon;
using FPSControl.Data;

namespace FPSControl.Definitions
{
    [System.Serializable]
    public class FPSControlWeaponDefinition
    {
        //Archetype Info
        public string weaponName = "Weapon";

        //Visual
        public Vector3 pivot = Vector3.zero;
        public Vector3 euler = Vector3.zero;
        public Vector3 scopePivot = Vector3.zero;
        public Vector3 scopeEuler = Vector3.zero;
        public float scopeFOV = 20;

        //Damage
        [SerializeField]
        public float maxDamagePerHit;

        //Timing
        public float chargeTime = .1F; //amount of time the mouse has to be held down to classify as a charge

        public FPSControlWeaponDefinition() { }

    }

}
