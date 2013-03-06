using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.States.Weapon;

namespace FPSControl
{
    public class FPSControlMeleeWeapon : FPSControlWeapon
    {
        new public WeaponState currentState
        {
            get
            {
                return (WeaponState)base.currentState;
            }
            protected set
            {
                Debug.Log("setting current state to " + value.name);
                base.currentState = value;
            }
        }
        
        //Damage
        public Collider damageTrigger;
        public override bool hasAmmo { get { return true; } } // you don't have to reload a crowbar...

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
            gameObject.SetActive(true);
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

            gameObject.SetActive(false);
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
