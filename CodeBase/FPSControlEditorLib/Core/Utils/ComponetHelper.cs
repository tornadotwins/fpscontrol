using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPSControl;

namespace FPSControlEditor
{

    public enum CopyComponetStyle
    {
        inclusive,
        exclusive
    }

    class ComponetHelper
    {

        public static void CopyComponets(Transform fromObject, Transform toObject)
        {
            CopyComponets(fromObject, toObject, CopyComponetStyle.exclusive, null);
        }

        public static void CopyComponets(Transform fromObject, Transform toObject, CopyComponetStyle copyStyle, params System.Type[] types)
        {
            CopyComponets(fromObject, toObject, copyStyle, true, types);
        }

        public static void CopyComponets(Transform fromObject, Transform toObject, CopyComponetStyle copyStyle, bool removeOld, params System.Type[] types)
        {
            //Remove all the componets on the target            
            if (removeOld) {
                Component[] toComponets = toObject.GetComponents<Component>();
                foreach (Component c in toComponets)
                {
                    bool destory = false;
                    if (copyStyle == CopyComponetStyle.exclusive && c.GetType() != typeof(Transform) && !IsOfType(c, types)) destory = true;
                    if (copyStyle == CopyComponetStyle.inclusive && c.GetType() != typeof(Transform) && IsOfType(c, types)) destory = true;
                    if (destory)  GameObject.DestroyImmediate(c);;
                }
            }

            Component[] fromComponets = fromObject.GetComponents<Component>();
            foreach (Component c in fromComponets)
            {
                if (c.GetType() != typeof(Transform) && !IsOfType(c, types))
                {
                    var newComponet = toObject.gameObject.AddComponent(c.GetType());
                    EditorUtility.CopySerialized(c, newComponet);
                }
            }
        }

        private static bool IsOfType(Component component, System.Type[] ignoreTypes)
        {
            if (ignoreTypes == null) return false;
            bool _isOfType = false;
            foreach (System.Type type in ignoreTypes)
            {
                if (component.GetType() == type) _isOfType = true;
            }
            return _isOfType;
        }

    }
}
