//#define DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    public sealed class GameSettingsModule : FPSControlEditorModule
    {

        #region Keys
        internal const string EDITED_DURING_PLAY = "_FPSControl_GameSettingsEditedDuringPlay";
        #endregion // Keys

        #region Assets

        //Path

        const string PATH = "Game Settings/";

        //GFX
        Texture title;
        Texture movementKeyMappingBG;
        Texture mouseSettingsBG;
        Texture precisionBG;

        //Text Assets
        TextAsset templateFile;

        #endregion // Assets

        #region GUI Properties

        Rect titleRect = new Rect(259, 67, 157, 26);
        Rect keyMapRect = new Rect(249, 103, 330, 184);
        Rect mouseSettingsRect = new Rect(579, 103, 330, 184);

        Rect saveButtonRect = new Rect(803, 62, 98, 25);

        int dropDownX = 426;
        Vector2 dropDownSize = new Vector2(140, 15);

        #endregion // GUI Properties

        #region Editor Properties

        public RBFPSControllerLogic target = null;

        //Key Binding
        public KeyCode runLeft = KeyCode.A;
        public KeyCode runRight = KeyCode.D;
        public KeyCode interact = KeyCode.E;
        public KeyCode reload = KeyCode.Q;
        public KeyCode escape = KeyCode.Escape;

        //Mouse Settings
        public float sensitivityX = 1F;
        public float sensitivityY = 1F;
        public float mouseFilterBufferSize = .1F;
        public float gunLookDownOffsetThreshold = 0F;
        public float minimumX = 0F;
        public float minimumY = 0F;
        public float maximumX = 0F;
        public float maximumY = 0F;

        
        bool drawPrecisionBox = false;
        Rect precisionBox = new Rect();
        float precisionValue = 0F;
        float[] allVals = new float[8];

        internal static bool didSaveDuringPlaymode
        {
            get
            {
                return EditorPrefs.GetBool(EDITED_DURING_PLAY,false);
            }
            set
            {
                EditorPrefs.SetBool(EDITED_DURING_PLAY, value);
            }
        }

        #endregion

        #region String Replacement

        string sRunLeft = "$RUN_LEFT";
        string sRunRight = "$RUN_RIGHT";
        string sInteract = "$INTERACT";
        string sReload = "$RELOAD";
        string sEscape = "$ESCAPE";

        string sSensitivityX = "$SENSITIVITY_X";
        string sSensitivityY = "$SENSITIVITY_Y";
        string sMouseFilterBufferSize = "$MOUSE_FILTER_BUFFER";
        string sGunLookDownOffsetThreshold = "$GUN_LOOK_DOWN_OFFSET";
        string sMinimumX = "$MINIMUM_X";
        string sMinimumY = "$MINIMUM_Y";
        string sMaximumX = "$MAXIMUM_X";
        string sMaximumY = "$MAXIMUM_Y";

        #endregion // String Replacement

        public GameSettingsModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.GameSettings;
        }

        #region Persistant Data Storage 
        /*
        void PushSettings()
        {
            EditorPrefs.SetInt("_FPSControl_GameSettings_runLeft", (int)runLeft);
            EditorPrefs.SetInt("_FPSControl_GameSettings_runRight", (int)runRight);
            EditorPrefs.SetInt("_FPSControl_GameSettings_interact", (int)interact);
            EditorPrefs.SetInt("_FPSControl_GameSettings_reload", (int)reload);
            EditorPrefs.SetInt("_FPSControl_GameSettings_escape", (int)escape);

            EditorPrefs.SetFloat("_FPSControl_GameSettings_sensitivityX", sensitivityX);
            EditorPrefs.SetFloat("_FPSControl_GameSettings_sensitivityX", sensitivityY);
            EditorPrefs.SetFloat("_FPSControl_GameSettings_mouseFilterBufferSize", mouseFilterBufferSize);
            EditorPrefs.SetFloat("_FPSControl_GameSettings_gunLookDownOffsetThreshold", gunLookDownOffsetThreshold);
            EditorPrefs.SetFloat("_FPSControl_GameSettings_minimumX", minimumX);
            EditorPrefs.SetFloat("_FPSControl_GameSettings_minimumY", minimumY);
            EditorPrefs.SetFloat("_FPSControl_GameSettings_maximumX", maximumX);
            EditorPrefs.SetFloat("_FPSControl_GameSettings_maximumY", maximumY);
        }

        void PullSettings()
        {
            if(!EditorPrefs.HasKey("_FPSControl_GameSettings_runLeft")) PushSettings();

            runLeft = (KeyCode)EditorPrefs.GetInt("_FPSControl_GameSettings_runLeft");
            runRight = (KeyCode)EditorPrefs.GetInt("_FPSControl_GameSettings_runRight");
            interact = (KeyCode)EditorPrefs.GetInt("_FPSControl_GameSettings_interact");
            reload = (KeyCode)EditorPrefs.GetInt("_FPSControl_GameSettings_reload");
            escape = (KeyCode)EditorPrefs.GetInt("_FPSControl_GameSettings_escape");

            sensitivityX = EditorPrefs.GetFloat("_FPSControl_GameSettings_sensitivityX");
            sensitivityY = EditorPrefs.GetFloat("_FPSControl_GameSettings_sensitivityX");
            mouseFilterBufferSize = EditorPrefs.GetFloat("_FPSControl_GameSettings_mouseFilterBufferSize");
            gunLookDownOffsetThreshold = EditorPrefs.GetFloat("_FPSControl_GameSettings_gunLookDownOffsetThreshold");
            minimumX = EditorPrefs.GetFloat("_FPSControl_GameSettings_minimumX");
            minimumY = EditorPrefs.GetFloat("_FPSControl_GameSettings_minimumY");
            maximumX = EditorPrefs.GetFloat("_FPSControl_GameSettings_maximumX");
            maximumY = EditorPrefs.GetFloat("_FPSControl_GameSettings_maximumY");
        }
         */
        #endregion // Persistant Data Storage

        public override void Init()
        {

            AcquireTarget();
            
            title = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "title.png");
            precisionBG = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "precision_box_bg.png");
            movementKeyMappingBG = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "key_mapping_bg.png");
            mouseSettingsBG = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "mouse_settings_bg.png");

            templateFile = Load<TextAsset>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEXT_ASEETS + "GameSettings_Template.txt");

            base.Init();

            //PullSettings();
           // Knobs.OnInteractionBegan = OnPress;
            //Knobs.OnInteract = OnDrag;
            //Knobs.OnInteractionEnd = OnRelease;
        }

        void AcquireTarget()
        {
            RBFPSControllerLogic[] scripts = (RBFPSControllerLogic[])Resources.FindObjectsOfTypeAll(typeof(RBFPSControllerLogic));

            if (scripts.Length > 0)
            {
                target = scripts[0];
            }
            else
            {
                if (EditorUtility.DisplayDialog("Component not found!", "There is no Controller Logic in this scene. Create one?", "OK", "Cancel"))
                {
                    GameObject go = new GameObject("Player");
                    target = (RBFPSControllerLogic)go.AddComponent(typeof(RBFPSControllerLogic));
                }
                else
                {
                    (_editor as FPSControlMainEditor).LoadModule(FPSControlModuleType.NONE);
                }
            }

            //collect values and store them here.
            if (target)
            {
                runLeft = target._runKeyLeft;
                runRight = target._runKeyRight;
                interact = target._interactionKey;
                reload = target._reloadKey;
                escape = target._escapeKey;

                sensitivityX = target._sensitivityX;
                sensitivityY = target._sensitivityY;
                mouseFilterBufferSize = target._mouseFilterBufferSize;
                gunLookDownOffsetThreshold = target._gunLookDownOffsetThreshold;
                minimumX = target._minimumX;
                minimumY = target._minimumY;
                maximumX = target._maximumX;
                maximumY = target._maximumY;
            }
        }

        //void OnPress(Vector2 mPos)
        //{
        //    drawPrecisionBox = true;
        //}

        //void OnRelease(Vector2 mPos)
        //{
        //    drawPrecisionBox = false;
        //}

        //void OnDrag(Vector2 mPos)
        //{
        //    if (!drawPrecisionBox)
        //    {
        //        _editor.Repaint();
        //        return;
        //    }

        //    precisionBox = new Rect(mPos.x - 18, mPos.y + 45, 80, 25);
        //    precisionValue = Knobs.interactValue;
        //}

        public override void Update()
        {
            if(didSaveDuringPlaymode && !Application.isPlaying)
            {
                Debug.Log("Saving changes made during Play mode!");
                AcquireTarget();
                FPSControlGameSettingsTemp.Load(ref target);
                didSaveDuringPlaymode = false;
            }
        }

        public override void OnGUI()
        {
            if (!target) AcquireTarget();
            
            int depth = GUI.depth;
            GUI.DrawTexture(titleRect, title);
            
            //GUI.enabled = !Application.isPlaying; //potential tmp bug prevention?
            if(GUI.Button(saveButtonRect, "Save")) SaveToFile();
            GUI.enabled = true;

            GUI.Box(keyMapRect, movementKeyMappingBG, GUIStyle.none);

            runLeft = (KeyCode) EditorGUI.EnumPopup(new Rect(dropDownX, 150, dropDownSize.x, dropDownSize.y), runLeft);
            runRight = (KeyCode) EditorGUI.EnumPopup(new Rect(dropDownX, 170, dropDownSize.x, dropDownSize.y), runRight);
            interact = (KeyCode) EditorGUI.EnumPopup(new Rect(dropDownX, 190, dropDownSize.x, dropDownSize.y), interact);
            reload = (KeyCode) EditorGUI.EnumPopup(new Rect(dropDownX, 210, dropDownSize.x, dropDownSize.y), reload);
            escape = (KeyCode) EditorGUI.EnumPopup(new Rect(dropDownX, 230, dropDownSize.x, dropDownSize.y), escape);

            GUI.Box(mouseSettingsRect, mouseSettingsBG, GUIStyle.none);

            Knobs.Theme(Knobs.Themes.WHITE, _editor);

            allVals[0] = sensitivityX = Knobs.MinMax(new Vector2(615,138),sensitivityX,1,75,0);
            allVals[1] = sensitivityY = Knobs.MinMax(new Vector2(685, 138), sensitivityY, 1, 75, 1);
            allVals[2] = mouseFilterBufferSize = Knobs.Rotary(new Vector2(755, 138), (int)mouseFilterBufferSize, 2);
            allVals[3] = gunLookDownOffsetThreshold = Knobs.MinMax(new Vector2(825, 138), gunLookDownOffsetThreshold, -30, 30, 3);
            allVals[4] = minimumX = Knobs.Rotary(new Vector2(615, 210), (int)minimumX, 4);
            allVals[5] = minimumY = Knobs.Rotary(new Vector2(685, 210), (int)minimumY, 5);
            allVals[6] = maximumX = Knobs.Rotary(new Vector2(755, 210), (int)maximumX, 6);
            allVals[7] = maximumY = Knobs.Rotary(new Vector2(825, 210), (int)maximumY, 7);

            if(Knobs.interactID != -1)
            {
                GUIStyle gs = new GUIStyle();
                gs.normal.textColor = Color.white;
                gs.alignment = TextAnchor.MiddleCenter;
                gs.normal.background = (Texture2D) precisionBG;

                //Debug.Log(Knobs.interactID);
                GUI.Box(precisionBox, ""+allVals[Knobs.interactID],gs);
                _editor.Repaint();
            }

            /*if(drawPrecisionBox && Knobs.interactID >= 0)
            {
                //GUI.depth = -1000;
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = Color.grey;

                GUIStyle gs = new GUIStyle();
                gs.normal.textColor = Color.white;
                gs.alignment = TextAnchor.MiddleCenter;

                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, "");
                GUI.Box(precisionBox, allVals[Knobs.interactID]+"", gs);
                GUI.backgroundColor = bc;
                //GUI.depth = depth;
                
            }*/

        }

        void SaveToFile()
        {
            didSaveDuringPlaymode = Application.isPlaying;
            if (didSaveDuringPlaymode)
            {
                FPSControlGameSettingsTemp tmp = new FPSControlGameSettingsTemp(this);
            }

            target._runKeyLeft = runLeft;
            target._runKeyRight = runRight;
            target._interactionKey = interact;
            target._reloadKey = reload;
            target._escapeKey = escape;

            target._sensitivityX = sensitivityX;
            target._sensitivityY = sensitivityY;
            target._mouseFilterBufferSize = (int) mouseFilterBufferSize;
            target._gunLookDownOffsetThreshold = gunLookDownOffsetThreshold;
            target._minimumX = minimumX;
            target._minimumY = minimumY;
            target._maximumX = maximumX;
            target._maximumY = maximumY;

            //DEPRECATED//
            /*
            PushSettings();
            
            //Pull down the template
            string templateString = templateFile.text;

            //do simple replacements, insuring to append the necessary stuff.
            templateString = templateString.Replace(sRunLeft, "KeyCode." + runLeft.ToString());
            templateString = templateString.Replace(sRunRight, "KeyCode." + runRight.ToString());
            templateString = templateString.Replace(sInteract, "KeyCode." + interact.ToString());
            templateString = templateString.Replace(sEscape, "KeyCode." + escape.ToString());
            templateString = templateString.Replace(sReload, "KeyCode." + reload.ToString());

            templateString = templateString.Replace(sSensitivityX, sensitivityX + "F");
            templateString = templateString.Replace(sSensitivityY, sensitivityY + "F");
            templateString = templateString.Replace(sMouseFilterBufferSize, mouseFilterBufferSize + "F");
            templateString = templateString.Replace(sGunLookDownOffsetThreshold, gunLookDownOffsetThreshold + "F");
            templateString = templateString.Replace(sMinimumX, minimumX + "F");
            templateString = templateString.Replace(sMinimumY, minimumY + "F");
            templateString = templateString.Replace(sMaximumX, maximumX + "F");
            templateString = templateString.Replace(sMaximumY, maximumY + "F");

            //Now the IO stuff
            
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(templateString);

            using (StreamWriter outfile = new StreamWriter("Assets/Scripts/GameSettings.cs"))
            {
                outfile.Write(sb.ToString());
            }

            //And refresh our Project panel for good measure.
            AssetDatabase.Refresh(ImportAssetOptions.TryFastReimportFromMetaData);
            */
        }
    }

    
}
