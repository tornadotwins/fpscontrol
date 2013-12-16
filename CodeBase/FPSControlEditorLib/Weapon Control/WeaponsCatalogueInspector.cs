using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using FPSControl.Data;

namespace FPSControlEditor
{
    [CustomEditor(typeof(WeaponsCatalogue))]
    public class WeaponsCatalogueInspector : Editor
    {
        WeaponsCatalogue t { get { return (WeaponsCatalogue)target; } }

        GameObject _toAdd;

        public override void OnInspectorGUI()
        {

            foreach (KeyValuePair<string, GameObject> kvp in t.GetCollection())
            {
                string key = kvp.Key;
                GUILayout.BeginHorizontal();

                GUILayout.Label(kvp.Key);
                GameObject tmp = kvp.Value;
                GameObject newPrefab = (GameObject) EditorGUILayout.ObjectField(kvp.Value, typeof(GameObject),false);
                if (kvp.Value != tmp)
                {
                    try
                    {
                        t.ChangeKeyOf(tmp,newPrefab);
                        t[kvp.Key] = newPrefab;
                        Save();
                    }
                    catch (System.Exception err)
                    {
                        t[kvp.Key] = tmp; // set it back
                        Debug.LogError("Action resulted in caught exception: " + err.Message);
                    }
                }

                if (GUILayout.Button("Remove",GUILayout.Width(80)))
                {
                    t.RemoveKey(kvp.Key);
                    Save();
                    Repaint();
                    break;
                }

                GUILayout.EndHorizontal();

            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            _toAdd = (GameObject)EditorGUILayout.ObjectField(_toAdd, typeof(GameObject), false);
            
            GUI.enabled = _toAdd && !t.ContainsKey(_toAdd.name);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                t.Add(_toAdd.name, _toAdd);
                Save();
                _toAdd = null;
                Repaint();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

        }

        void Save()
        {
            EditorUtility.SetDirty(t);
            AssetDatabase.SaveAssets();
        }

        public static WeaponsCatalogue Create()
        {
            WeaponsCatalogue catalogue = WeaponsCatalogue.Create();
            if(!AssetDatabase.LoadAssetAtPath(WeaponsCatalogue.PATH, typeof(WeaponsCatalogue)))
            {
                AssetDatabase.CreateAsset(catalogue, WeaponsCatalogue.PATH);
            }
            return (WeaponsCatalogue) AssetDatabase.LoadAssetAtPath(WeaponsCatalogue.PATH, typeof(WeaponsCatalogue));
        }

    }
}
