using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class MeshCombiner : MonoBehaviour
{
    public void Start()
    {
        Combine();
    }

    [ContextMenu("Combine")]
    public void Combine()
    { 
        // Find all mesh filter submeshes and separate them by their cooresponding materials
        ArrayList materials = new ArrayList (); 
        ArrayList combineInstanceArrays = new ArrayList (); 
        Matrix4x4 myTransform = transform.worldToLocalMatrix;

        MeshFilter[] meshFilters = this.gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            // Handle bad input
            if (!meshRenderer)
            {
                Debug.LogError("MeshFilter does not have a coresponding MeshRenderer.");
                continue;
            }
            if (meshRenderer.materials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                Debug.LogError("Mismatch between material count and submesh count. Is this the correct MeshRenderer?");
                continue;
            }
            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex;
                for (materialArrayIndex = 0; materialArrayIndex < materials.Count; materialArrayIndex++)
                {
                    if ((UnityEngine.Object)materials[materialArrayIndex] == meshRenderer.sharedMaterials[s])
                        break;
                }

                if (materialArrayIndex == materials.Count)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    combineInstanceArrays.Add(new ArrayList());
                }

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = myTransform * meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }
        // For MeshFilter
        {
            // Get / Create mesh filter
            MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
            if (!meshFilterCombine)
                meshFilterCombine = gameObject.AddComponent<MeshFilter>();

            // Combine by material index into per-material meshes
            // also, Create CombineInstance array for next step
            Mesh[] meshes = new Mesh[materials.Count];
            CombineInstance[] combineInstances = new CombineInstance[materials.Count];

            for (int m = 0; m < materials.Count; m++)
            {
                CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
                meshes[m] = new Mesh();
                meshes[m].CombineMeshes(combineInstanceArray, true, true);
                combineInstances[m] = new CombineInstance();
                combineInstances[m].mesh = meshes[m];
                combineInstances[m].subMeshIndex = 0;
            }

            // Combine into one
            meshFilterCombine.sharedMesh = new Mesh();
            meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);

            // Destroy other meshes
            foreach (Mesh mesh in meshes)
            {
                mesh.Clear();
                DestroyImmediate(mesh);
            }
        }

        // For MeshRenderer
        {
            // Get / Create mesh renderer
            MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
            if (!meshRendererCombine)
                meshRendererCombine = gameObject.AddComponent<MeshRenderer>();

            // Assign materials
            Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
            meshRendererCombine.materials = materialsArray;
        }
    }
}