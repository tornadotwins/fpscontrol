using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor.Utils
{
    public class AssetLoader
    {
        public static Texture LoadPNG(string folderName, string assetName)
        {
            return Load<Texture>(FPSControlMainEditor.GRAPHICS + folderName, assetName + ".png");
        }

        public static T Load<T>(string folder, string file) where T : UnityEngine.Object
        {
            string assetPath = FPSControlMainEditor.ASSET_PATH + folder + "/" + file;
            if (!File.Exists(assetPath))
            {
                Debug.Log("Can't load asset: " + assetPath);
                return null;
            }
            return Load<T>(assetPath);
        }

        public static T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));

            #if DEBUG 
            //Debug.Log("Loading asset at: " + assetPath + ". asset: " + obj);
            #endif

            return (T)obj;
        }
    }
}
