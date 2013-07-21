using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControlEditor.Utils;

namespace FPSControlEditor
{
    public abstract class FPSControlAbstractInspector : Editor
    {
        internal Texture LoadPNG(string folderName, string assetName)
        {
            return AssetLoader.LoadPNG(folderName, assetName);
        }

        internal T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            return AssetLoader.Load<T>(assetPath);
        }
    }
}
