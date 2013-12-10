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
        [HideInInspector]
        public FPSControlRangeWeaponDefinition rangeDefinition = new FPSControlRangeWeaponDefinition();
        //[HideInInspector]
        public FPSControlWeaponPath weaponPath;
        [HideInInspector]
        public GameObject projectileA;
        [HideInInspector]
        public GameObject projectileB;
        public LayerMask gunDamageLayers;

        public GameObject bulletHole;
        public float contactForce = 10.0F;
        

        [HideInInspector]
        public FalloffData damageFalloff = new FalloffData();

        [HideInInspector]
        new public WeaponState currentState
        {
            get
            {
                return (WeaponState) base.currentState;
            }
            protected set
            {
                base.currentState = value;                
            }
        }

        [HideInInspector]
        public override bool canScope
        {
            get
            {
                return true;
            }
        }

        float scopeSpeed { get { return Parent.Player.playerCamera.scopeSpeed;} }

        [HideInInspector]
        [SerializeField] int _currentClipContents = 0;
        [HideInInspector]
        [SerializeField] int _currentClips = 0;
        [HideInInspector]
        [SerializeField] int _currentAmmo = 0;

        [HideInInspector]
        [SerializeField] float _currentEnergy = 100F;
        [HideInInspector]
        float _timeLastActive = -1F;
        [HideInInspector]
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
            weaponPath.Initialize(this);
            base.OnInitialize();
            if (damageFalloff.distance == 0) damageFalloff.distance = 20;
        }

        //adds ammo, returns the amount of bullets left over
        public int AddAmmo(int clips)
        {
            int maxAmmo = (int)rangeDefinition.clipCapacity * rangeDefinition.maxClips;
            int remainder = maxAmmo - ((clips * (int)rangeDefinition.clipCapacity) + _currentAmmo);
            _currentClips = Mathf.Min(clips, rangeDefinition.maxClips);
            _currentAmmo = Mathf.Min((clips * (int)rangeDefinition.clipCapacity) + _currentAmmo, maxAmmo);
            //return the remainder
            return remainder;
        }

        //Added by Efraim
        public int[] GetAmmo()
        {
            var ammo = new int[] { _currentClipContents, _currentClips, _currentAmmo };
            return ammo;
        }

        public void SetAmmo(int ammo, int clips)
        {
            _currentClipContents = Mathf.Min(ammo, (int)rangeDefinition.clipCapacity);
            _currentClips = Mathf.Min(clips, rangeDefinition.maxClips);
            _currentAmmo = (int)rangeDefinition.clipCapacity * _currentClips;
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
            if (!canUse)
            {
                Debug.Log("Attempting to reload an inactive weapon.");
            }
            else if (
                rangeDefinition.reloadType == ReloadType.UnlimitedAmmo ||
                rangeDefinition.reloadType == ReloadType.Recharge)
            {
                Debug.Log("Weapon type:" + rangeDefinition.reloadType + " is incompatible with reload function");
                return false;
            }
            else if (rangeDefinition.reloadType == ReloadType.Clips && _currentClipContents >= rangeDefinition.clipCapacity)
            {
                Debug.Log("Trying to reload a full clip.");
                return false;
            }

            if (_currentAmmo <= 0)
            {
                Debug.Log("Trying to reload a weapon with no ammo!");
                return false;
            }

            currentState = reloadState;

            weaponAnimation.animationCompleteCallback = ReloadCompleted;
            weaponAnimation.Reload();
            
            return true;
        }

        void ReloadCompleted()
        {
            int space = (int)rangeDefinition.clipCapacity - _currentClipContents;
            int lessAmmo = Mathf.Min(space, _currentAmmo);
            _currentAmmo -= lessAmmo;

            _currentClips = (int)_currentAmmo / (int)rangeDefinition.clipCapacity;
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
            if (rangeDefinition.reloadType == ReloadType.Recharge)
			{
				float last = _timeSinceActivation;
				_timeSinceActivation += Time.deltaTime;
				//if we've moved from say 1.9999 to 2.00001 it we can increase a tick.
                if (Mathf.Floor(_timeSinceActivation) > Mathf.Floor(last)) _currentEnergy = Mathf.Clamp(_currentEnergy + rangeDefinition.regenerationRate, 0, 100F);
			}
			
			if (scoped)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, definition.scopePivot, Time.deltaTime * scopeSpeed); //this is actually more for debugging purposes, actually.
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(definition.scopeEuler), Time.deltaTime * scopeSpeed);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, definition.pivot, Time.deltaTime * scopeSpeed); //this is actually more for debugging purposes, actually.
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(definition.euler), Time.deltaTime * scopeSpeed);
            }

        }

        public override void Fire()
        {
            //Debug.Log(canUse + ":" + firing + ":" + defending + ":" + reloading);
            if (canFire && hasAmmo)
            {
                weaponPath.Fire();
            }
            
            if (canFire) //this is more like, can we actually pull the trigger?
            {
                currentState = fireState;
                weaponAnimation.animationCompleteCallback = FireCompleted;
                if (hasAmmo)
                {
                    weaponAnimation.Fire(); //play our fire animation
                    
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

                    if (rangeDefinition.rangedType == FPSControlRangedWeaponType.Bullets)
                    {
                        Vector3 shootFrom = Parent.Player.interactionManager.transform.position; //Grab the position of the interactionManager
                        if (weaponPath.origin != null) shootFrom += Parent.Player.interactionManager.transform.forward * Vector3.Distance(shootFrom, weaponPath.origin.position); //if we have an origin to shot from we want to get the distance from the interation controler to it. This way we cant hit people from our face and only where the weapon is

                        for (int i = 0; i < rangeDefinition.raycasts; i++)
                        {

                            float randX = 0;
                            float randY = 0;

                            if (rangeDefinition.disperseRadius > 0) //if we have a disperse radius
                            {
                                randX = (UnityEngine.Random.value * (rangeDefinition.disperseRadius * 2)) - rangeDefinition.disperseRadius; //randomly get a position within the radius
                                randY = (UnityEngine.Random.value * (rangeDefinition.disperseRadius * 2)) - rangeDefinition.disperseRadius;
                            }
                            
                            //Vector3 perspectiveDistortion = new Vector3(rangeDefinition.spread * randX, rangeDefinition.spread * randY, 0);
                            
                            Vector3 origin = shootFrom + new Vector3(randX, randY, 0);
                            Ray ray = new Ray(origin, Parent.Player.interactionManager.transform.forward);
                             
                            Debug.DrawRay(ray.origin, ray.direction * damageFalloff.distance, Color.red, .5F);

                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit, damageFalloff.distance, gunDamageLayers.value))//did we actually hit anything?
                            {
                                Vector3 contact = hit.point;
                                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                                if (hit.rigidbody)
                                    hit.rigidbody.AddForceAtPosition(contactForce * ray.direction, contact);
                                
                                //ImpactControl impact = Parent.Player.GetComponent<ImpactControl>();
                                //if (impact != null)
                                //{
                                //    impact.OnImpact(hit);
                                //}

                                //if ((hit.transform.tag != "NoBulletHoles") && (hit.transform.tag != "Untagged") && (hit.transform.tag != "Enemy"))
                                //{
                                //    if (bulletHole)
                                //    {
                                //        GameObject tr = (GameObject)Instantiate(bulletHole, contact, rot);
                                //        tr.SendMessage("SurfaceType", hit);
                                //        tr.transform.parent = hit.transform; // parent to hit object so the bullet holes move with object
                                //   }
                                //}

                                DamageSource damageSource = new DamageSource();
                                float distance = hit.distance;
                                float normalizedDistance = distance / damageFalloff.distance; //normalize distance, 9F/10F = .9F
                                float damageInflicted = (definition.maxDamagePerHit * damageFalloff.Evaluate(normalizedDistance)) / (float)rangeDefinition.raycasts; //divy the damage inflicted by number of raycasts
                                damageSource.appliedToPosition = hit.point;
                                damageSource.damageAmount = damageInflicted;
                                damageSource.fromPosition = weaponPath.origin.position;
                                damageSource.hitCollider = hit.collider;
                                damageSource.sourceObjectType = DamageSource.DamageSourceObjectType.Player;
                                damageSource.sourceType = DamageSource.DamageSourceType.GunFire;
                                damageSource.sourceObject = Parent.Player.gameObject;
                                
                                Damageable damageable = hit.transform.GetComponent<Damageable>();
                                if (damageable != null)
                                {
                                   damageable.ApplyDamage(damageSource);
                                }
                                else
                                {
                                    //Debug.Log(hit.collider.gameObject.name+": No Damageable found. Reverting to SendMessage.");
                                    hit.collider.SendMessage("ApplyDamage", damageSource, SendMessageOptions.DontRequireReceiver);
                                }
                            }
                        }
                    }
                    else if (rangeDefinition.rangedType == FPSControlRangedWeaponType.Projectile)
                    {
                        if (projectileA != null)
                        {
                            GameObject obj = (GameObject)GameObject.Instantiate(projectileA) as GameObject;
                            Rigidbody rb = obj.GetComponent<Rigidbody>();
                            if (rb == null)
                            {
                                Debug.LogWarning("Projectile dosen't have a Rigidbody");
                            }
                            else
                            {
                                rb.transform.position = weaponPath.origin.position;
                                rb.transform.rotation = weaponPath.origin.rotation;
                                rb.AddForce(weaponPath.shootVelocity, ForceMode.Impulse);
                            }                            
                        }
                        
                    }                    
                }
                else //we are empty, play the empty animation
                {
                    //Debug.Log("empty!");
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

            weaponPath.Initialize(this); // re-initialize path.
            weaponAnimation.Activate();
        }

        void WeaponBecameActive()
        {
            Debug.Log("weapon: " + name + " became active");
            canUse = true;
            weaponAnimation.Idle();
            currentState = idleState;
			
			_timeSinceActivation = 0;
			float timeSinceLastActive = _timeLastActive >= 0 ? Time.time - _timeLastActive : 0;
			
			//we should make sure that we recharged while holstered
            if (rangeDefinition.reloadType == ReloadType.Recharge)
			{
                float regen = Mathf.Floor(timeSinceLastActive) * rangeDefinition.regenerationRate;
				_currentEnergy = Mathf.Min(_currentEnergy+regen,100F);
			}

            FPSControlPlayerEvents.ActivateWeapon(this);
        }
        
        public override void Deactivate(System.Action cbFunc)
        {
            Debug.Log("Deactivating weapon:" + name + " " + (cbFunc != null ? "  with callback." : "."));
            _deactivateCallback = cbFunc;
            canUse = false;
            _timeLastActive = Time.time;
            weaponAnimation.animationCompleteCallback = WeaponBecameInactive;
            //play deactivate animation
            weaponAnimation.Deactivate();
        }

        void WeaponBecameInactive() 
        {
            gameObject.SetActive(false);
            if(_deactivateCallback != null) _deactivateCallback();
            _deactivateCallback = null;

            FPSControlPlayerEvents.DeactivateWeapon(this);
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
            if (!weaponPath || !weaponPath.origin) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(weaponPath.origin.position, rangeDefinition.disperseRadius);
        }
    }
}
