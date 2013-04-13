using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    class UnavailableModule : FPSControlEditorModule
    {
         
        string message = "The Editor you are trying to access is currently in development. \n Check our Development Roadmap to see when this Editor will be available.";
        internal Texture bg;

        public UnavailableModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.UNAVAILABLE;
        }

        public override void Init()
        {
            bg = null;
            base.Init();
        }

        public override void OnGUI()
        {
            if (bg) GUI.Box(MODULE_SIZE, bg, GUIStyle.none);
            
            GUIStyle alignment = new GUIStyle();
            alignment.alignment = TextAnchor.MiddleCenter;
            alignment.normal.textColor = Color.white;
            
            GUILayout.BeginArea(MODULE_SIZE);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(message, alignment, NONE);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("View Development Roadmap", new GUILayoutOption[1] { GUILayout.Width(250) })) Application.OpenURL("http://www.fpscontrol.com/upgrade");

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndArea();
        }

        internal void SetBG(string path)
        {
            bg = Load<Texture>(path);
        }
    }
}
