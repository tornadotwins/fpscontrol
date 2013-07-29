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
        IEditorControlMapVis mapVis;

        Texture background;

        bool isDefault { get { return catalogue[_platform].DefaultKey == catalogue[_platform].GetKeys()[_currentMap]; } }

        #region Editor Properties 

        int _currentMap = -1;
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
            LoadAssets();
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

            GUILayout.BeginArea(MODULE_SIZE);
            GUI.DrawTexture(new Rect(0, 0, background.width, background.height), background);

            bool guiEnabled = GUI.enabled = activeEditor == null && addEditor == null;

            if (!catalogue) AcquireTarget();

            GUILayout.BeginHorizontal(GUILayout.Height(90));
            GUILayout.Space(10);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal(GUILayout.Height(35));
            GUILayout.Space(20);
            GUILayout.Label("Set Controls to: ");

            FPSControlPlatform prevPlatform = _platform;
            bool b;

            b = _platform == FPSControlPlatform.Mac;
            b = GUILayout.Toggle(b, "Mac", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.Mac : _platform;
            GUILayout.Space(10);
            b = _platform == FPSControlPlatform.PC;
            b = GUILayout.Toggle(b, "PC", EditorStyles.radioButton);
            _platform = b ? FPSControlPlatform.PC : _platform;
            GUI.enabled = false;
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
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            //This is where we set the active map we will edit:
            GUILayout.Label("Set scheme to: ");
            int prevMap = _currentMap;
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
            bool changedFocusedMap = prevMap != _currentMap || prevPlatform != _platform;
            //now we provide controls for saving, creating/deleting maps, etc.

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
                changedFocusedMap = true;
            }
            GUI.enabled = (!isDefault) ? guiEnabled : false; //if it is the default already, disable button
            if (GUILayout.Button("Mark Default")) catalogue[_platform].MarkDefault(groupKeys[_currentMap]);
            GUI.enabled = guiEnabled;
            GUILayout.FlexibleSpace();

            if (changedFocusedMap) LoadVisualization();


            GUILayout.EndHorizontal();

            mapVis.Draw();//DrawMap();
            DrawMappingButtons();

            GUI.depth++;
            if (addEditor != null)
            {
                GUI.enabled = true;
                activeEditor = null;

                //GUI.Box(new Rect(0, 0, MODULE_SIZE.width, MODULE_SIZE.height), "");
                addEditor.Draw();
                GUI.enabled = guiEnabled;
            }

            if (activeEditor != null)
            {
                GUI.enabled = true;
                //GUI.Box(new Rect(0, 0, MODULE_SIZE.width, MODULE_SIZE.height), "");
                activeEditor.Draw();
                GUI.enabled = guiEnabled;
            }
            GUI.depth--;

            GUILayout.EndArea();
            
            Repaint();
        }

        void LoadVisualization()
        {
            switch (_platform)
            {
                case FPSControlPlatform.Mac: mapVis = new MacMapVis(catalogue.mac[_currentMapName], Repaint); break;
                case FPSControlPlatform.PC: mapVis = new PCMapVis(catalogue.pc[_currentMapName], Repaint); break;
                //case FPSControlPlatform.Ouya: mapVis = new MacMapVis(catalogue.mac[_currentMapName]); break;    
            }
        }

        void LaunchAddEditor()
        {
            switch (_platform)
            {
                case FPSControlPlatform.Mac: 
                    addEditor = new AddMapEditor<DesktopControlMapGroup, DesktopControlMap>(catalogue.mac, OnAddMapClose, OnAddMapApply); 
                    break;
                case FPSControlPlatform.PC:
                    addEditor = new AddMapEditor<DesktopControlMapGroup, DesktopControlMap>(catalogue.pc, OnAddMapClose, OnAddMapApply);
                    break;
                //case FPSControlPlatform.PC:

            }

            addEditor.Open();
        }

        /*
        void DrawMap()
        {
            if (catalogue == null) return;
            switch (_platform)
            {
                case FPSControlPlatform.Mac:
                    //Debug.Log("current map: " + _currentMapName);
                    if (!catalogue.mac.ContainsKey(_currentMapName) || catalogue.mac[_currentMapName] == null) return; 
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
                default: Debug.LogError("invalid"); break;
            }
        }*/

        void OnAddMapApply()
        {
            //Selection.activeObject = catalogue;
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
            DesktopButton[] weaponButtons;
            
            Color c = GUI.backgroundColor;
            switch (_platform)
            {
                case FPSControlPlatform.Mac:

                    GUILayout.BeginArea(new Rect(22, 485, 500, 125));

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();

                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Fire);
                    if (GUILayout.Button("Action/Fire")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].fire, "Fire/Action Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Reload);
                    if (GUILayout.Button("Reload")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].reload, "Reload Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Scope);
                    if (GUILayout.Button("Scope")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].scope, "Scope Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Interact);
                    if (GUILayout.Button("Interaction")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].interact, "Interaction Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Defend);
                    if (GUILayout.Button("Defend")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].defend, "Defend Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Weapon1);
                    weaponButtons = new DesktopButton[4]{catalogue.mac[_currentMapName].weapon1, catalogue.mac[_currentMapName].weapon2, catalogue.mac[_currentMapName].weapon3, catalogue.mac[_currentMapName].weapon4};
                    if (GUILayout.Button("Weapon Select")) { activeEditor = new DesktopWeaponSelectEditor(weaponButtons, "Weapon Select Controls", OnActiveEditorClose, OnActiveEditorApply); }
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.WeaponCycle);
                    if (GUILayout.Button("Cycle Weapon")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].weaponToggle, "Weapon Cycle Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Look);
                    if (GUILayout.Button("Look")) activeEditor = new DesktopAxisEditor(catalogue.mac[_currentMapName].look, "Look Controls", OnActiveEditorClose, OnActiveEditorApply);
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Move);
                    if (GUILayout.Button("Movement")) activeEditor = new DesktopAxisEditor(catalogue.mac[_currentMapName].movement, "Movement Contols", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Jump);
                    if (GUILayout.Button("Jump")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.mac[_currentMapName].jump, "Jump Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Run);
                    if (GUILayout.Button("Run")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].run, "Run Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Crouch);
                    if (GUILayout.Button("Crouch")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.mac[_currentMapName].crouch, "Crouch Control", OnActiveEditorClose, OnActiveEditorApply);
                    GUILayout.EndVertical();

                    GUILayout.Space(50);

                    GUILayout.EndHorizontal();

                    GUILayout.EndArea();
                   break;
                case FPSControlPlatform.PC:

                   GUILayout.BeginArea(new Rect(22, 485, 500, 125));

                   GUILayout.BeginHorizontal();
                   GUILayout.BeginVertical();

                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Fire);
                   if (GUILayout.Button("Action/Fire")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.pc[_currentMapName].fire, "Fire/Action Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Reload);
                   if (GUILayout.Button("Reload")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.pc[_currentMapName].reload, "Reload Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Scope);
                   if (GUILayout.Button("Scope")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.pc[_currentMapName].scope, "Scope Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Interact);
                   if (GUILayout.Button("Interaction")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.pc[_currentMapName].interact, "Interaction Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUILayout.EndVertical();

                   GUILayout.BeginVertical();
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Defend);
                   if (GUILayout.Button("Defend")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.pc[_currentMapName].defend, "Defend Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Weapon1);
                   weaponButtons = new DesktopButton[4] { catalogue.pc[_currentMapName].weapon1, catalogue.pc[_currentMapName].weapon2, catalogue.pc[_currentMapName].weapon3, catalogue.pc[_currentMapName].weapon4 };
                   if (GUILayout.Button("Weapon Select")) { activeEditor = new DesktopWeaponSelectEditor(weaponButtons, "Weapon Select Controls", OnActiveEditorClose, OnActiveEditorApply); }
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.WeaponCycle);
                   if (GUILayout.Button("Cycle Weapon")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.pc[_currentMapName].weaponToggle, "Weapon Cycle Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Look);
                   if (GUILayout.Button("Look")) activeEditor = new DesktopAxisEditor(catalogue.pc[_currentMapName].look, "Look Controls", OnActiveEditorClose, OnActiveEditorApply);
                   GUILayout.EndVertical();

                   GUILayout.BeginVertical();
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Move);
                   if (GUILayout.Button("Movement")) activeEditor = new DesktopAxisEditor(catalogue.pc[_currentMapName].movement, "Movement Contols", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Jump);
                   if (GUILayout.Button("Jump")) activeEditor = new DesktopButtonEditor<DesktopButton>(catalogue.pc[_currentMapName].jump, "Jump Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Run);
                   if (GUILayout.Button("Run")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.pc[_currentMapName].run, "Run Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUI.backgroundColor = MacMapVis.ColorByControl(ControlMap.ControlID.Crouch);
                   if (GUILayout.Button("Crouch")) activeEditor = new DesktopButtonEditor<DesktopPersistantButton>(catalogue.pc[_currentMapName].crouch, "Crouch Control", OnActiveEditorClose, OnActiveEditorApply);
                   GUILayout.EndVertical();

                   GUILayout.Space(50);

                   GUILayout.EndHorizontal();

                   GUILayout.EndArea();
                   break;
            }

            GUI.backgroundColor = c;

            if (activeEditor != null && !hasActiveEditor) activeEditor.Open();
        }
    }
}
