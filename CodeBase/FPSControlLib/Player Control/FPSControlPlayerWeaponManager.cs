using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FPSControl;
using FPSControl.States.Player;

namespace FPSControl
{
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

        public FPSControlWeapon[] weaponActors; //all possible weapons should be setup here
        Dictionary<string, FPSControlWeapon> _weaponsCatalogue = new  Dictionary<string, FPSControlWeapon>(); //the catalogue of weapons, built dynamically from weaponActors array
        [HideInInspector]
        List<FPSControlWeapon> _availableWeapons = new List<FPSControlWeapon>(); //the available weapons (max 
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
        bool _mouseDownL = false;
        [HideInInspector]
        bool _mouseWasDownL = false;
        [HideInInspector]
        bool _mouseDownR = false;
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

        void Start()
        {
            _transform = transform;
            _parent = _transform.parent;
            int added = 0;
            for (int i = 0; i < weaponActors.Length; i++)
            {
                FPSControlWeapon weapon = weaponActors[i];
                if (weapon == null) continue;
                weapon.transform.localPosition = weapon.definition.pivot;
                weapon.transform.localRotation = Quaternion.Euler(weapon.definition.euler);
                _weaponsCatalogue.Add(weapon.definition.weaponName, weapon);
                if (addWeaponsToInventory)
                {
                    added++;
                    if (added < 4)
                    {
                        Add(weapon.definition.weaponName, (i == 0));
                        if (weapon.GetType() == typeof(FPSControlRangedWeapon))
                        {
                           ((FPSControlRangedWeapon)weapon).SetAmmo((int)((FPSControlRangedWeapon)weapon).rangeDefinition.clipCapacity, 0);
                        }
                    }
                }
            }

            crosshairAnimator.SetCrossHair(defaultCrosshair);
        }

        public bool CanAddWeapon(string weaponName)
        {
            return _availableWeapons.Count < 4 && !_availableWeapons.Contains(_weaponsCatalogue[weaponName]);
        }

        //Add a weapon to the list of available weapons
        public void Add(string weaponName, bool makeCurrent)
        {
            Debug.Log(weaponName);
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
                return _availableWeapons.Contains(_weaponsCatalogue[weaponName]) ? _weaponsCatalogue[weaponName] : null;
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
            Debug.Log("cycling from : " + currentIndex);
            int _currIndex = currentIndex;

            if (_currentWeapon)
            {
                _currIndex++;
                if (_currIndex >= _availableWeapons.Count) _currIndex = 0; //wrap
                //_currIndex = (int) Mathf.Repeat(_currIndex + 1, _availableWeapons.Count-1);
                ActivateWeaponAt(_currIndex);
            }
            else
            {
                ActivateWeaponAt(0);
            }
        }

        public void ActivateWeaponAt(int index)
        {
            Debug.Log(index + ":" + _availableWeapons.Count);
            if (index >= _availableWeapons.Count || index >= 4) return;

            if (_currentWeapon && _currentWeapon == _availableWeapons[index])
            {
                //Debug.Log("Already there");
                return; //already there
            }
            else if (_currentWeapon)
            {
                _queuedWeapon = _availableWeapons[index];
                _currentWeapon.Deactivate(_ActivateQueuedWeapon);
                crosshairAnimator.SetCrossHair(null);
            }
            else
            {
                _currentWeapon = _availableWeapons[index];
                _currentWeapon.Activate(this);
                crosshairAnimator.SetCrossHair(_currentWeapon.crosshair);
            }
        }

        internal void _ActivateQueuedWeapon()
        {
            _currentWeapon = _queuedWeapon;
            _currentWeapon.Activate(this);
            crosshairAnimator.SetCrossHair(_currentWeapon.crosshair);
            _queuedWeapon = null;
        }

        public void Initialize(FPSControlPlayerCamera playerCamera, FPSControlPlayerMovement playerMovement, FPSControlPlayer player)
        {
            _playerMovement = playerMovement;
            _playerCamera = playerCamera;
            _transform.localPosition = shouldersOffset; //position the shoulders below the head            
            Initialize(player);
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
            if (!_currentWeapon) return;

            _mouseDownL = Input.GetMouseButton(0);
            _mouseDownR = Input.GetMouseButton(1);

            crosshairAnimator.Fire(_mouseDownL);

            if(_mouseDownL && _mouseWasDownL && _mouseCounter < _currentWeapon.definition.chargeTime)
            {
                _mouseCounter += Time.deltaTime;
            }
            else if (_mouseDownL && _mouseWasDownL && _mouseCounter >= _currentWeapon.definition.chargeTime)
            {
                _mouseCounter += Time.deltaTime;
                _currentWeapon.Charge(Time.deltaTime);
            }
            else if(!_mouseDownL && _mouseWasDownL)
            {
                _mouseCounter = 0;
                _currentWeapon.Fire();
            }

            if (_currentWeapon.canScope)
            {
                if (_mouseDownR && !_mouseWasDownR)
                {
                    _currentWeapon.Scope();
                    Player.playerCamera.Scope(true, _currentWeapon.definition.scopeFOV);
                    crosshairAnimator.StartZoom();
                }
                else if (!_mouseDownR && _mouseWasDownR)
                {
                    _currentWeapon.ExitScope();
                    Player.playerCamera.Scope(false, Player.playerCamera.baseFOV);
                    crosshairAnimator.EndZoom();
                }
            }

            if (Input.GetKeyDown(reloadKey)) _currentWeapon.Reload();
            if (Input.GetKey(defendKey)) _currentWeapon.Defend();

            if (Input.GetKeyDown(weaponToggle)) CycleToNextWeapon(_availableWeapons.IndexOf(_currentWeapon));
            else if (Input.GetKeyDown(weaponKey1)) ActivateWeaponAt(0);
            else if (Input.GetKeyDown(weaponKey2)) ActivateWeaponAt(1);
            else if (Input.GetKeyDown(weaponKey3)) ActivateWeaponAt(2);
            else if (Input.GetKeyDown(weaponKey4)) ActivateWeaponAt(3);

            _mouseWasDownL = _mouseDownL;
            _mouseWasDownR = _mouseDownR;
        }

        public override void DoLateUpdate()
        {
            _transform.localPosition = shouldersOffset;
        }
    }
}
