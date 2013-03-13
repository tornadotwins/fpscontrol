using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.States.Weapon;
using FPSControl.Data;
using FPSControl.Definitions;

namespace FPSControl
{

    public class FPSControlRangedWeapon : FPSControlWeapon
    {

        public FPSControlRangeWeaponDefinition rangeDefinition;

        new public WeaponState currentState
        {
            get
            {
                return (WeaponState) base.currentState;
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
                return true;
            }
        }

        int _currentClipContents = 0;
        int _currentClips = 0;
        int _currentAmmo = 0;

        float _currentEnergy = 100F;
        float _timeLastActive = -1F;
		float _timeSinceActivation = 0F;

        public override bool hasAmmo
        {
            get
            {
                if (rangeDefinition.reloadType == ReloadType.Clips)
                {
                    return _currentClipContents > 0;
                }
                else if (rangeDefinition.reloadType == ReloadType.Recharge)
                {
                    return _currentEnergy >= rangeDefinition.burstAmount;
                }
                return true;
            }
        }

        //Callbacks
        System.Action _deactivateCallback;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        //adds ammo, returns the amount of bullets left over
        public int AddAmmo(int clips)
        {
            int maxAmmo = rangeDefinition.clipCapacity * rangeDefinition.maxClips;
            int remainder = maxAmmo - ((clips * rangeDefinition.clipCapacity) + _currentAmmo);
            _currentClips = Mathf.Min(clips, rangeDefinition.maxClips);
            _currentAmmo = Mathf.Min((clips * rangeDefinition.clipCapacity) + _currentAmmo, maxAmmo);
            //return the remainder
            return remainder;
        }

        public void SetAmmo(int ammo, int clips)
        {
            _currentClipContents = Mathf.Min(ammo, rangeDefinition.clipCapacity);
            _currentClips = Mathf.Min(clips, rangeDefinition.maxClips);
            _currentAmmo = rangeDefinition.clipCapacity * _currentClips;
        }

        public void SetAmmo(float energy)
        {
            _currentEnergy = Mathf.Clamp(energy,0,100);
        }

        public override void StartRun()
        {
            canUse = false;
            currentState = definition.idleState;
            definition.weaponAnimation.Run();
        }

        public override void EndRun()
        {
            canUse = true;
            currentState = definition.idleState;
            definition.weaponAnimation.Idle();
        }

        public override bool Reload()
        {
            if (!canUse || rangeDefinition.reloadType == ReloadType.UnlimitedAmmo || rangeDefinition.reloadType == ReloadType.Recharge || (rangeDefinition.reloadType == ReloadType.Clips && _currentClipContents >= rangeDefinition.clipCapacity)) return false;

            if (_currentAmmo <= 0) return false;

            currentState = definition.reloadState;

            definition.weaponAnimation.animationCompleteCallback = ReloadCompleted;
            definition.weaponAnimation.Reload();
            
            return true;
        }

        void ReloadCompleted()
        {
            int space = rangeDefinition.clipCapacity - _currentClipContents;
            int lessAmmo = Mathf.Min(space, _currentAmmo);
            _currentAmmo -= lessAmmo;

            _currentClips = (int)_currentAmmo / rangeDefinition.clipCapacity;
            _currentClipContents += lessAmmo;
            currentState = definition.idleState;
            definition.weaponAnimation.Idle();
        }
        
        public override void CancelReload()
        {
            //nothing yet
        }

        public override void Scope()
        {
            if (firing) return;
            scoped = true;
        }

        public override void ExitScope()
        {
            scoped = false;
        }

        void Update()
        {
            if (rangeDefinition.reloadType == ReloadType.Recharge)
			{
				float last = _timeSinceActivation;
				_timeSinceActivation += Time.deltaTime;
				//if we've moved from say 1.9999 to 2.00001 it we can increase a tick.
                if (Mathf.Floor(_timeSinceActivation) > Mathf.Floor(last)) _currentEnergy = Mathf.Clamp(_currentEnergy + rangeDefinition.regenerationRate, 0, 100F);
			}
			
			if (scoped)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, definition.scopePivot, Time.deltaTime * 3F); //this is actually more for debugging purposes, actually.
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(definition.scopeEuler), Time.deltaTime * 3F);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, definition.pivot, Time.deltaTime * 3F); //this is actually more for debugging purposes, actually.
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(definition.euler), Time.deltaTime * 3F);
            }
        }

        public override void Fire()
        {
            //Debug.Log(canUse + ":" + firing + ":" + defending + ":" + reloading);
            if (canFire) //this is more like, can we actually pull the trigger?
            {
                //Debug.Log("pew pew");
                currentState = definition.fireState;
                definition.weaponAnimation.animationCompleteCallback = FireCompleted;
                if (hasAmmo)
                {
                    definition.weaponAnimation.Fire(); //play our fire animation

                    if (rangeDefinition.reloadType == ReloadType.Clips)
					{
                        int expense = Mathf.Min(rangeDefinition.burstAmount, _currentClipContents); //we'll decrement our current clip contents by the burst amount, or whatever is left...
                    	_currentClipContents -= expense;
					}
                    else if (rangeDefinition.reloadType == ReloadType.Recharge)
					{
                        _currentEnergy -= rangeDefinition.burstAmount;
					}
                    //do the raycasting stuff here

                    for (int i = 0; i < rangeDefinition.raycasts; i++)
                    {
                        
                        
                        float randX = 0;
                        float randY = 0;

                        if (rangeDefinition.disperseRadius > 0) //if we have a disperse radius
                        {
                            randX = (UnityEngine.Random.value * (rangeDefinition.disperseRadius * 2)) - rangeDefinition.disperseRadius; //randomly get a position within the radius
                            randY = (UnityEngine.Random.value * (rangeDefinition.disperseRadius * 2)) - rangeDefinition.disperseRadius;
                        }

                        Vector3 perspectiveDistortion = new Vector3(rangeDefinition.spread * randX, rangeDefinition.spread * randY, 0);

                        Vector3 origin = rangeDefinition.rayOrigin.position + new Vector3(randX, randY, 0); //apply offset to origin

                        Ray ray = new Ray(origin, rangeDefinition.rayOrigin.forward);// + (Vector3.Scale(rayOrigin.forward,perspectiveDistortion)));

                        Debug.DrawRay(ray.origin, ray.direction * rangeDefinition.range, Color.red, .5F);

                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, rangeDefinition.range))//did we actually hit anything?
                        {
                           
                            //a bit of psuedo code here:
                            /*
                             * //grab a damage receiver (fake class name)
                             * DamageReceiver dr = hit.collider.GetComponent<DamageReceiver>();
                             * 
                             * //how far out we we?
                             * float distance = hit.distance;
                             * float normalizedDistance = distance/range; //normalize distance, 9F/10F = .9F
                             * float damageInflicted = (maxDamagePerHit * damageFalloff.Evaluate(normalizedDistance)) / (float) raycasts; //divy the damage inflicted by number of raycasts
                             * 
                             * dr.Inflict(damageInflicted);
                             * 
                             **/
                        }
                        
                    }
                    
                }
                else //we are empty, play the empty animation
                {
                    Debug.Log("empty!");
                    definition.weaponAnimation.Empty();
                }
            }
            else
            {
                //Debug.Log("could not fire: " + canUse + ", " + firing + ", " + reloading + ", " + defending + " [" + currentState.name + "]");
            }
        }
        
        void FireCompleted()
        {
            //Debug.Log("fire completed.");
            currentState = definition.idleState;
            //Debug.Log("current state: " + currentState.name);
            definition.weaponAnimation.Idle();
        }

        public override void Activate(FPSControlPlayerWeaponManager parent)
        {
            gameObject.SetActive(true);
            Parent = parent;
            definition.weaponAnimation.animationCompleteCallback = WeaponBecameActive;
            definition.weaponAnimation.Activate();
        }

        void WeaponBecameActive()
        {
            //Debug.Log("weapon: " + weaponName + " became active");
            canUse = true;
            definition.weaponAnimation.Idle();
            currentState = definition.idleState;
			
			_timeSinceActivation = 0;
			float timeSinceLastActive = _timeLastActive >= 0 ? Time.time - _timeLastActive : 0;
			
			//we should make sure that we recharged while holstered
            if (rangeDefinition.reloadType == ReloadType.Recharge)
			{
                float regen = Mathf.Floor(timeSinceLastActive) * rangeDefinition.regenerationRate;
				_currentEnergy = Mathf.Min(_currentEnergy+regen,100F);
			}
        }
        
        public override void Deactivate(System.Action cbFunc)
        {
            _deactivateCallback = cbFunc;
            definition.weaponAnimation.animationCompleteCallback = WeaponBecameInactive;
            //play deactivate animation
            definition.weaponAnimation.Deactivate();
        }

        void WeaponBecameInactive()
        {
            canUse = false;
            
            _deactivateCallback();
            _deactivateCallback = null;
			_timeLastActive = Time.time;
            gameObject.SetActive(false);
        }
        
        public override void Charge(float accum)
        {
            Fire();
            //_accumulatedCharge += accum;
            //currentState = chargeState;
        }
        public override void CancelCharge(){}
        public override void Defend(){}
        public override void ExitDefend(){}

        void OnDrawGizmos()
        {
            if (!rangeDefinition.rayOrigin) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(rangeDefinition.rayOrigin.position, rangeDefinition.disperseRadius);
        }
    }
}
