using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FPSControl;
using FPSControl.Data;
using FPSControl.States.Player;
using FPSControl.PersistentData;

namespace FPSControl
{
    public class FPSControlPlayerWeaponManagerSaveData
    {
        public const string IDENTIFIER = "Weapons Manager";
        public FPSControlWeaponSaveData[] weapons;
        public string activeWeaponName;

        public FPSControlPlayerWeaponManagerSaveData() { }

        public FPSControlPlayerWeaponManagerSaveData(FPSControlPlayerWeaponManager manager)
        {

            FPSControlWeapon[] available = manager.availableWeapons;
            weapons = new FPSControlWeaponSaveData[available.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                if (available[i] is FPSControlMeleeWeapon) weapons[i] = new FPSControlWeaponSaveData((FPSControlMeleeWeapon) available[i]);
                else weapons[i] = new FPSControlWeaponSaveData((FPSControlRangedWeapon) available[i]);
            }

            if (manager.currentWeapon == null) 
                activeWeaponName = "<NULL>";
            else 
                activeWeaponName = manager.currentWeapon.definition.weaponName;
        }

        public void Update(FPSControlPlayerWeaponManager manager)
        {
            List<FPSControlWeapon> availableWeapons = new List<FPSControlWeapon>();
            for (int i = 0; i < weapons.Length; i++)
            {
                foreach(FPSControlWeapon actor in manager.WeaponActors)
                {
                    if (actor.definition.weaponName == weapons[i].name)
                    {
                        availableWeapons.Add(actor);
                        if (weapons[i].type == FPSControlWeaponSaveData.WeaponType.Melee)
                            weapons[i].Update((FPSControlMeleeWeapon)actor);
                        else
                            weapons[i].Update((FPSControlRangedWeapon)actor);
                    }
                }
            }
            manager._PDLoadAvailableWeapons(availableWeapons);

            if (activeWeaponName != "<NULL>")
                manager.ActivateWeapon(activeWeaponName);

        }
    }
    
    /// <summary>
    /// this class will manage weapons, such as swapping, and the actual attacking
    /// </summary>
    public class FPSControlPlayerWeaponManager : FPSControlPlayerComponent, IEnumerable
    {
        public KeyCode weaponKey1 = KeyCode.Alpha1;
        public KeyCode weaponKey2 = KeyCode.Alpha2;
        public KeyCode weaponKey3 = KeyCode.Alpha3;
        public KeyCode weaponKey4 = KeyCode.Alpha4;
        public KeyCode weaponToggle = KeyCode.Q;
        public KeyCode reloadKey = KeyCode.R;
        public KeyCode defendKey = KeyCode.X;
        public bool addWeaponsToInventory = true;
        public string defaultWeaponName;

        [HideInInspector]
        FPSControlWeapon[] _weaponActors; //all possible weapons should be setup here
        public FPSControlWeapon[] WeaponActors { get { return _weaponActors; } }

        Dictionary<string, FPSControlWeapon> _weaponsCatalogue = new  Dictionary<string, FPSControlWeapon>(); //the catalogue of weapons, built dynamically from weaponActors array
        [HideInInspector]
        List<FPSControlWeapon> _availableWeapons = new List<FPSControlWeapon>(); //the available weapons (max
        public FPSControlWeapon[] availableWeapons { get { return _availableWeapons.ToArray(); } }
        public WeaponsCatalogue weaponPrefabsCatalogue;

        internal void _PDLoadAvailableWeapons(List<FPSControlWeapon> weapons)
        {
            _availableWeapons = weapons;
        }

        [HideInInspector]
        Transform _transform;
        [HideInInspector]
        Transform _parent;
        [HideInInspector]
        FPSControlPlayerMovement _playerMovement;
        [HideInInspector]
        FPSControlPlayerCamera _playerCamera;
        [HideInInspector]
        public Vector3 shouldersOffset;
        [HideInInspector]
        FPSControlWeapon _currentWeapon = null;
        [HideInInspector]
        public FPSControlWeapon currentWeapon{get{return _currentWeapon;}}
        [HideInInspector]
        FPSControlWeapon _queuedWeapon = null;
        [HideInInspector]
        bool _running = false;
        [HideInInspector]
        PlayerState _prevState;
        [HideInInspector]
        bool _fireDown = false;
        [HideInInspector]
        bool _mouseWasDownL = false;
        [HideInInspector]
        bool _scopeDown = false;
        [HideInInspector]
        bool _mouseWasDownR = false;
        [HideInInspector]
        float _mouseCounter = 0;

        public IntelliCrosshair defaultCrosshair;
        public CrosshairAnimator crosshairAnimator;

        public override void SetState(States.State state)
        {
            base.SetState(state);
            if (_prevState != state && _prevState == Player.runState) EndRun();
            else if (_prevState != state && state == Player.runState) StartRun();
            _prevState = this.state;
        }

        void Awake()
        {
            _transform = transform;
            _parent = _transform.parent;

            crosshairAnimator.SetCrossHair(defaultCrosshair);
            
            List<FPSControlWeapon> _collectedActors = new List<FPSControlWeapon>();

            Dictionary<string, IntelliCrosshair> _allCrosshairs = new Dictionary<string,IntelliCrosshair>();
            foreach (IntelliCrosshair ch in Resources.FindObjectsOfTypeAll(typeof(IntelliCrosshair)))
            {
                if (_allCrosshairs.ContainsKey(ch.name))
                    Debug.LogWarning("Duplicate Crosshair name detected: " + ch.name);
                else
                _allCrosshairs.Add(ch.name, ch);
            }

            Dictionary<string, FPSControlWeapon> preexistingComponents = new Dictionary<string,FPSControlWeapon>();
            foreach (Transform t in transform)
            {
                FPSControlWeapon c = t.GetComponent<FPSControlWeapon>();
                if(c)
                {
                    if(preexistingComponents.ContainsKey(c.name))
                    {
                        Debug.LogError(string.Format("Found weapon named '{0}' more than once!",c.name));
                        continue;
                    }
                    preexistingComponents.Add(c.name, c);
                }
            }

            foreach (GameObject g in weaponPrefabsCatalogue.Values)
            {
                GameObject go;
                if (preexistingComponents.ContainsKey(g.name))
                {
                    go = preexistingComponents[g.name].gameObject;
                }
                else
                {
                    go = (GameObject) Instantiate(g);
                    go.name = g.name;
                    go.SetActive(false);
                    go.transform.parent = transform;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                }

                FPSControlWeapon weapon = go.GetComponent<FPSControlWeapon>();
                if (weapon)
                {
                    _collectedActors.Add(weapon);
                    weapon.transform.localPosition = weapon.definition.pivot;
                    weapon.transform.localRotation = Quaternion.Euler(weapon.definition.euler);
                    
                    if(_allCrosshairs.ContainsKey(weapon.crossHairName))
                        weapon.crosshair = _allCrosshairs[weapon.crossHairName];
                    else
                        Debug.LogWarning("Could not find Crosshair with name: " + weapon.crossHairName);
                    
                    weapon.definition.weaponName = go.name; // Insure names are synced correctly.
                    _weaponsCatalogue.Add(weapon.definition.weaponName, weapon);
                }
            }
            
            _weaponActors = _collectedActors.ToArray();
        }

        void Start()
        {
            // If we have persistent data saved
            if (PersistentData.PersistentData.Exists<FPSControlPlayerWeaponManagerSaveData>(PersistentData.PersistentData.NS_WEAPONS, FPSControlPlayerWeaponManagerSaveData.IDENTIFIER)) 
            {
                // Load the data
                FPSControlPlayerWeaponManagerSaveData saveData =
                    PersistentData.PersistentData.Read<FPSControlPlayerWeaponManagerSaveData>(PersistentData.PersistentData.NS_WEAPONS, FPSControlPlayerWeaponManagerSaveData.IDENTIFIER);

                // Iterate through the data and add weapons to inventory, and activate the last active weapon.
                for (int i = 0; i < saveData.weapons.Length; i++)
                {
                    FPSControlWeaponSaveData savedWeapon = saveData.weapons[i];
                    AddToInventory(savedWeapon.name, saveData.activeWeaponName == savedWeapon.name);
                }
            }
            else if (addWeaponsToInventory) // If we don't have any data, and we are told to add weapons to inventory - do so now.
            {
                int added = 0;
                bool first = true;
                foreach (FPSControlWeapon weapon in _weaponActors)
                {
                    added++;
                    if (added < 4)
                    {
                        if (defaultWeaponName == "")
                            AddToInventory(weapon.definition.weaponName, first);
                        else
                            AddToInventory(weapon.definition.weaponName, weapon.definition.weaponName == defaultWeaponName);

                        first = false;
                        if (weapon.GetType() == typeof(FPSControlRangedWeapon))
                            ((FPSControlRangedWeapon)weapon).SetAmmo((int)((FPSControlRangedWeapon)weapon).rangeDefinition.clipCapacity, 0);
                    }
                }
            } // Otherwise do nothing

        }

        public bool CanAddWeapon(string weaponName)
        {
            return _availableWeapons.Count < 4 && !_availableWeapons.Contains(_weaponsCatalogue[weaponName]);
        }

        //Add a weapon to the list of available weapons
        public void AddToInventory(string weaponName, bool makeCurrent)
        {
            //Debug.Log(weaponName);
            if(_availableWeapons.Count == 4) return; //max capacity            
            FPSControlWeapon weapon = _weaponsCatalogue[weaponName];            
            _availableWeapons.Add(weapon);
            if (makeCurrent)
            {
                ActivateWeaponAt(_availableWeapons.IndexOf(weapon));
            }            
        }

        //remove from the list of available weapons
        public void Remove(FPSControlWeapon weapon)
        {
            if (weapon.isActiveWeapon)
            {
                weapon.Deactivate(_RemoveAfterDeactivation);
                
            }
            else
            {
                _availableWeapons.Remove(weapon);
            }
            
        }

        void _RemoveAfterDeactivation()
        {
            int currentWeaponIndex = _availableWeapons.IndexOf(_currentWeapon);
            _availableWeapons.Remove(_currentWeapon);
            _currentWeapon = null;
            CycleToNextWeapon(currentWeaponIndex);
        }

        public T Get<T>(int index) where T : FPSControlWeapon
        {
            return (T) this[index];
        }

        public T Get<T>(string weaponName) where T : FPSControlWeapon
        {
            return (T)this[weaponName];
        }

        public FPSControlWeapon this[string weaponName]
        {
            get
            {
                if (_weaponsCatalogue.ContainsKey(weaponName))
                    return _weaponsCatalogue[weaponName];
                else
                    Debug.LogWarning("The weapon named \"" + weaponName + "\" could not be found!");
                return null;
            }
        }

        public FPSControlWeapon this[int index]
        {
            get
            {
                return _availableWeapons[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _availableWeapons.Count; i++)
            {
                yield return _availableWeapons[i];
            }
        }

        public void CycleToNextWeapon(int currentIndex)
        {
            //Debug.Log("cycling from : " + currentIndex);
            int _currIndex = currentIndex;

            if (_currentWeapon)
            {
                _currIndex++;
                if (_currIndex >= _availableWeapons.Count) _currIndex = 0; //wrap
                //_currIndex = (int) Mathf.Repeat(_currIndex + 1, _availableWeapons.Count-1);
                if (_currIndex == currentIndex)
                {
                    Debug.Log("Only one weapon - bypassing.");
                    return;
                }
                ActivateWeaponAt(_currIndex);
            }
            else
            {
                ActivateWeaponAt(0);
            }
        }

        public void DeactivateCurrentWeapon()
        {

            crosshairAnimator.SetCrossHair(null);
            FPSControlWeapon tmp = _currentWeapon;
            _currentWeapon = null;
            _queuedWeapon = null;
            tmp.Deactivate(() => { });
        } 

        public void ActivateWeapon(string weaponName)
        {
            ActivateWeaponAt(_availableWeapons.IndexOf(this[weaponName]));
        }

        public void ActivateWeaponAt(int index)
        {
            try
            {

                Debug.Log(string.Format("Activating weapon {0} of {1}", index + 1, _availableWeapons.Count));
                if (index >= _availableWeapons.Count)
                {
                    return;
                }
                if (index >= 4)
                {
                    Debug.LogWarning("Attempting to activate a weapon which index exceeds the maximum of 4.");
                    return;
                }

                string wpnName = "";

                if (_currentWeapon && _currentWeapon == _availableWeapons[index])
                {
                    Debug.LogWarning("Attempting to activate the current weapon, but it is already activated!");
                    return; //already there
                }
                else if (_currentWeapon)
                {
                    //Debug.LogWarning("activating new");
                    _queuedWeapon = _availableWeapons[index];
                    FPSControlWeapon _weaponBeingDeactivated = _currentWeapon;
                    crosshairAnimator.SetCrossHair(null);
                    _currentWeapon.Deactivate(() => { Debug.Log("Deactivation complete. Activating queued weapon."); _ActivateQueuedWeapon(); });
                    _currentWeapon = null;

                    wpnName = _queuedWeapon.impactName == "None" ? _queuedWeapon.name : _queuedWeapon.impactName;
                }
                else
                {
                    //Debug.Log("No current weapon. Activating fresh.");
                    if (index < _availableWeapons.Count)
                    {
                        _currentWeapon = _availableWeapons[index];
                    }
                    else
                    {
                        Debug.LogError(string.Format("Out of scope. Attempting to activate weapon at index ({0}) when available weapons length is {1}", index, _availableWeapons.Count));
                        return;
                    }

                    _currentWeapon._Activate(this, ActivateCrosshair);

                    wpnName = _currentWeapon.impactName == "None" ? _currentWeapon.name : _currentWeapon.impactName;
                }

                ImpactControl impact = Player.GetComponent<ImpactControl>();
                if (impact != null)
                {
                    impact.ActivateImpactEffect(wpnName);
                }
            }
            catch (System.Exception err) { Debug.LogWarning("Caught Exeption: " + err.Message); }
        }

        void ActivateCrosshair()
        {
            //Debug.Log("Setting crosshair and calling activate weapon event.");
            if (crosshairAnimator)
                crosshairAnimator.SetCrossHair(_currentWeapon.crosshair);
            else
                Debug.LogWarning("Crosshair animator could not be found!");
        }

        internal void _ActivateQueuedWeapon()
        {
            _currentWeapon = _queuedWeapon;
            _currentWeapon.Activate(this);
            crosshairAnimator.SetCrossHair(_currentWeapon.crosshair);
            _queuedWeapon = null;
            FPSControlPlayerEvents.ActivateWeapon(_currentWeapon);
        }

        public void Initialize(FPSControlPlayerCamera playerCamera, FPSControlPlayerMovement playerMovement, FPSControlPlayer player)
        {
            _playerMovement = playerMovement;
            _playerCamera = playerCamera;
            _transform.localPosition = shouldersOffset; //position the shoulders below the head            
            Initialize(player);

            //if (PersistentData.PersistentData.Exists<FPSControlPlayerWeaponManagerSaveData>( PersistentData.PersistentData.NS_WEAPONS, "Weapon Manager"))
            //{
            //    FPSControlPlayerWeaponManagerSaveData saveData = PersistentData.PersistentData.Read<FPSControlPlayerWeaponManagerSaveData>(
            //        PersistentData.PersistentData.NS_WEAPONS,
            //        "Weapon Manager");

            //    saveData.Update(this);
            //}
            //else
            //{
            //    FPSControlPlayerData.SaveWeaponData(); // Save the data to kick off.
            //}
        }

        public void StartRun()
        {
            if(_currentWeapon)_currentWeapon.StartRun();
        }

        public void EndRun()
        {
            if (_currentWeapon) _currentWeapon.EndRun();
        }

        public override void DoUpdate()
        {
            if (!FPSControlPlayerData.visible || FPSControlPlayerData.frozen) return;
            if (_currentWeapon)
            {

                _fireDown = FPSControlInput.IsFiring();
                _scopeDown = FPSControlInput.IsScoping();

                crosshairAnimator.Fire(_fireDown);

                if (_fireDown && _mouseWasDownL && _mouseCounter < _currentWeapon.definition.chargeTime)
                {
                    _mouseCounter += Time.deltaTime;
                }
                else if (_fireDown && _mouseWasDownL && _mouseCounter >= _currentWeapon.definition.chargeTime)
                {
                    _mouseCounter += Time.deltaTime;
                    _currentWeapon.Charge(Time.deltaTime);
                }
                else if (!_fireDown && _mouseWasDownL)
                {
                    _mouseCounter = 0;
                    _currentWeapon.Fire();
                }

                if (_currentWeapon.canScope)
                {
                    if (_scopeDown && !_mouseWasDownR)
                    {
                        _currentWeapon.Scope();
                        Player.playerCamera.Scope(true, _currentWeapon.definition.scopeFOV);
                        crosshairAnimator.StartZoom();
                    }
                    else if (!_scopeDown && _mouseWasDownR)
                    {
                        _currentWeapon.ExitScope();
                        Player.playerCamera.Scope(false, Player.playerCamera.baseFOV);
                        crosshairAnimator.EndZoom();
                    }
                }

                if (FPSControlInput.IsReloading())
                {
                    //Debug.Log("Reload key!");
                    _currentWeapon.Reload();
                }
                if (FPSControlInput.IsDefending()) _currentWeapon.Defend();

                _mouseWasDownL = _fireDown;
                _mouseWasDownR = _scopeDown;
            }

            if (FPSControlInput.IsTogglingWeapon()) CycleToNextWeapon(_availableWeapons.IndexOf(_currentWeapon));
            //#error Removed stuff
            else if (FPSControlInput.IsSelectingWeapon(0)) {
                //Debug.Log("Pressed ActivateAt0"); 
                ActivateWeaponAt(0); 
            }
            else if (FPSControlInput.IsSelectingWeapon(1)) ActivateWeaponAt(1);
            else if (FPSControlInput.IsSelectingWeapon(2)) ActivateWeaponAt(2);
            else if (FPSControlInput.IsSelectingWeapon(3)) ActivateWeaponAt(3);

            
        }

        public override void DoLateUpdate()
        {
            _transform.localPosition = shouldersOffset;
        }
    }
}
