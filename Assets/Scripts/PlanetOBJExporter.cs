using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlanetOBJExporter
{

    public static void SavePlanetMesh (GameObject[] planes, string name) {
        StringBuilder obj = new StringBuilder ();
        int numVertsPerMesh = planes[0].GetComponent<MeshFilter> ().mesh.vertexCount;

        for(int i = 0; i < 6; i++) {
            Mesh m = planes[i].GetComponent<MeshFilter> ().mesh;
            Vector3[] verts = m.vertices;
            int[] tris = m.triangles;
            for (int j = 0; j < m.vertexCount; j++) {
                Vector3 v = planes[i].transform.TransformPoint (verts[j]);
                obj.Append ("\nv " + v.x + " " + v.y + " " + v.z);
            }
            for (int k = 0; k < tris.Length; k += 3) {
                obj.Append ("\nf " + (tris[k] + i * numVertsPerMesh + 1) + " " + (tris[k + 1] + i * numVertsPerMesh + 1) + " " + (tris[k + 2] + i * numVertsPerMesh + 1)); // first vertex is 1 not zero
            }
        }

        string path = Application.dataPath + "/" + name + "_mesh.obj";

        File.WriteAllText (path, obj.ToString());
    }
}
