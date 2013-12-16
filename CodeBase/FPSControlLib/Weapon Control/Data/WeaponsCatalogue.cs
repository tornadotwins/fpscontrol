using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSControl.Data
{
    public class WeaponsCatalogue : ScriptableObject, IEnumerable
    {
        public const string PATH = "Assets/FPSControlAssets/Data/WeaponsCatalogue.asset";

        [SerializeField]
        List<string> keys = new List<string>();
        public string[] Keys { get { return keys.ToArray(); } }
        [SerializeField]
        List<GameObject> values = new List<GameObject>();
        public GameObject[] Values { get { return values.ToArray(); } }

        public KeyValuePair<string, GameObject>[] GetCollection()
        {
            List<KeyValuePair<string, GameObject>> list = new List<KeyValuePair<string, GameObject>>();
            foreach (KeyValuePair<string, GameObject> kvp in this)
                list.Add(kvp);
            return list.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                yield return new KeyValuePair<string, GameObject>(keys[i], values[i]);
            }
        }

        public void ChangeKeyOf(GameObject oldValue, GameObject newValue)
        {
            if (!values.Contains(oldValue)) // If we didn't have the old value to begin with...
                throw new Exception("Could not find key matching value.");
            int indexOf = values.IndexOf(oldValue);
            if (values[indexOf] == newValue) return;
            if (keys.Contains(newValue.name) && keys[indexOf] != newValue.name)
                throw new Exception("Key already exists!");

            keys[indexOf] = newValue.name;
        }

        public void Add(string key, GameObject value)
        {
            if (keys.Contains(key)) throw new System.Exception(string.Format("Collection already contains key '{0}'.", key));
            keys.Add(key);
            values.Add(value);
        }

        public GameObject this[string key]
        {
            get
            {
                return values[keys.IndexOf(key)];
            }
            set
            {
                values[keys.IndexOf(key)] = value;
            }
        }

        public int Count { get { return keys.Count; } }

        public bool ContainsKey(string key)
        {
            return keys.Contains(key);
        }

        public bool ContainsValue(GameObject value)
        {
            return values.Contains(value);
        }

        public void RemoveKey(string key)
        {
            int removeAt = keys.IndexOf(key);
            keys.RemoveAt(removeAt);
            values.RemoveAt(removeAt);
        }

        public static WeaponsCatalogue Create()
        {
            return ScriptableObject.CreateInstance<WeaponsCatalogue>();
        }        
    }
}
