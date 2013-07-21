using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControl;

namespace FPSControl
{
    [System.Serializable]
    public class ControlMapCatalogue : ScriptableObject
    {
        public const string FILE = "Assets/FPSControlCore/Resources/ControlMappings.asset";

        [SerializeField] DesktopControlMapGroup _macMaps;
        [SerializeField] DesktopControlMapGroup _pcMaps;
        [SerializeField] MobileControlMapGroup _iOSMaps;
        [SerializeField] MobileControlMapGroup _androidMaps;
        [SerializeField] OuyaControlMapGroup _ouyaMaps;
        [SerializeField] SteamboxControlMapGroup _steamboxMaps;

        public DesktopControlMapGroup mac { get { return _macMaps; } }
        public DesktopControlMapGroup pc { get { return _pcMaps; } }
        public MobileControlMapGroup iOS { get { return _iOSMaps; } }
        public MobileControlMapGroup android { get { return _androidMaps; } }
        public OuyaControlMapGroup ouya { get { return _ouyaMaps; } }
        public SteamboxControlMapGroup steambox { get { return _steamboxMaps; } }

        public static implicit operator bool(ControlMapCatalogue catalogue)
        {
            return catalogue != null;
        }

        public IControlMapGroup this[FPSControlPlatform platform]
        {
            get
            {
                switch (platform)
                {
                    case FPSControlPlatform.Mac: return mac;
                    case FPSControlPlatform.PC: return pc;
                    case FPSControlPlatform.Ouya: return ouya;
                    case FPSControlPlatform.iOS: return iOS;
                    case FPSControlPlatform.Android: return android;
                    case FPSControlPlatform.Steambox: return steambox;
                }
                return null;
            }
        }

        public ControlMapCatalogue()
        {
            _macMaps = new DesktopControlMapGroup();
            _pcMaps = new DesktopControlMapGroup();
            _iOSMaps = new MobileControlMapGroup();
            _androidMaps = new MobileControlMapGroup();
            _ouyaMaps = new OuyaControlMapGroup();
            _steamboxMaps = new SteamboxControlMapGroup();
        }

        public ControlMap GetMap(FPSControlPlatform platform)
        {
            return GetMap(platform, "");
        }

        public ControlMap GetMap(FPSControlPlatform platform, string key)
        {
            switch (platform)
            {
                case FPSControlPlatform.Mac: return _macMaps[key];
                case FPSControlPlatform.PC: return _pcMaps[key];
                case FPSControlPlatform.Ouya: return _ouyaMaps[key];
                case FPSControlPlatform.iOS: return _iOSMaps[key];
                case FPSControlPlatform.Android: return _androidMaps[key];
                case FPSControlPlatform.Steambox: return _steamboxMaps[key];
            }

            return null;
        }

        public void Add(FPSControlPlatform platform, string key)
        {
            switch (platform)
            {
                case FPSControlPlatform.Mac: _macMaps.Add(key, new DesktopControlMap(key)); break;
                case FPSControlPlatform.PC: _pcMaps.Add(key, new DesktopControlMap(key)); break;
                case FPSControlPlatform.Ouya: _ouyaMaps.Add(key, new OuyaControlMap(key)); break;
                case FPSControlPlatform.iOS: _iOSMaps.Add(key, new MobileControlMap(key)); break;
                case FPSControlPlatform.Android: _androidMaps.Add(key, new MobileControlMap(key)); break;
                case FPSControlPlatform.Steambox: _steamboxMaps.Add(key, new SteamboxControlMap(key)); break;
            }
        }

        public void Remove(FPSControlPlatform platform, string key)
        {
            switch (platform)
            {
                case FPSControlPlatform.Mac: _macMaps.Remove(key); break;
                case FPSControlPlatform.PC: _pcMaps.Remove(key); break;
                case FPSControlPlatform.Ouya: _ouyaMaps.Remove(key); break;
                case FPSControlPlatform.iOS: _iOSMaps.Remove(key); break;
                case FPSControlPlatform.Android: _androidMaps.Remove(key); break;
                case FPSControlPlatform.Steambox: _steamboxMaps.Remove(key); break;
            }
        }
    }
}
