using UnityEngine;
using System.Collections;

/*
This works like the CombineChildren script except that it allows children to
mark the parent as dirty (if they change).  The Recombiner will then check every 
few seconds as to whether it needs to reinit.
*/

namespace FPSControlEditor
{
//[AddComponentMenu("Mesh/Recombiner")]
public class MeshRecombiner : MonoBehaviour {
	
    public float recombineDelay = 10.0f;
	/// Usually rendering with triangle strips is faster.
	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
	/// Best is to try out which value is faster in practice.
	public bool generateTriangleStrips = true;
	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.

    private bool dirty;

    void CheckRecombine () {
        if (dirty) {
            StartCoroutine (Recombine ());
            dirty = false;
        }
    }

    void MarkDirty () {
        if (dirty) {
            Scheduler.GetInstance().UpdateSchedule (recombineDelay, "CheckRecombine", gameObject);
        } else {
            bool hasCombined = true;
            while (hasCombined) {
                Transform child = transform.Find ("Combined mesh");
                if (child) {
                    child.parent = null;
                    Destroy (child.gameObject);
                } else {
                    hasCombined = false;
                }
            }
            foreach (Transform child in transform) {
		        Component[] filters  = child.GetComponentsInChildren(typeof(MeshFilter));
                foreach (MeshFilter filter in filters) {
                    filter.renderer.enabled = true;
                }
            }
            Scheduler.GetInstance().AddSchedule (recombineDelay, "CheckRecombine", false, gameObject);
            dirty = true;
        }
    }

	IEnumerator Recombine () {
        yield return null;
	
		Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Hashtable materialToMesh= new Hashtable();
		
		for (int i=0;i<filters.Length;i++) {
			MeshFilter filter = (MeshFilter)filters[i];
			Renderer curRenderer  = filters[i].renderer;
			MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance ();
			instance.mesh = filter.sharedMesh;
			if (curRenderer != null && curRenderer.enabled == true && instance.mesh != null) {
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
				Material[] materials = curRenderer.sharedMaterials;
				for (int m=0;m<materials.Length;m++) {
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
					ArrayList objects = (ArrayList)materialToMesh[materials[m]];
					if (objects != null) {
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

		foreach (DictionaryEntry de  in materialToMesh) {
			ArrayList elements = (ArrayList)de.Value;
			MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

            Mesh mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
            if (mesh == null) {
                SplitInstances (instances, (Material)de.Key);
            } else { 
			    GameObject go = new GameObject("Combined mesh");
			    go.transform.parent = transform;
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
	}

    void SplitInstances (MeshCombineUtility.MeshInstance[] instances, Material mat) {
        int left = instances.Length/2 + /* grab odd */ instances.Length % 2;
        int right = instances.Length/2;
        MeshCombineUtility.MeshInstance[] leftInst = new MeshCombineUtility.MeshInstance[left];
        MeshCombineUtility.MeshInstance[] rightInst = new MeshCombineUtility.MeshInstance[right];
        for (int i = 0; i < left; i ++) {
            leftInst[i] = instances[i];
        }
        for (int i = 0; i < right; i ++) {
            rightInst[i] = instances[i + left];
        }

        Mesh mesh = MeshCombineUtility.Combine(leftInst, generateTriangleStrips);
        if (mesh == null) {
            SplitInstances (leftInst, mat);
        } else {
		    GameObject go = new GameObject("Combined mesh");
		    go.transform.parent = transform;
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
        if (mesh == null) {
            SplitInstances (rightInst, mat);
        } else {
		    GameObject go = new GameObject("Combined mesh");
		    go.transform.parent = transform;
		    go.transform.localScale = Vector3.one;
		    go.transform.localRotation = Quaternion.identity;
		    go.transform.localPosition = Vector3.zero;
		    go.AddComponent(typeof(MeshFilter));
		    go.AddComponent("MeshRenderer");
		    go.renderer.material = mat;
		    MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
		    filter.mesh = mesh;
        }
    }
}
}
