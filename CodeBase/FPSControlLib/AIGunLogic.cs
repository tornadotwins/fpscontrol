/*
jmpp: Done!
*/
using UnityEngine;
using System;
using System.Collections;

namespace FPSControl
{
    
    /*!
    Posible shooting directions.
    */
    public enum AIShootDirection
    {
        /*! Corresponds to Vector3.right */
        Right,
        /*! Corresponds to Vector3.left */
        Left,
        /*! Corresponds to Vector3.forward */
        Forward,
        /*! Corresponds to Vector3.back */
        Backward
    }
 
    /*!
    Accuracy of a weapon.
    */
    public enum Accuracy
    {
        /*!
        Causes an actual damage equal to half of that of the highest accuracy
        */
        Poor,
        /*!
        Cuases an actual damage equal to two thirds of that of the highest accuracy
        */
        Medium,
        /*!
        Cuases the highest possible damage
        */
        High
    }
 
 
    //! Non-melee weapons AI. 
    /*!
    The AIGunLogic component manages all the logic related to the use of non-melee weapons, e.g. firing, available ammo, reloading and switching between them, etc.
    */
    public class AIGunLogic : MonoBehaviour
    {
  
        /*!      
        Weapon name identifying the animation clips that should be played on various events, e.g. weapon activating, firing, reloading, etc.
        */
        public string                    _weaponsName;
        /*!
        GameObject for the character employing the weapon.
        */
        public GameObject                _weaponOwner;

        //GameObject     _animator;
        /*!
        Accuracy of the weapon action, wheighin the actual damage it causes, with a efault value of medium.
        */
        public Accuracy              accuracy = Accuracy.Medium;
        /*!
        %Damage the weapon causes, with a default value of 5.
        */
        public float                 _damage = 5f;
        /*!
        Colliders ignored by the action of the weapon.
        */
        public Collider[]                _ignoreColliders;
        /*!
        Bitmask of layers that should be affected by the action of the weapon.
        */
        public LayerMask             _layerMask;
        /*!
        Whether the action of the weapon is powered by expendable ammunition, with a default value of true.
        */
        public bool                      _usesAmmo = true;
        /*!
        Whether the weapon should be hidden from view, with a default value of false.
        */
        public bool                      _hideWeapon = false;
        /*!
        The gameObject for the actual weapon.
        */
        public GameObject                _weapon;
     
        // make sure you have enough ammo gui pics to cover this amount. pic array length will be divided into this numbers
        /*!
        Maximum ammount of ammo, with a default value of 200.
        */
        public float                 _maxAmmo = 200f;
        /*!
        Maximum number of shots per weapon actioning (i.e. rounds). If zero (default), the weapon fires continually and acts as an automatic.
        */
        public int                       _burstMax = 0;
        /*!
        Time delay between shooting rounds, with a default value of 0.
        */
        public float                 _burstDelay = 0f;
        private int                      _burstCount = 0;
        private float                    _burstTimer = 0f;
        /*!
        Empty weapon sound.
        */
        public AudioClip             _emptyMagazineSnd;
        /*!
        Wait for reload animation...
        */
        public float                 _reloadingTime;
     
        //--- Weapon related
        /*!
        Direction in which the weapon shoots, with a default value of forward.
        */
        public AIShootDirection      _shootDirection = AIShootDirection.Forward;
        private Vector3              _shootVector;
        /*!
        Maximum reach of the projectiles fired by the weapon, with a default value of 100.
        */
        public float                 _weaponMaxRange = 100f;
        /*!
        Recoil force experienced by the target hit by the weapon, with a default value of 10.
        */
        public float                 _force = 10f;
        /*!
        GameObject representing the bullet hole left by the action of the weapon's projectiles.
        */
        public GameObject                _bulletHole;
        /*!
        GameObject representing the sparks erupting upon impact of the weapon's projectiles.
        */
        public GameObject                _impactSparks;
        /*!
        Array of AudioClips to be played randomly upon actioning of the weapon.
        */
        public AudioClip[]           _fireAudioList;
        /*!
        AudioClip to be played upon weapon reloading.
        */
        public AudioClip             _reloadAudio;
        /*!
        AudioClip to be played upon first using a weapon, e.g. by switching to it.
        */
        public AudioClip             _equipAudio;
        /*!
        AudioSource to be used as the player for the various weapon actioning sounds.
        */
        public AudioSource           _audioSrcFire1;
        /*!
        Delay between shots of a single round, with a default value of 0.5 seconds.
        */
        public float                     _fireRate1 = 0.5f;
        private float                    _nextFire1 = 0f;
        /*!
        Layer of the weapon gameObject.
        */
        public int                       _gunLayer = -1;
        /*!
        Transform whose position should be used as the origin of the weapon's shots.
        */
        public Transform             _1stShootFrom;
        /*!
        Transform of the gameObject that's currently being aimed at.
        */
        public Transform             _1stShootTo;
        /*!
        GameObject representing the visual blast of the detonations upon weapon actioning.
        */
        public GameObject                _muzzleBlast;
        /*!
        Light of the blasts of the weapon detonations.
        */
        public Light                 _muzzleBlastLight;
        /*!
        Intensity of the #_muzzleBlastLight, with a default value of 0.5.
        */
        public float                 _blastInsensity = 0.5f;
        /*!
        Whether the weapon is currently out of ammo, with a default value of false.
        */
        public bool                      _empty = false;
     
        /*!
        Reference to the DataController component that keeps track of the currently available ammo.
        */
        public DataController            dataController;
     
        // offset for when player is looking down
        /*!
        Normal position of the hands holding the weapon.
        */
        public Vector3                   _handsNormalPos;
        /*!
        Vertical offset that the position of the weapon should be offset when the Player character is looking down. 
        */
        public Vector3                   _lookDownOffset;
     
        // VARIABLES for RAYCAST SHOOTING
        private RaycastHit           _hit;
        private Rect                 _gunSightRect;
        private bool                 _weaponInUse = false;
        private bool                 _firstSetUp = true;

     

        /*!
        Sets up the shooting direction for the weapon by invoking the ReturnShootDirection() method.
        */
        public void Awake ()
        {
            _shootVector = ReturnShootDirection ();
        }


        /*!
        Sets up the selected weapon for first use.
        */
        public void Start ()
        {    
            // setup animation layers
            //if( _animator.animation[("fire-"+_weaponsName)] != null ) _animator.animation[("fire-"+_weaponsName)].layer = 2;
            //if( _animator.animation[("empty-"+_weaponsName)] != null ) _animator.animation[("empty-"+_weaponsName)].layer = 3;
            //if( _animator.animation[("reload-"+_weaponsName)] != null ) _animator.animation[("reload-"+_weaponsName)].layer = 1;
            //if( _animator.animation[("change-"+_weaponsName)] != null ) _animator.animation[("change-"+_weaponsName)].layer = 2;
            //if( _animator.animation[("activate-"+_weaponsName)] != null ) _animator.animation[("activate-"+_weaponsName)].layer = 2;
            //if( _animator.animation[("deactivate-"+_weaponsName)] != null ) _animator.animation[("deactivate-"+_weaponsName)].layer = 2;
         
            UseWeapon ();
        }


        /*!
        Activates the blast of the weapon detonations and gradually decreases the intensity of the blast's light.
        */
        public void LateUpdate ()
        {
            if (_muzzleBlastLight) {
                if (_muzzleBlastLight.intensity > 0)
                    _muzzleBlastLight.intensity -= 0.05f;
            }
         
            if (_muzzleBlast) {
                _muzzleBlast.active = (_muzzleBlastLight.intensity <= 0) ? false : true;
            }
        }


        /*!
        Prepares the weapon for first use, initializing the ammo counting \link DataController dataController\endlink component and destroying the rigidbody and
        colliders the gameObject may posses. If the #_equipAudio AudioClip is not null, it is played here as an indication of weapon readiness.        
        */
        public void UseWeapon ()
        {
            _weaponInUse = true;
            //gameObject.layer = _gunLayer;
            //for (var child : Transform in transform)
            //{
            //    child.gameObject.layer = _gunLayer;
            //}
         
            if (GetComponent<Rigidbody>() != null)
                Destroy(GetComponent<Rigidbody>());
            if (GetComponent<Collider>() != null)
                Destroy(GetComponent<Collider>());
         
            // first setup of weapon
            if (_firstSetUp) {
                dataController.initialized = true;
                dataController.max = _maxAmmo;
                dataController.current = dataController.max;
                _firstSetUp = false;
            }
                 
            if (_equipAudio)
                _audioSrcFire1.PlayOneShot (_equipAudio);
             
            //--- should the weapon be hidden? (workaround for missing hands only FBX)
            //if( _hideWeapon )
            //{
            //   if( _weapon ) _weapon.SetActiveRecursively( false );
            //}
        }


        /*!
        Deactivates the weapon in use.
        */
        public void DropWeapon ()
        {
            _weaponInUse = false;
            //Debug.Log( "Physically Drop this Weapon: " + name );
            // instantiate pickup copy of weapon
            gameObject.SetActiveRecursively (false);
        }


        /*!
        Deactivates the weapon currently in use and switches to a new one after a slight time delay.
        
        \param clip Unused.
        */
        public IEnumerator SwitchWeapon (string clip)
        {
            _weaponInUse = false;
            //string str = clip + _weaponsName;
            //Debug.Log( "Switching this Weapon: " + name );
         
            // play deactivate aimation
            //if( _animator.animation[str] != null )
            //{
            //   _animator.animation.CrossFade (str);
            //}
         
            //if( _equipAudio ) UPSound.PlayOneShot( _equipAudio );
            //if( _equipAudio ) _audioSrcFire1.PlayOneShot( _equipAudio );
     
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActiveRecursively (false);
        }


        /*!
        Fires the primary weapon if the ammo-counting \link DataController dataController's\endlink \link DataController#current current\endlink property is greater than zero, indicating the
        availability of ammo, at maximum rate dictated by the #_fireRate1 time delay between shots. Otherwise, the #_emptyMagazineSnd AudioClip
        is played  if its non-null and the weapon is set to use ammo.
        
        If the maximum number of shots per round has been reached, and the round delay parameter #_burstDelay is non-zero, firing stops until a next
        round of shots is allowed.
        
        When actual shooting takes place, the direction of the shots are calculated based on the relative positions of the #_1stShootFrom and #_1stShootTo
        transforms, firing up to a distance of #_weaponMaxRange and colliding with those gameObjects located in any of the layers recorded in the #_layerMask
        bitmask. If #_fireAudioList is non-empty, a random AudioClip is selected and played, and the #_muzzleBlast gameObject is instantiated if its
        non-null (with the muzzle blast light's intensity resetted to its maximum value, dictated by the #_blastInsensity parameter).
        
        Targets hit are impacted with a force of magnitude dictated by the #_force parameter and in the direction of the shots, and a bullet
        hole marker is instantiated at the location of impact if the target is either not untagged, not tagged as "NoBulletHoles", or its tag does not
        contain the string "Interact". In any of these cases, the gameObject representing the projectile impact sparks (#_impactSparks) is always instantiated at the point of
        impact.
        
        Impact damage is calculated based on the #_damage paramenter and \link Accuracy weapon accuracy\endlink. If the accuracy is poor, actual damege is half of that dictated
        by the value of #_damage; if it is medium, actual damage is 2/3 its value; and finally a high accuracy does not affect the value of #_damage.
        
        Upon projectile impact, a DamageSource object is instantiated and configured with the characteristics of the weapon and the impact,
        and an "ApplyDamage" message is sent upward in the hierarchy of the impacted gameObject with this object as argument.
        
        \param fire Unused.
        \param empty Unused.
        */
        public void FirePrimary (string fire, string empty)
        {
            if (NextShot ()) {
                //--- if no ammo, play empty magazine sound
                if (_usesAmmo && (dataController.current <= 0)) {
                    if (_emptyMagazineSnd) {
                        _audioSrcFire1.clip = _emptyMagazineSnd;
                        _audioSrcFire1.Play ();
                    }
                }
     
                if (dataController.current > 0) {
                    //--- don't fire if _burstMax has been reached
                    if (BurstPause ())
                        return;
                 
                    if (_1stShootFrom && _1stShootTo) {
                        //           var dir:Vector3 = _1stShootFrom.TransformDirection ( _shootVector );
                        Vector3 dir = _1stShootTo.position - _1stShootFrom.position;
                     
         
                        // Did we hit anything?
                        if (Physics.Raycast (_1stShootFrom.position, dir, out _hit, _weaponMaxRange, _layerMask)) {
                            Vector3 contact = _hit.point;
                            Quaternion rot = Quaternion.FromToRotation (Vector3.up, _hit.normal);
                            // Apply a force to the rigidbody we hit
                            if (_hit.rigidbody)
                                _hit.rigidbody.AddForceAtPosition (_force * dir, contact);
                         
                            if (_hit.transform.tag.IndexOf ("Interact") == -1) {
                                if ((_hit.transform.tag != "NoBulletHoles") && (_hit.transform.tag != "Untagged")) {
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
                         
                         
                            float actualdamage = _damage;
                            if (accuracy == Accuracy.Poor) {
                                actualdamage = actualdamage / 2;
                            } else if (accuracy == Accuracy.Medium) {
                                actualdamage = actualdamage / 1.5f;
                            }
                         
                            DamageSource damageSource = new DamageSource ();
                            damageSource.damageAmount = actualdamage;
                            damageSource.fromPosition = _1stShootFrom.position;
                            damageSource.appliedToPosition = contact;
                            damageSource.sourceObject = _weaponOwner;
                            damageSource.sourceObjectType = DamageSource.DamageSourceObjectType.Player;
                            damageSource.sourceType = DamageSource.DamageSourceType.GunFire;
                            damageSource.hitCollider = _hit.collider;
                         
                            _hit.collider.SendMessageUpwards ("ApplyDamage", damageSource, SendMessageOptions.DontRequireReceiver);
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
                             
                    // reduce ammo
                    if (_usesAmmo) {
                        dataController.current --;
                        _burstCount++;
                    }
                 
                }
            }
        }


        /*!
        Stops firing by resetting the current round of shots.
        
        \param fire Reason for stopping fire.
        */
        public void FirePrimaryStop (string fire)
        {
            BurstReset ("fire stop");
        }


        /*!
        Determines whether shooting should take place, based on the values of the #_burstMax and #_burstDelay parameters.
        
        If #_burstMax is zero, the weapon is considered to be automatic and shooting never stops (i.e. this method never interrupts it).
        Otherwise, when enough shots have been fired by FirePrimary() and #_burstMax has been reached, this method returns false for the
        duration of the #_burstDelay time interval. Once this timer expires, false is returned once again and firing is allowed to recommence.
        
        \retval boolean True or false indicating if the weapon is allowed to fire. 
        */
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


        /*!
        Resets the counters keeping track of the shots fired per round.
        
        \param reason The reason for the reset.
        */
        public void BurstReset (string reason)
        {
            _burstTimer = 0f;
            _burstCount = 0;
            //Debug.Log( "Burst reset: " + reason );
        }


        /*!
        Calculates whether the weapon can fire a next shot within the same round, as dictated by the #_fireRate1 time delay parameter.
        
        \retval boolean True or false indicating if the weapon is allowed to fire.
        */
        public bool NextShot ()
        {
            bool canFire = false;
         
            if (Time.time > _nextFire1) {
                canFire = true;
                _nextFire1 = Time.time + _fireRate1;
            }
         
            return canFire;
        }


        /*!
        Sets the source transform (#_1stShootFrom) from whose position the shots emmanate.
        
        \param sft The source transform.
        */
        public void SetBulletShootFrom (Transform sft)
        {
            _1stShootFrom = sft;
        }


        /*!
        Sets the colliders that shots from the selected weapon should ignore.
        
        \param cols Array of colliders to ignore.
        */
        public void SetIgnoreColliders (Collider[] cols)
        {
            _ignoreColliders = cols;
        }


        /*!
        Kickstarts the reloading action of the weapon by playing the #_reloadAudio AudoClip if non-null.
        */
        public void ReloadWeaponStart ()
        {
            //   if( _reloadAudio ) UPSound.PlayOneShot( _reloadAudio );
            if (_reloadAudio)
                _audioSrcFire1.PlayOneShot (_reloadAudio);
        }


        /*!
        Performs the actual reloading of the weapon by resetting the ammo-counting \link DataController dataController's\endlink \link DataController#max max\endlink property to the
        value of the #_maxAmmo parameter, and by invoking the BurstReset() method to reset the firing round.
        */
        public void ReloadWeapon ()
        {
            dataController.max = _maxAmmo;
            dataController.current = dataController.max;
            _empty = false;
            BurstReset ("weapon reloaded");
        }


        /*!
        Calculates the shooting direction vector based on the value of #_shootDirection \link AIShootDirection enumerated parameter\endlink:
        to the right, to the left, forward or backward.
        
        \retval Vector3 The shooting direction.
        */
        public Vector3 ReturnShootDirection ()
        {
            Vector3 vec = Vector3.forward;
         
            switch (_shootDirection) {
            case AIShootDirection.Right:
                vec = Vector3.right;
                break;
            case AIShootDirection.Left:
                vec = Vector3.left;
                break;
            case AIShootDirection.Forward:
                vec = Vector3.forward;
                break;
            case AIShootDirection.Backward:
                vec = Vector3.back;
                break;
            }
            return vec;
        }
     
    }
}