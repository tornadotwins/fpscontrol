using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPSControl;

namespace FPSControlEditor
{
    class CopyObject
    {


        static string TEMP_DEF_PATH1 { get { return FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "test1.prefab"; } }
        static string TEMP_DEF_PATH2 { get { return FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "test2.prefab"; } }

        private static GameObject go1;
        private static GameObject go2;

        public static void Copy(GameObject go) {

            GameObject t1 = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
            Debug.Log(t1);

            go1 = PrefabUtility.CreatePrefab(TEMP_DEF_PATH1, go);
            //go2 = PrefabUtility.CreatePrefab(TEMP_DEF_PATH1, go);

            PrefabUtility.ReplacePrefab(go, go1, ReplacePrefabOptions.ConnectToPrefab);
            //PrefabUtility.ReplacePrefab(go, go2, ReplacePrefabOptions.ConnectToPrefab);

           // GameObject go2 = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go1);
           // PrefabUtility.ReconnectToLastPrefab(go2);

        }

        public static void RevertToSaved(GameObject go)
        {
            PrefabUtility.RevertPrefabInstance(go);
            //PrefabUtility.repl


        }

    }
}
