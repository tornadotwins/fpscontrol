#define DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

//---
//
//---
namespace FPSControlEditor
{
    public enum FPSControlModuleType
    {
        NONE = -2, //No Module. Empty Screen
        UNAVAILABLE = -1, //An UnavailableModule, this is just a message and a link to the development roadmap.
        Login = 0,
        GameSettings = 1,
        Multiplayer = 2,
        PlayerControl = 3,
        MissionControl = 4,
        Optimize = 5,
        WeaponControl = 6,
        Crosshairs = 7,
        Footsteps = 8,
        MusicControl = 9,
		TeamWork = 10
    }

    public class FPSControlMainEditor : EditorWindow
    {
        static Rect windowSize;

        #region Server Data

        const string HREF = @"http://www.fpscontrol.com/login.php";

        //bool _loggedIn = false;
        //string _user = "";
        //string _pass = "";

        #endregion

        #region Assets

        public const string ROOT_FOLDER = "Assets/FPSControlCore/";
        public const string ASSET_PATH = ROOT_FOLDER + "Editor/FPSControl/Editor Assets/";
        public const string RESOURCE_FOLDER = ROOT_FOLDER + "Resources/";
        public const string GRAPHICS = "Graphics/";
        public const string TEMP = "_TMP/";
        public const string TEXT_ASEETS = "Text Assets/";

        //BG Graphics
        Texture header;
        Texture sidebarBG;

        //Unavailable module BG paths
        public string missioncontrolBGPath = ASSET_PATH + GRAPHICS + "Mission Control/missioncontrol_bg.jpg";
        public string multiplayerBGPath = ASSET_PATH + GRAPHICS + "Multiplayer/multiplayer_bg.jpg";
        public string playercontrolBGPath = ASSET_PATH + GRAPHICS + "Player Control/playercontrol_bg.jpg";
        public string weaponcontrolBGPath = ASSET_PATH + GRAPHICS + "Weapon Control/weaponcontrol_bg.jpg";
        public string crosshairsBGPath = ASSET_PATH + GRAPHICS + "Crosshairs/crosshairs_bg.jpg";

        //Module Buttons
        Texture module_gameSettings_n; //normal
        Texture module_gameSettings_a; //active

        Texture module_multiplayer_n;
        Texture module_multiplayer_a;

        Texture module_playerControl_n;
        Texture module_playerControl_a;

        Texture module_missionControl_n;
        Texture module_missionControl_a;

        Texture module_optimizeControl_n;
        Texture module_optimizeControl_a;

        Texture module_weaponControl_n;
        Texture module_weaponControl_a;

        Texture module_crosshairs_n;
        Texture module_crosshairs_a;

        Texture module_footsteps_n;
        Texture module_footsteps_a;

        Texture module_musicControl_n;
        Texture module_musicControl_a;

        Texture module_teamwork_n;
        Texture module_teamwork_a;

        #endregion // Assets

        #region GUI Properties

        public static Vector2 WINDOW_SIZE { get { return new Vector2(910, 633); } }

        Rect sidebarRect = new Rect(3, 52, 241, 578);
        Rect headerRect = new Rect(3, 3, 904, 46);
        Rect dropdownRect = new Rect(800, 55, 100, 30);

        #endregion // GUI Properties

        #region Modules

        static FPSControlEditorModule loadedModule;

        public FPSControlEditorModule gameSettings;
        public FPSControlEditorModule footsteps;
        public FPSControlEditorModule missionControl;
        public FPSControlEditorModule optimize;
        public FPSControlEditorModule musicControl;
        public FPSControlEditorModule login;
        public FPSControlEditorModule teamwork;
        public FPSControlEditorModule weaponControl;

        #endregion // Modules

        #region Editor Properties

        string username = "";
        bool loggedIn = false;

        //int currentPage = 0; //current page of sidebar

        #endregion // Editor Properties

        [MenuItem("FPS Control/Editor")]
        static void Init()
        {
            //Debug.Log("Opening FPSControl.");
            windowSize = new Rect((Screen.currentResolution.width / 2) - 287.5f, (Screen.currentResolution.height / 2) - 243, WINDOW_SIZE.x, WINDOW_SIZE.y);

            FPSControlMainEditor window = EditorWindow.GetWindowWithRect<FPSControlMainEditor>(windowSize, true, "FPSControl", true);

            window.LoadAssets();
            window.PreloadModules();
            //window.Show();
        }

        internal static void OpenTo(FPSControlModuleType m)
        {
            windowSize = new Rect((Screen.currentResolution.width / 2) - 287.5f, (Screen.currentResolution.height / 2) - 243, WINDOW_SIZE.x, WINDOW_SIZE.y);
            FPSControlMainEditor window = EditorWindow.GetWindowWithRect<FPSControlMainEditor>(windowSize, true, "FPSControl", true);

            window.LoadAssets();
            window.PreloadModules(m);
        }

        void OnFocus()
        {
            bool rebuild = false;
            if (loadedModule == null)
            {
                //Debug.Log("Loaded module is null! Recreating!");
                LoadModule();
                rebuild = true;
            }
            loadedModule.OnFocus(rebuild);
        }

        void OnLostFocus()
        {
            bool rebuild = false;
            if (loadedModule == null)
            {
                //Debug.Log("Loaded module is null! Recreating!");
                LoadModule();
                rebuild = true;
            }
            loadedModule.OnLostFocus(rebuild);
        }

        void OnGUI()
        {
            if (loadedModule == null)
            {
                //Debug.Log("Loaded module is null! Recreating!");
                LoadModule();
            }
			
            //Black Background
            //GUI.Box(new Rect(0, 0, WINDOW_SIZE.x, WINDOW_SIZE.y), black, GUIStyle.none);

            //HEADER:
            GUI.Box(headerRect, header, GUIStyle.none);
            GUILayout.BeginArea(headerRect);
            //Header stuff should go here.
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(750);
            GUILayout.FlexibleSpace();
            if (loggedIn)
            {
                GUILayout.Label(username);
                if (GUILayout.Button("Logout", new GUILayoutOption[0] { }))
                {
                    //Logout stuff here.
                    FPSControlUserObject.LogOut();
                    loggedIn = false;
                    LoadModule(FPSControlModuleType.Login);
                }
            }

            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
            
            //SIDEBAR:
            if (loadedModule.type != FPSControlModuleType.Login) DrawSidebar();

            if (loadedModule != null) loadedModule.OnGUI();
        }

        void DrawSidebar()
        {
			if( header == null ) LoadAssets();

            //SIDERBAR
            GUI.Box(sidebarRect, sidebarBG, GUIStyle.none);
            GUILayout.BeginArea(sidebarRect);

            GUILayout.Space(18);

            GUILayout.BeginHorizontal();
            GUILayout.Space(18);
            if (GUILayout.Button((moduleType == FPSControlModuleType.GameSettings) ? module_gameSettings_a : module_gameSettings_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.GameSettings);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(module_multiplayer_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.UNAVAILABLE);
                (loadedModule as UnavailableModule).SetBG(multiplayerBGPath);
            }
            GUILayout.Space(17);
            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            GUILayout.Space(18);
            if (GUILayout.Button(module_playerControl_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.UNAVAILABLE);
                (loadedModule as UnavailableModule).SetBG(playercontrolBGPath);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(module_missionControl_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.UNAVAILABLE);
                (loadedModule as UnavailableModule).SetBG(missioncontrolBGPath);
            }
            GUILayout.Space(17);
            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            GUILayout.Space(18);
            if (GUILayout.Button((moduleType == FPSControlModuleType.Optimize) ? module_optimizeControl_a : module_optimizeControl_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.Optimize);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button((moduleType == FPSControlModuleType.WeaponControl) ? module_weaponControl_a : module_weaponControl_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.WeaponControl);
            }
            GUILayout.Space(17);
            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            GUILayout.Space(18);
            if (GUILayout.Button(module_crosshairs_n,GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.UNAVAILABLE);
                (loadedModule as UnavailableModule).SetBG(crosshairsBGPath);
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button((moduleType == FPSControlModuleType.Footsteps) ? module_footsteps_a : module_footsteps_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.Footsteps);
            }
            GUILayout.Space(17);
            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            GUILayout.Space(18);
            if (GUILayout.Button((moduleType == FPSControlModuleType.MusicControl) ? module_musicControl_a : module_musicControl_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.MusicControl);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button((moduleType == FPSControlModuleType.TeamWork) ? module_teamwork_a : module_teamwork_n, GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(97), GUILayout.Height(97) }))
            {
                LoadModule(FPSControlModuleType.TeamWork);
            }
            GUILayout.Space(17);
            GUILayout.EndHorizontal();
            GUILayout.Space(27);
            GUILayout.EndArea();
        }

        #region Module Interaction

        static FPSControlModuleType moduleType
        {
            get
            {
                if (loadedModule == null) return FPSControlModuleType.NONE;
                return loadedModule.type;
            }
        }

        public void LoadModule()
        {
            LoadModule((FPSControlModuleType)EditorPrefs.GetInt("_FPSControl_LoadedModule"));
        }

        public void LoadModule(FPSControlModuleType module)
        {
            EditorPrefs.SetInt("_FPSControl_LoadedModule", (int)module);

            if (loadedModule != null) loadedModule.Deinit(); //deinitialize current module.

            switch (module)
            {
                case FPSControlModuleType.NONE: loadedModule = new FPSControlEditorModule(this); break;

                case FPSControlModuleType.Login:
                    login = new LoginModule(this);
                    LoginModule _login = (LoginModule)login;
                    _login.OnLoginSuccess += OnLoginSuccess;
                    _login.OnLoginFail += OnLoginFail;
                    _login.OnOfflineModePress += OnOfflineMode;
                    _login.OnRegisterPress += OnRegister;
                    _login.OnLoginPress += OnLoginPress;
                    loadedModule = login;
                    break;

                case FPSControlModuleType.GameSettings:
                    if (gameSettings == null) gameSettings = new GameSettingsModule(this);
                    loadedModule = gameSettings;
                    break;

                case FPSControlModuleType.Footsteps:
                    if (footsteps == null) footsteps = new FootstepControlModule(this);
                    loadedModule = footsteps;
                    break;                
                   /*
                case FPSControlModuleType.MissionControl:
                    if (missionControl == null) missionControl = new MissionControlModule(this);
                    loadedModule = missionControl;
                    break;
                    */
                case FPSControlModuleType.Optimize:
                    if (missionControl == null) optimize = new OptimizeModule(this);
                    loadedModule = optimize;
                    break;

                case FPSControlModuleType.WeaponControl:
                    if (weaponControl == null) weaponControl = new WeaponControlModule(this);
                    loadedModule = weaponControl;
                    break;

                case FPSControlModuleType.MusicControl:
                    if (musicControl == null) musicControl = new MusicControlModule(this);
                    loadedModule = musicControl;
                    break;

                case FPSControlModuleType.TeamWork:
                    if (teamwork == null) teamwork = new TeamworkModule(this);
                    loadedModule = teamwork;
                    break;

                default:
                    loadedModule = new UnavailableModule(this);
                    break;
            }

            loadedModule.Init();
        }

        public void PreloadModules(FPSControlModuleType m)
        {
            DoPreload();
            LoadModule(m);
        }

        public void PreloadModules()
        {
            DoPreload();
            LoadModule();
        }

        void DoPreload()
        {
            gameSettings = new GameSettingsModule(this);
            footsteps = new FootstepControlModule(this);
            musicControl = new MusicControlModule(this);

            login = new LoginModule(this);
            loggedIn = CheckSession();
            //Here we'll need to check and see if we're logged in//
            if(loggedIn)
            {
                username = FPSControlUserObject.current.name;
                //we're already logged in. sweet.
                //we can likely just load the last loaded module.
            }
            else
            {
                EditorPrefs.SetInt("_FPSControl_LoadedModule", 0);
            }

            //if (EditorPrefs.HasKey("_FPSControl_LoadedModule")) EditorPrefs.DeleteKey("_FPSControl_LoadedModule");
            //if (!EditorPrefs.HasKey("_FPSControl_LoadedModule")) EditorPrefs.SetInt("_FPSControl_LoadedModule", 0);
        }

        #endregion // Module Interaction

        #region Server Communcation

        bool CheckSession()
        {
            if (FPSControlUserObject.current != null)
            {
                return FPSControlUserObject.current.loggedIn;
            }

            return false;
        }

        void OnLoginPress(object obj)
        {

        }

        void OnLoginSuccess(object obj)
        {
            FPSControlUserObject user = (FPSControlUserObject) obj;
            username = user.name;
            loggedIn = true;
            //Debug.Log(username + " successfully logged in.");
            LoadModule(FPSControlModuleType.NONE);
        }

        void OnLoginFail(object obj)
        {

        }

        void OnRegister(object obj)
        {
            Application.OpenURL("http://www.fpscontrol.com/upgrade/register");
        }

        void OnOfflineMode(object obj)
        {
            LoadModule(FPSControlModuleType.NONE);
        }

        #endregion

        #region Resource Loading

        void LoadAssets()
        {
            //Backgrounds
            header = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/header.png");
            sidebarBG = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/sidebar_background.png");
            //black = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/black1x1.png");

            //Module Buttons
            module_gameSettings_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_gamesettings_n.png"); //normal
            module_gameSettings_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_gamesettings_a.png"); //active

            module_multiplayer_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_multiplayer_n.png"); //normal
            module_multiplayer_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_multiplayer_a.png"); //active

            module_playerControl_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_playercontrol_n.png"); //normal
            module_playerControl_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_playercontrol_a.png"); //active

            module_missionControl_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_missioncontrol_n.png"); //normal
            module_missionControl_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_missioncontrol_a.png"); //active

            module_optimizeControl_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_optimize_n.png"); //normal
            module_optimizeControl_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_optimize_a.png"); //active

            module_weaponControl_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_weaponcontrol_n.png"); //normal
            module_weaponControl_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_weaponcontrol_a.png"); //active

            module_crosshairs_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_crosshairs_n.png"); //normal
            module_crosshairs_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_crosshairs_a.png"); //active

            module_footsteps_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_footsteps_n.png"); //normal
            module_footsteps_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_footsteps_a.png"); //active

            module_musicControl_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_musiccontrol_n.png"); //normal
            module_musicControl_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_musiccontrol_a.png"); //active

            module_teamwork_n = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_teamwork_n.png"); //normal
            module_teamwork_a = Load<Texture>(ASSET_PATH + GRAPHICS + "Main Editor/btn_teamwork_a.png"); //active
        }

        T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));

            #if DEBUG 
            //Debug.Log("Loading asset at: " + assetPath + ". asset: " + obj);
            #endif

            return (T)obj;
        }

        #endregion
    }
}
