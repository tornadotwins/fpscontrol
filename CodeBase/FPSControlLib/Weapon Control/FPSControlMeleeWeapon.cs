﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPSControl
{
    public class FPSControlMeleeWeapon : FPSControlWeapon
    {
        //Damage
        public Collider damageTrigger;
        public override bool hasAmmo { get { return true; } } // you don't have to reload a crowbar... OR do you???

        //Callbacks
        System.Action _deactivateCallback;

        public override bool Reload()
        {
            return false;
        }

        public override void CancelReload(){}
        public override void Scope(){}
        public override void ExitScope(){}

        public override void Fire()
        {
            if (canFire) //this is more like, can we actually pull the trigger?
            {
                currentState = fireState;
                weaponAnimation.animationCompleteCallback = FireCompleted;
                weaponAnimation.Attack();
            }
        }

        void FireCompleted()
        {
            currentState = idleState;
        }

        public override void Activate(FPSControlPlayerWeaponManager parent)
        {
            Parent = parent;
            weaponAnimation.animationCompleteCallback = WeaponBecameActive;
            weaponAnimation.Activate();
        }

        void WeaponBecameActive()
        {
            canUse = true;
            weaponAnimation.Idle();
            currentState = idleState;
        }

        public override void Deactivate(System.Action cbFunc)
        {
            _deactivateCallback = cbFunc;
            weaponAnimation.animationCompleteCallback = WeaponBecameInactive;
            //play deactivate animation
            weaponAnimation.Deactivate();
        }

        void WeaponBecameInactive()
        {
            canUse = false;
            _deactivateCallback();
            _deactivateCallback = null;
        }

        public override void Charge(){}
        public override void CancelCharge(){}
        public override void Defend(){}
        public override void ExitDefend(){}

        public override void StartRun()
        {
            throw new NotImplementedException();
        }

        public override void EndRun()
        {
            throw new NotImplementedException();
        }
    }
}
