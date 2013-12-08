using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using FPSControl.States;
using FPSControl.States.Player;

namespace FPSControl
{
    public class FPSControlPlayerData : object
    {        
        internal static FPSControlPlayer player;

        #region camera

        // get/set max view angles
        public static Vector2 cameraPitchConstraints
        {
            get { return player.playerCamera.pitchConstraints; }
            set { player.playerCamera.pitchConstraints = value; }
        }

        #endregion // camera

        #region movement

        public static bool canRun = true;

        public static float runForwardSpeed { get { return player.runState.forwardSpeed; } set { player.runState.forwardSpeed = value; } }
        public static float runStrafeSpeed { get { return player.runState.strafeSpeed; } set { player.runState.strafeSpeed = value; } }
        public static float runReverseSpeed { get { return player.runState.reverseSpeed; } set { player.runState.reverseSpeed = value; } }

        public static float jumpHeight { get { return player.movementController.jumpForce; } set { player.movementController.jumpForce = value; } }
        public static bool forceCrouching = false; //{ get; set; }
        public static Vector3 gravity { get { return Physics.gravity; } set { Physics.gravity = value; } }

        static bool _frozen = false;
        public static bool frozen { get { return _frozen; } set { _frozen = value; FPSControlPlayerEvents.Freeze(value); } }

        static Dictionary<Renderer, bool> _rendererStates = new Dictionary<Renderer, bool>();
        static KeyValuePair<IntelliCrosshair, bool> _wasCrosshairEnabled = new KeyValuePair<IntelliCrosshair, bool>(null, false);

        static bool _visible = true;
        public static bool visible
        {
            get { return _visible; }
            set
            {
                _visible = value;

                if (_visible) 
                {
                    foreach (KeyValuePair<Renderer, bool> kvp in _rendererStates)
                    {
                        kvp.Key.enabled = kvp.Value;
                    }

                    _rendererStates.Clear();

                    if (player.weaponManager.crosshairAnimator.crosshair && _wasCrosshairEnabled.Key != null)
                    {
                        _wasCrosshairEnabled.Key.enabled = _wasCrosshairEnabled.Value;
                    }
                }
                else
                {
                    _rendererStates = new Dictionary<Renderer, bool>();
                    foreach(Renderer r in player.transform.GetComponentsInChildren<Renderer>(true))
                    {
                        _rendererStates.Add(r, r.enabled);
                        r.enabled = false;
                    }

                    IntelliCrosshair c = player.weaponManager.crosshairAnimator.crosshair;

                    if (c)
                    {
                        
                        _wasCrosshairEnabled = new KeyValuePair<IntelliCrosshair, bool>(c, c.enabled);
                        c.enabled = false;
                    }
                }
            }
        }


        #endregion // movement

        #region health

        public static DataController healthData { get { return player.GetComponent<FPSControlPlayerStats>().healthData; } }

        #endregion // health

        #region weapons

        public static FPSControlWeapon[] availableWeapons { get { return player.weaponManager.availableWeapons; } }

        public static void AddWeapon(string name, bool equip) { player.weaponManager.Add(name, equip); }
        public static void RemoveWeapon(FPSControlWeapon weapon) { player.weaponManager.Remove(weapon); }

        public static FPSControlWeapon GetWeapon(string weaponName) { return player.weaponManager[weaponName]; }
        public static FPSControlWeapon currentWeapon { get { return player.weaponManager.currentWeapon; } }
        public static void Add(string weaponName, bool makeCurrent) 
        { 
            if (player.weaponManager.CanAddWeapon(weaponName)) player.weaponManager.Add(weaponName, makeCurrent); 
            else Debug.LogWarning("Couldn't add weapon to inventory. It may already be there or the maximum has been reached."); 
        }

        public static void EquipWeaponAt(int index) { player.weaponManager.ActivateWeaponAt(index); }
        public static void EquipWeapon(string weaponName) { player.weaponManager.ActivateWeapon(weaponName); }

        public static void SetAmmo(FPSControlRangedWeapon weapon, int ammo, int clips) { weapon.SetAmmo(ammo, clips); }
        public static void SetAmmo(FPSControlRangedWeapon weapon, float energy) { weapon.SetAmmo(energy); }

        public static int[] GetAmmo(FPSControlRangedWeapon weapon) { return weapon.GetAmmo();  }

        public static void DeactivateCurrentWeapon() 
        { 
            FPSControlWeapon w = currentWeapon; 
            player.weaponManager.DeactivateCurrentWeapon();
        }

        #endregion // weapons
    }
    
    [RequireComponent(typeof(FPSControlPlayerStats))]
    public class FPSControlPlayer : StateMachine
    {
        //Components
        public FPSControlPlayerCamera playerCamera;
        public FPSControlPlayerInteractionManager interactionManager;
        public FPSControlPlayerMovement movementController;
        public FPSControlPlayerWeaponManager weaponManager; 

        //Player States
        public PlayerState idleState;
        public PlayerState walkState;
        public PlayerState runState;
        public PlayerState jumpState;

        //player info
        public bool running { get { return currentState == runState; } }
        public bool moving { get { return currentState != idleState; } } 
        public bool jumping { get { return movementController.isJumping; } }
        public bool crouching { get { return movementController.isCrouching; } }
        public bool strafing { get { return movementController.isStrafing; } }
        public bool reversing { get { return movementController.isMovingBackwards; } }
        public Vector3 velocity { get { return movementController.Controller.velocity; } }
        public Vector3 forward { get { return transform.forward; } }
        public Vector3 aim { get { return playerCamera.transform.forward; } } 

        //Weapon States - for a later date

        new public PlayerState currentState
        { 
            get 
            { 
                return (PlayerState)base.currentState; 
            } 
            protected set
            {
                PlayerState newState = value;
                if (newState.fallThroughState) newState.SetFallthroughData(this.currentState);
                base.currentState = (PlayerState)value;
 
            } 
        } //i need to rethink the polymorphism here i believe, but this works for now.

        List<RendererState> _rendererStates;

        public void SetVisibility(bool visibility)
        {
            playerCamera.SetVisibility(visibility);
            
            if (visibility)
            {
                foreach (RendererState s in _rendererStates)
                {
                    s.renderer.enabled = s.wasEnabled;
                }
            }
            else
            {
                _rendererStates = new List<RendererState>();
                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                {
                    _rendererStates.Add(new RendererState(r.enabled, r));
                    r.enabled = false;
                }
            }
        }

        public bool Respawn()
        {
            return FPSControlPlayerSpawn.ReSpawn(this);
        }

        public bool RespawnAt(int ID)
        {
            return FPSControlPlayerSpawn.ReSpawnAt(this, ID);
        }

        void Awake()
        {
            FPSControlPlayerData.player = this;
            FPSControlInput.LoadControlMapping();
            
            //short cutting for demo purposes.
            Add(idleState);
            Add(walkState);
            Add(runState);
            Add(jumpState);
            
            //initialize our states here, and default our first
            Initialize(idleState);
        }

        void Start()
        {
            //Initialize our sub-components, we do this on Start instead of Awake to make sure that they have ample time to self-initialize if need be.
            playerCamera.Initialize(movementController,this);
            interactionManager.Initialize(this);
            movementController.Initialize(playerCamera, this);
            weaponManager.Initialize(playerCamera, movementController, this);
            FPSControlPlayerEvents.Spawn();
        }

        protected override void OnInitialize()
        {
            
        }

        protected override void OnStateChange()
        {
            playerCamera.SetState(currentState);
            interactionManager.SetState(currentState);
            movementController.SetState(currentState);
            weaponManager.SetState(currentState);
        }

        #region Loops

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            playerCamera.DoUpdate();
            movementController.DoUpdate();
            interactionManager.allowInteracting = currentState.canInteract;
            interactionManager.DoUpdate();
            weaponManager.DoUpdate();
        }

        void LateUpdate()
        {
            playerCamera.DoLateUpdate();
            weaponManager.DoLateUpdate();
        }

        #endregion // Loops

        class RendererState
        {
            public bool wasEnabled;
            public Renderer renderer;

            public RendererState(bool enabled, Renderer r)
            {
                wasEnabled = enabled;
                renderer = r;
            }
        }
    
    }
}
