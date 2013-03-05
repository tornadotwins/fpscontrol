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
	/*public class Node
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
	*/
	
	//---
	//
	//---
	public class MusicControlEditor : EditorWindow 
	{
		Vector2 scrollPosition;
		Vector2 nodePosition;
		public MusicControl target;
		
		private bool _playModeChanged = false;
		
		//Node Type: 1 - Position, 2 - Rotation, 3 - Scale
		List<MusicControlNode> node = new List<MusicControlNode>(); 
		
		string[] filterOptions = new string[3]{"Music Tracks", "Effect Tracks", "All Tracks"};
		int filter = 2;
		
	
		//---
		//
		//---
		//[MenuItem ("FPS Control/Music Manager")]
		static void Init ()
		{
			// Get existing open window or if none, make a new one:
		    EditorWindow.GetWindow (typeof (MusicControlEditor));
		}
		
		
		//---
		//
		//---
		void OnEnable ()
		{
			target = null;
			CheckForTarget(true);
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
			
	
		//---
		//
		//---
		void OnGUI ()
		{
			if (!target)
			{
				GUILayout.Label ("Please select the GameObject with the MusicManager Script Attached", EditorStyles.boldLabel);
				if (GUILayout.Button("Refresh"))
				{
					CheckForTarget(true);
				}
			}
			else
			{
				GUILayout.Label ("Music Editor", EditorStyles.boldLabel);
	
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical (GUILayout.MaxWidth(120));
				nodePosition = EditorGUILayout.BeginScrollView ( nodePosition);
				
				if (GUILayout.Button ("Add Music Track"))
				{
					AddNode(new Node(1));
				}
				
				if (GUILayout.Button ("Add Effect Track"))
				{
					AddNode(new Node(2));
				}
				
				if (node.Count > 0)
				{
					GUILayout.FlexibleSpace();
					if(GUILayout.Button ("Save Tracks", GUILayout.MaxWidth(120)))
					{
						UpdateNodes();
					}
				}
				
				GUILayout.Space(15);
				EditorGUILayout.EndScrollView();    
				GUILayout.EndVertical();
	
				GUILayout.BeginVertical ();
				
				if (node.Count > 0)
				{
					filter = EditorGUILayout.Popup (filter, filterOptions);
				}
				
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
				
				//--- Overall Settings Boc
				GUILayout.BeginHorizontal ("box");
				GUILayout.BeginVertical ();
				GUILayout.Label ("Settings", EditorStyles.boldLabel);
				AudioListener.volume = EditorGUILayout.Slider("Master Volume",AudioListener.volume,0.0f,1);
				target.fadeSpeed = EditorGUILayout.Slider("Track Fade Speed",target.fadeSpeed,0.0f,5.0f);
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();
			}
			
		}
	
	
		//---
		//
		//---
		void CheckForTarget ( bool _refresh )
		{
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