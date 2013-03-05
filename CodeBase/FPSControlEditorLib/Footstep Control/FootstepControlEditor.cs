//=====
//=====


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
	public class FootstepControlEditor : EditorWindow 
	{
		static Rect windowSize;
	
		Rect audioViewRect = new Rect(0, 0, 180, 1);
		Rect audioAreaRect = new Rect(10, 80, 200, 150);
		Rect textureViewRect = new Rect(0, 0, 1, 130);
		Rect textureAreaRect = new Rect(10, 275, 550, 150);
		int defIndex = 0;
		int currentIndex = 0;
		bool showEditor = false;
		int selectedDefIndex;
			
		public static FootstepControlDefinitions footstepDef;
		public static FOOTSTEPTYPES typeSelect = FOOTSTEPTYPES.Player;
		
		private bool _needRefresh = false;
		private bool _didPlay = false;
		
		private static string assetEditPath = @"Assets/Resources/FootstepControlDefinitions.asset";
		private static string assetPlayPath = @"Assets/Resources/FootstepControlDefinitionsP.asset";
	    
		
		//---
		//
		//---
	    //[MenuItem ("FPS Control/Footstep Editor")]
	    static void Init() 
		{
	        FootstepControlEditor window = (FootstepControlEditor)EditorWindow.GetWindow (typeof (FootstepControlEditor));
			windowSize = new Rect( ( Screen.currentResolution.width / 2 ) - 287.5f, ( Screen.currentResolution.height /2 ) - 243, 575,486);
	        window.position = windowSize;
	        window.Show();
			
			LoadDefinitions();		
	    }
		
		
		//---
		//
		//---
		void OnFocus()
		{
			//--- if play mode changed, need to refresh target reference. otherwise, it holds on to the
			//--- game play version and won't update correctly when switched back into editor mode
			/*
			Debug.Log( "new focus" );
			if( _playModeChanged )
			{
				FootstepEditor.LoadDefinitions();
				_playModeChanged = false;
			}
			*/
		}
		
		
		//---
		// 
		//---
		void OnInspectorUpdate()
		{
			//--- if switching to play mode, make a copy of edit mode settings
			if( EditorApplication.isPlaying && !_didPlay )
			{
				_didPlay = true;
				AssetDatabase.DeleteAsset( assetPlayPath );
				AssetDatabase.CopyAsset( assetEditPath, assetPlayPath );
				Debug.Log( "edit 2 play" );
			}
			
			//--- if defs are null or switching from play mode, reload edit mode version of settings
			if( ( footstepDef == null ) || (_didPlay && !EditorApplication.isPlaying) )
			{
				_needRefresh = false;
				if( !EditorApplication.isPlaying )_didPlay = false;
				Debug.Log ( "null defs" );
				FootstepControlEditor.LoadDefinitions();
				this.Repaint();
			}
			
			//??? can remove?
			if( ! EditorApplication.isPlaying && _needRefresh )
			{
				Debug.Log ( "***** need refresh" );
				_needRefresh = false;
				footstepDef = null;
			}
		}
			
	
		//---
		//
		//---
	    void OnGUI() 
		{
			//Show the bar with the label
			GUILayout.Box("", GUILayout.Height(30), GUILayout.Width(this.position.width-10));
			GUI.Label(new Rect(10, 10, 200, 20), "GamePrefabs.com | Footsteps Editor" );
			typeSelect = (FOOTSTEPTYPES)EditorGUI.EnumPopup( new Rect(this.position.width-212 ,10, 200, 20), "Edit footsteps for:", typeSelect);	
			
			//$$$ can remove?
			if( ( footstepDef == null ) ) 
			{
				Debug.Log ( "***** ongui load defs" );
				FootstepControlEditor.LoadDefinitions();
			}
			
			//Check if we have all the bits	we need			
			if(footstepDef != null )
			{
				ShowDefinitionSelect();
						
				if(showEditor)
				{
					FootstepControlDefinition footDef = footstepDef.footsteps[selectedDefIndex];
					
					//--- Background
					GUI.Box(new Rect(3, 55, this.position.width-10, this.position.height - 105), "");
					
					//--- Close Button
					if( GUI.Button( new Rect(this.position.width-45, 60, 32, 32), "x" ) )
					{
						footstepDef.footsteps.RemoveAt( selectedDefIndex );
					}
					
					//---
					// Audio area
					//---
					GUI.Label(new Rect(10, 60, 100,20), "Audio Clips:");
					GUI.Box(new Rect(10, 80, 200, 150),"");
					footDef.audioScroll = GUI.BeginScrollView(audioAreaRect, footDef.audioScroll, audioViewRect);
					Rect currentAudioRect = audioViewRect;
					currentAudioRect.height = 0;
					for(int i = 0; i < footDef.sounds.Count; i++)
					{
						GUI.Label(new Rect(5,20 * i, 160, 20), footDef.sounds[i].name);
						if(GUI.Button(new Rect(165, 20 * i , 20, 20),"x"))
						{
							currentAudioRect.height -= 20;
							footDef.sounds.RemoveAt( i );
							GUI.changed = true;
						} 
						else 
							currentAudioRect.height += 20;	
					}
					
					audioViewRect.height = currentAudioRect.height;
					
					GUI.EndScrollView();
					 
					//--- Audio Slider controls
					GUILayout.BeginArea(new Rect(215, 80, 300, 150));
						GUILayout.BeginVertical();					
							GUILayout.BeginHorizontal();
								GUILayout.Label("Pitch: ", GUILayout.Width(50));
								GUILayout.Label(decimal.Round((decimal)footDef.minValPitch, 2)+" / "+decimal.Round((decimal)footDef.maxValPitch, 2));
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
								GUILayout.Label("Min: "+footDef.minLimitPitch);
								EditorGUILayout.MinMaxSlider(ref footDef.minValPitch, ref footDef.maxValPitch, footDef.minLimitPitch, footDef.maxLimitPitch, GUILayout.Width(200));
								GUILayout.Label("Max: "+footDef.maxLimitPitch);
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
								GUILayout.Label("Volume: ", GUILayout.Width(50));
								GUILayout.Label(decimal.Round((decimal)footDef.minValVolume, 2)+" / "+decimal.Round((decimal)footDef.maxValVolume, 2));
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
								GUILayout.Label("Min: "+footDef.minLimitVolume);
								EditorGUILayout.MinMaxSlider(ref footDef.minValVolume, ref footDef.maxValVolume, footDef.minLimitVolume, footDef.maxLimitVolume, GUILayout.Width(200));
								GUILayout.Label("Max: "+footDef.maxLimitVolume);
							GUILayout.EndHorizontal();
							 	footDef.tag = EditorGUILayout.TagField( "Object Tag:", footDef.tag, GUILayout.Width(200));
						GUILayout.EndVertical();
					GUILayout.EndArea();
					
					
					if(Event.current.type == EventType.DragExited)
					{
						if( audioAreaRect.Contains( Event.current.mousePosition ) )
						{
							//Take the selection and append it to the array
							Object[] clips = DragAndDrop.objectReferences;
									
							foreach ( Object obj in clips )
							{
								if( obj is AudioClip )
								{
									AudioClip clip = (AudioClip) obj;
									if( !footDef.sounds.Contains(clip) )
									{
					            	footDef.sounds.Add(clip);
										GUI.changed = true;
									}
								}
							}						
						}
					}
					
					//---
					// Texture area
					//---
					GUI.Label(new Rect(10, 250, 100,20), "Textures:");
					GUI.Box(textureAreaRect, "");
					textureViewRect.width = (105.0f * footDef.textures.Count);
					footDef.textureScroll = GUI.BeginScrollView(textureAreaRect, footDef.textureScroll, textureViewRect);
						for(int j = 0; j < footDef.textures.Count; j++)
						{
							Rect labelRect;
							Rect previewRect;
							if(j == 0)	
							{
								labelRect = new Rect(5, 5, 100, 20);
								previewRect = new Rect(5, 20, 100, 100);
							}
							else
							{
								labelRect = new Rect(105 * j + 5, 5, 100, 135);
								previewRect = new Rect(105 * j + 5, 20, 100, 100);
							}
						
							GUI.Label(labelRect, footDef.textures[j].name);
							EditorGUI.DrawPreviewTexture( previewRect, footDef.textures[j] );
						
							if(GUI.Button( new Rect(previewRect.x+80, previewRect.y, 20, 20), "x") )
							{
								footDef.textures.RemoveAt( j );
								GUI.changed = true;
							}	
					}
					GUI.EndScrollView();
					
					if(Event.current.type == EventType.DragExited)
					{			
						if( textureAreaRect.Contains( Event.current.mousePosition ) )
						{
							//Take the selection and append it to the array
							Object[] clips = DragAndDrop.objectReferences;
									
							foreach ( Object obj in clips )
							{
								if( obj is Texture2D )
								{
									Texture2D texture = (Texture2D) obj;
									if( !footDef.textures.Contains(texture) )
									{
					            	footDef.textures.Add(texture);
										GUI.changed = true;
									}
								}
							}													
						}
					}
				}
				else
				{
					GUI.Label(new Rect(this.position.width/2 - 125, this.position.height/2-25, 250, 50), "No definitions exist for this type. \nPlease add one using the '+' icon at the bottom.");
				}
				
				if(GUI.Button(new Rect(this.position.width - 39, this.position.height - 45, 32, 32), "+"))
				{			
					FootstepNameDialogue window = (FootstepNameDialogue)EditorWindow.GetWindow<FootstepNameDialogue>();
					window.position = new Rect( ( Screen.currentResolution.width / 2 ) - 150, ( Screen.currentResolution.height /2 ) - 40, 300,80);
					window.Show();
				}
			}
			
			//--- save any changes and force an UI update
	 		if( GUI.changed )
			{
				if( ! EditorApplication.isPlaying )
				{
					EditorUtility.SetDirty( footstepDef );
					AssetDatabase.SaveAssets();
				}
				else
				{
					_needRefresh = true;
				}
				
				this.Repaint();
			}
		}
		
	
		//---
		//
		//---
		private void ShowDefinitionSelect()
		{
			List<string> temp = new List<string>();
			foreach(FootstepControlDefinition def in footstepDef.footsteps)
			{
				if(def.type == typeSelect)
					temp.Add(def.name);
			}
			
			if(temp.Count > 0)
			{
				GUI.Label(new Rect(5, 35, 150, 20), "Select a definition to edit: ");
				defIndex = EditorGUI.Popup(new Rect(155, 37, 100, 32), defIndex, temp.ToArray() );
				
				if(defIndex != currentIndex)
				{
					//If it is find us a new one
					foreach(FootstepControlDefinition def in footstepDef.footsteps)
					{
						if(def.type == typeSelect && def.name == temp[defIndex])
						{
							currentIndex = defIndex;
							selectedDefIndex = footstepDef.footsteps.IndexOf( def );
						}
					}
				}
				
				showEditor = true;
			}
			else 
			{
				showEditor = false;
			}
		}
		
		
		//---
		// load footstep definions from an AssetDatabase
		//---
		private static void LoadDefinitions()
		{
			string assetPath = "";
			
			assetPath  = ( EditorApplication.isPlaying ) ? assetPlayPath : assetEditPath;
			
			//Check if our scriptable object exists
			//Don't question why I use ImportAsset.. it just makes this work. Don't touch it.
			Debug.Log( "load defs " + assetPath );
			AssetDatabase.ImportAsset( assetPath );
			footstepDef = (FootstepControlDefinitions)AssetDatabase.LoadAssetAtPath( assetPath, typeof(FootstepControlDefinitions));
			
			if(footstepDef == null)
			{
				//Check if directory exists
				if(!Directory.Exists(Application.dataPath+"/Resources"))
				{
					Directory.CreateDirectory(Application.dataPath+"/Resources");
					EditorApplication.RepaintProjectWindow(); //This may actually not work as intended	
				}
				
				footstepDef = (FootstepControlDefinitions)ScriptableObject.CreateInstance(typeof(FootstepControlDefinitions));
				AssetDatabase.CreateAsset(footstepDef, assetPath);
				AssetDatabase.ImportAsset( assetPath );
			}		
		}
	}
}