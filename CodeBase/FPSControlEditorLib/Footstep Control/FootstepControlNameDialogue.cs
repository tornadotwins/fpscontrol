//DEPRICATED//
//Use Popup class instead
//If your class inheartes from FPSControlEditorModule you can use Prompt

////=====
////=====


//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using FPSControl;


////---
////
////---
//namespace FPSControlEditor
//{
//    public class FootstepNameDialogue : EditorWindow 
//    {    
//        private string m_name = "";
//        private bool exists = false;
		
//        void OnGUI()
//        {
//            //Label
//            GUILayout.Label("Enter a name for the new footstep sounds.");
			
//            m_name = GUILayout.TextField(m_name);
			
//            if(GUILayout.Button("Add"))
//            {
//                FootstepControlDefinitions footstepDef = FootstepControlModule.footstepDef;
				
//                foreach(FootstepControlDefinition def in footstepDef.footsteps)
//                {
//                    if(def.type == FootstepControlModule.typeSelect && def.name == m_name)
//                    {
//                        Debug.LogWarning("Definitions for type ["+FootstepControlModule.typeSelect+"] already contains "+m_name);
//                        exists = true;
//                    }
//                }
				
//                if(!exists)
//                {
//                    FootstepControlDefinition footDef = new FootstepControlDefinition();
					
//                    footDef.name 			= m_name;
//                    footDef.type 			= FootstepControlModule.typeSelect;
//                    footDef.audioScroll 	= Vector2.zero;
//                    footDef.textureScroll 	= Vector2.zero;
//                    footDef.minValPitch 	= -1f;
//                    footDef.minLimitPitch 	= -3f;
//                    footDef.maxValPitch 	= 1f;
//                    footDef.maxLimitPitch 	= 3f;
//                    footDef.minValVolume 	= .2f;
//                    footDef.minLimitVolume 	= 0f;
//                    footDef.maxValVolume 	= .5f;
//                    footDef.maxLimitVolume 	= 1f;

//                    FootstepControlModule.footstepDef.footsteps.Add(footDef);
//                    FootstepControlModule.RecollectFiltered();
                    

//                    /*EditorUtility.SetDirty( footstepDef );
//                    AssetDatabase.SaveAssets();
//                    Debug.Log( "saving name" );
//                     * */
//                }
				
//                this.Close();
//            }
//        }
//    }
//}

