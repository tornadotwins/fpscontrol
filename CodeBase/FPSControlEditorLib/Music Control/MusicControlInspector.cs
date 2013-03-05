//=====
//=====


using UnityEngine;
using UnityEditor;
using System.Collections;
using FPSControl;


//---
//
//---
namespace FPSControlEditor
{
	[CustomEditor(typeof(MusicControl))]
	public class MusicControlInspector : Editor
	{
	
		//---
		// Use this for initialization
		//---
		public MusicControlInspector () 
		{
		}
	
		
		//---
		// display the custom inspector button
		//---
		public override void OnInspectorGUI ()
		{
	        EditorGUILayout.BeginVertical();
			GUILayout.Space(10);
			
	       	if (GUILayout.Button("Open Track Editor"))
			{
                FPSControlMainEditor.OpenTo(FPSControlModuleType.MusicControl);
                //EditorWindow.GetWindow (typeof (FPSControlMainEditor));
			}
			
	        EditorGUILayout.EndVertical();	
		}
	}
}