using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSControl
{
    public enum FPSControlPlatform
    {
        Mac,PC,Linux,iOS,Android,Ouya,Steambox
    }
    
    public class FPSControlInput
    {
        static ControlMap _map;
        static FPSControlPlatform _platform = FPSControlPlatform.Mac;

        /// <summary>
        /// Loads the Default map on the current running platform
        /// </summary>
        public static void LoadControlMapping()
        {
            FPSControlPlatform p = FPSControlPlatform.Mac;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsWebPlayer: p = FPSControlPlatform.Mac; break;

                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.OSXEditor: p = FPSControlPlatform.Mac; break;
            }
            LoadControlMapping(p);
        }
        
        public static void LoadControlMapping(FPSControlPlatform platform)
        {
            LoadControlMapping(platform, ""); //Just load the default for the platform.
        }

        public static void LoadControlMapping(FPSControlPlatform platform, string id)
        {
            ControlMapCatalogue cat = (ControlMapCatalogue) Resources.Load("ControlMappings");
            _map = cat.GetMap(platform, id);
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

//        public static bool IsSelectingWeapon(int id)
//        {
//            return false;
//        }

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
