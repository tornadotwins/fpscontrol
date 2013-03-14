using UnityEngine;
using System.Collections;
using System;
using FPSControl;
using FPSControl.States;
using FPSControl.States.Weapon;
using FPSControl.Data;

namespace FPSControl.Definitions
{
    public class FPSControlWeaponAnimationDefinition
    {
        public FiringPatternType patternType = FiringPatternType.OncePerAnimation;
        public bool blend;

        //General/Shared Animation States
        [HideInInspector]
        public string ACTIVATE = "Activate";
        [HideInInspector]
        public string DEACTIVATE = "Deactivate";
        [HideInInspector]
        public string RUN = "Run";
        [HideInInspector]
        public string WALK = "Walk";
        [HideInInspector]
        public string IDLE = "Idle";
        //Ranged
        [HideInInspector]
        public string SCOPE_IO = "Scope IO";
        [HideInInspector]
        public string SCOPE_LOOP = "Scope Loop";
        [HideInInspector]
        public string FIRE = "Fire";
        [HideInInspector]
        public string RELOAD = "Reload";
        [HideInInspector]
        public string EMPTY = "Empty";
        //Melee
        [HideInInspector]
        public string CHARGE = "Charge";
        [HideInInspector]
        public string ATTACK = "Attack";
        [HideInInspector]
        public string DEFEND_ENTER = "Defend Enter";
        [HideInInspector]
        public string DEFEND_LOOP = "Defend Loop";
        [HideInInspector]
        public string DEFEND_EXIT = "Defend Exit";

        public FPSControlWeaponAnimationDefinition() { }

    }
}
