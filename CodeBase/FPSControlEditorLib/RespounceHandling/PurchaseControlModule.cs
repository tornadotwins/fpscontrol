using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    class PurchaseControlModule : FPSControlEditorModule
    {

        public PurchaseControlModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.NeedsPurchasing;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnGUI()
        {
            GUIStyle alignment = new GUIStyle();
            alignment.alignment = TextAnchor.MiddleCenter;
            alignment.normal.textColor = Color.white;

            GUILayout.BeginArea(MODULE_SIZE);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("NEEDS PURCHING", alignment, NONE);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Purchase", new GUILayoutOption[1] { GUILayout.Width(250) })) Application.OpenURL(RespounceHandler.purchaseURL);

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndArea();
        }


    }
}
