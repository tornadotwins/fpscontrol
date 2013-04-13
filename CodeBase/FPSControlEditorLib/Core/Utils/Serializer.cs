using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;

namespace FPSControlEditor
{
    public class Serializer
    {

        public static T LoadData<T>(string key, bool encrypted = false)
        {            
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string dataString = EditorPrefs.GetString(key);
            if (encrypted)
            {
                dataString = Crypto.DecryptStringAES(dataString, "sdfiudsfsn939nm");
            }
            StringReader sr = new StringReader(dataString);
            return (T)serializer.Deserialize(sr);
        }

        public static void SaveData<T>(string key, T source, bool save = true, bool encrypt = false)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, source);
            string dataString = sw.ToString();
            if (encrypt)
            {
                dataString = Crypto.EncryptStringAES(dataString, "sdfiudsfsn939nm");
            }
            EditorPrefs.SetString(key, dataString);
            if (save) PlayerPrefs.Save();
        }

    }
}
