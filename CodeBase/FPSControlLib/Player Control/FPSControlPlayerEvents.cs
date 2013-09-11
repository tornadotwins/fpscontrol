using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using FPSControl.States;
using FPSControl.States.Player;

namespace FPSControl
{
    public class FPSControlPlayerEvents
    {        
        public static FPSControlPlayer player;

        public static event System.Action OnDeath;
        public static event System.Action<DamageSource> OnReceiveDamage;
        public static event System.Action OnSpawn;
        public static event System.Action<bool> OnFreeze;
        public static event System.Action<FPSControlWeapon> OnWeaponDeactivate;
        public static event System.Action<FPSControlWeapon> OnWeaponActivate;

        internal static void Spawn() { if(OnSpawn != null) OnSpawn(); }
        internal static void ReceiveDamage(DamageSource src) { if(OnReceiveDamage != null) OnReceiveDamage(src); }
        internal static void Death() { if(OnDeath != null) OnDeath(); }
        internal static void Freeze(bool b) { if (OnFreeze != null) OnFreeze(b); }
        internal static void DeactivateWeapon(FPSControlWeapon weapon) { if (OnWeaponDeactivate != null) OnWeaponDeactivate(weapon); }
        internal static void ActivateWeapon(FPSControlWeapon weapon) { if (OnWeaponActivate != null) OnWeaponActivate(weapon); }
    }
}
