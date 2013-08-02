using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControl.Controls;

namespace FPSControlEditor.Controls
{
    class OuyaAxisEditor : ControlEditor<OuyaAxis>, IControlEditor
    {
        Texture _bg;

        public OuyaAxisEditor(OuyaAxis t, string title, System.Action onClose, System.Action onApply) : base(onClose, onApply)
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

            target.type = (AxisType) EditorGUILayout.Popup((int)target.type, new string[2] { "Use Analogue Sticks", "Use Buttons" });

            if (target.type == AxisType.Analogue)
            {
                target.xAxis = EditorGUILayout.IntPopup("X Axis:", target.xAxis, OuyaSticks.DisplayToArray(), new int[4] { 0, 1, 2, 3 });
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Deadzone X:");
                target.deadZoneX = GUILayout.HorizontalSlider(target.deadZoneX,0F,1F);
                GUILayout.Space(10);
                target.deadZoneX = EditorGUILayout.FloatField(target.deadZoneX,GUILayout.Width(60));
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                target.yAxis = EditorGUILayout.IntPopup("Y Axis:", target.yAxis, OuyaSticks.DisplayToArray(), new int[4] { 0, 1, 2, 3 });
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Deadzone Y:");
                target.deadZoneY = GUILayout.HorizontalSlider(target.deadZoneY, 0F, 1F);
                GUILayout.Space(10);
                target.deadZoneY = EditorGUILayout.FloatField(target.deadZoneY, GUILayout.Width(60));
                GUILayout.EndHorizontal();
            }
            else
            {
                string[] buttonNames = OuyaButtons.DisplayToArray();
                int[] vals = new int[buttonNames.Length];
                for (int i = 0; i < vals.Length; i++) vals[i] = i;
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Left",EditorStyles.boldLabel);
                target.leftButton = EditorGUILayout.IntPopup(target.leftButton, buttonNames, vals);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Right", EditorStyles.boldLabel);
                target.rightButton = EditorGUILayout.IntPopup(target.rightButton, buttonNames, vals);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Down", EditorStyles.boldLabel);
                target.downButton = EditorGUILayout.IntPopup(target.downButton, buttonNames, vals);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Up", EditorStyles.boldLabel);
                target.upButton = EditorGUILayout.IntPopup(target.upButton, buttonNames, vals);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndArea();
        }

        override protected void WaitForKeyInput(string key) { }
        override protected void MapKey(KeyCode key) { }
    }
}