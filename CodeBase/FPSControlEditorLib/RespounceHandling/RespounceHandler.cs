using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using FPSControlEditor;
//#if UNITY_EDITOR
using UnityEditor;
//#endif

namespace FPSControlEditor
{
    
    internal static class RespounceHandler
    {
        private static string RESPOUNCE_DATA_KEY = "FPSCONTROL:RESPOUNCE_DATA_KEY";

        private static RespounceData _respounceData;
        private static RespounceData respounceData 
        {
            get
            {
                if (_respounceData == null) _respounceData = Serializer.LoadData<RespounceData>(RESPOUNCE_DATA_KEY, true, true);
                return _respounceData;
            }
            set
            {
                _respounceData = value;
                Serializer.SaveData<RespounceData>(RESPOUNCE_DATA_KEY, _respounceData, true, true, true);
            }
        }

        public static bool validUser
        {
            get
            {
                return respounceData.login.userExsist;
            }
        }

        public static bool validPassword
        {
            get
            {
                return respounceData.login.passwordMatch;
            }
        }


        private static string _URL;
        public static string purchaseURL
        {
            get
            {
                if (_URL == null) return "http://www.fpscontrol.com/plugins";
                return _URL;
            }
        }

        private static bool discardUpdate = false;
        internal static bool CheckForPurchase(FPSControlModuleType module)
        {
            if (module == FPSControlModuleType.NONE || module == FPSControlModuleType.UNAVAILABLE || module == FPSControlModuleType.NeedsPurchasing) return true;
            if (respounceData == null) return false;
            if (respounceData.purchaseData.ContainsKey(module.ToString()))
            {
                PurchaseModuleData mData = respounceData.purchaseData[module.ToString()];
                if (mData.purchased)
                {
                    if (FPSControlMainEditor.modules[module].version < mData.version)
                    {
                        if (!discardUpdate && EditorUtility.DisplayDialog("Update!", "There is an update for this module would you like to update?", "Update", "Cancel"))
                        {
                            Debug.Log("Downloading new version of " + module);
                            discardUpdate = true;
                            DownloadAndUpdate(mData); 
                        }                        
                        discardUpdate = true;
                    }
                    _URL = mData.url;
                    return true;
                }
            }
            _URL = null;
            return false;
        }

        
        internal static void DownloadAndUpdate(PurchaseModuleData mData)
        {
            string filepath = mData.purl;
            MonoBehaviour invoker = (MonoBehaviour)GameObject.FindObjectOfType(typeof(MonoBehaviour));
            invoker.StartCoroutine(Download(filepath));
        }

        static IEnumerator Download(string url)
        {
            WWW www = new WWW(url);

            while (!www.isDone)
            {
                EditorUtility.DisplayProgressBar("Update Manager", "Downloading update", www.progress);
            }

            if (www.error != null)
            { 
                Debug.LogWarning("ERROR DOWNLOADING UPDATE: " + www.error);
            } 
            else
            {
                EditorUtility.DisplayProgressBar("Update Manager", "Installing update..", www.progress);
                string tempPath = Path.Combine(Path.GetTempPath(), "temp.unitypackage");
                System.IO.FileStream _FileStream = new System.IO.FileStream(tempPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                _FileStream.Write(www.bytes, 0, www.bytes.Length);
                _FileStream.Close();
                AssetDatabase.ImportPackage(tempPath, false);
            }

            EditorUtility.ClearProgressBar();

            yield break;
        }

        internal static void LoadWebResult(string json)
        {
            respounceData = JSONDeserializer.Get<RespounceData>(json);
        }

        private static void CreateTestJSON()
        {
            RespounceData test = new RespounceData();

            PurchaseModuleData a = new PurchaseModuleData();
            a.purchased = true;
            a.version = 1.0f;
            //test.data.Add(FPSControlModuleType.MusicControl, a);

            PurchaseModuleData b = new PurchaseModuleData();
            b.purchased = true;
            b.version = 1.0f;
           // test.data.Add(FPSControlModuleType.WeaponControl, b);

            test.login.passwordMatch = true;
            test.login.userExsist = true;
            //test.login.subscription = "free";

           // Debug.Log(JSONDeserializer.GenerateJSON(test));
        }

    }
}
