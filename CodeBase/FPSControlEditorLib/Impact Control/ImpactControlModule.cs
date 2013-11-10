using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPSControl;
using FPSControl.Definitions;

namespace FPSControlEditor
{
    public class ImpactControlModule : FPSControlEditorModule
    {
         #region Keys
        //internal const string SAVED_DURING_PLAY = "_FPSControl_ImpactsSavedDuringPlay";
        //internal const string EDITED_BEFORE_PLAY = "_FPSControl_ImpactsEditedBeforePlay";
        //internal const string WAS_PLAYING_WHEN_LOST_FOCUS = "_FPSControl_ImpactsFocusedBeforePlay";
        //internal const string JUST_LEFT_TO_CREATE_NEW = "_FPSControl_LeftImpactsToCreateNew";
        #endregion // Keys
        static string TEMP_DEF_PATH { get { return FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "_TempImpacts.asset"; } }
        //Sstatic Rect windowSize;
	
		//Rect audioViewRect = new Rect(0, 0, 180, 1);
		Rect audioAreaRect = new Rect(225, 155, 210, 75);
        Rect particleAreaRect = new Rect(15, 155, 210, 75);
        Rect decalAreaRect = new Rect(430, 155, 210, 75);
		Rect textureViewRect = new Rect(0, 0, 1, 130);
		Rect textureAreaRect = new Rect(30, 380, 620, 135);
		int defIndex = 0;
		int currDefIndex = -1;
        int groupIndex = 0;
        ImpactControlDefinition currentDef = null;
		bool showEditor = false;
        bool settingName = false;
		string prevName;
			
		//GFX
		Texture background;
        Texture itemBG;
		//Texture closeButton;
		GUIStyle TextStyle = new GUIStyle();
		//GUIStyle PopupStyle = new GUIStyle();
		GUIStyle ButtonStyle = new GUIStyle();
      	Texture precisionBG;
        ImpactControl component;

		//Path
		const string PATH = "Impact/";

		#region GUI Properties

		Rect areaRect = new Rect(245, 50, 660, 580);

		#endregion // GUI Properties
		
		//bool drawPrecisionBox = false;
		//Rect precisionBox = new Rect();
		//float precisionValue = 0F;
		float[] allVals = new float[4];

        static List<ImpactControlDefinition> icDefs = new List<ImpactControlDefinition>();
        static List<string> defNames = new List<string>();

		public static ImpactControlDefinitions impactDef;
		
        static bool justGrabbed = false;

        static string currentName = null;

        private static string assetEditPath = FPSControlMainEditor.RESOURCE_FOLDER + "ImpactControlDefinitions.asset";

        public ImpactControlModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.ImpactControl;
        }
 
		public override void Init()
		{
            base.Init();
            LoadAssets();
            currentDef = null;
            currentName = null;
            defIndex = 0;
            currDefIndex = -1;
            groupIndex = 0;

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
            ImpactControl[] scripts = (ImpactControl[])Resources.FindObjectsOfTypeAll(typeof(ImpactControl));

            if (scripts.Length > 0)
            {
                bool setupCorrect = false;
                for (int i = 0; i < scripts.Length; i++)
                {
                    ImpactControl script = scripts[i];
                    FPSControlPlayer player = script.GetComponent<FPSControlPlayer>();
                    if (player)
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
                    
                    if(EditorUtility.DisplayDialog("Component not found!", "There is no ImpactControl Component attached to an FPSControlPlayer in this scene. Create one?", "OK", "Cancel"))
                    {
                        FPSControlPlayer[] players = (FPSControlPlayer[])Resources.FindObjectsOfTypeAll(typeof(FPSControlPlayer));
                        if (players.Length > 0)
                        {
                            foreach (FPSControlPlayer c in players)
                            {
                                if (c.GetComponent<ImpactControl>() == null) c.gameObject.AddComponent<ImpactControl>();
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error!", "There is no FPSControlPlayer in this scene. Insure that you have a correctly set up character in your scene.", "OK");
                        }
                    }
                     
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("Component not found!", "There is no ImpactControl Component attached to an FPSControlPlayer in this scene. Create one?", "OK", "Cancel"))
                {
                    FPSControlPlayer[] players = (FPSControlPlayer[])Resources.FindObjectsOfTypeAll(typeof(FPSControlPlayer));
                    if (players.Length > 0)
                    {
                        foreach (FPSControlPlayer c in players)
                        {
                            if (c.GetComponent<ImpactControl>() == null) c.gameObject.AddComponent<ImpactControl>();
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error!", "There is no FPSControlPlayer in this scene. Insure that you have a correctly set up character in your scene.", "OK");
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
            //itemBG = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "button.png");
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
            if (!rebuild) SaveTempDefinitions(impactDef);
            base.OnLostFocus(rebuild);
         }

		
		//---
		//
		//---
	    public override void OnGUI() 
		{            
            if ((impactDef == null))
            {
               // LoadDefinitions();
            }

            Color c = GUI.backgroundColor;
            
            int depth = GUI.depth;

     		GUI.DrawTexture(areaRect, background);
			GUILayout.BeginArea(MODULE_SIZE);

            ShowDefinitionsGroups();

        //    GUI.enabled = showEditor;
            ImpactControlDefinition aDef = currentDef;

            if (currentDef == null)
            {
                showEditor = false;
                aDef = new ImpactControlDefinition();
            }

            GUI.enabled = true;

            //---
            // particles area
            //---       
            GUILayout.BeginArea(particleAreaRect);

            aDef.particleScroll = GUILayout.BeginScrollView(aDef.particleScroll, new GUILayoutOption[0] { });

            GUIStyle btnBG = new GUIStyle();
            //btnBG.normal.background = itemBG as Texture2D;
            //btnBG.normal.textColor = Color.white;
            //btnBG.alignment = TextAnchor.MiddleLeft;
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            bool even = true;
            GUILayout.BeginVertical();
            for (int i = 0; i < aDef.particles.Count; i++)
            {
                if (!showEditor) break;
                GUILayout.Space(3);

                GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label(aDef.particles[i].effectObj.name, btnBG, new GUILayoutOption[1] { GUILayout.Height(20) });
                bool useNormal = aDef.particles[i].normalRotate;
                string norm = useNormal ? "N" : "U";

                if (GUILayout.Button(norm, btnBG, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(20) }))
                {
                    aDef.particles[i].normalRotate = !useNormal;
                }

                if (GUILayout.Button("X", btnBG, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(20) }))
                {
                    aDef.particles.RemoveAt(i);
                    GUI.changed = true;
                }

                GUILayout.EndHorizontal();

                //GUI.depth--;
                //GUI.backgroundColor = (even) ? Color.white : Color.black;
                //GUI.Box(GUILayoutUtility.GetLastRect(), "");
                //GUI.depth++;
                //GUI.backgroundColor = c;

                if (i == aDef.particles.Count - 1) GUILayout.Space(3);
                even = !even;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
					
			//---
			// Audio area
			//---       
			GUILayout.BeginArea(audioAreaRect);

            aDef.audioScroll = GUILayout.BeginScrollView(aDef.audioScroll, new GUILayoutOption[0] { });

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            even = true;
            GUILayout.BeginVertical();
            for (int i = 0; i < aDef.sounds.Count; i++)
			{
                if (!showEditor) break;
                GUILayout.Space(3);
                        
                GUILayout.BeginHorizontal();
                        
                GUILayout.Space(10);
                GUILayout.Label(aDef.sounds[i].name, btnBG, new GUILayoutOption[1] { GUILayout.Height(20) });
                if (GUILayout.Button("X", btnBG, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(20) }))
				{
                    aDef.sounds.RemoveAt(i);
					GUI.changed = true;
				}

                GUILayout.EndHorizontal();

                //GUI.depth--;
                //GUI.backgroundColor = (even) ? Color.white : Color.black;
                //GUI.Box(GUILayoutUtility.GetLastRect(), "");
                //GUI.depth++;
                //GUI.backgroundColor = c;

                if (i == aDef.sounds.Count - 1) GUILayout.Space(3);
                even = !even;
			}
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();					
			GUILayout.EndScrollView();
            GUILayout.EndArea();

            //---
            // decal area
            //---       
            GUILayout.BeginArea(decalAreaRect);

            aDef.decalScroll = GUILayout.BeginScrollView(aDef.decalScroll, new GUILayoutOption[0] { });

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            even = true;
            GUILayout.BeginVertical();
            for (int i = 0; i < aDef.decals.Count; i++)
            {
                if (!showEditor) break;
                GUILayout.Space(3);

                GUILayout.BeginHorizontal();

                GUILayout.Space(10);
                GUILayout.Label(aDef.decals[i].name, btnBG, new GUILayoutOption[1] { GUILayout.Height(20) });
                if (GUILayout.Button("X", btnBG, new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(20) }))
                {
                    aDef.decals.RemoveAt(i);
                    GUI.changed = true;
                }

                GUILayout.EndHorizontal();

                //GUI.depth--;
                //GUI.backgroundColor = (even) ? Color.white : Color.black;
                //GUI.Box(GUILayoutUtility.GetLastRect(), "");
                //GUI.depth++;
                //GUI.backgroundColor = c;

                if (i == aDef.decals.Count - 1) GUILayout.Space(3);
                even = !even;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            Knobs.Theme(Knobs.Themes.BLACK, _editor);
            GUI.enabled = showEditor;
            allVals[0] = aDef.minValPitch = Knobs.MinMax(new Vector2(220, 270), aDef.minValPitch, aDef.minLimitPitch, aDef.maxLimitPitch, 0);
            allVals[1] = aDef.maxValPitch = Knobs.MinMax(new Vector2(280, 270), aDef.maxValPitch, aDef.minLimitPitch, aDef.maxLimitPitch, 1);
            allVals[2] = aDef.minValVolume = Knobs.MinMax(new Vector2(360, 270), aDef.minValVolume, aDef.minLimitVolume, aDef.maxLimitVolume, 2);
            allVals[3] = aDef.maxValVolume = Knobs.MinMax(new Vector2(420, 270), aDef.maxValVolume, aDef.minLimitVolume, aDef.maxLimitVolume, 3);

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
            aDef.tag = EditorGUI.TagField(new Rect(30, 550, 70, 15), aDef.tag);

            GameObject[] particles;
            if (Drag.DragArea<GameObject>(particleAreaRect, out particles, Drag.Styles.Hidden) == DragResultState.Drag)
            {
                bool exists = false;
                foreach (GameObject p in particles)
                {
                    foreach (ImpactControlEffect effect in aDef.particles)
                    {
                        if (effect.effectObj == p)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        ImpactControlEffect effect = new ImpactControlEffect();
                        effect.effectObj = p;
                        aDef.particles.Add(effect);
                        //GUI.changed = true;
                    }
                }
            }

            AudioClip[] clips;
            if (Drag.DragArea<AudioClip>(audioAreaRect, out clips, Drag.Styles.Hidden) == DragResultState.Drag)
            {
                foreach (AudioClip clip in clips)
                {
                    if (!aDef.sounds.Contains(clip))
                    {
                        aDef.sounds.Add(clip);
                        //GUI.changed = true;
                    }
                }
            }

            GameObject[] decals;
            if (Drag.DragArea<GameObject>(decalAreaRect, out decals, Drag.Styles.Hidden) == DragResultState.Drag)
            {
                foreach (GameObject obj in decals)
                {
                    if (!aDef.decals.Contains(obj))
                    {
                        aDef.decals.Add(obj);
                        //GUI.changed = true;
                    }
                }
            }

			//---
			// Texture area
			//---
            textureViewRect.width = (105.0f * aDef.textures.Count);

            GUILayout.BeginArea(textureAreaRect);

            aDef.textureScroll = GUILayout.BeginScrollView(aDef.textureScroll, new GUILayoutOption[0] { });
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            for (int j = 0; j < aDef.textures.Count; j++)
			{
                if (!showEditor) break;
                GUILayout.Box("", GUIStyle.none, new GUILayoutOption[2] { GUILayout.Width(110), GUILayout.Height(110) });

                Rect previewRect = GUILayoutUtility.GetLastRect();//new Rect(120 * j + 5, 15, 110, 115);

                EditorGUI.DrawPreviewTexture(previewRect, aDef.textures[j]);

                if (GUI.Button(new Rect(previewRect.x + 85, previewRect.y + 5, 20, 20), "X", ButtonStyle))
				{
                    aDef.textures.RemoveAt(j);
					GUI.changed = true;
				}

                if (j < aDef.textures.Count - 1) GUILayout.Space(5);
			}
            GUILayout.EndHorizontal();
			GUILayout.EndScrollView();

            GUILayout.EndArea();

            Texture2D[] textures;
            if (Drag.DragArea<Texture2D>(textureAreaRect, out textures, Drag.Styles.Hidden) == DragResultState.Drag)
            {
                foreach (Texture2D t in textures)
                {
                    if (!aDef.textures.Contains(t))
                    {
                        aDef.textures.Add(t);
                        GUI.changed = true;
                    }
                }
            }


            if (currentName == null)
            {
                Rect labelRect = new Rect(0, 85, MODULE_SIZE.width, MODULE_SIZE.height - 125);//                    660/2 - 125, 578/2-25, 300, 50);
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
                GUI.Label(labelRect, "No Impact definitions defined. \nPlease add one using the \"New Definition\" button at the top.", gs);
            }
			else if(!showEditor)
			{
				Rect labelRect = new Rect(0,125,MODULE_SIZE.width,MODULE_SIZE.height-145);//                    660/2 - 125, 578/2-25, 300, 50);
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
                GUI.Label(labelRect, "No groups exist for this definition. \nPlease add one using the \"New Group\" button at the top.", gs);
			}

            GUI.enabled = true;
			
			//--- save any changes and force an UI update
	 		if( GUI.changed )
			{
				if( ! EditorApplication.isPlaying )
				{
					EditorUtility.SetDirty( impactDef );
					AssetDatabase.SaveAssets();
				}
				else
				{
					//_needRefresh = true;
				}
				
				this.Repaint();
			}

            //--- Terrain Check Button
            // TODO: this can look better
            GUIContent terrainGC = new GUIContent("Check Terrain", "Enable checking Impacts against terrain");
            impactDef.terrainCheck = GUI.Toggle(new Rect(300, 10, 105, 35), impactDef.terrainCheck, terrainGC);

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
            ImpactControlDefinitions impactDef = ImpactControlModule.impactDef;

            if (currentName == null)
            {
                settingName = false;

                foreach (ImpactControlDefinition def in impactDef.impacts)
                {
                    if (def.name == name)
                    {
                        Debug.LogWarning("Definitions already contains " + name);
                        currentName = prevName;
                        return;
                    }
                }

                currentName = name;
                defNames.Add(currentName);
                defIndex = defNames.Count - 1;
                return;
            }

            foreach (ImpactControlDefinition def in impactDef.impacts)
            {
                if (def.name == currentName && def.group == name)
                {
                    Debug.LogWarning("Definition " + currentName + " already contains " + name);
                    exists = true;
                }
            }

            if (!exists)
            {
                ImpactControlDefinition newDef = new ImpactControlDefinition();
                newDef.name = currentName;
                newDef.group = name;
                newDef.audioScroll = Vector2.zero;
                newDef.particleScroll = Vector2.zero;
                newDef.decalScroll = Vector2.zero;
                newDef.textureScroll = Vector2.zero;
                newDef.minValPitch = -1f;
                newDef.minLimitPitch = -3f;
                newDef.maxValPitch = 1f;
                newDef.maxLimitPitch = 3f;
                newDef.minValVolume = .2f;
                newDef.minLimitVolume = 0f;
                newDef.maxValVolume = .5f;
                newDef.maxLimitVolume = 1f;

                impactDef.impacts.Add(newDef);
                RecollectFiltered();
            }
        }
		
	
		//---
		//
		//---
		private void ShowDefinitionsGroups()
		{
            List<string> groups = new List<string>();
            List<ImpactControlDefinition> temp = new List<ImpactControlDefinition>();

            if (defNames.Count > 0)
            {
                showEditor = true;

                defIndex = EditorGUI.Popup(new Rect(205, 60, 150, 15), defIndex, defNames.ToArray());

                if (!settingName)
                    currentName = defNames[defIndex];

				if(defIndex != currDefIndex)
				{
					//If it is find us a new one
                    currDefIndex = defIndex;
                    groupIndex = 0;

                    RecollectFiltered(false);
				}

                foreach (ImpactControlDefinition def in icDefs)
				{
                    if (!groups.Contains(def.group))
					{
                        groups.Add(def.group);
                        temp.Add(def);
					}
				}

                if (groups.Count > 0)
                {
                    groupIndex = EditorGUI.Popup(new Rect(205, 100, 150, 15), groupIndex, groups.ToArray());

                    currentDef = temp[groupIndex];
                }
                else
                {
                    currentDef = null;
                    EditorGUI.Popup(new Rect(205, 100, 150, 15), 0, groups.ToArray());
                }
			}
			else 
			{
                currentDef = null;
                EditorGUI.Popup(new Rect(205, 60, 150, 15), 0, defNames.ToArray());
			}

            GUI.enabled = defNames.Count > 0;

            if (GUI.Button(new Rect(355, 60, 20, 20), "X", ButtonStyle))
            {
                for (int i = impactDef.impacts.Count - 1; i >= 0; --i)
                {
                    currentDef = impactDef.impacts[i];

                    if (currentDef.name == currentName)
                    {
                        impactDef.impacts.RemoveAt(i);
                    }
                }

                defIndex = 0;
                currDefIndex = -1;
                groupIndex = 0;
                currentName = null;
                currentDef = null;

                RecollectFiltered();
            }

            GUI.enabled = true;

            if (GUI.Button(new Rect(380, 60, 130, 20), "New Definition"))
            {
                prevName = currentName;
                currentName = null;
                settingName = true;
                Prompt("Enter a name for the new Impact Definition.");
            }

            GUI.enabled = groups.Count > 0;

            if (GUI.Button(new Rect(355, 98, 20, 20), "X", ButtonStyle))
            {
                if (currentDef != null)
                    impactDef.impacts.Remove(currentDef);

                currentDef = null;
                groupIndex = 0;

                RecollectFiltered();
            }

            GUI.enabled = currentName != null;

            if (GUI.Button(new Rect(380, 98, 130, 20), "New Group"))
            {
                Prompt("Enter a name for the new Impact Group.");
            }
		}
		
		
		//---
		// load Impact definions from an AssetDatabase
		//---
		public static void LoadDefinitions()
		{
			string assetPath = "";

            assetPath = assetEditPath;//( EditorApplication.isPlaying ) ? assetPlayPath : assetEditPath;
			
			//Check if our scriptable object exists
			//Don't question why I use ImportAsset.. it just makes this work. Don't touch it.
			//Debug.Log( "load defs " + assetPath );
			AssetDatabase.ImportAsset( assetPath );

            ImpactControlDefinitions  loadedDef = (ImpactControlDefinitions)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ImpactControlDefinitions));

            if (loadedDef == null)
			{
				//Check if directory exists
                if (!Directory.Exists(FPSControlMainEditor.RESOURCE_FOLDER))
				{
                    Directory.CreateDirectory(FPSControlMainEditor.RESOURCE_FOLDER);
					EditorApplication.RepaintProjectWindow(); //This may actually not work as intended	
				}

                loadedDef = (ImpactControlDefinitions)ScriptableObject.CreateInstance(typeof(ImpactControlDefinitions));
                AssetDatabase.CreateAsset(loadedDef, assetPath);
				AssetDatabase.ImportAsset( assetPath );
			}

            impactDef = (ImpactControlDefinitions) Object.Instantiate(loadedDef);

            RecollectFiltered();
		}

        internal static void RecollectFiltered(bool resetNames = true)
        {
            icDefs.Clear();

            if (resetNames)
            {
                defNames.Clear();
                foreach (ImpactControlDefinition def in impactDef.impacts)
                {
                    if (!defNames.Contains(def.name))
                        defNames.Add(def.name);
                }
            }

            if (currentName == null)
            {
                return;
            }
     
            for (int i = 0; i < impactDef.impacts.Count; i++)
            {
                if (impactDef.impacts[i].name == currentName)
                    icDefs.Add(impactDef.impacts[i]);
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
            AssetDatabase.CreateAsset(impactDef, assetPath);
            AssetDatabase.ImportAsset(assetPath);

            LoadDefinitions();
            //ImpactDef = (ImpactControlDefinitions)Object.Instantiate(ImpactDef);

        }

        public static void SaveTempDefinitions(ImpactControlDefinitions tmpDefs)
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
            ImpactControlDefinitions loadedDef = (ImpactControlDefinitions) AssetDatabase.LoadAssetAtPath(TEMP_DEF_PATH, typeof(ImpactControlDefinitions));
            //Debug.Log("Loaded: " + loadedDef);

            if (loadedDef == null)
            {
                //Debug.LogWarning("Temporary Definition is null. Loading from saved resource.");
                LoadDefinitions();
            }
            else
            {
                impactDef = (ImpactControlDefinitions)Object.Instantiate(loadedDef);
                RecollectFiltered();
                AssetDatabase.DeleteAsset(TEMP_DEF_PATH);
                justGrabbed = true;
                
            }
            
        }
	}
}
