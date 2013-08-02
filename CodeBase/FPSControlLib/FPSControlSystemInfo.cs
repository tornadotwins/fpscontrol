using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSControl
{
    public class FPSControlSystemInfo
    {
        public static FPSControlPlatform GetPlatform()
        {
            if (Application.platform == RuntimePlatform.Android) return FPSControlPlatform.Ouya;
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.OSXEditor) return FPSControlPlatform.Mac;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer) return FPSControlPlatform.PC;
            return FPSControlPlatform.UNSUPPORTED;
        }
    }
}
