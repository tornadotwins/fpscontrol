using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControl.Controls;

namespace FPSControlEditor.Controls
{
    class OuyaButtonEditor : ControlEditor<OuyaButton>, IControlEditor
    {
        Texture _bg;

        public OuyaButtonEditor(OuyaButton t, string title, System.Action onClose, System.Action onApply) : base(onClose, onApply)
        {
            this.title = title;
            target = t;
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

            string[] buttonNames = OuyaButtons.DisplayToArray();
            int[] vals = new int[buttonNames.Length];
            for (int i = 0; i < vals.Length; i++) vals[i] = i; 
                
            GUILayout.BeginHorizontal();
            GUILayout.Label("Left",EditorStyles.boldLabel);
            target.button = EditorGUILayout.IntPopup(target.button, buttonNames, vals);
            GUILayout.EndHorizontal();

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