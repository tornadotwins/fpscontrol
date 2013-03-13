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
    public class FPSControlRangeWeaponDefinition
    {
        //Ranged-Only
        public FPSControlRangedWeaponType rangedType = FPSControlRangedWeaponType.Bullets;
        public FPSControlWeaponPath weaponPath;

        //Raycasting
        public Transform rayOrigin;
        public float disperseRadius = 0;
        public int raycasts = 1;
        public float range = 10; //in meters
        public float spread = 1.5F;

        //damage
        public FalloffData damageFalloff;

        //Capacity
        public ReloadType reloadType = ReloadType.Clips;
        public int clipCapacity = 10;
        public int maxClips = 3;
        public int burstAmount = 1;

        //Regen weapons
        public bool constantRegeneration = true;
        public float maximumRounds = 100f;
        public float regenerationRate = .5F; //points per second - if constantRegeneration is TRUE
        public float fullRegenerationTime = 8F; //if constantRegeneration is FALSE
    }

}
