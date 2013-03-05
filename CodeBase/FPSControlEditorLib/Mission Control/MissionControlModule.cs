using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using FPSControl;
using System;
using System.Net;
using System.Text;
using System.IO;

namespace FPSControlEditor
{
	public class MissionControlModule : FPSControlEditorModule
	{

        private enum MissionStage
		{
			CheckForSelection,
			BadSelection,
			MergeStart,
			MergeDone
		}
		
		MissionStage stage = MissionStage.CheckForSelection;
		public GameObject target;
		public bool generateTriangleStrips = true;
		/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
		
		#region Assets

		//Path

		const string PATH = "Mission Control/";
		string currentProject = Application.dataPath;
		string previousProject;
		string saveFile;
		
		//Status
		string thisOfThat = "";
		int stageStep = 0;
		int currPos = 0;
		int maxPos = 0;
		int counter = 0;
		FileInfo preInfo;
		FileInfo curInfo;
		bool found;
		
		
		//Lists
		List<string> newFiles;
		string[] previousFiles;
		string[] currentFiles;
		List<string> pre;
		List<string> curr;
		
		//GFX
		Texture stage1_background;
		Texture stage2_background;
		Texture stage3_background;
		Texture stage4_background;

		#endregion // Assets

		#region GUI Properties

		Rect areaRect = new Rect(245, 50, 660, 580);
		Rect statusRect = new Rect(245+280, 50+312, 120, 25);

		#endregion // GUI Properties

		
		
		public MissionControlModule(EditorWindow editorWindow) : base(editorWindow)
		{
		   _type = FPSControlModuleType.MissionControl;
		}
        /*
		
		public override void Init()
		{
            //return; //bypass breakage... these files don't exist in my version?
            stage1_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage1_background.png");
      	    stage2_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage2_background.png");
      	    stage3_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage3_background.png");
      	    stage4_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage4_background.png");
		}
		
		
		public override void OnGUI()
		{
			int depth = GUI.depth;
            //return; //bypass breakage
			switch( stage )
			{
				case MissionStage.CheckForSelection:
         		GUI.DrawTexture(areaRect, stage1_background);
					CheckForSelection();
					break;
				case MissionStage.BadSelection:
         		GUI.DrawTexture(areaRect, stage2_background);
					BadSelection();
					break;
				case MissionStage.MergeStart:
         		GUI.DrawTexture(areaRect, stage3_background);
					MergeStart();
					break;
				case MissionStage.MergeDone:
         		GUI.DrawTexture(areaRect, stage4_background);
					MergeDone();
					break;
			}
		}
		
		
		void CheckForSelection()
		{
			if( GUI.Button( new Rect(245+290, 60+315, 80, 25), "Continue" ) )
			{
				CheckForTarget( false );
				
				//currentProject = EditorUtility.OpenFolderPanel("Select the current version's asset folder", "", "Assets");
				previousProject = EditorUtility.OpenFolderPanel("Select the previous version's asset folder", "", "Assets");
				if (previousProject == "")
				{
					Debug.LogError("No previous version folder selected! Ending Check...");
				}
				else
				{
					stage = MissionStage.BadSelection;
				}
				
			}		
		}
		
		
		void BadSelection()
		{
			if( GUI.Button( new Rect(245+290, 60+315, 80, 25), "Try Again" ) )
			{
				stage = MissionStage.CheckForSelection;
			}			
		}
		
		
		void PackagingProgress()
		{
			switch( stageStep )
			{
				case 0:
					newFiles = new List<string>();
					pre = new List<string>();
					curr = new List<string>();
					stageStep++;
					Repaint();
					break;
				case 1:
					GUI.Label(statusRect, "Scanning previous project..." );
					previousFiles = Directory.GetFiles(previousProject, "*.*", SearchOption.AllDirectories);
					foreach (string file in previousFiles)
					{
						pre.Add(file);
					}
					stageStep++;
					Repaint();
					break;
				case 2:
					GUI.Label(statusRect, "Scanning current project..." );
					currentFiles = Directory.GetFiles(currentProject, "*.*", SearchOption.AllDirectories);
					foreach (string file in currentFiles)
					{
						curr.Add(file);
					}
					maxPos = curr.Count;
					stageStep++;
					Repaint();
					break;
				case 3:
					thisOfThat = String.Format("Comparing {0:0.#} / {1:0.#}", currPos, maxPos);
					GUI.Label(statusRect, thisOfThat);
					
					for( ; currPos < maxPos; currPos++ )
					{
						found = false;
						curInfo = new FileInfo(curr[currPos]);
						for (int x = 0; x < pre.Count; x++)
						{
						   if (pre[x].Replace(previousProject, "") == curr[currPos].Replace(currentProject, ""))
						   {
								preInfo = new FileInfo(pre[x]);
								found = true;
								if (preInfo.LastWriteTimeUtc < curInfo.LastWriteTimeUtc)
								{
									newFiles.Add(curr[currPos]);
									if (curr[currPos].EndsWith(".meta"))
									{
										newFiles.Add(curr[currPos].Replace(".meta", ""));
									}
								}
								pre.RemoveAt(x);
								break;
						   }
						}
						if (!found)
						{
						   newFiles.Add(currentFiles[currPos]);
						}
						
						Repaint();
					}
					
					stageStep++;
					Repaint();
					break;
				case 4:
					GUI.Label(statusRect, "Creating Update package..." );
					for (int i = 0; i < newFiles.Count; i++)
					{
						newFiles[i] = newFiles[i].Replace(currentProject, "Assets");
					}
					
					stageStep++;
					if (newFiles.Count > 0)
					{
						AssetDatabase.ExportPackage(newFiles.ToArray(), saveFile, ExportPackageOptions.IncludeDependencies);
					}
					else
					{
						Debug.Log("Folders are identical!");
					}
					break;
				case 5:
					stage = MissionStage.MergeDone;
					break;
			}
			
		}
		
		
		void MergeDone()
		{
		}
		
		
		//---
		//
		//---
		void CheckForTarget ( bool _refresh )
		{
			if (Selection.activeGameObject)
			{
//				if( _refresh )
//				{
//					node = new List<Node>(); 
//				}
				
				target = Selection.activeGameObject;
				if (target)
				{
					stage = MissionStage.MergeStart;
				}
			}
			else
			{
				stage = MissionStage.BadSelection;
			}
			
		}
		
		
		void MergeStart()
		{
			//yield return null;
	
			Component[] filters  = target.GetComponentsInChildren<MeshFilter>();
			Matrix4x4 myTransform = target.transform.worldToLocalMatrix;
			Hashtable materialToMesh= new Hashtable();
			
			//---
			if( filters.Length == 0 )
			{
				stage = MissionStage.BadSelection;
				return;
			}
		
			for (int i=0;i<filters.Length;i++)
			{
				MeshFilter filter = (MeshFilter)filters[i];
				Renderer curRenderer  = filters[i].renderer;
				MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
				instance.mesh = filter.sharedMesh;
				if (curRenderer != null && curRenderer.enabled == true && instance.mesh != null)
				{
					instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
					Material[] materials = curRenderer.sharedMaterials;
					for (int m=0;m<materials.Length;m++)
					{
						instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
						ArrayList objects = (ArrayList)materialToMesh[materials[m]];
						if (objects != null)
						{
							objects.Add(instance);
						}
						else
						{
							objects = new ArrayList ();
							objects.Add(instance);
							materialToMesh.Add(materials[m], objects);
						}
					}
				
					curRenderer.enabled = false;
				}
			}

			foreach (DictionaryEntry de  in materialToMesh)
			{
				ArrayList elements = (ArrayList)de.Value;
				MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

            Mesh mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
            if (mesh == null)
				{
					SplitInstances (instances, (Material)de.Key);
            }
				else
				{ 
				    GameObject go = new GameObject("Combined mesh");
				    go.transform.parent = target.transform;
				    go.transform.localScale = Vector3.one;
				    go.transform.localRotation = Quaternion.identity;
				    go.transform.localPosition = Vector3.zero;
				    go.AddComponent(typeof(MeshFilter));
				    go.AddComponent("MeshRenderer");
				    go.renderer.material = (Material)de.Key;
				    MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				    filter.mesh = mesh;
				}
			}
			
			stage = MissionStage.MergeDone;
		}
		
		
		void SplitInstances (MeshCombineUtility.MeshInstance[] instances, Material mat)
		{
			int left = instances.Length/2 + instances.Length % 2;
			int right = instances.Length/2;
			MeshCombineUtility.MeshInstance[] leftInst = new MeshCombineUtility.MeshInstance[left];
			MeshCombineUtility.MeshInstance[] rightInst = new MeshCombineUtility.MeshInstance[right];
			for (int i = 0; i < left; i ++)
			{
				leftInst[i] = instances[i];
			}
			for (int i = 0; i < right; i ++)
			{
				rightInst[i] = instances[i + left];
			}

			Mesh mesh = MeshCombineUtility.Combine(leftInst, generateTriangleStrips);
			if (mesh == null)
			{
				SplitInstances (leftInst, mat);
			}
			else
			{
				GameObject go = new GameObject("Combined mesh");
				go.transform.parent = target.transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent("MeshRenderer");
				go.renderer.material = mat;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				filter.mesh = mesh;
			}

			mesh = MeshCombineUtility.Combine(rightInst, generateTriangleStrips);
			if (mesh == null)
			{
				SplitInstances (rightInst, mat);
			}
			else
			{
				GameObject go = new GameObject("Combined mesh");
				go.transform.parent = target.transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent("MeshRenderer");
				go.renderer.material = mat;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				filter.mesh = mesh;
			}
		}*/			
	}
		
}


