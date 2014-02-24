using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine;
using FPSControl;
using System.IO;
using FPSControl.Data.Config;

namespace FPSControl.Data
{
    public class ScreenShotSaveData
    {
        public string filePath;
        public string guid;
        public DateTime timestamp;

        public ScreenShotSaveData() { }
        public ScreenShotSaveData(string filePath)
        {
            this.filePath = filePath;
            guid = Guid.NewGuid().ToString("N");
            timestamp = DateTime.UtcNow;
        }
    }
    
    public class ScreenShot : MonoBehaviour
    {
        const string FOLDER_NAME = "ScreenShots";
        public static string PATH { get { return PersistentData.PATH + "/" + FOLDER_NAME; } }

        static ScreenShot _Instance;
        static ScreenShot Instance
        {
            get
            {
                if (!_Instance)
                {
                    GameObject go = new GameObject("[Screen Shot]");
                    go.hideFlags = HideFlags.HideAndDontSave;
                    _Instance = go.AddComponent<ScreenShot>();
                }
                
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }

        static ScreenShotLibraryConfig library;
        static Dictionary<string,ScreenShotComponent> _components = new Dictionary<string,ScreenShotComponent>();
        static string _fileName = null;

        // Static constructor
        static ScreenShot()
        {
            library = (ScreenShotLibraryConfig)Resources.Load(ScreenShotLibraryConfig.PATH);
        }

        Texture2D _temp;

        internal static void __RegisterScreenShotComponent(ScreenShotComponent component)
        {
            _components.Add(component.id,component);
        }

        public static void Capture(string cameraID, FPSControlPlayerSaveData saveData, System.Action onCaptureComplete)
        {
            Capture(cameraID, BindScreenShotToSave(saveData), onCaptureComplete);
        }

        public static void Capture(string cameraID, string fileName, System.Action onCaptureComplete)
        {
            _components[cameraID].__Capture(onCaptureComplete);
            _fileName = fileName;
        }

        internal static void __Save(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();

            if (string.IsNullOrEmpty(_fileName))
            {
                _fileName = "_unnamedScreenShot_" + Guid.NewGuid().ToString();
                Debug.LogWarning("Screen Shot file has no name. Assigning unique identifier: " + _fileName);
            }

            // create the directory if it doesn't already
            if (!Directory.Exists(PATH)) Directory.CreateDirectory(PATH);

            string filePath = PATH + "/" + _fileName + ".png";
            File.WriteAllBytes(filePath, bytes);
            PersistentData.Write<ScreenShotSaveData>(PersistentData.NS_SCREENSHOTS, _fileName, new ScreenShotSaveData(filePath));
        }

        /// <summary>
        /// Generates a unique file ID based on the save data, and returns the screen shot's name as it will appear in the file system.
        /// Note: This does not save the Player Data persistently.
        /// </summary>
        /// <param name="saveData">The player save object</param>
        /// <returns>the screen shot's unique file name (without the .png)</returns>
        public static string BindScreenShotToSave(FPSControlPlayerSaveData saveData)
        {
            string fileName = saveData.guid + "_" + Guid.NewGuid().ToString("N");
            saveData.screenshotID = fileName;
            return fileName;
        }

        public static void Load(string name, System.Action<Texture2D> onLoadComplete)
        {
            // If we have a pre-serialized library, just load it from Resources, otherwise get it from PersistentData
            if (library)
            {
                Texture2D tmp = (Texture2D) Resources.Load("ScreenShots/"+name);
                onLoadComplete(tmp);
            }
            else
            {
                Instance.StartCoroutine(Instance._Load(name, onLoadComplete));
            }
        }

        IEnumerator _Load(string name, System.Action<Texture2D> onLoadComplete)
        {
            Texture2D texture;
            WWW www = new WWW("file://" + PATH + "/" + name + ".png");

            yield return www;

            if (!www.texture || !string.IsNullOrEmpty(www.error))
            {
                Debug.LogWarning("Texture is missing or encountered an error: " + www.error);
                texture = null;
            }
            else
            {
                texture = www.texture;
            }

            if (onLoadComplete == null)
            {
                Debug.LogWarning("onLoadComplete callback is null");
            }
            else
            {
                onLoadComplete(texture);
            }
        }
    }
}
