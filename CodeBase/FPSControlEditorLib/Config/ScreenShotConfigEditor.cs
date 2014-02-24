using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using FPSControl.Data;
using FPSControl.Data.Config;

namespace FPSControlEditor.Config
{
    [CustomEditor(typeof(ScreenShotConfig))]
    public class ScreenShotConfigEditor : ConfigEditorBase
    {
        const string ASSET_PATH = "Assets/FPSControlCore/Config/ScreenShot.asset";
        
        [MenuItem(MENU_ITEM_PATH + "Create Screen Shot Config")]
        static void CreateAsset()
        {
            ScreenShotConfig config = (ScreenShotConfig) AssetDatabase.LoadAssetAtPath(ASSET_PATH, typeof(ScreenShotConfig));
            if (config)
            {
                Debug.Log("Configuration Asset 'ScreenShot.asset' already exists.");
                EditorGUIUtility.PingObject(config);
                return;
            }

            AssetDatabase.CreateFolder("Assets/FPSControlCore", "Config");
            config = Create<ScreenShotConfig>("Assets/FPSControlCore/Config", "ScreenShot");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //if (EditorApplication.isPlaying)
            //{
            //    GUILayout.Space(25);
            //    if (GUILayout.Button("Screen Shot Test"))
            //    {
            //        ScreenShot.Capture("Player Camera", "_TestFile", () =>
            //            {
            //                Debug.Log("Capture complete.");
            //            });
            //    }
            //}
        }
    }
}
