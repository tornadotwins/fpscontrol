using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine;
using FPSControl;
using System.IO;

namespace FPSControl.Data.Config
{
    public abstract class ConfigBase : ScriptableObject
    {
        public static T Create<T>(string objName) where T : ConfigBase
        {
            T obj = ScriptableObject.CreateInstance<T>();
            obj.name = objName;
            obj.Construct(); // just a hook for initial creation
            return obj;
        }

        protected virtual void Construct() { }
    }
}
