﻿using System;
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

        static string PATH { get {
            //Debug.Log(Application.persistentDataPath);
            return Application.persistentDataPath; } }

        /// <summary>
        /// Writes the data to a .txt file in Unity's default <see cref="Application.persistentDataPath"/>
        /// </summary>
        /// <typeparam name="T">The generic type class</typeparam>
        /// <param name="nameSpace">Serves as the .txt file's name</param>
        /// <param name="identifier">Serves as the unique identifier for a specific instance of data in the given namespace</param>
        /// <param name="obj">The object that will be serialized.</param>
        /// <param name="append">Whether or not the data should append to the end of the .txt file. By default this is false, as it should be in most general cases.
        /// NOTE: Except in small edge cases, this parameter should always be false. Injecting true may lead to confusion and conflicting identifiers!
        /// </param>
        public static void Write<T>(string nameSpace, string identifier, T obj, bool append = false)
        {
            string path = PATH + "/" + nameSpace + ".txt";
            //Debug.Log("Writing to: " + path);

            PersistentDataNameSpace<T> loadedNameSpace = new PersistentDataNameSpace<T>();
            //Debug.Log("File check...");
            if (File.Exists(path) && append)
                loadedNameSpace = ReadAll<T>(nameSpace);

            //Debug.Log("Identifier check...");
            int indexOf = (loadedNameSpace == null) ? -1 : loadedNameSpace.IndexOf(identifier);

            //Debug.Log("Creating Container...");
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

            //Debug.Log("JSON Conversion...");
            // Convert to JSON
            string json = JsonMapper.ToJson(loadedNameSpace);
            //Debug.Log("Created JSON: \n" + json);
            // Write to File
            StreamWriter sw = new StreamWriter(path,false,Encoding.UTF8);
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
            return File.Exists(PATH + "/" + nameSpace + ".txt");
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
                
                PersistentDataNameSpace<T> loadedNameSpace = ReadAll<T>(nameSpace);
                return loadedNameSpace.Contains(identifier);
            }
            return false;
        }

        static PersistentDataNameSpace<T> ReadAll<T>(string nameSpace)
        {
            StreamReader sr = new StreamReader(PATH + "/" + nameSpace + ".txt");
            string json = sr.ReadToEnd();
            sr.Close();
            return JsonMapper.ToObject<PersistentDataNameSpace<T>>(json);
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

            PersistentDataNameSpace<T> loadedNameSpace = ReadAll<T>(nameSpace);
            return (T)(loadedNameSpace.GetData(identifier) as PersistentDataContainer<T>).data;
        }
    }

    public class PersistentDataNameSpace<T> : object, IEnumerable
    {
        public PersistentDataContainer<T>[] content;
        public PersistentDataNameSpace() { }

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
            if (content.Length == 0) return new T[0] { };

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
    }

    public class PersistentDataContainer<T> : object
    {
        public string identifier;
        public T data;
        
        public PersistentDataContainer() : base(){}
    }
}
