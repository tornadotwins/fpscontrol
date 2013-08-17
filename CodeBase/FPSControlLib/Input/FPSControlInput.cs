using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSControl
{
    public enum FPSControlPlatform
    {
        UNSUPPORTED=-1,Mac=0,PC=1,Linux=2,iOS=3,Android=4,Ouya=5,Steambox=6
    }
    
    public class FPSControlInput
    {
        public static event System.Action<ControlMap> OnMapLoad;
        
        static ControlMap _map;
        static FPSControlPlatform _platform = FPSControlPlatform.Mac;

        /// <summary>
        /// Loads the Default map on the current running platform
        /// </summary>
        public static void LoadControlMapping()
        {
            FPSControlPlatform p = FPSControlSystemInfo.GetPlatform();

            LoadControlMapping(p);
        }
        
        public static void LoadControlMapping(FPSControlPlatform platform)
        {
            GameObject adapter;
            if (platform == FPSControlPlatform.Ouya)
            {
                GameObject tmp = (GameObject)Resources.Load("[Ouya Controller]", typeof(GameObject));
                adapter = (GameObject) Object.Instantiate(tmp);
                tmp.name = adapter.name;
            }

            LoadControlMapping(platform, ""); //Just load the default for the platform.
        }

        public static void LoadControlMapping(FPSControlPlatform platform, string id)
        {
            ControlMapCatalogue cat = (ControlMapCatalogue) Resources.Load("ControlMappings");
            _map = cat.GetMap(platform, id);
            _map.Initialize();
            //Screen.lockCursor = true; Debug.Log("locking cursor!");
            if (OnMapLoad != null) OnMapLoad(_map);
        }

        public static bool invertedLook { get { return _map.invertedLook; } }

        public static bool IsInteracting()
        {
            return _map.GetInteract();
        }

        public static bool IsFiring()
        {
            return _map.GetFire();
        }

        public static bool IsScoping()
        {
            return _map.GetScope();
        }

        public static bool IsReloading()
        {
            return _map.GetReload();
        }

        public static bool IsDefending()
        {
            return _map.GetDefend();
        }

        public static bool IsJumping()
        {
            return _map.GetJump();
        }

        public static bool IsCrouching()
        {
            return _map.GetCrouch();
        }

        public static bool IsRunning()
        {
            return _map.GetRun();
        }

        public static bool IsTogglingWeapon()
        {
            return _map.GetWeaponToggle();
        }

        public static bool IsSelectingWeapon(int id)
        {
            switch (id)
            {
                case 0: return _map.GetWeapon1();
                case 1: return _map.GetWeapon2();
                case 2: return _map.GetWeapon3();
                case 3: return _map.GetWeapon4();
            }

            return false;
        }

        public static Vector2 GetMoveDirection()
        {
            return _map.GetMovement();
        }

        public static Vector2 GetLookDirection()
        {
            return _map.GetLook();
        }

        
    }
}
