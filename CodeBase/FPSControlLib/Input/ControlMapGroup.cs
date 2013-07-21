using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControl;

namespace FPSControl
{
    public interface IControlMapGroup
    {
        string[] GetKeys();
        string DefaultKey { get; }
        void MarkDefault(string key);
        void Remove(string key);
        int Count { get; }
    }
    
    [System.Serializable]
    public class DesktopControlMapGroup : ControlMapGroup<DesktopControlMap> { }
    [System.Serializable]
    public class OuyaControlMapGroup : ControlMapGroup<OuyaControlMap> { }
    [System.Serializable]
    public class MobileControlMapGroup : ControlMapGroup<MobileControlMap> { }
    [System.Serializable]
    public class SteamboxControlMapGroup : ControlMapGroup<SteamboxControlMap> { }

    [System.Serializable]
    public abstract class ControlMapGroup : object
    {
    }

    [System.Serializable]
    public abstract class ControlMapGroup<T> : ControlMapGroup, IControlMapGroup, IEnumerable where T : ControlMap
    {
        [SerializeField] string _defaultKey = "";
        public string DefaultKey { get { return _defaultKey; } }

        T Default { get { return this[_defaultKey]; } }
        
        [SerializeField]
        List<string> _keys = new List<string>();
        [SerializeField]
        List<T> _values = new List<T>();

        public string[] GetKeys() { return _keys.ToArray(); }

        public void MarkDefault(string key)
        {
            _defaultKey = key;
        }

        public bool ContainsKey(string key)
        {
            return _keys.Contains(key);
        }

        public bool ContainsValue(T value)
        {
            return _values.Contains(value);
        }

        public void Add(string key, T value)
        {
            if (_keys.Count == 0) _defaultKey = key;
            if (ContainsKey(key)) throw new System.Exception("Already contains key \"" + key + "\"");
            _keys.Add(key);
            _values.Add(value);
        }

        public void Remove(string key)
        {
            int i = IndexOf(key);
            _keys.RemoveAt(i);
            _values.RemoveAt(i);
            if (_defaultKey == key) _defaultKey = _keys[0];
        }

        public void Remove(T map)
        {
            int i = IndexOf(map);
            string tmpKey = _keys[i];
            _keys.RemoveAt(i);
            if (_defaultKey == tmpKey) _defaultKey = _keys[0];
            _values.RemoveAt(i);
            
        }

        int IndexOf(string key)
        {
            return _keys.IndexOf(key);
        }

        int IndexOf(T value)
        {
            return _values.IndexOf(value);
        }

        public T this[string key]
        { 
            get
            {
                if (key == "") return Default;
                return _values[IndexOf(key)];
            }
        }
        public string this[T val] { get { return _keys[IndexOf(val)]; } }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _values.Count; i++)
            {
                yield return _values[i];
            }
        }

        public int Count { get { return _values.Count; } }
    }
}
