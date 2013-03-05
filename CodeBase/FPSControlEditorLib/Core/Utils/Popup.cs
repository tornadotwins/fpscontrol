using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    class Popup : EditorWindow
    {
        public static event System.Action<string> OnUserInput;
        public static event System.Action OnCancleInput;
        public static bool showing {get; private set;}

        private static Popup window; 
        private static string inputPromptText = "";
        private static bool userInputedText = false;
        private static EditorWindow _parentWindow;

        private string inputText = "";

        public static bool Prompt()
        {
            return Prompt("", "");
        }

        public static bool Prompt(string promptText)
        {
            return Prompt(promptText, "");
        }

        public static bool Prompt(string promptText, EditorWindow parentWindow)
        {
            return Prompt(promptText, "", parentWindow);
        }

        public static bool Prompt(string promptText, string promptTitle, EditorWindow parentWindow = null)
        {
            if (showing) return false;
            _parentWindow = parentWindow;
            window = (Popup)EditorWindow.GetWindow<Popup>(true);
            window.position = new Rect((Screen.currentResolution.width / 2) - 150, (Screen.currentResolution.height / 2) - 40, 300, 80);
            window.Show();
            window.title = promptTitle;
            showing = true;
            userInputedText = false;
            inputPromptText = promptText;
            return true;
        }

        public static void ClosePrompt()
        {
            window.Close();
        }

        void OnGUI()
        {
            Event e = Event.current;
            GUILayout.Label(inputPromptText);
            inputText = GUILayout.TextField(inputText);
            if (GUILayout.Button("Add"))
            {
                if (OnUserInput != null) OnUserInput(inputText);
                userInputedText = true;
                window.Close();
            }
        }

        void OnDestroy()
        {
            if (!userInputedText && OnCancleInput != null) OnCancleInput();
            if (_parentWindow != null) _parentWindow.Focus();
            showing = false;
            OnCancleInput = null;
            OnUserInput = null;
        }

    }
}
