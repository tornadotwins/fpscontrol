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

        public FPSControlWeapon[] weaponActors; //all possible weapons should be setup here
        Dictionary<string, FPSControlWeapon> _weaponsCatalogue = new  Dictionary<string, FPSControlWeapon>(); //the catalogue of weapons, built dynamically from weaponActors array
        List<FPSControlWeapon> _availableWeapons = new List<FPSControlWeapon>(); //the available weapons (max 

        Transform _transform;
        Transform _parent;

        FPSControlPlayerMovement _playerMovement;
        FPSControlPlayerCamera _playerCamera;

        public Vector3 shouldersOffset;

        FPSControlWeapon _currentWeapon = null;
        public FPSControlWeapon currentWeapon{get{return _currentWeapon;}}
        FPSControlWeapon _queuedWeapon = null;

        bool _running = false;
        PlayerState _prevState;

        bool _mouseDown = false;
        bool _mouseWasDown = false;
        float _mouseCounter = 0;

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

            for (int i = 0; i < weaponActors.Length; i++)
            {
                FPSControlWeapon weapon = weaponActors[i];
                weapon.transform.localPosition = weapon.pivot;
                weapon.transform.localRotation = Quaternion.Euler(weapon.euler);
                _weaponsCatalogue.Add(weapon.weaponName, weapon);
            }
        }

        public bool CanAddWeapon(string weaponName)
        {
            return _availableWeapons.Count < 4 && !_availableWeapons.Contains(_weaponsCatalogue[weaponName]);
        }

        //Add a weapon to the list of available weapons
        public void Add(string weaponName, bool makeCurrent)
        {
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
            if (index >= _availableWeapons.Count || index >= 4) return;

            if (_currentWeapon && _currentWeapon == _availableWeapons[index])
            {
                return; //already there
            }
            else if (_currentWeapon)
            {
                _queuedWeapon = _availableWeapons[index];
                _currentWeapon.Deactivate(_ActivateQueuedWeapon);
            }
            else
            {
                _currentWeapon = _availableWeapons[index];
                _currentWeapon.Activate(this);
            }
        }

        internal void _ActivateQueuedWeapon()
        {
            _currentWeapon = _queuedWeapon;
            _currentWeapon.Activate(this);
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
            _currentWeapon.StartRun();
        }

        public void EndRun()
        {
            _currentWeapon.EndRun();
        }

        public override void DoUpdate()
        {
            if (!_currentWeapon) return;

            _mouseDown = Input.GetMouseButton(0);

            if(_mouseDown && _mouseWasDown && _mouseCounter < _currentWeapon.chargeTime)
            {
                _mouseCounter += Time.deltaTime;
            }
            else if (_mouseDown && _mouseWasDown && _mouseCounter >= _currentWeapon.chargeTime)
            {
                _mouseCounter += Time.deltaTime;
                _currentWeapon.Charge(Time.deltaTime);
            }
            else if(!_mouseDown && _mouseWasDown)
            {
                _mouseCounter = 0;
                _currentWeapon.Fire();
            }

            if (Input.GetMouseButton(1)) _currentWeapon.Scope();
            if (Input.GetKeyDown(reloadKey)) _currentWeapon.Reload();
            if (Input.GetKey(defendKey)) _currentWeapon.Defend();

            if (Input.GetKeyDown(weaponToggle)) CycleToNextWeapon(_availableWeapons.IndexOf(_currentWeapon));
            else if (Input.GetKeyDown(weaponKey1)) ActivateWeaponAt(0);
            else if (Input.GetKeyDown(weaponKey2)) ActivateWeaponAt(1);
            else if (Input.GetKeyDown(weaponKey3)) ActivateWeaponAt(2);
            else if (Input.GetKeyDown(weaponKey4)) ActivateWeaponAt(3);

            _mouseWasDown = _mouseDown;
        }

        public override void DoLateUpdate()
        {
            _transform.localPosition = shouldersOffset;
        }
    }
}
