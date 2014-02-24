using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FPSControl;
using FPSControl.Data.Config;
using System.IO;
using UnityEditor;
using FPSControlEditor;

namespace FPSControlEditor.Config
{
    public abstract class ConfigEditorBase : Editor
    {
        protected const string MENU_ITEM_PATH = "Assets/FPSControl/Config/";

        public static T Create<T>(string pathRelativeToProject, string fileName, bool overwrite = false) where T : ConfigBase
        {
            string path = Application.dataPath + "/" + pathRelativeToProject;
            T obj = ConfigBase.Create<T>(fileName);

            if (File.Exists(path + "/" + fileName + ".asset") && !overwrite)
                return null;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            AssetDatabase.CreateAsset(obj, pathRelativeToProject + "/" + fileName + ".asset");

            return obj;
        }

    }
}
