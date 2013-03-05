using FPSControlEditor;
using FPSControl;
using UnityEditor;
using UnityEngine;

namespace FPSControlEditor
{
    [CustomEditor(typeof(RBFPSControllerLogic))]
    class RBFPSControllerLogicInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            if (GameSettingsModule.didSaveDuringPlaymode && !Application.isPlaying)
            {
                RBFPSControllerLogic t = (RBFPSControllerLogic)target;
                FPSControlGameSettingsTemp.Load(ref t);
                GameSettingsModule.didSaveDuringPlaymode = false;
            }
            
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);

            if (GUILayout.Button("Open Editor"))
            {
                FPSControlMainEditor.OpenTo(FPSControlModuleType.GameSettings);
                //EditorWindow.GetWindow(typeof(FPSControlMainEditor));
            }

            EditorGUILayout.EndVertical();
        }
    }
}
