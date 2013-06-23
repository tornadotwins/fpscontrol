using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    public abstract class FPSControlAbstractInspector : Editor
    {
        internal Texture LoadPNG(string folderName, string assetName)
        {
            string assetPath = FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + folderName + assetName + ".png";
            if (!File.Exists(assetPath))
            {
                Debug.Log("Cant load GUI asset: " + assetPath);
                return null;
            }
            return Load<Texture>(assetPath);
        }

        internal T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));

            #if DEBUG 
                        Debug.Log("Loading asset at: " + assetPath + ". asset: " + obj);
            #endif

            return (T)obj;
        }
    }
}
