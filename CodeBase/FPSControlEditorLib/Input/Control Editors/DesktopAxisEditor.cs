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
    class DesktopAxisEditor : ControlEditor<DesktopAxis>, IControlEditor
    {
        const string NEGATIVE_X = "A";
        const string POSITIVE_X = "D";
        const string NEGATIVE_Y = "S";
        const string POSITIVE_Y = "W";

        Texture _bg;
        bool _waitingForInput = false;

        string _focusedKey = "";

        public DesktopAxisEditor(DesktopAxis t, string title, System.Action onClose, System.Action onApply) : base(onClose,onApply)
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

            if (_waitingForInput)
            {

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Press any key on your keyboard.");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                KeyCode k;
                if(DetectInput(out k)) MapKey(k);
                GUILayout.EndArea();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Space(5);

            target.type = (AxisType) EditorGUILayout.Popup((int)target.type, new string[2] { "Use Mouse", "Use Keyboard" });

            if (target.type == AxisType.Analogue)
            {
                GUILayout.Label("X Axis: " + DesktopAxis.MOUSE_X);
                GUILayout.Label("Y Axis: " + DesktopAxis.MOUSE_Y);
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X (-)",EditorStyles.boldLabel);
                target.negativeX = (KeyCode) EditorGUILayout.EnumPopup(target.negativeX);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Map By Input")) WaitForKeyInput(NEGATIVE_X);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("X (+)", EditorStyles.boldLabel);
                target.positiveX = (KeyCode)EditorGUILayout.EnumPopup(target.positiveX);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Map By Input")) WaitForKeyInput(POSITIVE_X);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y (-)", EditorStyles.boldLabel);
                target.negativeY = (KeyCode)EditorGUILayout.EnumPopup(target.negativeY);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Map By Input")) WaitForKeyInput(NEGATIVE_Y);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y (+)", EditorStyles.boldLabel);
                target.positiveY = (KeyCode)EditorGUILayout.EnumPopup(target.positiveY);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Map By Input")) WaitForKeyInput(POSITIVE_Y);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndArea();
        }

        override protected void WaitForKeyInput(string key)
        {
            _waitingForInput = true;
            _focusedKey = key;
        }

        override protected void MapKey(KeyCode key)
        {
            _waitingForInput = false;
            switch (_focusedKey)
            {
                case NEGATIVE_X: target.negativeX = key; break;
                case POSITIVE_X: target.positiveX = key; break;
                case NEGATIVE_Y: target.negativeY = key; break;
                case POSITIVE_Y: target.positiveY = key; break;
            }
            _focusedKey = "";
        }
    }
}
