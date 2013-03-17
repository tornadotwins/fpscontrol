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
        [SerializeField]
        public FPSControlRangedWeaponType rangedType = FPSControlRangedWeaponType.Bullets;

        //Raycasting    
        [SerializeField]
        public float disperseRadius = 0;
        [SerializeField]
        public float raycasts = 1;
        //[SerializeField]
        //public float range = 10; //in meters
        [SerializeField]
        public float spread = 1.5F;

        //Projectiles
        public float twirlingSpeed = 5;
        public Vector3 twirl;

        //Capacity
        [SerializeField]
        public ReloadType reloadType = ReloadType.Clips;
        [SerializeField]
        public float clipCapacity = 10;
        [SerializeField]
        public int maxClips = 3;
        [SerializeField]
        public int burstAmount = 1;

        //Regen weapons
        [SerializeField]
        public bool constantRegeneration = true;
        [SerializeField]
        public float maximumRounds = 100f;
        [SerializeField]
        public float regenerationRate = .5F; //points per second - if constantRegeneration is TRUE
        [SerializeField]
        public float fullRegenerationTime = 8F; //if constantRegeneration is FALSE

        public FPSControlRangeWeaponDefinition() { }

    }

}
