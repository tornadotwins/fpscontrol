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
    public class DesktopWeaponSelectEditor : ControlEditor<DesktopButton>, IControlEditor
    {
        DesktopButton[] _buttons;
        bool _waitingForInput = false;
        Texture _bg;
        int _focusedKey = -1;

        public DesktopWeaponSelectEditor(DesktopButton[] buttons, string title, System.Action onClose, System.Action onApply) : base(onClose, onApply)
        {
            if (buttons.Length != 4) throw new System.Exception("Incorrect amount.");
            this.title = title;
            _buttons = buttons;
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
            GUILayout.Label(title, EditorStyles.whiteLargeLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close")) Close();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (_waitingForInput)
            {

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Press any key on your keyboard.");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                KeyCode k;
                if (DetectInput(out k)) MapKey(k);
                GUILayout.EndArea();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Space(5);

            for(int i = 0; i < _buttons.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Weapon "+(i+1), EditorStyles.boldLabel);
                _buttons[i].key = (KeyCode)EditorGUILayout.EnumPopup(_buttons[i].key);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Map By Input")) WaitForKeyInput(i);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndArea();
        }

        override protected void WaitForKeyInput(string s) { }
        protected void WaitForKeyInput(int i)
        {
            _waitingForInput = true;
            _focusedKey = i;
        }

        override protected void MapKey(KeyCode key)
        {
            _waitingForInput = false;
            _buttons[_focusedKey].key = key;
            _focusedKey = -1;
        }


    }
}
