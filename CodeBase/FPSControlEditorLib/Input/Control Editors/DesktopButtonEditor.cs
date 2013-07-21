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
    public class DesktopPersistantButtonEditor : DesktopButtonEditor<DesktopPersistantButton>
    {
        public DesktopPersistantButtonEditor(DesktopPersistantButton t, string title, System.Action onClose, System.Action onApply) : base(t,title,onClose,onApply)
        {

        }
    }

    public class DesktopButtonEditor<T> : ControlEditor<T>, IControlEditor where T : DesktopButton
    {
        bool _waitingForInput = false;
        Texture _bg;
        
        public DesktopButtonEditor(T t, string title, System.Action onClose, System.Action onApply) : base(onClose, onApply)
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
            GUILayout.Label(title);
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

            target.peripheral = (Peripheral)EditorGUILayout.EnumPopup(target.peripheral);

            if (target.peripheral == Peripheral.Mouse)
            {
                target.mouseButton = EditorGUILayout.IntPopup(target.mouseButton, new string[3] { "Left", "Middle", "Right" }, new int[3] { 0, 2, 1 });
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Key", EditorStyles.boldLabel);
                target.key = (KeyCode)EditorGUILayout.EnumPopup(target.key);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Map By Input")) WaitForKeyInput("");
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
        }

        override protected void MapKey(KeyCode key)
        {
            _waitingForInput = false;
            target.key = key;
        }
    }
}
