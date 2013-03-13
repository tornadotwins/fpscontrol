using UnityEngine;
using System.Collections;
using System;
using FPSControl;
using FPSControl.States;
using FPSControl.States.Weapon;
using FPSControl.Data;
using FPSControl.Definitions;

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
        [HideInInspector]
        public FPSControlPlayerWeaponManager Parent { get; protected set; }
        
        public FPSControlWeaponDefinition definition;

        //Internal Stats
        [HideInInspector]
        protected float _accumulatedCharge = 0F;

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
        public bool reloading { get { return currentState == definition.reloadState; } }
        public bool firing { get { return currentState == definition.fireState; } }
        public bool defending { get { return currentState == definition.defendState; } }
        public bool charging { get { return currentState == definition.chargeState; } }
        public bool canFire { get { return canUse && !firing && !defending && !reloading; } }
        public bool canUse { get; protected set; }
        [HideInInspector] bool __hasAmmo = false;
        public virtual bool hasAmmo { get { return __hasAmmo; } }
        public virtual bool canScope { get; protected set; }

        void Awake()
        {

            gameObject.SetActive(false);
            
            definition.idleState.name = "Idle";
            definition.fireState.name = "Fire";
            definition.reloadState.name = "Reload";
            definition.defendState.name = "Defend";
            definition.chargeState.name = "Charge";

            Add(definition.idleState);
            Add(definition.fireState);
            Add(definition.reloadState);
            Add(definition.defendState);
            Add(definition.chargeState);
            
            Initialize(definition.idleState);
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
