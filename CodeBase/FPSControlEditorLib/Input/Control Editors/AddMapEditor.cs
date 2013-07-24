using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControl.Controls;

//---
//
//---
namespace FPSControlEditor.Controls
{
    class AddMapEditor<TGroup,TMap> : ControlEditor<TGroup>, IControlEditor where TMap : ControlMap,new() where TGroup : ControlMapGroup<TMap>
    {
        TGroup _group;
        Texture _bg;

        string _key = "";
        bool _makeDefault = false;

        public AddMapEditor(TGroup group, System.Action onClose, System.Action onApply) : base(onClose, onApply)
        {
            title = "Add Control Map";
            _group = group;
        }

        public void Open()
        {
            _bg = FPSControlEditor.Utils.AssetLoader.LoadPNG("Weapon Control", "window_placeholder"); 
        }

        public void Close()
        {
            _OnClose();
            _OnClose = null;
            _OnApply = null;
        }

        public void Apply()
        {
            _OnApply();
        }

        public void Draw()
        {
            Color c = GUI.contentColor;

            GUI.contentColor = Color.black;
            GUI.Label(position, _bg);
            GUI.contentColor = c;

            GUILayout.BeginArea(position);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(title, EditorStyles.whiteLargeLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close")) Close();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Space(5);

            GUILayout.Label("Name");
            _key = GUILayout.TextField(_key);
            _makeDefault = GUILayout.Toggle(_makeDefault, "Make Default For Platform?");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = !_group.ContainsKey(_key);
            if (GUILayout.Button("Create Map"))
            {
                TMap map = ControlMap.Create<TMap>(_key);
                _group.Add(_key, map);
                if (_makeDefault) _group.MarkDefault(_key);
                Apply();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndArea();
        }

        protected override void MapKey(KeyCode key)
        {
            throw new System.NotImplementedException();
        }

        protected override void WaitForKeyInput(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}
