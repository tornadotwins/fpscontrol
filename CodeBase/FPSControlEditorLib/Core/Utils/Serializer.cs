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

        public static T LoadData<T>(string key)
        {            
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(EditorPrefs.GetString(key));
            return (T)serializer.Deserialize(sr);
        }

        public static void SaveData<T>(string key, T source, bool save = true)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, source);
            EditorPrefs.SetString(key, sw.ToString());
            if (save) PlayerPrefs.Save();
        }

    }
}
