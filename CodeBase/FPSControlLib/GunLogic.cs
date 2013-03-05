/*
jmpp: Need to write, use comments from AIGunLogic.
*/
using UnityEngine;
using System;
using System.Collections;

//---
//
//---
namespace FPSControl
{
    public class GunLogic : MonoBehaviour
    {
     
        public enum ShootDirection
        {
            Right,
            Left,
            Forward,
            Backward
        }
 
 
        // weapon stats
        public bool              _defaultWeapon = false;
        public String            _weaponsName;
        public GameObject       _weaponOwner; //This will be set to the AI "enemy target" - the body that the AI is trying to kill
        public GameObject        _animator;
        public int               _damage = 5;
        public Collider[]        _ignoreColliders;
        public LayerMask     _layerMask;
        public bool              _usesAmmo = true;
        public bool              _hideCrosshair = false;
        public bool              _hideWeapon = false;
        public bool              _hideHands = false;
        public GameObject        _weapon;
        public GameObject        _hands;
     
        // make sure you have enough ammo gui pics to cover this amount. pic array length will be divided into this numbers
        public float         _maxAmmo = 200;
        public int               _burstMax = 0;
        public float         _burstDelay = 0.0F;
        private int              _burstCount = 0;
        private float            _burstTimer = 0.0F;
        public AudioClip     _emptyMagazineSnd;
        // Cmd-Y ---> wait for reload animation.
        public float         _reloadingTime;
     
        //--- Weapon related
        public ShootDirection    _shootDirection = ShootDirection.Right;
        private Vector3          _shootVector;
        public float         _weaponMaxRange = 100.0F;
        public float         _force = 10.0F;
        public GameObject        _bulletHole;
        public GameObject        _impactSparks;
        public AudioClip        _fireAudio1;
        public AudioClip[]       _fireAudioList;
        public AudioClip     _reloadAudio;
        public AudioClip     _equipAudio;
        public AudioSource       _audioSrcFire1;
        public float         _fireRate1 = 0.5F;
        private float            _nextFire1 = 0.0F;
        public int               _gunLayer = -1;
        public Transform     _1stShootFrom;
        public GameObject        _muzzleBlast;
        public Light         _muzzleBlastLight;
        public float         _blastInsensity = 0.5F;
        public bool              _empty = false;
     
        //--- crosshair
        public GameObject        _crosshair;
        public float         _crosshairTime;
        private IntelliCrosshair _crosshairAnimator;
     
        // this is linked to a singleton controlled data bar
        public DataController    _dataController;
     
        // offset for when player is looking down
        public Vector3           _handsNormalPos;
        public Vector3           _lookDownOffset;
     
        // VARIABLES for RAYCAST SHOOTING
        private RaycastHit       _hit;
        //private Rect           _gunSightRect;
        private bool         _weaponInUse = false;
     
        // Player's body, mainly used for recoil effects
        private RBFPSControllerLogic _body;
        private bool         _firstSetUp = true;
 
 
        //---
        //
        //---
        public void Awake ()
        {
            _body = (RBFPSControllerLogic)GameObject.FindObjectOfType (typeof(RBFPSControllerLogic));
            //startingRotation = transform.localEulerAngles.x;
            _shootVector = ReturnShootDirection ();
            if (_crosshair) {
                _crosshairAnimator = (IntelliCrosshair)_crosshair.GetComponent (typeof(IntelliCrosshair));
            }
        }
 
 
        //---
        //
        //---
        public void Start ()
        {
            if (_defaultWeapon) {
                _body.UseWeaponNow (gameObject, false);
                _defaultWeapon = false;
            }
         
            // setup animation layers
            if (_animator.animation [("fire-" + _weaponsName)] != null)
                _animator.animation [("fire-" + _weaponsName)].layer = 2;
            if (_animator.animation [("empty-" + _weaponsName)] != null)
                _animator.animation [("empty-" + _weaponsName)].layer = 3;
            if (_animator.animation [("reload-" + _weaponsName)] != null)
                _animator.animation [("reload-" + _weaponsName)].layer = 1;
            if (_animator.animation [("change-" + _weaponsName)] != null)
                _animator.animation [("change-" + _weaponsName)].layer = 2;
            if (_animator.animation [("activate-" + _weaponsName)] != null)
                _animator.animation [("activate-" + _weaponsName)].layer = 2;
            if (_animator.animation [("deactivate-" + _weaponsName)] != null)
                _animator.animation [("deactivate-" + _weaponsName)].layer = 2;
        }
 
 
        //---
        //
        //---
        public void LateUpdate ()
        {
            if (_muzzleBlastLight) {
                if (_muzzleBlastLight.intensity > 0)
                    _muzzleBlastLight.intensity -= 0.05F;
            }
         
            if (_muzzleBlast) {
                _muzzleBlast.active = (_muzzleBlastLight.intensity <= 0) ? false : true;
            }
        }
     
     
        //---
        //
        //---
        public bool WeaponInUse ()
        {
            return(_weaponInUse);
        }
 
 
        //---
        //
        //---
        public void UseWeapon ()
        {
            _weaponInUse = true;
            gameObject.layer = _gunLayer;
            foreach (Transform child in transform) {
                child.gameObject.layer = _gunLayer;
            }
         
            if (rigidbody)
                Destroy (rigidbody);
            if (gameObject.collider)
                Destroy (gameObject.collider);
         
            // first setup of weapon
            if (_firstSetUp) {
                _dataController.initialized = true;
                _dataController.max = _maxAmmo;
                _dataController.current = _dataController.max;
                _firstSetUp = false;
            }
         
            //--- does this weapon hide the crosshair?
            if (_crosshair) {
                //Debug.Log( "setting crosshair to " + (!_hideCrosshair) );
                _crosshair.SetActiveRecursively (! _hideCrosshair);
            }
             
            if (_equipAudio) {
                if ((!_audioSrcFire1.isPlaying) || (_audioSrcFire1.clip != _equipAudio)) {
                    //Debug.Log( "--- equip audio (" + _equipAudio.name + ") ---" );
                    if (_audioSrcFire1.clip != null)
                        Debug.Log ("currSnd (" + _audioSrcFire1.clip.name + ")");
                    _audioSrcFire1.PlayOneShot (_equipAudio);
                }
            }
         
            //--- should the hands be hidden?
            if (_hideHands) {
                if (_hands)
                    _hands.SetActiveRecursively (false);
            }
         
            //--- should the weapon be hidden? (workaround for missing hands only FBX)
            if (_hideWeapon) {
                //Debug.Log( "hiding weapon" );
                if (_weapon)
                    _weapon.SetActiveRecursively (false);
            }
        }
 
 
        //---
        //
        //---
        public void DropWeapon ()
        {
            _weaponInUse = false;
            //Debug.Log( "Physically Drop this Weapon: " + name );
            // instantiate pickup copy of weapon
            gameObject.SetActiveRecursively (false);
        }
 
 
        //---
        //
        //---
        public IEnumerator SwitchWeapon (String clip)
        {
            _weaponInUse = false;
            String str = clip + _weaponsName;
            //Debug.Log( "Switching this Weapon: " + _weaponsName );
         
            // play deactivate aimation
            if (_animator.animation [str] != null) {
                _animator.animation.CrossFade (str);
            }
         
            //if( _equipAudio ) UPSound.PlayOneShot( _equipAudio );
            //if( _equipAudio ) _audioSrcFire1.PlayOneShot( _equipAudio );
     
            yield return new WaitForSeconds (0.5F);
            gameObject.SetActiveRecursively (false);
            _body.ActivateWeapon ();
        }
 
 
        //---
        //
        //---
        public void FirePrimary (String fire, String empty)
        {
            if (NextShot ()) {
                //--- if no ammo, play empty magazine sound
                if (_usesAmmo && (_dataController.current <= 0)) {
                    if (_emptyMagazineSnd) {
                        _audioSrcFire1.clip = _emptyMagazineSnd;
                        _audioSrcFire1.Play ();
                    }
                }
     
                if (_dataController.current > 0) {
                    //--- don't fire if _burstMax has been reached
                    if (BurstPause ())
                        return;
                 
                    if (_1stShootFrom) {
                        AnimateCrosshair ();
                        Vector3 dir = _1stShootFrom.TransformDirection (_shootVector);
         
                        // Did we hit anything?
                        if (Physics.Raycast (_1stShootFrom.position, dir, out _hit, _weaponMaxRange, _layerMask)) {
                            Vector3 contact = _hit.point;
                            Quaternion rot = Quaternion.FromToRotation (Vector3.up, _hit.normal);
                         
                            if (! _hit.collider.CompareTag ("Player")) {
                                // Apply a force to the rigidbody we hit
                                if (_hit.rigidbody)
                                    _hit.rigidbody.AddForceAtPosition (_force * dir, contact);
                             
                                if (_hit.transform.tag.IndexOf ("Interact") == -1) {
                                    if ((_hit.transform.tag != "NoBulletHoles") && (_hit.transform.tag != "Untagged") && (_hit.transform.tag != "Enemy")) {
                                        // clone a temp marker
                                        if (_bulletHole) {
                                            GameObject tr = (GameObject)Instantiate (_bulletHole, contact, rot);
                                            tr.SendMessage ("SurfaceType", _hit);
                                            tr.transform.parent = _hit.transform; // parent to hit object so the bullet holes move with object
                                        }
                                    }
                                }
                                if (_impactSparks) {
                                    Instantiate (_impactSparks, contact, rot);
                                }
                             
                                // Send a damage message to the hit object
                                DamageSource damageSource = new DamageSource
                             {
                                 damageAmount = _damage,
                                 fromPosition = _1stShootFrom.position,
                                 appliedToPosition = contact,
                                 sourceObject = _weaponOwner,
                                 sourceObjectType = DamageSource.DamageSourceObjectType.Player,
                                 sourceType = DamageSource.DamageSourceType.GunFire,
                                           hitCollider = _hit.collider
                             };
 
                                _hit.collider.SendMessageUpwards ("ApplyDamage", damageSource, SendMessageOptions.DontRequireReceiver);
                            }
                        }
                     
                    }
                 
                    // has sound fx so play it
                    if (_fireAudioList.Length > 0) {
                        int audioChoice = (int)UnityEngine.Random.Range (0, _fireAudioList.Length);
                        _audioSrcFire1.PlayOneShot (_fireAudioList [audioChoice]);
                    }
                 
                    if (_usesAmmo) {
                        _muzzleBlast.active = true;
                        _muzzleBlastLight.intensity = _blastInsensity;
                    }
                 
                    // finally play the animation clip
                    if (_animator.animation [(fire + _weaponsName)] != null) {
                        _animator.animation.CrossFade (fire + _weaponsName);
                    }
                 
                    // reduce ammo
                    if (_usesAmmo) {
                        _dataController.current --;
                        _burstCount++;
                    }
                 
                    if (_dataController.current <= 0) {
                        if (!_empty) {
                            _animator.animation.Stop ();
                            if (_animator.animation [(empty + _weaponsName)] != null) {
                                _animator.animation.CrossFade (empty + _weaponsName);
                            }
                            _empty = true;
                        }
                    }
                }
            }
        }
 
 
        //---
        //
        //---
        public void FirePrimaryStop (String fire)
        {
            BurstReset ("fire stop");
        }
 
 
        //---
        // return true if bursting should be paused
        //---
        public bool BurstPause ()
        {
            bool burstDontFire = false;
     
            if ((_burstMax > 0) && (_burstDelay > 0.0)) {
                //Debug.Log( "burst count " + _burstCount );
                if (_burstCount == _burstMax) {      
                    burstDontFire = true;
             
                    if (_burstTimer > 0.0) {
                        if (Time.time > _burstTimer) {
                            BurstReset ("timer expired");
                            burstDontFire = false;
                        }
                    } else {
                        _burstTimer = Time.time + _burstDelay;
                        //Debug.Log( "set burst timer" );
                    }
                }
            }
         
            return(burstDontFire);
        }
 
 
        //---
        //
        //---
        public void BurstReset (String reason)
        {
            _burstTimer = 0.0F;
            _burstCount = 0;
            //Debug.Log( "Burst reset: " + reason );
        }
 
 
        //---
        //
        //---
        public IEnumerator AnimateCrosshair ()
        {
            if (_crosshairAnimator) {
                _crosshairAnimator.shoot = true;
                yield return new WaitForSeconds( _crosshairTime );
                _crosshairAnimator.shoot = false;
            }
        }
 
 
        //---
        //
        //---
        public bool NextShot ()
        {
            bool canFire = false;
         
            if (Time.time > _nextFire1) {
                canFire = true;
                _nextFire1 = Time.time + _fireRate1;
            }
         
            return canFire;
        }
 
 
        //---
        //
        //---
        public void SetBulletShootFrom (Transform sft)
        {
            _1stShootFrom = sft;
        }
 
 
        //---
        //
        //---
        public void SetIgnoreColliders (Collider[] cols)
        {
            _ignoreColliders = cols;
        }
 
 
        //---
        //
        //---
        public void ReloadWeaponStart ()
        {
            //   if( _reloadAudio ) UPSound.PlayOneShot( _reloadAudio );
            if (_reloadAudio)
                _audioSrcFire1.PlayOneShot (_reloadAudio);
        }
 
 
        //---
        //
        //---
        public void ReloadWeapon ()
        {
            _dataController.max = _maxAmmo;
            _dataController.current = _dataController.max;
            _empty = false;
            BurstReset ("weapon reloaded");
        }
 
 
        //---
        //
        //---
        public Vector3 ReturnShootDirection ()
        {
            Vector3 vec = Vector3.forward;
         
            switch (_shootDirection) {
            case ShootDirection.Right:
                vec = Vector3.right;
                break;
            case ShootDirection.Left:
                vec = Vector3.left;
                break;
            case ShootDirection.Forward:
                vec = Vector3.forward;
                break;
            case ShootDirection.Backward:
                vec = Vector3.back;
                break;
            }
            return vec;
        }
    }
}