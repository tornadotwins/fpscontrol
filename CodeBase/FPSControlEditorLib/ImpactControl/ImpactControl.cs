using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPSControl;
using FPSControl.Definitions;

namespace FPSControlEditor
{
    public class ImpactControl : FPSControlEditorModule
    {

        const string guiFolder = "ImpactControl/";

        private bool paidVersion = true;

        #region GUI Properties
        private Rect backgroundRect = new Rect(247, 50, 660, 580);
        private Rect[] infoRects = new Rect[3]{
            new Rect(258, 105, 206, 31),
            new Rect(258, 143, 206, 31),
            new Rect(258, 182, 206, 31)
        };
        #endregion

        #region GUI
        public override void OnGUI()
        {
            GUI.DrawTexture(backgroundRect, gui_background);            
        }
        #endregion

        #region Overrides
        public override void Init()
        {
            base.Init();
            LoadAssets();
        }

        public override string version
        {
            get
            {
                return "1.0.0";
            }
        }

        public override void HandlePaidStatus(bool paid)
        {
            //Debug.Log(paid);
            paidVersion = paid;
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
        public ImpactControl(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.ImpactControl;
        }
        #endregion

        #region Asset Handling
        Texture gui_background;

        private void LoadAssets()
        {
            gui_background = LoadPNG(guiFolder, "background");            
        }
        #endregion
    }
}
