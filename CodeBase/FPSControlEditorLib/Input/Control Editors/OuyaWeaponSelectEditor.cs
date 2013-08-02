using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControl.Controls;

namespace FPSControlEditor.Controls
{
    class OuyaWeaponSelectEditor : ControlEditor<OuyaButton>, IControlEditor
    {
        Texture _bg;
        OuyaButton[] _buttons;
        public OuyaWeaponSelectEditor(OuyaButton[] arr, string title, System.Action onClose, System.Action onApply) : base(onClose, onApply)
        {
            this.title = title;
            _buttons = arr;
        }

        public void Open()
        {
            //simple setup?
            _bg = FPSControlEditor.Utils.AssetLoader.LoadPNG("Weapon Control", "window_placeholder");
        }

        public void Close()
        {
            _OnClose(); //call our event callback
        }

        public void Apply()
        {
            _OnApply(); //call our event callback
        }

        public void Draw()//Rect area)
        {
            Color c = GUI.contentColor;

            GUI.contentColor = Color.black;
            GUI.Label(position, _bg);
            GUI.contentColor = c;

            GUILayout.BeginArea(position);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(title,EditorStyles.whiteLargeLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close")) Close();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Space(5);

            string[] buttonNames = OuyaButtons.ToArray();
            int[] vals = new int[buttonNames.Length];
            for (int i = 0; i < vals.Length; i++) vals[i] = i;

            for (int i = 0; i < _buttons.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Weapon " + (i+1), EditorStyles.boldLabel);
                _buttons[i].button = EditorGUILayout.IntPopup(_buttons[i].button, buttonNames, vals);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndArea();
        }

        override protected void WaitForKeyInput(string key){}
        override protected void MapKey(KeyCode key) { }
    }
}