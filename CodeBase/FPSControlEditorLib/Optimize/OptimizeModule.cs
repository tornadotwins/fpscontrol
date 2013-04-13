using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using FPSControl;
using System;
using System.Net;
using System.Text;
using System.IO;

/**
 * Just a note: I just completely rewrote the mesh merge routine. i commented out most everything that is deprecated
 * 
 */
namespace FPSControlEditor
{
    public class OptimizeModule : FPSControlEditorModule
    {
        private enum OptimizeStage
        {
            CheckForSelection,
            BadSelection,
            MergeStart,
            MergeDone
        }

        OptimizeStage stage = OptimizeStage.CheckForSelection;
        public GameObject target;
        public bool generateTriangleStrips = true;
        /// This option has a far longer preprocessing time at startup but leads to better runtime performance.

        #region Assets

        //Path

        const string PATH = "Optimize/";
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
        Rect statusRect = new Rect(245 + 280, 50 + 312, 120, 25);

        #endregion // GUI Properties



        public OptimizeModule(EditorWindow editorWindow)
            : base(editorWindow)
        {
            _type = FPSControlModuleType.Optimize;
        }


        public override void Init()
        {
            //return; //bypass breakage... these files don't exist in my version?
            stage1_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage1_background.png");
            stage2_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage2_background.png");
            stage3_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage3_background.png");
            stage4_background = Load<Texture>(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + PATH + "stage4_background.png");
            base.Init();
        }


        public override void OnGUI()
        {
            int depth = GUI.depth;
            //return; //bypass breakage
            switch (stage)
            {
                case OptimizeStage.CheckForSelection:
                    GUI.DrawTexture(areaRect, stage1_background);
                    CheckForSelection();
                    break;
                case OptimizeStage.BadSelection:
                    GUI.DrawTexture(areaRect, stage2_background);
                    BadSelection();
                    break;
                case OptimizeStage.MergeStart:
                    GUI.DrawTexture(areaRect, stage3_background);
                    MergeStart();
                    break;
                case OptimizeStage.MergeDone:
                    GUI.DrawTexture(areaRect, stage4_background);
                    MergeDone();
                    break;
            }
        }


        void CheckForSelection()
        {
            if (GUI.Button(new Rect(245 + 290, 60 + 315, 80, 25), "Continue"))
            {
                CheckForTarget(false);
                /*
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
                */
            }
        }


        void BadSelection()
        {
            if (GUI.Button(new Rect(245 + 290, 60 + 315, 80, 25), "Try Again"))
            {
                stage = OptimizeStage.CheckForSelection;
            }
        }

        /*
        void PackagingProgress()
        {
            switch (stageStep)
            {
                case 0:
                    newFiles = new List<string>();
                    pre = new List<string>();
                    curr = new List<string>();
                    stageStep++;
                    Repaint();
                    break;
                case 1:
                    GUI.Label(statusRect, "Scanning previous project...");
                    previousFiles = Directory.GetFiles(previousProject, "*.*", SearchOption.AllDirectories);
                    foreach (string file in previousFiles)
                    {
                        pre.Add(file);
                    }
                    stageStep++;
                    Repaint();
                    break;
                case 2:
                    GUI.Label(statusRect, "Scanning current project...");
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

                    for (; currPos < maxPos; currPos++)
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
                    GUI.Label(statusRect, "Creating Update package...");
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
                    stage = OptimizeStage.MergeDone;
                    break;
            }

        }
        */

        void MergeDone()
        {
            if (GUI.Button(new Rect(245 + 280, 60 + 315, 100, 25), "Optimize More"))
            {
                stage = OptimizeStage.CheckForSelection;
            }
        }


        //---
        //
        //---
        
        void CheckForTarget(bool _refresh)
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
                    stage = OptimizeStage.MergeStart;
                }
            }
            else
            {
                stage = OptimizeStage.BadSelection;
            }

        }
        

        void MergeStart()
        {
            /*
            //yield return null;

            Component[] filters = target.GetComponentsInChildren<MeshFilter>();
            Matrix4x4 myTransform = target.transform.worldToLocalMatrix;
            Hashtable materialToMesh = new Hashtable();

            //---
            if (filters.Length == 0)
            {
                stage = OptimizeStage.BadSelection;
                return;
            }

            for (int i = 0; i < filters.Length; i++)
            {
                MeshFilter filter = (MeshFilter)filters[i];
                Renderer currRenderer = filters[i].renderer;
                MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
                instance.mesh = (Mesh) UnityEngine.Object.Instantiate(filter.sharedMesh);
                if (currRenderer != null && currRenderer.enabled == true && instance.mesh != null)
                {
                    instance.transform = myTransform * filter.transform.localToWorldMatrix;

                    Material[] materials = currRenderer.sharedMaterials;
                    for (int m = 0; m < materials.Length; m++)
                    {
                        instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                        ArrayList objects = (ArrayList)materialToMesh[materials[m]];
                        if (objects != null)
                        {
                            objects.Add(instance);
                        }
                        else
                        {
                            objects = new ArrayList();
                            objects.Add(instance);
                            materialToMesh.Add(materials[m], objects);
                        }
                    }

                    currRenderer.enabled = false;
                }
            }

            foreach (DictionaryEntry de in materialToMesh)
            {
                ArrayList elements = (ArrayList)de.Value;
                MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

                Mesh mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                if (mesh == null)
                {
                    SplitInstances(instances, (Material)de.Key);
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
            */

            stage = (Merge()) ? OptimizeStage.MergeDone : OptimizeStage.BadSelection;
        }

        static bool Merge()
        {
            GameObject[] gameObjects = Selection.gameObjects; //need all the selected go's
            List<MeshFilter> filters = new List<MeshFilter>(); //this is where we'll store all the meshfilters that are eligible for merge
            List<Transform> cachedTransforms = new List<Transform>(); //we'll cache the transforms for later.
            bool firstInSelection = true;
            Material[] prevMats = new Material[0] { };

            /*
             * Criteria requiring a mesh merge: 
             *  Containing GameObjects must be marked static.
             *  MeshRenderers must have the EXACT SAME sharedMaterials array
             */
            foreach (GameObject go in gameObjects)
            {
                //check if the game object even has a mesh filter at all
                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (mf == null) continue;

                //check against static
                if (!go.isStatic)
                {
                    Debug.LogWarning("One of the selected Meshes is not static. Make sure that the Mesh's GameObject must be marked as static!");
                    continue;
                }

                //we'll need to do a material array comparison
                if (firstInSelection)
                {
                    prevMats = mf.renderer.sharedMaterials; //store the material array for the first go's mesh
                    firstInSelection = false;
                }
                else
                {
                    bool _continue = false; //flag for whether or not this renderer matches the others
                    for (int i = 0; i < mf.renderer.sharedMaterials.Length; i++)
                    {
                        //check against the previously stored
                        if (mf.renderer.sharedMaterials[i] != prevMats[i])
                        {
                            Debug.LogWarning("One or more of the selected Meshes does not share the same sharedMaterials array as the others.");
                            _continue = true;
                            break;
                        }
                    }

                    if (_continue) continue;
                }

                filters.Add(mf);
            }

            //we need at least 2 to have passed the criteria test.
            if (filters.Count < 2)
            {
                Debug.LogWarning("Mesh Merging requires at least 2 MeshFilter components! Review the meshes and try again.");
                return false;
            }

            //now we look through the filters and cache their transforms so we can move them back afterwards
            for (int i = 0; i < filters.Count; i++)
            {
                cachedTransforms.Add(filters[i].transform.parent);
            }

            CombineInstance[] combine = new CombineInstance[filters.Count];

            //We'll need a containing object to do the merge of children.
            GameObject mergedContainer = new GameObject();
            mergedContainer.transform.parent = null;
            mergedContainer.name = "Merged (" + filters[0].gameObject.name + ")"; //give it an identifying name based on one of the merged objects
            mergedContainer.transform.localPosition = Vector3.zero;
            mergedContainer.transform.rotation = Quaternion.Euler(Vector3.zero);
            mergedContainer.transform.localScale = Vector3.one;
            mergedContainer.AddComponent<MeshFilter>();
            MeshRenderer mergedRenderer = mergedContainer.AddComponent<MeshRenderer>();
            mergedRenderer.sharedMaterials = filters[0].renderer.sharedMaterials;

            if (filters[0].GetComponent<MeshCollider>() != null) //give it a mesh collider if need be.
                mergedContainer.AddComponent<MeshCollider>();

            //set the container to be static and have the same layer as the first selected filter.
            mergedContainer.isStatic = true;
            mergedContainer.layer = filters[0].gameObject.layer;

            //make sure all the meshes we're combining reside under the merged parent before we do our merge otherwise it won't work!
            foreach (MeshFilter mf in filters)
            {
                mf.transform.parent = mergedContainer.transform;
            }

            //setup everything for a combine
            for (int j = 0; j < combine.Length; j++)
            {
                combine[j].mesh = filters[j].sharedMesh;
                combine[j].transform = filters[j].transform.localToWorldMatrix;
                filters[j].gameObject.active = false; //set the original gameobject to false to prevent confusion
            }

            //need to give the container an empty mesh to start with
            MeshFilter mergedFilter = mergedContainer.GetComponent<MeshFilter>();
            Mesh mergedMesh = new Mesh();
            mergedMesh.name = "Combined Mesh (" + filters[0].gameObject.name + ")"; //give it an identifying name based on one of the merged objects

            //the grand finale - merge stuff.
            mergedMesh.CombineMeshes(combine, true, true); //do our combine

            mergedFilter.sharedMesh = mergedMesh; //set the combined mesh to the merged gameobject's meshfilter

            //add collider if need be
            if (filters[0].GetComponent<MeshCollider>() != null)
                mergedContainer.GetComponent<MeshCollider>().sharedMesh = mergedMesh;

            //and move all the original transforms back!
            for (int l = 0; l < filters.Count; l++)
            {
                filters[l].transform.parent = cachedTransforms[l];
            }

            return true;

        }

        /*
        void SplitInstances(MeshCombineUtility.MeshInstance[] instances, Material mat)
        {
            int left = instances.Length / 2 + instances.Length % 2;
            int right = instances.Length / 2;
            MeshCombineUtility.MeshInstance[] leftInst = new MeshCombineUtility.MeshInstance[left];
            MeshCombineUtility.MeshInstance[] rightInst = new MeshCombineUtility.MeshInstance[right];
            for (int i = 0; i < left; i++)
            {
                leftInst[i] = instances[i];
            }
            for (int i = 0; i < right; i++)
            {
                rightInst[i] = instances[i + left];
            }

            Mesh mesh = MeshCombineUtility.Combine(leftInst, generateTriangleStrips);
            if (mesh == null)
            {
                SplitInstances(leftInst, mat);
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
                SplitInstances(rightInst, mat);
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


