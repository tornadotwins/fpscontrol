﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPSControl;
using FPSControl.Definitions;

namespace FPSControlEditor
{

    class BuildControlModule : FPSControlEditorModule
    {

        const string guiFolder = "BuildControl/";

        private bool paidVersion = false;

        #region GUI Properties
        private Rect backgroundRect = new Rect(247, 50, 660, 580);
        private Rect[] infoRects = new Rect[3]{
            new Rect(258, 105, 206, 31),
            new Rect(258, 143, 206, 31),
            new Rect(258, 182, 206, 31)
        };
        #endregion

        #region GUI
        private int currentOver = 0;
        public override void OnGUI()
        {
            GUI.DrawTexture(backgroundRect, gui_background);
            if (!paidVersion) ShowAd();
            Event evt = Event.current;
            for(int i = 0; i < infoRects.Length; i++)
            {
                if (infoRects[i].Contains(evt.mousePosition)) currentOver = i;
                if (GUI.Button(infoRects[i], "", GUIStyle.none)) HandleClick(i);
            }

            GUI.DrawTexture(new Rect(481, 105, 421, 181), info_panels[currentOver]);
        }

        private void HandleClick(int index)
        {
            currentOver = index;
            switch (index)
            {
                case 0:
                    Debug.Log("Launching Build Control");
                    break;
                case 1:
                    Debug.Log("Launching Grid Control");
                    break;
                case 2:
                    Debug.Log("Launching Decal Control");
                    break;
            }
        }

        private void ShowAd()
        {
            if (GUI.Button(new Rect(247, 400, 660, 249), button_ad, GUIStyle.none))
            {
                PurchasePro();
            }
        }
        #endregion

        #region Overrides
        public override void Init()
        {
            base.Init();
            LoadAssets();
        }

        override public void OnFocus(bool rebuild)
        {
            //Stuff Here
            base.OnFocus(rebuild);
        }

        override public void OnLostFocus(bool rebuild)
        {
            //Stuff Here
            base.OnLostFocus(rebuild);
        }

        public override void Deinit()
        {
            base.Deinit();
        }

        public override void Update()
        {

        }
        #endregion

        #region Logic
        public BuildControlModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.BuildControl;
        }

        private void PurchasePro()
        {
            Debug.Log("Purchasing Pro Version");
        }
        #endregion

        #region Asset Handling
        Texture gui_background;
        Texture button_ad;
        Texture[] info_panels;

        private void LoadAssets()
        {
            gui_background = LoadPNG(guiFolder, "background");
            button_ad = LoadPNG(guiFolder, "button_ad");
            info_panels = new Texture[3];
            info_panels[0] = LoadPNG(guiFolder, "info_buildcontrol");
            info_panels[1] = LoadPNG(guiFolder, "info_grid");
            info_panels[2] = LoadPNG(guiFolder, "info_decal");
        }
        #endregion

    }

}
