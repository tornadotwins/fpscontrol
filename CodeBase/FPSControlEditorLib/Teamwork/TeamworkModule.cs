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
	public class TeamworkModule : FPSControlEditorModule
	{
		private enum TeamworkStage
		{
			LocateOldProject,
			SelectDestination,
			PackagingProgress,
			PackagingDone
		}
		
		TeamworkStage stage = TeamworkStage.LocateOldProject;
		
		#region Assets

		//Path

		const string PATH = "Teamwork/";
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

		
		
		public TeamworkModule(EditorWindow editorWindow) : base(editorWindow)
		{
		   _type = FPSControlModuleType.TeamWork;
		}

		
		public override void Init()
		{
      		stage1_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage1_background.png");
      		stage2_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage2_background.png");
      		stage3_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage3_background.png");
      		stage4_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage4_background.png");
		}
		
		
		public override void OnGUI()
		{
			int depth = GUI.depth;
			
			switch( stage )
			{
				case TeamworkStage.LocateOldProject:
         		GUI.DrawTexture(areaRect, stage1_background);
					LocateOldProject();
					break;
				case TeamworkStage.SelectDestination:
         		GUI.DrawTexture(areaRect, stage2_background);
					SelectDestination();
					break;
				case TeamworkStage.PackagingProgress:
         		GUI.DrawTexture(areaRect, stage3_background);
					PackagingProgress();
					break;
				case TeamworkStage.PackagingDone:
         		GUI.DrawTexture(areaRect, stage4_background);
					PackagingDone();
					break;
			}
		}
		
		
		void LocateOldProject()
		{			
			if( GUI.Button( new Rect(245+290, 50+315, 80, 25), "Locate >" ) )
			{
				//currentProject = EditorUtility.OpenFolderPanel("Select the current version's asset folder", "", "Assets");
				previousProject = EditorUtility.OpenFolderPanel("Select the previous version's asset folder", "", "Assets");
				if (previousProject == "")
				{
					Debug.LogError("No previous version folder selected! Ending Check...");
				}
				else
				{
					stage = TeamworkStage.SelectDestination;
				}
			}			
		}
		
		
		void SelectDestination()
		{
			if( GUI.Button( new Rect(245+250, 50+310, 80, 25), "< cancel" ) )
			{
				stage = TeamworkStage.LocateOldProject;
			}			
			if( GUI.Button( new Rect(245+335, 50+310, 80, 25), "save as >" ) )
			{
				saveFile = EditorUtility.SaveFilePanel("Select location to save Update", Application.dataPath.Replace("Assets", "Updates"), "Latest-Update (" + System.DateTime.Now.ToString("dd MMM yyyy - hh mm ss tt") + ")", "unitypackage");
				if( saveFile != "" )
				{
					stage = TeamworkStage.PackagingProgress;
					stageStep = 0;
				}
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
					stage = TeamworkStage.PackagingDone;
					break;
			}
			
		}
		
		
		void PackagingDone()
		{
		}
		
	}
		
}


