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
	public class FootstepControlModule : FPSControlEditorModule 
	{
        #region Keys
        //internal const string SAVED_DURING_PLAY = "_FPSControl_FootstepsSavedDuringPlay";
        //internal const string EDITED_BEFORE_PLAY = "_FPSControl_FootstepsEditedBeforePlay";
        //internal const string WAS_PLAYING_WHEN_LOST_FOCUS = "_FPSControl_FootstepsFocusedBeforePlay";
        //internal const string JUST_LEFT_TO_CREATE_NEW = "_FPSControl_LeftFootstepsToCreateNew";
        #endregion // Keys
        static string TEMP_DEF_PATH { get { return FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "_TempFootsteps.asset"; } }
        //Sstatic Rect windowSize;
	
		//Rect audioViewRect = new Rect(0, 0, 180, 1);
		Rect audioAreaRect = new Rect(25, 160, 210, 135);
		Rect textureViewRect = new Rect(0, 0, 1, 130);
		Rect textureAreaRect = new Rect(25, 330, 620, 135);
		int defIndex = 0;
		int currentIndex = 0;
		bool showEditor = false;
		int selectedDefIndex;
			
		//GFX
		Texture background;
		//Texture closeButton;
		GUIStyle TextStyle = new GUIStyle();
		//GUIStyle PopupStyle = new GUIStyle();
		GUIStyle ButtonStyle = new GUIStyle();
      	Texture precisionBG;
        FootstepControl component;

		//Path
		const string PATH = "Footstep/";

		#region GUI Properties

		Rect areaRect = new Rect(245, 50, 660, 580);

		#endregion // GUI Properties
		
		//bool drawPrecisionBox = false;
		//Rect precisionBox = new Rect();
		//float precisionValue = 0F;
		float[] allVals = new float[4];

        static List<FootstepControlDefinition> playerDefs = new List<FootstepControlDefinition>();
        static List<FootstepControlDefinition> npcDefs = new List<FootstepControlDefinition>();

		public static FootstepControlDefinitions footstepDef;
		public static FOOTSTEPTYPES typeSelect = FOOTSTEPTYPES.Player;
		
        static bool justGrabbed = false;

        private static string assetEditPath = FPSControlMainEditor.RESOURCE_FOLDER + "FootstepControlDefinitions.asset";

        public FootstepControlModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.Footsteps;
        }
 
		public override void Init()
		{
            base.Init();
            LoadAssets();

            if (wasPlaying != Application.isPlaying)
            {
                LoadTempDefinitions();
            }
            else
            {
                LoadDefinitions();
            }

            AcquireTarget();
		}

        public override void Deinit()
        {
            base.Deinit();
        }

        void AcquireTarget()
        {
            FootstepControl[] scripts = (FootstepControl[])Resources.FindObjectsOfTypeAll(typeof(FootstepControl));

            if (scripts.Length > 0)
            {
                bool setupCorrect = false;
                for (int i = 0; i < scripts.Length; i++)
                {
                    FootstepControl script = scripts[i];
                    FPSControlPlayerMovement playerMovement = script.GetComponent<FPSControlPlayerMovement>();
                    if (playerMovement)
                    {
                        component = script;
                        setupCorrect = true;
                        break;
                    }
                }
                if (setupCorrect)
                {
                    //we're good, don't worry
                }
                else
                {
                    if(EditorUtility.DisplayDialog("Component not found!", "There is no FootstepControl Component attached to an FPSControlPlayerMovement in this scene. Create one?", "OK", "Cancel"))
                    {
                        FPSControlPlayerMovement[] playerMovements = (FPSControlPlayerMovement[])Resources.FindObjectsOfTypeAll(typeof(FPSControlPlayerMovement));
                        if (playerMovements.Length > 0)
                        {
                            foreach (FPSControlPlayerMovement c in playerMovements)
                            {
                                if (c.GetComponent<FootstepControl>() == null) c.gameObject.AddComponent<FootstepControl>();
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error!", "There is no FPSControlPlayerMovement in this scene. Insure that you have a correctly set up character in your scene.", "OK");
                        }
                    }
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("Component not found!", "There is no FootstepControl Component attached to an FPSControlPlayerMovement in this scene. Create one?", "OK", "Cancel"))
                {
                    FPSControlPlayerMovement[] playerMovements = (FPSControlPlayerMovement[])Resources.FindObjectsOfTypeAll(typeof(FPSControlPlayerMovement));
                    if (playerMovements.Length > 0)
                    {
                        foreach (FPSControlPlayerMovement c in playerMovements)
                        {
                            if (c.GetComponent<FootstepControl>() == null) c.gameObject.AddComponent<FootstepControl>();
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error!", "There is no FPSControlPlayerMovement in this scene. Insure that you have a correctly set up character in your scene.", "OK");
                    }
                }
            }
        }

        void LoadAssets()
        {
            TextStyle.normal.textColor = Color.white;
            ButtonStyle = EditorStyles.miniButton;
            background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "background.png");
            precisionBG = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "precision_box_bg.png");

            //Knobs.OnInteractionBegan = OnPress;
            //Knobs.OnInteract = OnDrag;
            //Knobs.OnInteractionEnd = OnRelease;
        }
		
		//---
		//
		//---
		override public void OnFocus(bool rebuild)
		{            
            //Debug.Log("Focused.");
            allVals = new float[4];
            LoadAssets();
            if (!rebuild && !justReturnedFromPopup) LoadTempDefinitions();
            //else Debug.Log("just rebuilt or came back from popup.");
            base.OnFocus(rebuild);
		}

        override public void OnLostFocus(bool rebuild)
        {            
            //Debug.Log("Lost Focus.");
            justGrabbed = false;
            if (justReturnedFromPopup) return;
            if (!rebuild) SaveTempDefinitions(footstepDef);
            base.OnLostFocus(rebuild);
         }

	
        //void OnPress(Vector2 mPos)
        //{
        //   drawPrecisionBox = true;
        //}
		
        //void OnRelease(Vector2 mPos)
        //{
        //   drawPrecisionBox = false;
        //}
		
        //void OnDrag(Vector2 mPos)
        //{
        //   if (!drawPrecisionBox)
        //   {
        //       _editor.Repaint();
        //       return;
        //   }
		
        //   precisionBox = new Rect(mPos.x - 18, mPos.y + 45, 80, 25);
        //   precisionValue = Knobs.interactValue;
        //}


        //void AudioDragArea(ref FootstepControlDefinition footDef, Rect r, bool drawDebug)
        //{
        //    Event evt = Event.current;

        //    if (drawDebug)
        //    {
        //        Color c = GUI.backgroundColor;

        //        GUI.backgroundColor = Color.yellow;
        //        GUI.Box(r, "DEBUG AUDIO DRAG AREA");
        //        GUI.backgroundColor = c;
        //    }

        //    switch (evt.type)
        //    {
        //        case EventType.DragUpdated:
        //        case EventType.DragPerform:
        //            if (!r.Contains(evt.mousePosition))
        //                return;

        //            DragAndDrop.visualMode = DragAndDropVisualMode.Link;

        //            if (evt.type == EventType.DragPerform)
        //            {

        //                Debug.Log("drop! " + DragAndDrop.objectReferences.Length);

        //                DragAndDrop.AcceptDrag();

        //                Object[] clips = DragAndDrop.objectReferences;

        //                foreach (Object obj in clips)
        //                {
        //                    if (obj is AudioClip)
        //                    {
        //                        AudioClip clip = (AudioClip)obj;
        //                        if (!footDef.sounds.Contains(clip))
        //                        {
        //                            footDef.sounds.Add(clip);
        //                            GUI.changed = true;
        //                        }
        //                    }
        //                }
        //            }

        //            break;
        //    }
        //}

        //void TextureDragArea(ref FootstepControlDefinition footDef, Rect r, bool drawDebug)
        //{
        //    Event evt = Event.current;

        //    if (drawDebug)
        //    {
        //        Color c = GUI.backgroundColor;

        //        GUI.backgroundColor = Color.blue;
        //        GUI.Box(r, "DEBUG TEXTURE DRAG AREA");
        //        GUI.backgroundColor = c;
        //    }

        //    switch (evt.type)
        //    {
        //        case EventType.DragUpdated:
        //        case EventType.DragPerform:
        //            if (!r.Contains(evt.mousePosition))
        //                return;

        //            DragAndDrop.visualMode = DragAndDropVisualMode.Link;

        //            if (evt.type == EventType.DragPerform)
        //            {

        //                Debug.Log("drop! " + DragAndDrop.objectReferences.Length);

        //                DragAndDrop.AcceptDrag();

        //                Object[] clips = DragAndDrop.objectReferences;

        //                foreach (Object obj in clips)
        //                {
        //                    if (obj is Texture2D)
        //                    {
        //                        Texture2D texture = (Texture2D)obj;
        //                        if (!footDef.textures.Contains(texture))
        //                        {
        //                            footDef.textures.Add(texture);
        //                            GUI.changed = true;
        //                        }
        //                    }
        //                }
        //            }

        //            break;
        //    }
        //}
		
		//---
		//
		//---
	    public override void OnGUI() 
		{            
            
            if ((footstepDef == null))
            {
               // LoadDefinitions();
            }

            Color c = GUI.backgroundColor;
            
            int depth = GUI.depth;

     		GUI.DrawTexture(areaRect, background);
			GUILayout.BeginArea(MODULE_SIZE);
            
            typeSelect = (FOOTSTEPTYPES)EditorGUI.EnumPopup(new Rect(125, 64, 130, 15), typeSelect);

			ShowDefinitionSelect();

            GUI.enabled = showEditor;

            FootstepControlDefinition[] filteredDefs = ((typeSelect == FOOTSTEPTYPES.Player) ? playerDefs : npcDefs).ToArray();
            FootstepControlDefinition footDef;
            
            if (filteredDefs.Length <= 0)
            {
                showEditor = false;
                footDef = new FootstepControlDefinition();
            }
            else if (selectedDefIndex >= filteredDefs.Length)
            {
                selectedDefIndex = filteredDefs.Length - 1;
                footDef = filteredDefs[selectedDefIndex];
            }
            else
            {
                footDef = filteredDefs[selectedDefIndex];
            }

            
					
			//--- Close Button
            GUI.enabled = !(filteredDefs.Length <= 0);
            if (GUI.Button(new Rect(660 - 300, 98, 20, 20), "X", ButtonStyle))
			{
                footstepDef.footsteps.Remove(filteredDefs[selectedDefIndex]);
                RecollectFiltered();
			}
            GUI.enabled = true;
					
			//---
			// Audio area
			//---
                    
			GUILayout.BeginArea(audioAreaRect);
                    
            footDef.audioScroll = GUILayout.BeginScrollView(footDef.audioScroll, new GUILayoutOption[0]{});

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            bool even = true;
            GUILayout.BeginVertical();
            for(int i = 0; i < footDef.sounds.Count; i++)
			{
                if (!showEditor) break;
                GUILayout.Space(3);
                        
                GUILayout.BeginHorizontal();
                        
                GUILayout.Space(10);
                GUILayout.Label(footDef.sounds[i].name, new GUILayoutOption[1] {GUILayout.Height(20) });
                if (GUILayout.Button("X",new GUILayoutOption[2]{GUILayout.Width(20),GUILayout.Height(20)}))
				{
					footDef.sounds.RemoveAt( i );
					GUI.changed = true;
				}

                GUILayout.EndHorizontal();

                //GUI.depth--;
                //GUI.backgroundColor = (even) ? Color.white : Color.black;
                //GUI.Box(GUILayoutUtility.GetLastRect(), "");
                //GUI.depth++;
                //GUI.backgroundColor = c;
                         
                if (i == footDef.sounds.Count - 1) GUILayout.Space(3);
                even = !even;
			}
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();					
			GUILayout.EndScrollView();
            GUILayout.EndArea();


            Knobs.Theme(Knobs.Themes.BLACK, _editor);
            GUI.enabled = showEditor;
            allVals[0] = footDef.minValPitch = Knobs.MinMax(new Vector2(280,192),footDef.minValPitch,footDef.minLimitPitch,footDef.maxLimitPitch,0);
            allVals[1] = footDef.maxValPitch = Knobs.MinMax(new Vector2(340,192),footDef.maxValPitch,footDef.minLimitPitch,footDef.maxLimitPitch,1);
            allVals[2] = footDef.minValVolume = Knobs.MinMax(new Vector2(420,192),footDef.minValVolume,footDef.minLimitVolume,footDef.maxLimitVolume,2);
            allVals[3] = footDef.maxValVolume = Knobs.MinMax(new Vector2(480,192),footDef.maxValVolume,footDef.minLimitVolume,footDef.maxLimitVolume,3);

            //if(Knobs.interactID != -1)
            //{
            //    GUIStyle gs = new GUIStyle();
            //    gs.normal.textColor = Color.white;
            //    gs.alignment = TextAnchor.MiddleCenter;
            //    gs.normal.background = (Texture2D) precisionBG;

            //    GUI.Box(precisionBox, ""+allVals[Knobs.interactID],gs);
            //    _editor.Repaint();
            //}
            GUI.enabled = true;				 
			footDef.tag = EditorGUI.TagField( new Rect(550,210, 70, 15), footDef.tag);

            AudioClip[] clips;
            if (Drag.DragArea<AudioClip>(audioAreaRect, out clips, Drag.Styles.Hidden) == DragResultState.Drag)
            {
                foreach (AudioClip clip in clips)
                {
                    if (!footDef.sounds.Contains(clip))
                    {
                        footDef.sounds.Add(clip);
                        //GUI.changed = true;
                    }
                }
            }

			//---
			// Texture area
			//---
			textureViewRect.width = (105.0f * footDef.textures.Count);

            GUILayout.BeginArea(textureAreaRect);

			footDef.textureScroll = GUILayout.BeginScrollView(footDef.textureScroll,new GUILayoutOption[0]{});
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            for(int j = 0; j < footDef.textures.Count; j++)
			{
                if (!showEditor) break;
                GUILayout.Box("", GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(110), GUILayout.Height(110) });

                Rect previewRect = GUILayoutUtility.GetLastRect();//new Rect(120 * j + 5, 15, 110, 115);
						
				EditorGUI.DrawPreviewTexture( previewRect, footDef.textures[j] );

                if (GUI.Button(new Rect(previewRect.x + 85, previewRect.y + 5, 20, 20), "X", ButtonStyle))
				{
					footDef.textures.RemoveAt( j );
					GUI.changed = true;
				}

                if (j < footDef.textures.Count - 1) GUILayout.Space(5);
			}
            GUILayout.EndHorizontal();
			GUILayout.EndScrollView();

            GUILayout.EndArea();

            Texture2D[] textures;
            if (Drag.DragArea<Texture2D>(textureAreaRect, out textures, Drag.Styles.Hidden) == DragResultState.Drag)
            {
                foreach (Texture2D t in textures)
                {
                    if (!footDef.textures.Contains(t))
                    {
                        footDef.textures.Add(t);
                        GUI.changed = true;
                    }
                }
            }

			if(!showEditor)
			{
				Rect labelRect = new Rect(0,85,MODULE_SIZE.width,MODULE_SIZE.height-125);//                    660/2 - 125, 578/2-25, 300, 50);
                Color clr = GUI.backgroundColor;
                GUI.backgroundColor = Color.black;
                for (int i = 0; i < 20; i++) //hack way to get a darker box
                {
                    GUI.Box(labelRect, "");
                }
                GUI.backgroundColor = clr;
                GUIStyle gs = new GUIStyle();
                gs.normal.textColor = Color.white;
                gs.alignment = TextAnchor.MiddleCenter;
                GUI.Label(labelRect, "No definitions exist for this type. \nPlease add one using the \"New Definition\" button at the bottom.", gs);
			}

            GUI.enabled = true;
				
			if(GUI.Button(new Rect(660 - 150, 578 - 30, 130, 20), "New Definition"))
			{
                Prompt("Enter a name for the new footstep sounds.");
			}

            GUI.enabled = true;
			
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
					//_needRefresh = true;
				}
				
				this.Repaint();
			}

            GUIContent toolGC = new GUIContent("Reload", "Reverts all changes to the last save.");

            if (GUI.Button(new Rect(430, 10, 110, 20), toolGC))
            {
                LoadDefinitions();
            }

            if (GUI.Button(new Rect(545, 10, 110, 20), "Save"))
            {
                SaveDefinitions();
            }

            GUILayout.EndArea();
		}

        override public void OnPromptInput(string name)
        {
            bool exists = false;
            FootstepControlDefinitions footstepDef = FootstepControlModule.footstepDef;

            foreach (FootstepControlDefinition def in footstepDef.footsteps)
            {
                if (def.type == FootstepControlModule.typeSelect && def.name == name)
                {
                    Debug.LogWarning("Definitions for type [" + FootstepControlModule.typeSelect + "] already contains " + name);
                    exists = true;
                }
            }

            if (!exists)
            {
                FootstepControlDefinition footDef = new FootstepControlDefinition();

                footDef.name = name;
                footDef.type = FootstepControlModule.typeSelect;
                footDef.audioScroll = Vector2.zero;
                footDef.textureScroll = Vector2.zero;
                footDef.minValPitch = -1f;
                footDef.minLimitPitch = -3f;
                footDef.maxValPitch = 1f;
                footDef.maxLimitPitch = 3f;
                footDef.minValVolume = .2f;
                footDef.minLimitVolume = 0f;
                footDef.maxValVolume = .5f;
                footDef.maxLimitVolume = 1f;

                FootstepControlModule.footstepDef.footsteps.Add(footDef);
                FootstepControlModule.RecollectFiltered();
            }
        }
		
	
		//---
		//
		//---
		private void ShowDefinitionSelect()
		{
			List<string> temp = new List<string>();
            foreach (FootstepControlDefinition def in (List<FootstepControlDefinition>)((typeSelect == FOOTSTEPTYPES.Player) ? playerDefs : npcDefs))
			{
				if(def.type == typeSelect)
					temp.Add(def.name);
			}
			
			if(temp.Count > 0)
			{
                defIndex = EditorGUI.Popup(new Rect(205, 100, 150, 15), defIndex, temp.ToArray());
				
				if(defIndex != currentIndex)
				{
					//If it is find us a new one
                    foreach (FootstepControlDefinition def in (List<FootstepControlDefinition>)((typeSelect == FOOTSTEPTYPES.Player) ? playerDefs : npcDefs))
					{
						if(def.type == typeSelect && def.name == temp[defIndex])
						{
							currentIndex = defIndex;
                            selectedDefIndex = (((typeSelect == FOOTSTEPTYPES.Player) ? playerDefs : npcDefs) as List<FootstepControlDefinition>).IndexOf(def);
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
		public static void LoadDefinitions()
		{
			string assetPath = "";

            assetPath = assetEditPath;//( EditorApplication.isPlaying ) ? assetPlayPath : assetEditPath;
			
			//Check if our scriptable object exists
			//Don't question why I use ImportAsset.. it just makes this work. Don't touch it.
			//Debug.Log( "load defs " + assetPath );
			AssetDatabase.ImportAsset( assetPath );

            FootstepControlDefinitions  loadedDef = (FootstepControlDefinitions)AssetDatabase.LoadAssetAtPath(assetPath, typeof(FootstepControlDefinitions));

            if (loadedDef == null)
			{
				//Check if directory exists
                if (!Directory.Exists(FPSControlMainEditor.RESOURCE_FOLDER))
				{
                    Directory.CreateDirectory(FPSControlMainEditor.RESOURCE_FOLDER);
					EditorApplication.RepaintProjectWindow(); //This may actually not work as intended	
				}

                loadedDef = (FootstepControlDefinitions)ScriptableObject.CreateInstance(typeof(FootstepControlDefinitions));
                AssetDatabase.CreateAsset(loadedDef, assetPath);
				AssetDatabase.ImportAsset( assetPath );
			}

            footstepDef = (FootstepControlDefinitions) Object.Instantiate(loadedDef);

            RecollectFiltered();
		}

        internal static void RecollectFiltered()
        {
            playerDefs = new List<FootstepControlDefinition>();
            npcDefs = new List<FootstepControlDefinition>();
            for (int i = 0; i < footstepDef.footsteps.Count; i++)
            {
                if (footstepDef.footsteps[i].type == FOOTSTEPTYPES.Player)
                    playerDefs.Add(footstepDef.footsteps[i]);
                else
                    npcDefs.Add(footstepDef.footsteps[i]);
            }
        }

        public static void SaveDefinitions()
        {
            string assetPath = "";
            assetPath = assetEditPath;// (EditorApplication.isPlaying) ? assetPlayPath : assetEditPath;

            if (!Directory.Exists(FPSControlMainEditor.RESOURCE_FOLDER))
            {
                Directory.CreateDirectory(FPSControlMainEditor.RESOURCE_FOLDER);
                //EditorApplication.RepaintProjectWindow(); //This may actually not work as intended	
            }

            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.CreateAsset(footstepDef, assetPath);
            AssetDatabase.ImportAsset(assetPath);

            LoadDefinitions();
            //footstepDef = (FootstepControlDefinitions)Object.Instantiate(footstepDef);

        }

        public static void SaveTempDefinitions(FootstepControlDefinitions tmpDefs)
        {            
            AssetDatabase.DeleteAsset(TEMP_DEF_PATH);
            AssetDatabase.CreateAsset(tmpDefs, TEMP_DEF_PATH);
        }

        public static void LoadTempDefinitions()
        {
            if (justGrabbed) //kind of a hack... but
            {
                //Debug.Log("Just grabbed...");
                justGrabbed = false;
                return;
            }
            //AssetDatabase.Refresh(ImportAssetOptions.Default);
            FootstepControlDefinitions loadedDef = (FootstepControlDefinitions) AssetDatabase.LoadAssetAtPath(TEMP_DEF_PATH, typeof(FootstepControlDefinitions));
            //Debug.Log("Loaded: " + loadedDef);

            if (loadedDef == null)
            {
                //Debug.LogWarning("Temporary Definition is null. Loading from saved resource.");
                LoadDefinitions();
            }
            else
            {
                footstepDef = (FootstepControlDefinitions)Object.Instantiate(loadedDef);
                RecollectFiltered();
                AssetDatabase.DeleteAsset(TEMP_DEF_PATH);
                justGrabbed = true;
                
            }
            
        }
	}
}