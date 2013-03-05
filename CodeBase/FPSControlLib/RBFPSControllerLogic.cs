//=====
//   Acknowledgements:
//   Firstly this script started out as a mash-up of several freely available scripts from the Unity Wiki
//   I then put these all together into a re-usable FPS Controller prefab and added my own additions.
//   
//   All thanks to the original developers of the following scripts
//   http://www.unifycommunity.com/wiki/index.php?title=Headbobber
//   http://www.unifycommunity.com/wiki/index.php?title=PhysicsFPSWalker
//   http://www.unifycommunity.com/wiki/index.php?title=RigidbodyFPSWalker
//   http://www.unifycommunity.com/wiki/index.php?title=VariableSpeedFPSwalker
//
//   Enjoy! Cheers thylaxene.
//
// PLAYER VARIABLES
// cache all possible weapons here
// each weapon is charged with looking after its own ammo count etc
// they are only shown as a selection option if the player's backpack has it loaded
//=====


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using Legion.Core;
//using Legion.Ontology;



//---
//Please shoot the guy who wrote this for not using a statemachine
//---
namespace FPSControl
{
	public enum MoveState 
	{ 
		None = 0 ,
		Walking = 1,
		Running = 2, 
		Backwards = 3, 
		Crouching = 4, 
		Jumped = 5 
	}
	
	[RequireComponent(typeof(FootstepControl))]
	[Obsolete("This class is now obsolete. Use FPSControlPlayer instead.",false)]
	public class RBFPSControllerLogic : MonoBehaviour
	{
	    /* [HideInInspector] */public Collider[] _ignoreColliders;
	    /* [HideInInspector] */public GameObject[] _weaponsInUseSkins;
	    /* [HideInInspector] */private bool[] _weaponIsPickedUp;
	    /* [HideInInspector] */private int _currentWeapon = 0;
	    /* [HideInInspector] */private int _previousWeapon = -1;
	    /* [HideInInspector] */private int _availableWeapons = 1; // always have a default weapon even if it is just your hands
	 //private Array                 _myBackpack;
	
	 // FPS WALKER VARIABLES
	 // KEYS
	    /* [HideInInspector] */public KeyCode _runKeyLeft = KeyCode.LeftShift;
	    /* [HideInInspector] */public KeyCode _runKeyRight = KeyCode.RightShift;
	    /* [HideInInspector] */public KeyCode _interactionKey = KeyCode.E;
	    /* [HideInInspector] */public KeyCode _reloadKey = KeyCode.R;
	    /* [HideInInspector] */public KeyCode _escapeKey = KeyCode.Escape;
	    /* [HideInInspector] */public KeyCode _weaponKey1 = KeyCode.Alpha1;
	    /* [HideInInspector] */public KeyCode _weaponKey2 = KeyCode.Alpha2;
	    /* [HideInInspector] */public KeyCode _weaponKey3 = KeyCode.Alpha3;
	    /* [HideInInspector] */public KeyCode _weaponKey4 = KeyCode.Alpha4;
	    /* [HideInInspector] */public KeyCode _weaponToggle = KeyCode.Q;
	    
	    /* [HideInInspector] */public float _forwardSpeed = 6.0F;
	    /* [HideInInspector] */public float _runSpeedModifier = 2.0F; 
	    /* [HideInInspector] */public float _backwardSpeed = 3.0F; 
	    /* [HideInInspector] */public float _strafeSpeed = 5.0F;
	    /* [HideInInspector] */public float _climbSpeedModifier = 0.5F;
	    /* [HideInInspector] */public float _gravity = 20.0F;
	    /* [HideInInspector] */public float _maxVelocityChange = 12.0F;
	    /* [HideInInspector] */public bool _canJump = true;
	    /* [HideInInspector] */public float _jumpHeight = 1.2F;
	    /* [HideInInspector] */public float _jumpingRate = 1.5F;
	    /* [HideInInspector] */private float _nextJump = 0;
	    /* [HideInInspector] */private bool _grounded = false;
	    /* [HideInInspector] */private bool _ladderClimbing = false;
	//  private float                   _storedSpeed;
	//  private float                   _storedSensiX;
	//  private float                   _storedCamSensiX;
	//  private float                   _storedCamSensiY;
	//  private float                   _slowedSpeed;
	//  private float                   _slowedSensiX;
	//  private float                   _slowedCamSensiX;
	//  private float                   _slowedCamSensiY;
	    /* [HideInInspector] */private bool _offLadder = true;
	    /* [HideInInspector] */private GameObject _ladderTop;
	    /* [HideInInspector] */private bool _reloading = false;
	    /* [HideInInspector] */private string _currentClipPlaying = "";
	    
	     // INTERACTION
	    /* [HideInInspector] */public bool _allowInteracting = true;
	    /* [HideInInspector] */public float _interactionMaxRange = 2.0F;
	    /* [HideInInspector] */private bool _inInteractZone = false;
	    /* [HideInInspector] */private GameObject _interactObj;
	    /* [HideInInspector] */public CrosshairAnimator _crosshair;
	    /* [HideInInspector] */public float _scrollDelay = 0.25F;
	    /* [HideInInspector] */private float _scrollTimer = 0F;
	//  public Decoration               _decoration;
	
	 // GUN SETUP INTO HANDS
	    /* [HideInInspector] */public GameObject _shoulders;
	    /* [HideInInspector] */public string[] _gunInHandsAnimationClips;
	    /* [HideInInspector] */public AudioSource _audioSrcMomentary;
	    /* [HideInInspector] */public AudioClip _weaponNotPickedUpAudio;
	    
	     // ARTISTIC GUN STUFF
	    /* [HideInInspector] */public Transform _targetAt;
	    /* [HideInInspector] */public Transform _gunPivot;
	    /* [HideInInspector] */public float _slowDowner = 0.3F;
	    /* [HideInInspector] */public float _gunTurnRate = 8.0F; //--- delay rate at which the weaponHolder rotation is offset from player look rotation
	    /* [HideInInspector] */public float _gunTurnDistMax = 5.0F;
	    /* [HideInInspector] */private float _gunTurnVal = 0.0F; //--- holds momentary horizontal look delta
	     // used in the lerp, to slightly delay the gun's turning to match the player's view. Doesn't affect the aim at all!
	    /* [HideInInspector] */public float _turnRate = 8F;
	    /* [HideInInspector] */public float _sprayAmount = 0.2F; // 0.1 will spray bullets more  or less within the crosshair area
	    //  private float                   _nextTurn                   = 0.0F;
	    /* [HideInInspector] */public float _offsetGunHandsSmooth = 5; // speed of moving hands into look down position
	    /* [HideInInspector] */private Vector3 _offsetVec;
	    /* [HideInInspector] */public bool _offsetHandsToLookDownPosition = false;
	        
	 // HEAD BOBBING VARIABLES
	    /* [HideInInspector] */public bool _headBob = false;
	    /* [HideInInspector] */public float _bobbingSpeed = 0.055F;
	    /* [HideInInspector] */public float _bobbingAmount = 0.0085F;
	    /* [HideInInspector] */public float _camRestY = 0.9F; // rest position of the camera's Y
	    /* [HideInInspector] */public float _camRestX = 0.0F; // rest position of the camera's X
	    /* [HideInInspector] */public float _xBobDamper = 0.75F; // X movement should be more subtle then Y
	    /* [HideInInspector] */private float _timer = 0.0F;
	
	 // MOUSE LOOK
	    /* [HideInInspector] */private Transform _mouseLookXTransform;
	    /* [HideInInspector] */private Transform _mouseLookYTransform;
	    /* [HideInInspector] */private Transform _gunRenderCam;
	    /* [HideInInspector] */public float _dampeningAmt = 0.5F; // how much to dampen the initial movement of the mouse look
	    /* [HideInInspector] */public int _mouseFilterBufferSize = 10; // don't set this too high!
	    /* [HideInInspector] */public float _sensitivityX = 8.0F;
	    /* [HideInInspector] */public float _sensitivityY = 2.0F;
	    /* [HideInInspector] */public float _minimumX = -360.0F;
	    /* [HideInInspector] */public float _maximumX = 360.0F;
	    /* [HideInInspector] */public float _minimumY = -50.0F;
	    /* [HideInInspector] */public float _maximumY = 50.0F;
	    /* [HideInInspector] */public float _gunLookDownOffsetThreshold = -1;
	    /* [HideInInspector] */private Quaternion _originalRotationX;
	    /* [HideInInspector] */private Quaternion _originalRotationY;
	    /* [HideInInspector] */private float _rotationX = 0.0F;
	    /* [HideInInspector] */private float _rotationY = 0.0F;
	    /* [HideInInspector] */private List<float> _xInputBuffer;
	    /* [HideInInspector] */private List<float> _yInputBuffer;
	    /* [HideInInspector] */private List<float> _lutWeightMod;
	
	 // CACHING FOR SPEED
	    /* [HideInInspector] */private Camera _mainCam;
	    /* [HideInInspector] */private GunLogic _gunBrains;
	    /* [HideInInspector] */private RaycastHit _hit;
	
	 // FLOW CONTROLS
	//  private bool                    _aiming                         = false;
	//  private List                    _shootFroms                     = new List ();
	//  private bool                    _movingPltfrm                   = false;
	    /* [HideInInspector] */private bool _wasLocked = false;
	    /* [HideInInspector] */private bool _play = false;
	    /* [HideInInspector] */private bool _alive = true;
	
	 //@script RequireComponent(Rigidbody, CapsuleCollider)
	//  [RequireComponent(typeof(FootstepControl))]
	
	    /* [HideInInspector] */public MoveState state = MoveState.None;
	
	 
	 public void Awake()
	 {
	     rigidbody.freezeRotation = true;
	     rigidbody.useGravity = false;
	     // Caching
	     _mainCam = Camera.main;
	     _gunRenderCam = transform.Find ("gunRenderer");
	     _mouseLookXTransform = transform;
	     _mouseLookYTransform = _mainCam.transform;
	     // Set up some variables
	//      _storedFOV = _mainCam.fieldOfView;
	//      _camFOV = _storedFOV;
	//      _storedSpeed = _forwardSpeed;
	//      _storedSensiX = _sensitivityX;
	//      _storedCamSensiX = _sensitivityX;
	//      _storedCamSensiY = _sensitivityY;
	     // set-up the slow downed movement and look variables
	//      _slowedSensiX = _storedSensiX * _slowDowner;
	//      _slowedCamSensiX = _storedCamSensiX * _slowDowner;
	//      _slowedCamSensiY = _storedCamSensiY * _slowDowner;
	//      _slowedSpeed = _storedSpeed * _slowDowner;
	     _originalRotationX = _mouseLookXTransform.localRotation;
	     _originalRotationY = _mouseLookYTransform.localRotation;
	     // setup mouse filtering variables
	     SetupMouseFiltering ();
	     
	     //--- flag the Hands as the only picked up weapon
	     _weaponIsPickedUp = new bool[_weaponsInUseSkins.Length];
	     _weaponIsPickedUp[0] = true;
	     for( int i=1; i<_weaponsInUseSkins.Length; i++ )
	     {
	         _weaponIsPickedUp[i] = false;
	     }
	 }
	
	 
	 public IEnumerator Start()
	 {
	     // lock the cursor
	     Screen.lockCursor = true;
	     _wasLocked = true;
	     // small delay while game sets up
	     // normally this would be handled by the level setup and/or game manager singleton
	     yield return new WaitForSeconds(1.5F);
	     _play = true;
	 }
	
	 
	 public void Update()
	 {
	     if (!_play) return;
	     
	     //--- no user input if dead
	     if( !_alive ) return;
	         
	     // in here put things that need to be turned off if aiming, Snipers don't bob!
	     if ( _grounded )
	     {
	         if ( _headBob )
	         { // lets bob the head!
	             BobDaHead ();
	         }
	     }
	     
	     // the business end stuff
	     // the fire logic is actually part of the gun
	     // so see the GunLogic script attached to each gun prefab
	     // trigger 1
	     if( ! _reloading )
	     {
	         if( Input.GetButton ("Fire1") )     FirePrimary();
	         if( Input.GetButtonUp( "Fire1" ) )  FirePrimaryStop();
	     }
	     
	     // RELOAD WEAPON
	     if ( Input.GetKeyUp ( _reloadKey ) )
	     {
	         PlayWeaponsAnimationClip ("reload");
	         _gunBrains.ReloadWeaponStart();
	         StartCoroutine( WaitForAnimation ( _gunBrains._reloadingTime, "ReloadingDone") );
	         _reloading = true;
	         BroadcastMessage ("AnimationStarted");
	     }
	     
	     // MOUSE LOOK CONTROLS
	     MouseLookX (); // turn around
	     MouseLookY (); // look up and down
	     
	     // if player is looking down then offset the gun by lerping to offset position
	     if (_offsetHandsToLookDownPosition)
	         _weaponsInUseSkins[_currentWeapon].transform.localPosition = Vector3.Lerp ( _weaponsInUseSkins[_currentWeapon].transform.localPosition, _offsetVec, Time.deltaTime*_offsetGunHandsSmooth );
	     
	     // Check Interactions
	     if ( _allowInteracting )
	     {
	         Interact ();
	     }
	     
	     // User interaction stuff
	     if (Input.GetKeyUp ( _escapeKey )) Screen.lockCursor = false;
	     //$$if (_wasLocked && !Screen.lockCursor) Screen.lockCursor = true;
	     
	     // CHANGE WEAPON
	     if( !_reloading )
	     {
	         if( _availableWeapons > 1 )
	         {
	             _previousWeapon = _currentWeapon;
	             // Change weapon with Mouse Wheel
	             if (Input.GetAxis("Mouse ScrollWheel") != 0.0F )
	             {
	                 if( _scrollTimer <= Time.time ) 
	                 {
	                     _scrollTimer = Time.time + _scrollDelay;
	                     
	                     if (Input.GetAxis("Mouse ScrollWheel") > 0)
	                     {
	                         _currentWeapon++;
	                         //Debug.Log( "scroll weapon up to " + _currentWeapon );
	                     }
	                     else if (Input.GetAxis("Mouse ScrollWheel") < 0)
	                     {
	                         _currentWeapon--;
	                         //Debug.Log( "scroll weapon down to " + _currentWeapon );
	                     }
	                 }
	             }
	             
	             // Change weapon with set number keys
	             if ( Input.GetKeyDown(_weaponKey1) )
	             {
	                 _currentWeapon = 0;
	             }
	             else if ( Input.GetKeyDown(_weaponKey2) )
	             {
	                 _currentWeapon = 1;
	             }
	             else if ( Input.GetKeyDown(_weaponKey3) )
	             {
	                 _currentWeapon = 2;
	             }
	             else if ( Input.GetKeyDown(_weaponKey4) )
	             {
	                 _currentWeapon = 3;
	             }
	             
	             // tell weapon to change only if there is a change from current weapon in use
	             _currentWeapon = Mathf.Clamp (_currentWeapon, 0, (_weaponsInUseSkins.Length-1));
	             if (_currentWeapon != _previousWeapon)
	             {
	                 Debug.Log( "pressed weapon change key" );
	                 ChangeWeaponNow ();
	             }
	         }
	     }
	 }
	 
	 
	 public void SelectWeapon( int index )
	 {
	     for (int i=0;i<transform.childCount;i++)
	     {
	         // Activate the selected weapon
	         if (i == index)
	         {
	             transform.GetChild(i).gameObject.SetActiveRecursively(true);
	         }
	         // Deactivate all other weapons
	         else
	         {
	             Debug.Log( "deselecting " + i );
	             transform.GetChild(i).gameObject.SetActiveRecursively(false);
	         }
	     }
	 
	 }
	 
	 
	 public void FixedUpdate()
	 {
	     // MOVEMENT CONTROLS
	    if ( (_grounded || _ladderClimbing) && _play )
	     {
	         // Jump
	         if (_canJump)
	         {
	             if ( Time.time > _nextJump && Input.GetButton("Jump"))
	             {
	                 _grounded = false;
	                 state = MoveState.Jumped;
	                 _nextJump = Time.time + _jumpingRate;
	                 rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
	                 _ladderClimbing = false;
	             }
	         }
	     }
	 
	     //--- only check for Movement if grounded, alive, and not in mid-jump
	     if (_grounded && _play && ( Time.time > _nextJump ) )
	     {
	         // Forward. Back, Strafe
	         Movement ();
	     }
	     
	     if (_ladderClimbing)
	     {
	         ClimbingLadder ();
	     }
	     else
	     {
	         // We apply gravity manually for more tuning control
	         rigidbody.AddForce( new Vector3 (0, -_gravity * rigidbody.mass, 0));
	         _grounded = false;
	     }
	 }
	 
	 
	 /// <summary>
	 /// Lates the update.
	 /// </summary>
	 public void LateUpdate()
	 {
	     Quaternion lar;
	     float adjuster = 0F;
	     float effectiveTurnRate = Time.deltaTime * _turnRate;
	     float angleX = _mouseLookYTransform.localEulerAngles.x;
	     
	     if (!_play) return;
	     
	     // be the last thing to update
	     // do this just in case the player controller is jumping or something
	     // we want some minor delay for artistic reasons
	     //--- AngleAxis gives a horizontal delay between player look rotation and weaponHolder rotation
	///          Quaternion lar = _targetAt.rotation * Quaternion.AngleAxis( -_gunTurnVal*_gunTurnRate, Vector3.up );
	     
	     //---
	     // the idea here is to lock down the rotation when looking far up/down. otherwise the gun or arms
	     // get clipped againt the camera.
	     // this is still more ugly and abrupt than i'd like, but it might be a good start.
	     //---
	     if( (angleX > 60) && (angleX<90) )
	     {
	         float blah;
	         adjuster = (1F/(90F-angleX))*1F;
	         //Debug.Log( "1Xangle: " + angleX + ", blah: " + adjuster );
	         lar = _targetAt.rotation * Quaternion.AngleAxis( -_gunTurnVal*(_gunTurnRate*(adjuster/2)), Vector3.up );
	         _gunPivot.rotation = Quaternion.Lerp ( _gunPivot.rotation, lar, effectiveTurnRate*adjuster );
	     }
	     else if( (angleX > 270) && (angleX<330) )
	     {
	         float blah;
	         adjuster = (1F/(angleX-270))*1F;
	         //Debug.Log( "2Xangle: " + angleX + ", blah: " + blah );
	         lar = _targetAt.rotation * Quaternion.AngleAxis( -_gunTurnVal*(_gunTurnRate*(adjuster/2)), Vector3.up );
	         _gunPivot.rotation = Quaternion.Lerp ( _gunPivot.rotation, lar, effectiveTurnRate*adjuster );
	     }
	     else
	     {
	         //Debug.Log( "3Xangle: " + angleX + ", etr: " + effectiveTurnRate );
	         lar = _targetAt.rotation * Quaternion.AngleAxis( -_gunTurnVal*_gunTurnRate, Vector3.up );
	         _gunPivot.rotation = Quaternion.Lerp ( _gunPivot.rotation, lar, effectiveTurnRate );
	///              _gunPivot.rotation = Quaternion.Lerp ( _gunPivot.rotation, lar, Time.time * effectiveTurnRate );
	     }
	 }
	 
	 
	 public void OnTriggerEnter( Collider col )
	 {
	     if (!_offLadder && col.tag == "Ladder")
	     {
	         _ladderTop = col.gameObject;
	         Invoke ( "OffLadder", 1 );
	         Debug.Log("OFF LADDER!!");
	         _ladderClimbing = false;
	         // enable top of ladder collider
	         // this stops the player falling back down the ladder!
	         _ladderTop.SendMessage ( "ToggleTopStep" );
	     }
	     else if (col.tag == "ZoneInteract")
	     {
	         Debug.Log("ENTERED INTERACT ZONE!");
	         _inInteractZone = true;
	         _interactObj = col.gameObject;
	     }
	 }
	 
	 
	 public void OnTriggerExit( Collider col )
	 {
	     if (col.tag == "ZoneInteract")
	     {
	         Debug.Log("LEFT INTERACT ZONE!");
	         _inInteractZone = false;
	         _interactObj = null;
	         col.SendMessage ( "FinishedInteraction", SendMessageOptions.DontRequireReceiver );
	     }
	 }
	 
	 
	 public void OnCollisionEnter( Collision col )
	 {
	     if (col.transform.tag.Equals("Weapon") )
	     {
	         // grab the weapon!
	         //Debug.Log( "collision COLLECTED A WEAPON!");
	         StartCoroutine( CollectedNewWeapon ( col.gameObject, true ) );
	     }
	 }
	
	 
	 public void OnCollisionStay ( Collision col )
	 {
	     if ( col.transform.tag == "Wall" || col.transform.tag == "Ceiling" )
	     {
	         
	     }
	     else if ( col.transform.tag == "Ladder" )
	     {
	         if (_offLadder)
	         {
	             _ladderClimbing = true;
	             Debug.Log("LADDER!!");
	             _offLadder = false;
	         }
	         _grounded = false;
	     }
	     else
	     {
	         _grounded = true;
	     }
	 }
	 
	 
	 public void OnCollisionExit( Collision col )
	 {
	     //Debug.Log( "exit collision" );
	 }
	
	 
	 public void FirePrimary()
	 {
	     if (_gunBrains)
	     {
	         _gunBrains.FirePrimary ( _gunInHandsAnimationClips[3], _gunInHandsAnimationClips[4] );
	         // bullet spray
	         // @FIXME Not working like it use to
	         //var perlinVec:Vector3 = Random.insideUnitSphere * SmoothRandom.Get (_sprayAmount);
	         //_targetAt.localPosition = perlinVec;
	     }
	 }
	
	
	 public void FirePrimaryStop()
	 {
	     if( _gunBrains )
	     {
	         _gunBrains.FirePrimaryStop( _gunInHandsAnimationClips[3] );
	     }
	 }
	
	
	 public void OffLadder()
	 {
	     _offLadder = true;
	     _ladderTop.SendMessage ( "ToggleTopStep" );
	 }
	
	 
	 public float CalculateJumpVerticalSpeed()
	 {
	     // From the jump height and gravity we deduce the upwards speed 
	     // for the character to reach at the apex.
	     return Mathf.Sqrt(2 * _jumpHeight * _gravity);
	 }
	
	 
	 public void Movement()
	 {
	     //--- no user input if dead
	     if( !_alive )
	     {
	         state = MoveState.None;
	         
	         return;
	     }
	     
	     //--- don't allow movement if in mid jump
	     if( ( state == MoveState.Jumped ) && !_grounded )
	     {
	         return;
	     }
	         
	     // Step1: Get your input values 
	     float hi    = Input.GetAxis("Horizontal"); 
	     float vi    = Input.GetAxis("Vertical"); 
	     // Step 2: Limit the movement on angles and then multiply those results 
	     // by their appropriate speed variables 
	     float percentofpercent = Mathf.Abs(hi) + Mathf.Abs(vi) - 1.0F; 
	     if (percentofpercent > 0.1F)
	     {
	        // if we're here, then we're not moving in a straight line 
	        // my math here might be kinda confusing and sloppy...so don't look! 
	        percentofpercent = percentofpercent * 10000; 
	        percentofpercent = Mathf.Sqrt(percentofpercent); 
	        percentofpercent = percentofpercent / 100; 
	        float finalMultiplier = percentofpercent * 0.25F;
	        hi = hi - (hi * finalMultiplier); 
	        vi = vi - (vi * finalMultiplier); 
	     }
	 
	     // Walking
	     if (vi > 0) 
	     { 
	        vi = vi * _forwardSpeed;
	        if (!_reloading) PlayWeaponsAnimationClip ("forward");
	        
	        state = MoveState.Walking;
	     } 
	     else if (vi < 0) 
	     { 
	        vi = vi * _backwardSpeed;
	        if (!_reloading) PlayWeaponsAnimationClip ("backward");
	        
	        state = MoveState.Backwards;
	     } 
	     else 
	     {
	         if (!_reloading) PlayWeaponsAnimationClip ("idle");
	         
	         state = MoveState.None;
	     }
	     // Strafing
	     hi = hi * _strafeSpeed;
	     if (hi != 0) {
	         if (!_reloading) PlayWeaponsAnimationClip ("forward");
	         
	         state = MoveState.Walking;
	     }
	     // Running?
	     if ( Input.GetKey (_runKeyLeft) || Input.GetKey (_runKeyRight)  ) 
	     {       
	         vi *= _runSpeedModifier;
	         hi *= _runSpeedModifier;
	         if (vi != 0 || hi != 0) 
	         {
	             state = MoveState.Running;
	         
	             if (!_reloading) PlayWeaponsAnimationClip ("run");
	         }
	     }
	     
	     if( ( state & ( MoveState.Backwards | MoveState.Walking | MoveState.Running ) ) != MoveState.None )
	     {
	         CrosshairAnimator.ToggleSpread( true );
	     }
	     else
	     {
	         CrosshairAnimator.ToggleSpread( false );
	     }
	     
	     // Step 3: Derive a vector on which to travel, based on the combined 
	     // influence of BOTH axes 
	     Vector3 tubeFinalVector = transform.TransformDirection ( new Vector3(hi,0,vi) );
	     // Step 4: Apply the final movement in world space 
	     Vector3 vel = rigidbody.velocity;
	     vel.z = tubeFinalVector.z; 
	     vel.x = tubeFinalVector.x;
	     rigidbody.velocity = vel;
	 }
	 
	 
	 public void PlayWeaponsAnimationClip ( String which )
	 {
	     string clip = _gunInHandsAnimationClips[2]; // set to idle
	     
	     switch (which)
	     {
	         case "forward":
	             clip = _gunInHandsAnimationClips[6];
	         break;
	         case "backward":
	             clip = _gunInHandsAnimationClips[6];
	         break;
	         case "run":
	             clip = _gunInHandsAnimationClips[7];
	         break;
	         case "sprint":
	             clip = _gunInHandsAnimationClips[8];
	         break;
	         case "reload":
	             clip = _gunInHandsAnimationClips[5];
	         break;
	         case "idle":
	             clip = _gunInHandsAnimationClips[2];
	         break;
	     }
	     
	     if (clip != _currentClipPlaying)
	     {
	         _currentClipPlaying = clip;
	         if (_gunBrains) {
	             clip += _gunBrains._weaponsName;
	             if( _shoulders.animation[clip] != null )
	             {
	                 _shoulders.animation.CrossFade(clip);
	             }
	         }
	     }
	 }
	 
	 
	 public IEnumerator WaitForAnimation ( float waitTime, String callback )
	 {
	     yield return new WaitForSeconds (waitTime);
	     BroadcastMessage (callback);
	     BroadcastMessage ("AnimationEnded");
	 }
	
	 
	 public void ReloadingDone ()
	 {
	     _reloading = false;
	     // tell gun to reload
	     _gunBrains.ReloadWeapon ();
	 }
	
	 
	 public void ClimbingLadder ()
	 {
	     // Step1: Get your input values 
	     float hi = 0;
	     float vi = Input.GetAxis("Vertical"); 
	     // Step 2: Limit the movement on angles and then multiply those results 
	     // by their appropriate speed variables 
	     float percentofpercent = Mathf.Abs(hi) + Mathf.Abs(vi) - 1.0F; 
	     if (percentofpercent > 0.1F)
	     {
	        // if we're here, then we're not moving in a straight line 
	        // my math here might be kinda confusing and sloppy...so don't look! 
	        percentofpercent = percentofpercent * 10000; 
	        percentofpercent = Mathf.Sqrt(percentofpercent); 
	        percentofpercent = percentofpercent / 100; 
	        float finalMultiplier = percentofpercent * 0.25F; 
	        hi = hi - (hi * finalMultiplier); 
	        vi = vi - (vi * finalMultiplier); 
	     }
	     
	     if (vi > 0)
	     {
	        vi = vi * _forwardSpeed; 
	     }
	     
	     if (vi < 0)
	     {
	        vi = vi * _backwardSpeed; 
	     }
	     
	     vi *= _climbSpeedModifier;
	     //hi *= _climbSpeedModifier;
	     // Step 3: Derive a vector on which to travel, based on the combined 
	     // influence of BOTH axes 
	     Vector3 tubeFinalVector = transform.TransformDirection ( new Vector3(0,vi,0) ); 
	     // Step 4: Apply the final movement in world space 
	     Vector3 vel = rigidbody.velocity;
	     vel.y = tubeFinalVector.y; 
	     vel.x = 0;
	     rigidbody.velocity = vel;
	 }
	 
	 
	 /// FUNCTIONS
	 //
	 // INTERACTION
	 public void Interact ()
	 {
	     bool seen = false;
	     Vector3 dir = _mainCam.transform.TransformDirection ( Vector3.forward );
	     bool interactableInSight = Physics.Raycast (_mainCam.transform.position, dir, out _hit, _interactionMaxRange);
	     
	     if( interactableInSight ) 
	     {
	         if( _hit.transform.CompareTag( "Interact" ) || _hit.transform.CompareTag( "Weapon" ) )
	         {
	             seen = true;
	             _interactObj = _hit.transform.gameObject;
	             //_hit.transform.SendMessage( "Highlighted" );
	             //_hit.transform.renderer.material.color = Color.cyan;
	         }
	     }
	     
	     if( _interactObj != null )
	     {
	         if( seen )
	         {
	             _interactObj.transform.SendMessage( "Highlighted", SendMessageOptions.DontRequireReceiver );
	         }
	         else
	         {
	             if( _interactObj != null )
	             {
	                 _interactObj.transform.SendMessage( "NonHighlighted", SendMessageOptions.DontRequireReceiver );
	             }
	             _interactObj = null;
	         }
	     }
	         
	     if ( Input.GetKey ( _interactionKey ) )
	     {
	         if (_inInteractZone)
	         {
	             _interactObj.SendMessage ( "ReceivedInteraction", true, SendMessageOptions.DontRequireReceiver );
	         }
	         else
	         {
	             if ( _interactObj != null )
	             {
	                 if (_interactObj.tag.Equals("Weapon") )
	                 {
	                     // grab the weapon!
	                     if( ! _weaponIsPickedUp[ WeaponSlotByName( _interactObj.name ) ] )
	                     {
	                         //Debug.Log( "interact COLLECTED A WEAPON!");
	                         StartCoroutine( CollectedNewWeapon ( _interactObj, true ) );
	                     }
	                 }
	                 else
	                 {
	                     _interactObj.transform.SendMessage ( "ReceivedInteraction", false, SendMessageOptions.DontRequireReceiver );
	                 }
	             }
	         }
	     }
	 }
	 
	 
	 // HEAD BOB
	 public void BobDaHead ()
	 { // actually the effect is more like the gun bobbing up and down
	     float waveslice = 0.0F; 
	     float ih = Input.GetAxis("Horizontal"); 
	     float iv = Input.GetAxis("Vertical"); 
	     
	     Debug.Log( "BobDaHead" );
	     if (Mathf.Abs(ih) == 0 && Mathf.Abs(iv) == 0)
	     {
	        _timer = 0.0F; 
	     }
	     else
	     { 
	        waveslice = Mathf.Sin(_timer); 
	        _timer = _timer + _bobbingSpeed; 
	        if (_timer > Mathf.PI * 2)
	         {
	           _timer = _timer - (Mathf.PI * 2); 
	        } 
	     }
	     
	     if (waveslice != 0)
	     {
	         float translateChange = waveslice * _bobbingAmount; 
	         float totalAxes = Mathf.Abs(ih) + Mathf.Abs(iv); 
	         totalAxes = Mathf.Clamp (totalAxes, 0.0F, 1.0F); 
	         translateChange = totalAxes * translateChange; 
	         Vector3 lPos = _mainCam.transform.localPosition;
	         lPos.y = _camRestY + translateChange;
	         lPos.x = (_camRestX + translateChange) * _xBobDamper; 
	         _mainCam.transform.localPosition = lPos;
	     }
	     else
	     { 
	         Vector3 lPos = _mainCam.transform.localPosition;
	         lPos.y = _camRestY;
	         lPos.x = _camRestX; 
	         _mainCam.transform.localPosition = lPos;
	     }
	     
	     Vector3 glPos = _gunRenderCam.localPosition;
	     glPos.y = _mainCam.transform.localPosition.y;
	     glPos.x = _mainCam.transform.localPosition.x;
	     _gunRenderCam.localPosition = glPos;
	 }
	 
	 
	 //---
	 //
	 //---
	 public void PlayerDied()
	 {
	     _alive = false;
	     _currentWeapon = 0;
	//      _decoration.aspect.aspectName = "dead";
	     ChangeWeaponNow();
	     transform.localScale = new Vector3( 0.2F, 0.2F, 0.2F );
	     transform.eulerAngles = new Vector3( 320F + UnityEngine.Random.Range( 0F, 50F), 0F, 0F );
	 }
	
	 
	 // REGISTER WEAPON CHANGE
	 public IEnumerator CollectedNewWeapon ( GameObject gun, bool pickup )
	 {       
	     _weaponIsPickedUp[ WeaponSlotByName( gun.name ) ] = true;
	     ChangeWeaponNow ();
	     yield return new WaitForSeconds (0.5F);
	     UseWeaponNow ( gun, pickup );
	 }
	
	 
	 public void ChangeWeaponNow ()
	 {
	     // turn off previous
	     //Debug.Log( "ChangeWeaponNow: curr(" + _currentWeapon + "), prev(" + _previousWeapon + ")" );
	     if( _weaponIsPickedUp[_currentWeapon] && (_previousWeapon >= 0 ) )
	     {
	         //Debug.Log( "CWN: 1, " + _weaponsInUseSkins[_previousWeapon].name );
	         _weaponsInUseSkins[_previousWeapon].SendMessage ( "SwitchWeapon", _gunInHandsAnimationClips[9], SendMessageOptions.DontRequireReceiver );
	     }
	     else
	     {
	         //Debug.Log( "CWN: 2" );
	         if( _previousWeapon >= 0 )_currentWeapon = _previousWeapon;
	         if( _weaponNotPickedUpAudio ) _audioSrcMomentary.PlayOneShot( _weaponNotPickedUpAudio );
	     }
	 }
	
	 
	 public void ActivateWeapon ()
	 {
	     // enable selected weapon
	     //Debug.Log( "ActivateWeapon" );
	     UseWeaponNow (_weaponsInUseSkins[_currentWeapon], false);
	 }
	 
	 
	 public void UseWeaponNow ( GameObject gun, bool pickup )
	 {
	     String weaponsName = "";
	
	     if (pickup)
	     {
	         // turn off current weapon
	         // actually want to play deactivate animation here
	         //Debug.Log( "turning off current ( " + _currentWeapon + ")" );
	         _weaponsInUseSkins[_currentWeapon].SetActiveRecursively (false);
	         // get name of in-use-weapon to activate
	         // this should be hanlded better by reusing skinned animation meshes and just applying a different gun to the hand....
	         if( gun )
	         {
	             InteractLogic interactLogic = gun.GetComponent<InteractLogic>();
	             if( interactLogic != null ) interactLogic.NonHighlighted();
	             
	             PickUpLogic pul = gun.GetComponent<PickUpLogic>();
	             weaponsName = pul.weaponInUseName;
	             gun.SendMessage ( "Collected", SendMessageOptions.DontRequireReceiver );
	         }
	         else
	         {
	             Debug.Log( "*** gun disappeared" );
	         }
	         
	         // have new weapon that can be used!
	         _availableWeapons ++;
	     }
	     else
	     {
	            // enable default weapon (hand gun in this case)
	         weaponsName = gun.name;
	         //Debug.Log( "activating held weapon: " + weaponsName );
	     }
	     
	     if (weaponsName != "")
	     {
	         // find weapon's skin
	         for ( int i=0; i<_weaponsInUseSkins.Length; i++)
	         {
	             if (_weaponsInUseSkins[i].name == weaponsName)
	             {
	                 // set current weapon
	                 _currentWeapon = i;
	                 break;
	             }
	         }
	         
	         //Debug.Log( "are they diff? (" + _currentWeapon + ")(" + _previousWeapon + ")" );
	         //if( _currentWeapon != _previousWeapon )
	         //$$if( true )
	         //$${
	             //Debug.Log( "switching from " + _previousWeapon + " to " + _currentWeapon + "" );
	             if (!_weaponsInUseSkins[_currentWeapon].active)
	             {
	                 //Debug.Log( "activating current ( " + _weaponsInUseSkins[_currentWeapon].name + ")" );
	                 _weaponsInUseSkins[_currentWeapon].SetActiveRecursively (true);
	                 if( ! _weaponsInUseSkins[_currentWeapon].active )
	                 {
	                     //Debug.Log( "failed to activate" );
	                 }
	             }
	             else
	             {
	                 //Debug.Log( "NOT activating current" );
	             }
	
	             GameObject go = _weaponsInUseSkins[_currentWeapon];
	             _gunBrains = go.GetComponent<GunLogic>();
	             _gunBrains.SetBulletShootFrom ( _targetAt );
	             _gunBrains.UseWeapon ();
	             _shoulders = _gunBrains._animator;
	             // tell game manager that there is a new righthand data bar handler
	             Game.Manager.SetRighthandDataController (_gunBrains._dataController);
	         //$$}
	         //$$else
	         //$${
	         //$$    Debug.Log( "tried to activate same weapon" );
	         //$$}
	     }
	     else
	     {
	         Debug.Log ("In Use Weapon wasn't activated!");
	     }
	 }
	
	 // MOUSE LOOKS - in FPS shooters we only ever care about mouselook around and up and down
	 // MOUSELOOK X
	 public void MouseLookX ()
	 {
	     // Shift and Push the buffer
	     _xInputBuffer.RemoveAt (0);
	     _xInputBuffer.Add ( Input.GetAxis("Mouse X") * _sensitivityX );
	     float buffers = 0.0F;
	     for ( int i=0; i<_xInputBuffer.Count; i++ )
	     {
	         buffers += _xInputBuffer[i]*_lutWeightMod[i];
	     }
	     
	     //_rotationX += buffers / _mouseFilterBufferSize;
	     _rotationX += buffers;
	     _rotationX = ClampAngle (_rotationX, _minimumX, _maximumX);
	 
	     //--- offset that weaponHolder is from player look rotation 
	     _gunTurnVal = Mathf.Clamp( buffers, -_gunTurnDistMax, _gunTurnDistMax );
	 
	     Quaternion xQuaternion = Quaternion.AngleAxis (_rotationX, Vector3.up);
	     _mouseLookXTransform.localRotation = _originalRotationX * xQuaternion;
	 }
	 
	 
	 // MOUSELOOK Y
	 public void MouseLookY ()
	 {
	     // Shift and Push the buffer
	     _yInputBuffer.RemoveAt (0);
	     float movY = Input.GetAxis("Mouse Y") * _sensitivityY;
	     _yInputBuffer.Add ( movY );
	     float buffers = 0.0F;
	     for ( int i=0; i<_yInputBuffer.Count; i++ )
	     {
	         buffers += _yInputBuffer[i]*_lutWeightMod[i];
	     }
	     //_rotationY += buffers / _mouseFilterBufferSize;
	     
	     _rotationY += buffers;
	     
	     _rotationY = ClampAngle (_rotationY, _minimumY, _maximumY);
	 
	     Quaternion yQuaternion = Quaternion.AngleAxis (_rotationY, Vector3.left);
	     _mouseLookYTransform.localRotation = _originalRotationY * yQuaternion;
	     if (_gunRenderCam) _gunRenderCam.localRotation = _mouseLookYTransform.localRotation;
	     
	     // gun offset when looking down
	     //_maximumY
	     //$$if (_mouseLookYTransform.localEulerAngles.x > 17 && _mouseLookYTransform.localEulerAngles.x < 41)
	     //--- because of floating point inprecision, X angle can be slightly more than _maximumY. so adding
	     //--- a buffer of 1
	     if (_mouseLookYTransform.localEulerAngles.x > 17 && _mouseLookYTransform.localEulerAngles.x <= (_maximumY+1F) )
	     {
	         _offsetVec = _gunBrains._lookDownOffset;
	     }
	     else
	     {
	         _offsetVec = _gunBrains._handsNormalPos;
	     }
	 }
	 
	 
	 // UITILITY FUNCTIONS
	 // CLAMP
	 public float ClampAngle ( float ang, float min, float max )
	 {
	     if (ang < -360.0) ang += 360.0F;
	     
	     if (ang > 360.0) ang -= 360.0F;
	     
	     return Mathf.Clamp ( ang, min, max );
	 }
	 
	 
	 // create mouse filtering buffers
	 // as we going to implement this: http://www.flipcode.com/archives/Smooth_Mouse_Filtering.shtml
	 public void SetupMouseFiltering ()
	 {
	     _xInputBuffer = new List<float> (); 
	     _yInputBuffer = new List<float> ();
	     _lutWeightMod = new List<float> ();
	     for ( int i=0; i<_mouseFilterBufferSize; i++ )
	     {
	         _xInputBuffer.Add (0.0F);
	         _yInputBuffer.Add (0.0F);
	         if (i>0) _dampeningAmt = _dampeningAmt / 2;
	         _lutWeightMod.Add (_dampeningAmt);
	     }
	     
	     _lutWeightMod.Reverse ();
	     //print (_lutWeightMod);
	 }
	         
	         
	 //---
	 // find a weapons slot based on its name
	 //---
	 public int WeaponSlotByName( String weaponName )
	 {
	     int slot = 0;
	     String searchName = weaponName.Replace( "-PickUp", "-In-Use" );
	         
	     for ( int i=0; i<_weaponsInUseSkins.Length; i++)
	     {
	         if (_weaponsInUseSkins[i].name == searchName)
	         {
	             // set current weapon
	             slot = i;
	             break;
	         }
	     }
	     
	     return( slot );
	 }
	         
	         
	 public void OnApplicationQuit ()
	 {
	     // unlock the cursor
	     Screen.lockCursor = false;
	 }
	 
	 
	 public void OnApplicationPause ()
	 {
	     // unlock the cursor
	     Screen.lockCursor = false;
	 }
	}
}