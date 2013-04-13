//=====
//=====


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using FPSControl;


//---
//
//---
namespace FPSControlEditor
{
	
	public class Node
	{
		public int type;
		public bool shown = false;
		public string name = "Unnamed Track";
		public AudioClip track;
		public float volume = 0.5f;
		public bool backgroundMusic;
		public bool loop = false;
		public float time = 0.0f;
	
		public Node ( int type  )
		{
			this.type = type;
		}
	
	}
	
	
	//---
	//
	//---
	public class MusicControlModule : FPSControlEditorModule 
	{
		Vector2 scrollPosition;
		Vector2 nodePosition;
		public MusicControl target;
		
		//GFX
		Texture background;
		Texture closeButton;
		GUIStyle TextStyle = new GUIStyle();
		GUIStyle PopupStyle = new GUIStyle();
		GUIStyle PopupLongStyle = new GUIStyle();
		GUIStyle ButtonStyle = new GUIStyle();
      Texture precisionBG;

		//Path
		const string PATH = "Music/";

		#region GUI Properties

		Rect areaRect = new Rect(245, 50, 660, 580);

		#endregion // GUI Properties
		
		bool drawPrecisionBox = false;
		Rect precisionBox = new Rect();
		float precisionValue = 0F;
		float[] allVals = new float[2];
		
		private bool _playModeChanged = false;
		
		//Node Type: 1 - Position, 2 - Rotation, 3 - Scale
		List<Node> node = new List<Node>(); 
		
		string[] filterOptions = new string[3]{"Music Tracks", "Effect Tracks", "All Tracks"};
		int filter = 2;
		
	
		//---
		//
		//---

        public MusicControlModule(EditorWindow editorWindow) : base(editorWindow)
        {
            _type = FPSControlModuleType.MusicControl;
        }

		//---
		//
		//---	    
		public override void Init()
		{
            TextStyle.normal.textColor = Color.white;
            background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "background.png");
            closeButton = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "closeButton.png");
            PopupStyle.normal.textColor = Color.white;
            PopupStyle.padding.left = 5;
            PopupStyle.normal.background = Load<Texture2D>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "popup.png");
            PopupLongStyle.normal.textColor = Color.white;
            PopupLongStyle.padding.left = 5;
            PopupLongStyle.normal.background = Load<Texture2D>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "popup_long.png");
            ButtonStyle.normal.textColor = Color.white;
            ButtonStyle.alignment = TextAnchor.MiddleCenter;
            ButtonStyle.normal.background = Load<Texture2D>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "button.png");			
            precisionBG = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "precision_box_bg.png");
            base.Init();
             //Knobs.OnInteractionBegan = OnPress;
             //Knobs.OnInteract = OnDrag;
             //Knobs.OnInteractionEnd = OnRelease;
		}
		
		
		
		
		//---
		//
		//---
		void OnEnable ()
		{
			target = null;
			CheckForTarget(true);
			Debug.Log( "music onEnable" );
		}
	
		
		//---
		//
		//---
		void OnFocus()
		{
			//--- if play mode changed, need to refresh target reference. otherwise, it holds on to the
			//--- game play version and won't update correctly when switched back into editor mode
			if( _playModeChanged )
			{
				CheckForTarget(true);
				_playModeChanged = false;
			}
		}
		
		
		//---
		//
		//---
		void OnInspectorUpdate()
		{
			//--- keep track of when play mode changes (see OnFocus)
			if( EditorApplication.isPlaying )
			{
				_playModeChanged = true;
			}
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

		
		//---
		//
		//---
		override public void OnGUI ()
		{
			int depth = GUI.depth;
			
			if( target == null ) CheckForTarget(true);

     		GUI.DrawTexture(areaRect, background);
         GUILayout.BeginArea(MODULE_SIZE);
            
         if (!target)
			{
				GUI.Label( new Rect(230, 290, 380, 20), "Please select the GameObject with the MusicManager Script Attached", TextStyle );
				if (GUI.Button( new Rect(370, 320, 80, 20), "Refresh"))
				{
					CheckForTarget(true);
				}
			}
			else
			{
//##				GUILayout.Label ("Music Editor", EditorStyles.boldLabel);
	
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical (GUILayout.MaxWidth(120));
				
				if (node.Count > 0)
				{
					if(GUI.Button( new Rect( 545, 10, 110, 20), "Save"))
					{
						UpdateNodes();
					}
				}
				
				if (GUI.Button( new Rect(10, 60, 105, 20), "Add Music Track"))
				{
					AddNode(new Node(1));
				}
				
				if (GUI.Button( new Rect(10, 85, 105, 20), "Add Effect Track"))
				{
					AddNode(new Node(2));
				}
				
				nodePosition = EditorGUILayout.BeginScrollView ( nodePosition);
				
				GUILayout.Space(15);
				EditorGUILayout.EndScrollView();    
				GUILayout.EndVertical();
	
				GUILayout.BeginVertical();
				
				if (node.Count > 0)
				{
					filter = EditorGUI.Popup( new Rect(130, 60, 525, 15), filter, filterOptions, PopupLongStyle);
				}
				
				GUILayout.Space(80);
				scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
				
				for (int i = 0;i<node.Count;i++)
				{
					bool requestRemove = false;
					
					if (node[i].type == 1 && filter != 1)
					{
						GUI.color = Color.green;
						GUILayout.BeginVertical ("box");
						GUI.color = Color.white;
						GUILayout.BeginHorizontal();
						node[i].shown = EditorGUILayout.Foldout(node[i].shown,"Music Track: "  + node[i].name);
						
						if (GUILayout.Button("Delete",GUILayout.MaxWidth(50)))
						{
							requestRemove = true;
						}
						
						GUILayout.EndHorizontal();
						node[i].backgroundMusic = true;
						GUILayout.Space(5);
						
						if (node[i].shown)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Track Name", GUILayout.Width(140) );
							node[i].name = EditorGUILayout.TextField( node[i].name );
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Audio File", GUILayout.Width(140) );
							node[i].track = (AudioClip)EditorGUILayout.ObjectField( node[i].track, typeof( AudioClip ), true );
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Volume", GUILayout.Width(140) );
							node[i].volume = EditorGUILayout.Slider(node[i].volume,0.0f,1.0f);
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Loop Track", GUILayout.Width(140) );
							node[i].loop = EditorGUILayout.Toggle(node[i].loop);
							EditorGUILayout.EndHorizontal();
	
							EditorGUILayout.Separator();
						}
						
						GUILayout.EndVertical ();
					}
	
					if (node[i].type == 2  && filter > 0)
					{
						GUI.color = Color.red;
						GUILayout.BeginVertical ("box");
						GUI.color = Color.white;
						GUILayout.BeginHorizontal();
						
						node[i].shown = EditorGUILayout.Foldout(node[i].shown,"Effect Track: " + node[i].name);
						
						if (GUILayout.Button("Delete",GUILayout.MaxWidth(50)))
						{
							requestRemove = true;
						}
						
						GUILayout.EndHorizontal();
						GUILayout.Space(5);
						node[i].backgroundMusic = false;
						
						if (node[i].shown)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Effect Name", GUILayout.Width(140) );
							node[i].name = EditorGUILayout.TextField( node[i].name );
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Audio File", GUILayout.Width(140) );
							node[i].track = (AudioClip)EditorGUILayout.ObjectField( node[i].track,typeof(AudioClip), true );
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Volume", GUILayout.Width(140) );
							node[i].volume = EditorGUILayout.Slider(node[i].volume,0.0f,1.0f);
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Loop Effect", GUILayout.Width(140) );
							node[i].loop = EditorGUILayout.Toggle(node[i].loop);
							EditorGUILayout.EndHorizontal();
													
							EditorGUILayout.Separator();
						}
						
						GUILayout.EndVertical ();
					}
					
					if (requestRemove)
					{
						RemoveNode(i);
					}
				}
			
				EditorGUILayout.EndScrollView();

                Knobs.Theme(Knobs.Themes.BLACK, _editor);

            allVals[0] = target.fadeSpeed = Knobs.MinMax(new Vector2(9,500),target.fadeSpeed,0F,1F,0);
            allVals[1] = AudioListener.volume = Knobs.MinMax(new Vector2(67,500),AudioListener.volume,0F,5F,1);

            if(Knobs.interactID != -1)
            {
                GUIStyle gs = new GUIStyle();
                gs.normal.textColor = Color.white;
                gs.alignment = TextAnchor.MiddleCenter;
                gs.normal.background = (Texture2D) precisionBG;

                GUI.Box(precisionBox, ""+allVals[Knobs.interactID],gs);
                _editor.Repaint();
            }
				
/*				
				//--- Overall Settings Boc
				GUILayout.BeginHorizontal ("box");
				GUILayout.BeginVertical ();
				GUILayout.Label ("Settings", EditorStyles.boldLabel);
				AudioListener.volume = EditorGUILayout.Slider("Master Volume",AudioListener.volume,0.0f,1);
				target.fadeSpeed = EditorGUILayout.Slider("Track Fade Speed",target.fadeSpeed,0.0f,5.0f);
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal();
*/				
				GUILayout.Space(10);
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();
			}

            GUILayout.EndArea();
		}
	
	
		//---
		//
		//---
		void CheckForTarget ( bool _refresh )
		{
            MusicControl[] array = (MusicControl[])Resources.FindObjectsOfTypeAll(typeof(FPSControl.MusicControl));
            if (array.Length > 0)
            {
                Selection.activeGameObject = array[0].gameObject;
            }
            else
            {
                if (EditorUtility.DisplayDialog("Component not found!", "There is no Music Control in this scene, Create one?", "OK", "Cancel"))
                {
                    GameObject go = new GameObject("Music Control");
                    go.AddComponent(typeof(MusicControl));
                    Selection.activeGameObject = go;
                }
                else
                {
                    (_editor as FPSControlMainEditor).LoadModule(FPSControlModuleType.NONE);
                }
            }
            
            if (Selection.activeGameObject)
			{
				if( _refresh )
				{
					node = new List<Node>(); 
				}
				
				target = Selection.activeGameObject.GetComponent<MusicControl>();
				if (target)
				{
					CheckForNodes();
				}
			}
			
		}
		
		
		//---
		//
		//---
		void AddNode ( Node newNode  )
		{
			node.Add(newNode);
		}
		
		
		//---
		//
		//---
		void RemoveNode ( int index  )
		{
			if (node.Count > 1)
			{
				node.RemoveAt(index);
			}
			else
			{
				node = new List<Node>();
			}
		}
		
		
		//---
		//
		//---
		void CheckForNodes ()
		{
			if (target.audioTracks != null && target.audioTracks.Count > 0)
			{
				//List<Node> node = new List<Node>(); 
				for (int i= 0;i<target.audioTracks.Count;i++)
				{
					Node newNode;
					
					if (target.audioTracks[i].backgroundMusic)
					{
						newNode = new Node(1);
					}
					else
					{
						newNode = new Node(2);
					}
					
					newNode.name = target.audioTracks[i].name;
					newNode.track = target.audioTracks[i].track;
					newNode.volume = target.audioTracks[i].volume;
					newNode.loop = target.audioTracks[i].loop;
					node.Add(newNode);
				}
			}
		}
		
		
		//---
		//
		//---
		void UpdateNodes ()
		{
			//Update Nodes
			if (node.Count > 0)
			{		
				target.audioTracks = null;
				target.audioTracks = new List<MusicControlTrack>();
				
				for (int n=0;n<node.Count;n++)
				{
					MusicControlTrack newTrack = new MusicControlTrack();
					
					if (node[n].track)
					{
						newTrack.track = node[n].track;
					}
					
					if (node[n].name != "")
					{
						newTrack.name = node[n].name;
					}
					
					newTrack.volume = node[n].volume;
					newTrack.backgroundMusic = node[n].backgroundMusic;
					newTrack.loop = node[n].loop;
					target.audioTracks.Add(newTrack);
				}
				
				Debug.Log("Saved " + node.Count + " Tracks");
			}		
		}
	}
}