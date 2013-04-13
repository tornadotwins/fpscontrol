using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using FPSControlEditor;

namespace FPSControlEditor
{
    public class Serializer
    {

        public static T LoadData<T>(string key, bool encrypted, bool usejson)
        {                        
            if (!EditorPrefs.HasKey(key)) return default(T);
            string dataString = EditorPrefs.GetString(key); 
            if (encrypted)
            {
                dataString = Crypto.DecryptStringAES(dataString, "sdfiudsfsn939nm");
            }
            StringReader sr = new StringReader(dataString);
            if (usejson)
            {
                return JSONDeserializer.Get<T>(dataString);
            } 
            else 
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }            
        }

        public static void SaveData<T>(string key, T source, bool save, bool encrypt, bool usejson)
        {
            string dataString;
            if (usejson)
            {
                dataString = JSONDeserializer.Set<T>(source);
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringWriter sw = new StringWriter();
                serializer.Serialize(sw, source);
                dataString = sw.ToString();
            }            
            if (encrypt)
            {
                dataString = Crypto.EncryptStringAES(dataString, "sdfiudsfsn939nm");
            }
            EditorPrefs.SetString(key, dataString);
            if (save) PlayerPrefs.Save();
        }

    }
}
