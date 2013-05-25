using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using FPSControlEditor;
using FPSControl;
//#if UNITY_EDITOR
using UnityEditor;
//#endif

namespace FPSControlEditor
{

    internal static class RespounceHandler
    {
        private static string RESPOUNCE_DATA_KEY = "FPSCONTROL:RESPOUNCE_DATA_KEY";
        private static string LAST_UPDATE_TIME_KEY = "FPSCONTROL:LAST_UPDATE_TIME_KEY";

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

        public static FPSControlModuleType lastChecked
        {
            get
            {
                return _lastChecked;
            }
        }

        private static bool discardUpdate = false;
        private static bool importReady = false;
        private static FPSControlModuleType _lastChecked;
        internal static bool CheckForPurchase(FPSControlModuleType module)
        {
            if (module == FPSControlModuleType.NONE || module == FPSControlModuleType.UNAVAILABLE || module == FPSControlModuleType.NeedsPurchasing) return true;
            if (respounceData == null) return false;
            _lastChecked = module;
            if (respounceData.purchaseData.ContainsKey(module.ToString()))
            {
                PurchaseModuleData mData = respounceData.purchaseData[module.ToString()];
                if (mData.purchased)
                {
                    if (CompareVersions(mData.version, FPSControlMainEditor.modules[module].version) > 0)
                    {
                        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                        int secondsSinceEpoch = (int)t.TotalSeconds;
                        int lastUpdateTime = EditorPrefs.GetInt(LAST_UPDATE_TIME_KEY, 0);
                        int timeDif = secondsSinceEpoch - lastUpdateTime;
                        if (timeDif > 240 && !discardUpdate && EditorUtility.DisplayDialog("Update!", "There is an update for this module would you like to update?", "Update", "Cancel"))
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

        internal static int CompareVersions(String strA, String strB)
        {
            Version vA = new Version("0." + strA.Replace(",", "."));
            Version vB = new Version("0." + strB.Replace(",", "."));

            return vA.CompareTo(vB);
        }

        internal static void DownloadAndUpdate(PurchaseModuleData mData)
        {
            string filepath = mData.purl;
            MonoBehaviour invoker = (MonoBehaviour)GameObject.FindObjectOfType(typeof(MonoBehaviour));
            if (invoker == null)
            {
                new GameObject("Player", typeof(RBFPSControllerLogic));
                invoker = (MonoBehaviour)GameObject.FindObjectOfType(typeof(MonoBehaviour));
            }
            invoker.StartCoroutine(Download(filepath, invoker));
        }

        private static string tempPath = "";

        static IEnumerator Download(string url, MonoBehaviour invoker)
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
                tempPath = Path.Combine(Path.GetTempPath(), "temp.unitypackage");
                System.IO.FileStream _FileStream = new System.IO.FileStream(tempPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                int size = www.bytes.Length;
                _FileStream.Write(www.bytes, 0, www.bytes.Length);
                _FileStream.Close();
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;
                EditorPrefs.SetInt(LAST_UPDATE_TIME_KEY, secondsSinceEpoch);
                AssetDatabase.ImportPackage(tempPath, false);
            }
            EditorUtility.ClearProgressBar();
            yield break;
        }


        internal static void LoadWebResult(string json)
        {
            Debug.Log(json);
            respounceData = JSONDeserializer.Get<RespounceData>(json);
        }

        private static void CreateTestJSON()
        {
            RespounceData test = new RespounceData();

            PurchaseModuleData a = new PurchaseModuleData();
            a.purchased = true;
            a.version = "1.0";
            //test.data.Add(FPSControlModuleType.MusicControl, a);

            PurchaseModuleData b = new PurchaseModuleData();
            b.purchased = true;
            b.version = "1.0";
            // test.data.Add(FPSControlModuleType.WeaponControl, b);

            test.login.passwordMatch = true;
            test.login.userExsist = true;
            //test.login.subscription = "free";

            // Debug.Log(JSONDeserializer.GenerateJSON(test));
        }

    }
}
