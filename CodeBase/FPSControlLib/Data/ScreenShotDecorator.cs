using System.Collections.Generic;
using UnityEngine;

namespace FPSControl.Data
{
    public abstract class ScreenShotDecorator : MonoBehaviour
    {

        public abstract void OnPreRender(ScreenShotComponent component);
        public abstract void OnPostRender(ScreenShotComponent component);
    }
}
