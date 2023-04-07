/**
 * Original Github Gist: https://gist.github.com/blizzy78/1eb6fec9bf0442b9a856
 */

using UnityEngine;
using UnityEditor;

/*
Fixes a mesh's rotation after importing it from Blender. In Blender, the Z axis points in the
"up" direction, while in Unity, Z points in the "forward" direction. This script rotates the
mesh's vertices so that it is upright again.
To use this, create an object from a mesh in the hierarchy view, then locate the Mesh Filter
in the inspector. There, click on the new "Fix Rotation" button.
Unity forum thread: http://forum.unity3d.com/threads/axis-rotation-blender-vs-unity.312481/
See also: http://answers.unity3d.com/questions/304805/rotate-only-mesh-not-whole-gameobject.html
*/

[CustomEditor(typeof(MeshFilter))]
public class FixMeshRotationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Fix Rotation"))
        {
            MeshFilter meshFilter = (MeshFilter)target;
            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];
            Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                newVertices[i] = rotation * vertex;
            }
            mesh.vertices = newVertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            EditorUtility.SetDirty(meshFilter);
        }
    }
}