using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using FPSControl.States;
using FPSControl.States.Player;
using FPSControl.Data;
using FPSControl.Data.Config;

namespace FPSControl
{
    public class FPSControlPlayerSaveDataComparer : IComparer<FPSControlPlayerSaveData>
    {
        public int Compare(FPSControlPlayerSaveData x, FPSControlPlayerSaveData y)
        {
            return System.DateTime.Compare(x.timestamp, y.timestamp);
        }
    }
    
    public class FPSControlPlayerSaveData : System.IComparable<FPSControlPlayerSaveData>
    {
        internal const string IDENTIFIER = "PlayerSave";

        public float currentHealth;
        public string currentLevelName;
        public int spawnPoint;
        public System.DateTime timestamp;
        public string screenshotID;
        public bool HasScreenShot { get { return !string.IsNullOrEmpty(screenshotID); } }
        public string guid;

        public FPSControlPlayerSaveData() {}
        public FPSControlPlayerSaveData(string levelName, int spawnPoint, float currentHealth)
        {
            this.currentHealth = currentHealth;
            this.spawnPoint = spawnPoint;
            this.currentLevelName = levelName;
            guid = System.Guid.NewGuid().ToString("N");

            timestamp = System.DateTime.UtcNow;
        }

        public int CompareTo(FPSControlPlayerSaveData other)
        {
            if (this.timestamp == null || other.timestamp == null) 
                throw new System.NullReferenceException("Time Stamp of save data is null. If you are saving it manually make sure you are not using the parameterless constructor!");

            return timestamp.CompareTo(other.timestamp);
        }
    }
    
    public class FPSControlPlayerData : object
    {
        public static string currentWeaponSaveSlot { get; private set; }
        public static string currentPlayerSaveSlot { get; private set; }

        internal static FPSControlPlayer player;

        // Static constructor for setup on initial invokation
        static FPSControlPlayerData()
        {
        }

        #region Persistent Data

        /// <summary>
        /// Saves a temp state of the weapon manager. Useful for moving across scenes.
        /// </summary>
        public static void SaveTempWeaponData()
        {
            if (string.IsNullOrEmpty(currentWeaponSaveSlot)) currentWeaponSaveSlot = FPSControlPlayerWeaponManagerSaveData.IDENTIFIER + "_TEMP";
            _SaveWeaponData(currentWeaponSaveSlot,false);
        }

        /// <summary>
        /// Manually saves the save data to a temp cache. Useful for moving across scenes and wanting to specify changes when you get there.
        /// </summary>
        public static void SaveTempWeaponData(FPSControlPlayerWeaponManagerSaveData saveData)
        {
            PersistentData.Write<FPSControlPlayerWeaponManagerSaveData>(
                PersistentData.NS_WEAPONS,
                FPSControlPlayerWeaponManagerSaveData.IDENTIFIER + "_TEMP",
                saveData,
                true);
        }

        /// <summary>
        /// Loads the temporary weapon data and immediately deletes the persistent data for it
        /// </summary>
        public static FPSControlPlayerWeaponManagerSaveData LoadTempWeaponData()
        {
            PersistentDataNameSpace<FPSControlPlayerWeaponManagerSaveData> namespaceData = 
                PersistentData._ReadAll<FPSControlPlayerWeaponManagerSaveData>(PersistentData.NS_WEAPONS);

            PersistentDataContainer<FPSControlPlayerWeaponManagerSaveData> dataContainer = namespaceData.GetData(FPSControlPlayerWeaponManagerSaveData.IDENTIFIER + "_TEMP");
            if (dataContainer)
            {
                FPSControlPlayerWeaponManagerSaveData data = dataContainer.data;

                // Delete temp cache and write it
                namespaceData.Remove(dataContainer);
                PersistentData._WriteNameSpace<FPSControlPlayerWeaponManagerSaveData>(PersistentData._BuildPath(PersistentData.NS_WEAPONS), namespaceData);

                // return our data
                return data;
            }

            return null;
        }

        /// <summary>
        /// Saves the current weapon data to slot 0.
        /// NOTE: This is most useful if you are only allowing for 1 save slot.
        /// </summary>
        public static void SaveWeaponData()
        {
            if (string.IsNullOrEmpty(currentWeaponSaveSlot)) currentWeaponSaveSlot = FPSControlPlayerWeaponManagerSaveData.IDENTIFIER + "0";
            _SaveWeaponData(currentWeaponSaveSlot);
        }

        /// <summary>
        /// Saves the current weapon data to the specified slot.
        /// NOTE: This is only useful if you are allowing for multiple save slots.
        /// </summary>
        public static void SaveWeaponData(uint slot)
        {
            _SaveWeaponData(FPSControlPlayerWeaponManagerSaveData.IDENTIFIER + slot.ToString());
        }

        static void _SaveWeaponData(string identifier, bool allowEmpty = false)
        {
            FPSControlPlayerWeaponManagerSaveData saveData = null;

            if (allowEmpty)
            {
                if (player == null || player.weaponManager == null)
                    saveData = new FPSControlPlayerWeaponManagerSaveData(); // empty data
                    saveData.activeWeaponName = FPSControlPlayerWeaponManagerSaveData.NO_ACTIVE_WEAPON;
            }
            else
            {
                if (player == null)
                {
                    Debug.LogWarning("Can't save. Player object is null.");
                    return;
                }
                else if (player.weaponManager == null)
                {
                    Debug.LogWarning("Can't save. Weapon Manager object is null.");
                    return;
                }

                saveData = new FPSControlPlayerWeaponManagerSaveData(player.weaponManager);
            }

            PersistentData.Write<FPSControlPlayerWeaponManagerSaveData>(
                PersistentData.NS_WEAPONS,
                identifier,
                saveData,
                true);
        }

        /// <summary>
        /// Loads weapon data at the most recently loaded slot. If no weapon data has been loaded this session it will load at 0.
        /// </summary>
        public static FPSControlPlayerWeaponManagerSaveData LoadWeaponData()
        {
            if (string.IsNullOrEmpty(currentWeaponSaveSlot)) currentWeaponSaveSlot = FPSControlPlayerWeaponManagerSaveData.IDENTIFIER + "0";
            return _LoadWeaponData(currentWeaponSaveSlot);
        }

        /// <summary>
        /// Loads weapon data at the specified slot.
        /// </summary>
        public static FPSControlPlayerWeaponManagerSaveData LoadWeaponData(uint slot)
        {
            return _LoadWeaponData(FPSControlPlayerWeaponManagerSaveData.IDENTIFIER + slot.ToString());
        }

        internal static FPSControlPlayerWeaponManagerSaveData _LoadWeaponData(string identifier)
        {
            if (PersistentData.Exists<FPSControlPlayerWeaponManagerSaveData>(
                PersistentData.NS_WEAPONS, 
                identifier))
            {
                currentWeaponSaveSlot = identifier;
                return PersistentData.Read<FPSControlPlayerWeaponManagerSaveData>(
                    PersistentData.NS_WEAPONS, 
                    identifier);
            }

            return null;
        }

        /// <summary>
        /// Saves the data to slot 0.
        /// NOTE: This is most useful if you are only allowing one save
        /// </summary>
        public static void SavePlayerData(FPSControlPlayerSaveData data)
        {
            SavePlayerData(data, 0);
        }

        /// <summary>
        /// Saves the data to the specified slot number.
        /// </summary>
        public static void SavePlayerData(FPSControlPlayerSaveData data, uint slot, string screenshotID = null)
        {
            data.timestamp = System.DateTime.UtcNow; // Make sure the timestamp is up-to-date
            data.screenshotID = screenshotID;
            PersistentData.Write<FPSControlPlayerSaveData>(
                PersistentData.NS_PLAYER,
                FPSControlPlayerSaveData.IDENTIFIER+slot.ToString(),
                data,
                true);
        }

        /// <summary>
        /// Loads the save data with the most recent UTC timestamp
        /// </summary>
        public static FPSControlPlayerSaveData LoadMostRecentPlayerSaveData()
        {
            // Load everything, do a sort and return the first index.
            
            FPSControlPlayerSaveData[] array = PersistentData.ReadAll<FPSControlPlayerSaveData>(PersistentData.NS_PLAYER);

            if (array.Length == 0) return null;
            else if (array.Length == 1) return array[0];
            
            List<FPSControlPlayerSaveData> allData = new List<FPSControlPlayerSaveData>(array);
            allData.Sort((p1, p2) => new FPSControlPlayerSaveDataComparer().Compare(p1, p2));

            return allData[0];
        }

        /// <summary>
        /// Loads the save data at the specified slot
        /// </summary>
        public static FPSControlPlayerSaveData LoadPlayerSaveData(uint slot)
        {
            return _LoadPlayerSaveData(FPSControlPlayerSaveData.IDENTIFIER + slot.ToString());
        }

        /// <summary>
        /// Loads the save data at the most recently loaded slot this session.
        /// NOTE: if most current is empty it will save to slot 0, only useful if you aren't allowing multiple save states
        /// </summary>
        public static FPSControlPlayerSaveData LoadPlayerSaveData()
        {
            if (string.IsNullOrEmpty(currentPlayerSaveSlot)) currentPlayerSaveSlot = FPSControlPlayerSaveData.IDENTIFIER + "0";
            return _LoadPlayerSaveData(currentPlayerSaveSlot);
        }

        internal static FPSControlPlayerSaveData _LoadPlayerSaveData(string identifier)
        {
            FPSControlPlayerSaveData data = PersistentData.Read<FPSControlPlayerSaveData>(
                PersistentData.NS_PLAYER,
                identifier);

            currentPlayerSaveSlot = identifier;

            return data;
        }

        public static FPSControlPlayerSaveData[] GetAllSaves()
        {
            return PersistentData.ReadAll<FPSControlPlayerSaveData>(PersistentData.NS_PLAYER);
        }

        #endregion // Persistent Data

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

        public static void AddWeapon(string name, bool equip) { player.weaponManager.AddToInventory(name, equip); }
        public static void RemoveWeapon(FPSControlWeapon weapon) { player.weaponManager.Remove(weapon); }

        public static FPSControlWeapon GetWeapon(string weaponName) { return player.weaponManager[weaponName]; }
        public static FPSControlWeapon currentWeapon { get { return player.weaponManager.currentWeapon; } }
        public static void Add(string weaponName, bool makeCurrent) 
        { 
            if (player.weaponManager.CanAddWeapon(weaponName)) player.weaponManager.AddToInventory(weaponName, makeCurrent); 
            else Debug.LogWarning("Couldn't add weapon to inventory. It may already be there or the maximum has been reached."); 
        }

        public static void EquipWeaponAt(int index) { player.weaponManager.ActivateWeaponAt(index); }
        public static void EquipWeapon(string weaponName) { player.weaponManager.ActivateWeapon(weaponName); }

        public static void SetAmmo(FPSControlRangedWeapon weapon, int ammo, int clips) { weapon.SetAmmo(ammo, clips); }
        public static void SetAmmo(FPSControlRangedWeapon weapon, float energy) { weapon.SetAmmo(energy); }

        public static int[] GetAmmo(FPSControlRangedWeapon weapon) { return weapon ? weapon.GetAmmo() : new int[3] { 0, 0, 0 }; }

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

        public bool autoSpawnOnStart = true;
        public int defaultSpawnPointID = 0;
        

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
            playerCamera.Initialize(movementController, this);
            interactionManager.Initialize(this);
            movementController.Initialize(playerCamera, this);
            weaponManager.Initialize(playerCamera, movementController, this);

            if (autoSpawnOnStart)
            {
                if (!RespawnAt(defaultSpawnPointID))
                    throw new System.Exception(string.Format("Could not spawn at SpawnPoint[{0}]. Make sure your spawners are correctly set up and marked available.", defaultSpawnPointID));
            }
        }

        internal void _OnSpawn(FPSControlPlayerSpawn spawn)
        {
            Vector3 pos = spawn.transform.position;
            transform.position = pos;
            Quaternion rot = spawn.transform.rotation;
            transform.rotation = rot;
            playerCamera.ResetRotation();
        }

        protected override void OnInitialize(){}

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
