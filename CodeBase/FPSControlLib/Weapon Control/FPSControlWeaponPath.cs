using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPSControl
{


    public class FPSControlWeaponPath : FPSControlWeaponComponent
    {
        public bool render = true;
        public bool consistentRender = false;
        public bool isPreFire = false;

        public float maxDistance;
        public float leavingForce;

        public Transform origin;
        public Material material;

    }
}
