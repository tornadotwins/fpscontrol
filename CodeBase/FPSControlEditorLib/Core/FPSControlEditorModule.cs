//#define DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    public class FPSControlEditorModule
    {
        public static Rect MODULE_SIZE { get { return new Rect(246, 50, 660, 578); } }
        public static GUILayoutOption[] NONE { get { return new GUILayoutOption[0] { }; } }
        public float version = 1.0f; 

        protected EditorWindow _editor;

        protected FPSControlModuleType _type = FPSControlModuleType.NONE;
        public virtual FPSControlModuleType type { get { return _type; } }

        private string WAS_PLAYING_WHEN_LOST_FOCUS;
        private string JUST_LEFT_TO_CREATE_NEW;

        public FPSControlEditorModule(EditorWindow editorWindow)
        {
            _editor = editorWindow;
        }

        internal void Prompt(string promptText)
        {
            //Debug.Log("Prompting user1");
            justReturnedFromPopup = true;
            if (Popup.Prompt(promptText, _editor))
            {
                Popup.OnCancleInput += OnPromptCancle;
                Popup.OnUserInput += OnPromptInput;
            }
        }

        public virtual void OnPromptCancle() 
        {
            //Debug.Log("OnPromptCancle");
        }

        public virtual void OnPromptInput(string userInput) 
        {
           // Debug.Log(userInput);
        }

        public virtual void Init() {
            WAS_PLAYING_WHEN_LOST_FOCUS = "_FPSControl_" + _type + "FocusedBeforePlay";
            JUST_LEFT_TO_CREATE_NEW = "_FPSControl_" + _type + "FocusedBeforePlay";
            HandlePaidStatus(RespounceHandler.CheckForPurchase(_type));
        }

        public virtual void HandlePaidStatus(bool paid)
        {
            if (!paid) FPSControlMainEditor.OpenTo(FPSControlModuleType.NeedsPurchasing);
        }

        public virtual void Deinit() {
            wasPlaying = Application.isPlaying;
            justReturnedFromPopup = false;
        }

        public virtual void OnInspectorUpdate() { }
        public virtual void OnGUI() { }
        public virtual void Update() { }
        public virtual void OnFocus(bool rebuild) {
            justReturnedFromPopup = false;
        }

        public virtual void OnLostFocus(bool rebuild) {
            wasPlaying = Application.isPlaying;
        }

        public void Repaint()
        {
            _editor.Repaint();
        }

        internal bool justReturnedFromPopup
        {
            get
            {
                return EditorPrefs.GetBool(JUST_LEFT_TO_CREATE_NEW, false);
            }
            set
            {
                EditorPrefs.SetBool(JUST_LEFT_TO_CREATE_NEW, value);
            }
        }

        internal bool wasPlaying
        {
            get
            {
                return EditorPrefs.GetBool(WAS_PLAYING_WHEN_LOST_FOCUS, false);
            }
            set
            {
                EditorPrefs.SetBool(WAS_PLAYING_WHEN_LOST_FOCUS, value);
            }
        }

        internal Texture LoadPNG(string folderName, string assetName)
        {
            string assetPath = FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + folderName + assetName + ".png";
            if (!File.Exists(assetPath))
            {
                Debug.Log("Cant load GUI asset: " + assetPath);
                return null;
            }
            return Load<Texture>(assetPath);
        }

        internal T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));

            #if DEBUG 
            Debug.Log("Loading asset at: " + assetPath + ". asset: " + obj);
            #endif

            return (T)obj;
        }

    }
}
