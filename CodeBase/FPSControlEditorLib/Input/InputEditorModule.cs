//=====
//=====
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControl.Controls;
using FPSControlEditor.Controls;

//---
//
//---
namespace FPSControlEditor
{
    public class InputEditorModule : FPSControlEditorModule 
	{
        const string PATH = "Player Control/";
        //const int BUTTON_WIDTH =
        ControlMapCatalogue catalogue;
        IControlEditor activeEditor = null;
        IControlEditor addEditor = null;

        Texture background;

        bool isDefault { get { return catalogue[_platform].DefaultKey == catalogue[_platform].GetKeys()[_currentMap]; } }

        #region Editor Properties 

        int _currentMap = 0;
        string _currentMapName { get { return catalogue[_platform].GetKeys()[_currentMap]; } }
        FPSControlPlatform _platform;

        #endregion // Editor Properties

        #region GUI Properties

        Rect areaRect = new Rect(245, 50, 660, 580);

        #endregion // GUI Properties

        //[MenuItem("FPS Control/Create New Control Map Catalogue")]
        static void NewCatalogue()
        {
            ControlMapCatalogue cat = new ControlMapCatalogue();
            cat.Add(FPSControlPlatform.Mac, "Default OSX");
            cat.mac["Default OSX"].look = new DesktopAxis();
            cat.mac["Default OSX"].fire = new DesktopPersistantButton();
            cat.mac["Default OSX"].scope = new DesktopPersistantButton();
            (cat.mac["Default OSX"].scope as DesktopPersistantButton).mouseButton = 1;
            cat.mac["Default OSX"].jump = new DesktopButton();
            (cat.mac["Default OSX"].jump as DesktopButton).key = KeyCode.Space;

            AssetDatabase.CreateAsset(cat, ControlMapCatalogue.FILE);
        }

        public InputEditorModule(EditorWindow w) : base(w) {  }

        public override void Init()
        {
            base.Init();
            LoadAssets();

            AcquireTarget();
        }

        public override void OnFocus(bool rebuild)
        {
            AcquireTarget();
            base.OnFocus(rebuild);
        }

        void LoadAssets()
        {
            background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "background.png");
        }

        void AcquireTarget()
        {
            catalogue = (ControlMapCatalogue)AssetDatabase.LoadAssetAtPath(ControlMapCatalogue.FILE, typeof(ControlMapCatalogue));
        }

        void SaveAsset(ControlMapCatalogue obj)
        {
            ControlMapCatalogue tmp = (ControlMapCatalogue) Object.Instantiate(obj);
            AssetDatabase.DeleteAsset(ControlMapCatalogue.FILE);
            AssetDatabase.CreateAsset(tmp, ControlMapCatalogue.FILE);
            AssetDatabase.ImportAsset(ControlMapCatalogue.FILE);
            AcquireTarget();
        }

        public override void OnGUI()
        {
            //return;  
            Color c = GUI.backgroundColor;
            int depth = GUI.depth;
            
            //GUI.DrawTexture(areaRect, background);
            GUI.Box(areaRect, "");

            GUILayout.BeginArea(MODULE_SIZE);

            bool guiEnabled;// = activeEditor == null && addEditor == null;

            if (addEditor != null)
            {
                activeEditor = null;
                GUI.depth++;
                GUI.Box(new Rect(0, 0, MODULE_SIZE.width, MODULE_SIZE.height), "");
                addEditor.Draw();
                GUI.depth--;
            }

            if (activeEditor != null)
            {
                GUI.depth++;
                GUI.Box(new Rect(0, 0, MODULE_SIZE.width, MODULE_SIZE.height), "");
                activeEditor.Draw();
                GUI.depth--;
            }

            guiEnabled = GUI.enabled = activeEditor == null && addEditor == null;

            if (!catalogue) AcquireTarget();

            GUILayout.BeginHorizontal(GUILayout.Height(90));
            GUILayout.Label("HEADER PLACEHOLDER");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.Box(GUILayoutUtility.GetLastRect(), "");
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal(GUILayout.Height(35));

            GUILayout.Label("Set Controls to: ");

            bool b;

            b = _platform == FPSControlPlatform.Mac;
            b = GUILayout.Toggle(b, "Mac", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.Mac : _platform;
            GUI.enabled = false;
            GUILayout.Space(10);
            b = _platform == FPSControlPlatform.PC;
            b = GUILayout.Toggle(b, "PC", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.PC : _platform;
            GUILayout.Space(10);
            b = _platform == FPSControlPlatform.iOS;
            b = GUILayout.Toggle(b, "iOS", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.iOS : _platform;
            GUILayout.Space(10);
            b = _platform == FPSControlPlatform.Android;
            b = GUILayout.Toggle(b, "Android", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.Android : _platform;
            //GUI.enabled = guiEnabled;
            GUILayout.Space(10);
            b = _platform == FPSControlPlatform.Ouya;
            b = GUILayout.Toggle(b, "Ouya", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.Ouya : _platform;
            GUI.enabled = false;
            GUILayout.Space(10);
            b = _platform == FPSControlPlatform.Steambox;
            b = GUILayout.Toggle(b, "Steambox", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.Steambox : _platform;
            GUI.enabled = guiEnabled;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(); 

            GUILayout.Label("Set scheme to: ");

            _currentMap = Mathf.Clamp(_currentMap,0,catalogue[_platform].GetKeys().Length-1);

            string[] groupKeys = catalogue[_platform].GetKeys();

            if (groupKeys.Length == 0)
            {
                _currentMap = 0;
                GUI.enabled = false;
            }
            else
            {
                for(int i = 0; i < groupKeys.Length; i++)
                {
                    if(groupKeys[i] == catalogue[_platform].DefaultKey) groupKeys[i] += " (Default)";
                }
            }
            _currentMap = EditorGUILayout.Popup(_currentMap, groupKeys);
            GUILayout.Space(5);
            if (GUILayout.Button("Save")) SaveAsset(catalogue);
            GUILayout.Space(5);
            GUI.enabled = guiEnabled;
            if (GUILayout.Button("Create New")) LaunchAddEditor();
            GUI.enabled = (catalogue[_platform].Count > 1 && !isDefault) ? guiEnabled : false; //if it is the default or the only entry, prevent deletion
            
            if (GUILayout.Button("Remove"))
            {
                catalogue[_platform].Remove(groupKeys[_currentMap]);
                groupKeys = catalogue[_platform].GetKeys();//refresh
                _currentMap = 0;

            }
            GUI.enabled = (!isDefault) ? guiEnabled : false; //if it is the default already, disable button
            if (GUILayout.Button("Mark Default")) catalogue[_platform].MarkDefault(groupKeys[_currentMap]);
            GUI.enabled = guiEnabled;
            GUILayout.FlexibleSpace();

            //_currentMap = 

            GUILayout.EndHorizontal();

            //catalogue = (FOOTSTEPTYPES)EditorGUI.EnumPopup(new Rect(125, 64, 130, 15), typeSelect);

            DrawMap();
            DrawMappingButtons();

            GUILayout.EndArea();
            _editor.Repaint();
        }

        void LaunchAddEditor()
        {
            switch (_platform)
            {
                case FPSControlPlatform.Mac: 
                    addEditor = new AddMapEditor<DesktopControlMapGroup, DesktopControlMap>(catalogue.mac, OnAddMapClose, OnAddMapApply); 
                    
                    break;
                //case FPSControlPlatform.PC:

            }

            addEditor.Open();
        }

        void DrawMap()
        {
            switch (_platform)
            {
                case FPSControlPlatform.Mac:
                    GUILayout.Label("Movement>> " + catalogue.mac[_currentMapName].movement.ToString());
                    GUILayout.Label("Look>> " + catalogue.mac[_currentMapName].look.ToString());
                    GUILayout.Label("Jump>> " + catalogue.mac[_currentMapName].jump.ToString());
                    GUILayout.Label("Reload>> " + catalogue.mac[_currentMapName].reload.ToString());
                    GUILayout.Label("Run>> " + catalogue.mac[_currentMapName].run.ToString());
                    GUILayout.Label("Interaction>> " + catalogue.mac[_currentMapName].interact.ToString());
                    GUILayout.Label("Crouch>> " + catalogue.mac[_currentMapName].crouch.ToString());
                    GUILayout.Label("Fire>> " + catalogue.mac[_currentMapName].fire.ToString());
                    GUILayout.Label("Scope>> " + catalogue.mac[_currentMapName].scope.ToString());
                    GUILayout.Label("Weapon Cycle>> " + catalogue.mac[_currentMapName].weaponToggle.ToString());
                    GUILayout.Label("Defend>> " + catalogue.mac[_currentMapName].defend.ToString());
                    //GUILayout.Label("Movement: " + catalogue.mac[_currentMapName].movement.ToString());
                    break;
            }
        }

        void OnAddMapApply()
        {
            addEditor.Close();
        }

        void OnAddMapClose()
        {
            addEditor = null;
        }

        void OnActiveEditorApply()
        {
            activeEditor.Close();
        }

        void OnActiveEditorClose()
        {
            activeEditor = null;
        }

        public void DrawMappingButtons()
        {
            bool hasActiveEditor = activeEditor != null;
            
            switch (_platform)
            {
                case FPSControlPlatform.Mac:

                    GUILayout.BeginArea(new Rect(175, 440, 475, 125));

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    if (GUILayout.Button("Movement")) activeEditor = new DesktopAxisEditor(catalogue.mac[_currentMapName].movement, "Movement Contols", OnActiveEditorClose, OnActiveEditorApply);
                    bool g = GUI.enabled;
                    GUI.enabled = false;
                    if (GUILayout.Button("Weapon Select")) { }
                    GUI.enabled = g;
                    if (GUILayout.Button("Interaction")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].interact, "Interaction Control", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Jump")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].jump, "Jump Control", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Reload")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].reload, "Reload Control", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Run")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].run, "Run Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    if (GUILayout.Button("Look")) activeEditor = new DesktopAxisEditor(catalogue.mac[_currentMapName].look, "Look Controls", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Scope")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].scope, "Scope Control", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Action/Fire")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].fire, "Fire/Action Control", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Defend")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].defend, "Defend Control", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Weapon Cycle")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].weaponToggle, "Weapon Cycle Control", OnActiveEditorClose, OnActiveEditorApply);
                    if (GUILayout.Button("Crouch")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].crouch, "Crouch Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();
                    GUI.depth--;
                    GUI.Box(GUILayoutUtility.GetLastRect(), "");
                    GUI.depth++;

                    GUILayout.EndArea();
                    

                   break;
            }

            if (activeEditor != null && !hasActiveEditor) activeEditor.Open();
        }
    }
}
