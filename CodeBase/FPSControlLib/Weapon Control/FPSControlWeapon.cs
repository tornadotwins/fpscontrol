using UnityEngine;
using System.Collections;
using System;
using FPSControl;
using FPSControl.States;
using FPSControl.States.Weapon;
using FPSControl.Data;

namespace FPSControl
{
    public enum FPSControlRangedWeaponType
    {
        Bullets,Projectile
    }

    public enum ReloadType
    {
        UnlimitedAmmo,Clips,Recharge
    }
    
    public abstract class FPSControlWeapon : StateMachine
    {
        public FPSControlPlayerWeaponManager Parent { get; protected set; }
        
        //Archetype Info
        public string weaponName = "Weapon";
        

        //Sub-Components
        public FPSControlWeaponAnimation weaponAnimation;
        public FPSControlWeaponParticles weaponParticles;
        public FPSControlWeaponSound weaponSound;

        //Visual
        public Vector3 pivot;
        public Vector3 euler;
        public Vector3 scopePivot;
        public Vector3 scopeEuler;
        public float scopeFOV;
        

        //Damage
        public float maxDamagePerHit;
        public FalloffData damageFalloff;

        //Timing
        public float chargeTime = .1F; //amount of time the mouse has to be held down to classify as a charge
        
        //Internal Stats
        protected float _accumulatedCharge = 0F;

        //States
        public WeaponState idleState;
        public WeaponState fireState;
        public WeaponState reloadState;
        public WeaponState defendState;
        public WeaponState chargeState;

        //Info
        public bool isActiveWeapon
        { 
            get 
            { 
                if (!Parent) return false;
                else return Parent.currentWeapon == this; 
            } 
        }
        public bool scoped { get; protected set; }
        public bool reloading { get { return currentState == reloadState; } }
        public bool firing { get { return currentState == fireState; } }
        public bool defending { get { return currentState == defendState; } }
        public bool charging { get { return currentState == chargeState; } }
        public bool canFire { get { return canUse && !firing && !defending && !reloading; } }
        public bool canUse { get; protected set; }
        [HideInInspector] bool __hasAmmo = false;
        public virtual bool hasAmmo { get { return __hasAmmo; } }
        public virtual bool canScope { get; protected set; }

        void Awake()
        {
            gameObject.SetActive(false);
            
            idleState.name = "Idle";
            fireState.name = "Fire";
            reloadState.name = "Reload";
            defendState.name = "Defend";
            chargeState.name = "Charge";

            Add(idleState);
            Add(fireState);
            Add(reloadState);
            Add(defendState);
            Add(chargeState);
            
            Initialize(idleState);
        }

        protected override void OnInitialize()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnStateChange()
        {
            //throw new System.NotImplementedException();
        }

        /*
        void Update()
        {
            transform.localPosition = pivot; //this is actually more for debugging purposes, actually.
            transform.localRotation = Quaternion.Euler(euler);
        }
        */
        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, .03F);
        }

        public abstract bool Reload();
        public abstract void CancelReload();
        public abstract void Scope();
        public abstract void ExitScope();
        public abstract void Fire();
        public abstract void Activate(FPSControlPlayerWeaponManager parent);
        public abstract void Deactivate(System.Action cbFunc);
        public abstract void Charge(float accumTime);
        public abstract void CancelCharge();
        public abstract void Defend();
        public abstract void ExitDefend();
        public abstract void StartRun();
        public abstract void EndRun();
    }
}
