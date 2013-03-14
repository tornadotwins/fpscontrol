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
        public Vector3 pivot;
        public Vector3 euler;
        public Vector3 scopePivot;
        public Vector3 scopeEuler;
        public float scopeFOV;

        //Damage
        [SerializeField]
        public float maxDamagePerHit;

        //Timing
        public float chargeTime = .1F; //amount of time the mouse has to be held down to classify as a charge

        public FPSControlWeaponDefinition() { }

    }

}
