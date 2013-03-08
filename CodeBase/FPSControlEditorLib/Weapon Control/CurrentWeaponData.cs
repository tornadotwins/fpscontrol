using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{

    class CurrentWeaponData
    {
        public FPSControlWeapon weapon;
        public Transform transform;
        public Transform modelOffset;
        public Transform modelControler;        
        public bool isRanged;
    }

}
