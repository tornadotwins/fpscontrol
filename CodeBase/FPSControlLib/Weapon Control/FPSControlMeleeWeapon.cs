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

        public override bool canScope
        {
            get
            {
                return false;
            }
        }

        void Update()
        {            
            transform.localPosition = Vector3.Lerp(transform.localPosition, definition.pivot, Time.deltaTime * 3F);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(definition.euler), Time.deltaTime * 3F);
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
            _accumulatedCharge = 0;
            currentState = idleState;
            weaponAnimation.Idle();
        }

        public override void Activate(FPSControlPlayerWeaponManager parent)
        {
            _Activate(parent, null);
        }
        Action _cbFunc;

        internal override void _Activate(FPSControlPlayerWeaponManager parent, Action cbFunc)
        {
            Debug.Log("Internal _Activate" + (cbFunc != null ? "  with callback." : "."));
            _cbFunc = cbFunc;

            gameObject.SetActive(true);
            Parent = parent;

            weaponAnimation.animationCompleteCallback = () =>
            {
                Debug.Log("Lamda");
                if (_cbFunc != null) _cbFunc();
                _cbFunc = null;
                WeaponBecameActive();
            };

            weaponAnimation.Activate();
        }

        void WeaponBecameActive()
        {
            canUse = true;
            weaponAnimation.Idle();
            currentState = idleState;
            FPSControlPlayerEvents.ActivateWeapon(this);
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

            FPSControlPlayerEvents.DeactivateWeapon(this);
        }

        public override void Charge(float accum)
        {
            _accumulatedCharge += accum;
            if (!charging) weaponAnimation.Charge();
            currentState = chargeState;
        }

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
