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
    public class FPSControlWeaponPathDefinition
    {
        public bool render = true;
        public bool consistentRender = false;
        public bool isPreFire = false;

        public float maxDistance = 1000;
        public float leavingForce;
        public Color lineColor = Color.green;
        public float fadeOutTime = 0.25f;

        public FPSControlWeaponPathDefinition() { }
    }
}
