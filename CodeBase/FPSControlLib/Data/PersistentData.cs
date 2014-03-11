using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine;
using FPSControl;
using System.IO;

namespace FPSControl.Data
{
    public class PersistentData
    {
        public const string NS_WEAPONS = "Weapons";
        public const string NS_PLAYER = "PlayerData";
        public const string NS_SCREENSHOTS = "ScreenShots";

        public static string PATH { get { return Application.persistentDataPath; } }
        internal static string _BuildPath(string nameSpace)
        {
            return PATH + "/" + nameSpace + ".txt";
        }

        /// <summary>
        /// Deletes the data at namespace.
        /// </summary>
        public static void DeleteData(string nameSpace)
        {
            File.Delete(_BuildPath(nameSpace));
        }

        /// <summary>
        /// Writes the data to a .txt file in Unity's default <see cref="Application.persistentDataPath"/>
        /// </summary>
        /// <typeparam name="T">The generic type class</typeparam>
        /// <param name="nameSpace">Serves as the .txt file's name</param>
        /// <param name="identifier">Serves as the unique identifier for a specific instance of data in the given namespace</param>
        /// <param name="obj">The object that will be serialized.</param>
        /// <param name="append">Whether or not the data should append to the collective namespace. By default this is true, as it should be in most general cases.
        /// NOTE: Except in small edge cases, this parameter should always be true. 
        /// A value of FALSE would then override all currently saved data in the namespace, where TRUE will only override the matching identifier if it already exists.
        /// </param>
        public static void Write<T>(string nameSpace, string identifier, T obj, bool append = true)
        {
            string path = _BuildPath(nameSpace);
            //Debug.Log("Writing to: " + path);

            PersistentDataNameSpace<T> loadedNameSpace = new PersistentDataNameSpace<T>();
            //Debug.Log("File check...");
            if (File.Exists(path) && append)
                loadedNameSpace = _ReadAll<T>(nameSpace);

            //Debug.Log("Identifier check...");
            int indexOf = (loadedNameSpace == null) ? -1 : loadedNameSpace.IndexOf(identifier);

            Debug.Log("Creating Container..." + indexOf);
            PersistentDataContainer<T> container = new PersistentDataContainer<T>();
            container.identifier = identifier;
            container.data = obj;

            //Debug.Log("Updating Namespace Data...");
            // If the identifier already exists in the namespace, we need to replace the data
            // but if not, we'll just push it to the end
            if (indexOf == -1)
                loadedNameSpace.Add(container);
            else
                loadedNameSpace.content[indexOf] = container;

            _WriteNameSpace<T>(path, loadedNameSpace);
        }

        internal static void _WriteNameSpace<T>(string path, PersistentDataNameSpace<T> nameSpaceData)
        {
            //Debug.Log("JSON Conversion...");
            // Convert to JSON
            string json = JsonMapper.ToJson(nameSpaceData);
            //Debug.Log("Created JSON: \n" + json);
            // Write to File
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            sw.Write(json);
            sw.Close();
        }

        /// <summary>
        /// Does a file with the specified name space exist?
        /// </summary>
        /// <param name="nameSpace">The nameSpace</param>
        /// <returns>TRUE if the file exists, FALSE if not.</returns>
        public static bool NameSpaceExists(string nameSpace)
        {
            return File.Exists(_BuildPath(nameSpace));
        }

        /// <summary>
        /// Returns whether data exists in the provided namespace with the provided identifier. (type-safe)
        /// </summary>
        /// <typeparam name="T">The generic type. The data at the provided namespace and identifier MUST be of this type.</typeparam>
        /// <param name="nameSpace">the namespace the data resides within</param>
        /// <param name="identifier">the unique identifier of this data</param>
        /// <returns>TRUE if data exists, otherwise FALSE</returns>
        public static bool Exists<T>(string nameSpace, string identifier)
        {
            if (NameSpaceExists(nameSpace))
            {
                
                PersistentDataNameSpace<T> loadedNameSpace = _ReadAll<T>(nameSpace);
                return loadedNameSpace.Contains(identifier);
            }
            return false;
        }

        internal static PersistentDataNameSpace<T> _ReadAll<T>(string nameSpace)
        {
            Debug.Log("Reading all from: " + nameSpace);
            if (!NameSpaceExists(nameSpace))
            {
                Debug.Log("Namespace " + nameSpace + " does not exist.");
                return new PersistentDataNameSpace<T>();
            }
            
            StreamReader sr = new StreamReader(PATH + "/" + nameSpace + ".txt");
            string json = sr.ReadToEnd();
            sr.Close();
            return JsonMapper.ToObject<PersistentDataNameSpace<T>>(json);
        }

        public static T[] ReadAll<T>(string nameSpace)
        {
            PersistentDataNameSpace<T> loadedData = _ReadAll<T>(nameSpace);
            return loadedData.GetAll();
        }

        /// <summary>
        /// Gets the persistent data.
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="nameSpace">the namespace the data resides within</param>
        /// <param name="identifier">the unique identifier of this data</param>
        /// <returns>The data at nameSpace.identifier</returns>
        public static T Read<T>(string nameSpace, string identifier)
        {
            string path = PATH + "/" + nameSpace + ".txt";
            if (!File.Exists(path))
            {
                Debug.LogWarning(string.Format("Could not read, because file '{0}' does not exist.", path));
                return default(T);
            }

            PersistentDataNameSpace<T> loadedNameSpace = _ReadAll<T>(nameSpace);
            return (T)(loadedNameSpace.GetData(identifier) as PersistentDataContainer<T>).data;
        }
    }

    public class PersistentDataNameSpace<T> : object, IEnumerable
    {
        public PersistentDataContainer<T>[] content;
        public PersistentDataNameSpace() { }

        public static implicit operator bool(PersistentDataNameSpace<T> nameSpace) { return nameSpace != null; }

        public PersistentDataContainer<T> GetData(int i)
        {
            return content[i];
        }
        public PersistentDataContainer<T> GetData(string identifier)
        {
            if (content == null || content.Length == 0) return null;
            for (int i = 0; i < content.Length; i++)
                if (content[i].identifier == identifier) return (PersistentDataContainer<T>) content[i];
            return null;
        }

        public T[] GetAll()
        {
            if (content == null || content.Length == 0) return new T[0] { };

            T[] array = new T[content.Length];
            for (int i = 0; i < content.Length; i++)
                array[i] = content[i].data;
            return array;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < content.Length; i++)
                yield return content[i].data;
        }

        public bool Contains(string identifier)
        {
            if (content == null || content.Length == 0) return false;
            for (int i = 0; i < content.Length; i++)
                if (content[i].identifier == identifier) return true;
            return false;
        }

        public int IndexOf(string identifier)
        {
            if (content == null || content.Length == 0) return -1;
            for (int i = 0; i < content.Length; i++)
                if (content[i].identifier == identifier) return i;
            return -1;
        }

        public void Add(PersistentDataContainer<T> container)
        {
            if (content == null) content = new PersistentDataContainer<T>[0] { };
            List<PersistentDataContainer<T>> list = new List<PersistentDataContainer<T>>(content);
            list.Add(container);
            content = list.ToArray();
        }

        public void Remove(PersistentDataContainer<T> container)
        {
            List<PersistentDataContainer<T>> list = new List<PersistentDataContainer<T>>(content);
            list.Remove(container);
            content = list.ToArray();
        }

        public void RemoveAt(int index)
        {
            List<PersistentDataContainer<T>> list = new List<PersistentDataContainer<T>>(content);
            list.RemoveAt(index);
            content = list.ToArray();
        }
    }

    public class PersistentDataContainer<T> : object
    {
        public string identifier;
        public T data;
        
        public PersistentDataContainer() : base(){}
        public static implicit operator bool(PersistentDataContainer<T> container) { return container != null; }
    }
}
