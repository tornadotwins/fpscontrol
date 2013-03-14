using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    [System.Serializable]
    class WeaponData : UnityEngine.Object
    {
        [SerializeField]
        public FPSControlWeapon weapon;
        [SerializeField]
        public Transform modelOffset;
        [SerializeField]
        public Transform modelControler;
        [SerializeField]
        public bool isRanged = false;
        [SerializeField]
        public bool isDirty = false;
        public WeaponData() {}
    }

}
