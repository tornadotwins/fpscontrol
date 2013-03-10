using System;
using System.Collections.Generic;
using UnityEngine;
using FPSControl.States.Weapon;
using FPSControl.Data;

namespace FPSControl
{
    public class FPSControlRangedWeapon : FPSControlWeapon
    {
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

        //Ranged-Only
        public FPSControlRangedWeaponType rangedType = FPSControlRangedWeaponType.Bullets;
        public FPSControlWeaponPath weaponPath;

        //Raycasting
        public Transform rayOrigin;
        public float disperseRadius = 0;
        public int raycasts = 1;
        public float range = 10; //in meters
        public float spread = 1.5F;
		
		//damage
		public FalloffData damageFalloff;

        //Capacity
        public ReloadType reloadType = ReloadType.Clips;
        public int clipCapacity = 10;
        public int maxClips = 3;
        public int burstAmount = 1;
        int _currentClipContents = 0;
        int _currentClips = 0;
        int _currentAmmo = 0;
        public bool constantRegeneration = true;
        public float maximumRounds = 100f;
        public float regenerationRate = .5F; //points per second - if constantRegeneration is TRUE
        public float fullRegenerationTime = 8F; //if constantRegeneration is FALSE
        float _currentEnergy = 100F;
        float _timeLastActive = -1F;
		float _timeSinceActivation = 0F;

        public override bool hasAmmo
        {
            get
            {
                if (reloadType == ReloadType.Clips)
                {
                    return _currentClipContents > 0;
                }
                else if (reloadType == ReloadType.Recharge)
                {
                    return _currentEnergy >= burstAmount;
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
            int maxAmmo = clipCapacity * maxClips;
            int remainder = maxAmmo - ((clips * clipCapacity) + _currentAmmo);
            _currentClips = Mathf.Min(clips, maxClips);
            _currentAmmo = Mathf.Min((clips * clipCapacity) + _currentAmmo, maxAmmo);
            //return the remainder
            return remainder;
        }

        public void SetAmmo(int ammo, int clips)
        {
            _currentClipContents = Mathf.Min(ammo, clipCapacity);
            _currentClips = Mathf.Min(clips, maxClips);
            _currentAmmo = clipCapacity * _currentClips;
        }

        public void SetAmmo(float energy)
        {
            _currentEnergy = Mathf.Clamp(energy,0,100);
        }

        public override void StartRun()
        {
            canUse = false;
            currentState = idleState;
            weaponAnimation.Run();
        }

        public override void EndRun()
        {
            canUse = true;
            currentState = idleState;
            weaponAnimation.Idle();
        }

        public override bool Reload()
        {
            if (!canUse || reloadType == ReloadType.UnlimitedAmmo || reloadType == ReloadType.Recharge || (reloadType == ReloadType.Clips && _currentClipContents >= clipCapacity)) return false;

            if (_currentAmmo <= 0) return false; 

            currentState = reloadState;

            weaponAnimation.animationCompleteCallback = ReloadCompleted;
            weaponAnimation.Reload();
            
            return true;
        }

        void ReloadCompleted()
        {
            int space = clipCapacity - _currentClipContents;
            int lessAmmo = Mathf.Min(space, _currentAmmo);
            _currentAmmo -= lessAmmo;

            _currentClips = (int)_currentAmmo / clipCapacity;
            _currentClipContents += lessAmmo;
            currentState = idleState;
            weaponAnimation.Idle();
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
            if(reloadType == ReloadType.Recharge)
			{
				float last = _timeSinceActivation;
				_timeSinceActivation += Time.deltaTime;
				//if we've moved from say 1.9999 to 2.00001 it we can increase a tick.
				if(Mathf.Floor(_timeSinceActivation) > Mathf.Floor(last)) _currentEnergy = Mathf.Clamp(_currentEnergy+regenerationRate,0,100F);
			}
			
			if (scoped)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, scopePivot, Time.deltaTime * 3F); //this is actually more for debugging purposes, actually.
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(scopeEuler), Time.deltaTime * 3F);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, pivot, Time.deltaTime * 3F); //this is actually more for debugging purposes, actually.
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(euler), Time.deltaTime * 3F);
            }
        }

        public override void Fire()
        {
            //Debug.Log(canUse + ":" + firing + ":" + defending + ":" + reloading);
            if (canFire) //this is more like, can we actually pull the trigger?
            {
                //Debug.Log("pew pew");
                currentState = fireState;
                weaponAnimation.animationCompleteCallback = FireCompleted;
                if (hasAmmo)
                {
                    weaponAnimation.Fire(); //play our fire animation
                    
					if(reloadType == ReloadType.Clips)
					{
						int expense = Mathf.Min(burstAmount, _currentClipContents); //we'll decrement our current clip contents by the burst amount, or whatever is left...
                    	_currentClipContents -= expense;
					}
					else if(reloadType == ReloadType.Recharge)
					{
						_currentEnergy -= burstAmount;
					}
                    //do the raycasting stuff here
                    
                    for (int i = 0; i < raycasts; i++)
                    {
                        
                        
                        float randX = 0;
                        float randY = 0;

                        if (disperseRadius > 0) //if we have a disperse radius
                        {
                            randX = (UnityEngine.Random.value * (disperseRadius * 2)) - disperseRadius; //randomly get a position within the radius
                            randY = (UnityEngine.Random.value * (disperseRadius * 2)) - disperseRadius;
                        }

                        Vector3 perspectiveDistortion = new Vector3( spread*randX, spread *randY, 0);

                        Vector3 origin = rayOrigin.position + new Vector3(randX, randY, 0); //apply offset to origin

                        Ray ray = new Ray(origin, rayOrigin.forward);// + (Vector3.Scale(rayOrigin.forward,perspectiveDistortion)));

                        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, .5F);

                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, range))//did we actually hit anything?
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
                    weaponAnimation.Empty();
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
            currentState = idleState;
            //Debug.Log("current state: " + currentState.name);
            weaponAnimation.Idle();
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
            //Debug.Log("weapon: " + weaponName + " became active");
            canUse = true;
            weaponAnimation.Idle();
            currentState = idleState;
			
			_timeSinceActivation = 0;
			float timeSinceLastActive = _timeLastActive >= 0 ? Time.time - _timeLastActive : 0;
			
			//we should make sure that we recharged while holstered
			if(reloadType == ReloadType.Recharge)
			{
				float regen = Mathf.Floor(timeSinceLastActive) * regenerationRate;
				_currentEnergy = Mathf.Min(_currentEnergy+regen,100F);
			}
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
            if (!rayOrigin) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(rayOrigin.position, disperseRadius);
        }
    }
}
