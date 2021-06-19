using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlanetOBJExporter
{

    public static void SavePlanetMesh (GameObject[] planes, string name) {
        StringBuilder obj = new StringBuilder ("# Exported Mesh from Planet Workshop");
        int numVertsPerMesh = planes[0].GetComponent<MeshFilter> ().mesh.vertexCount;

        for(int i = 0; i < 6; i++) {
            Mesh m = planes[i].GetComponent<MeshFilter> ().mesh;
            Vector3[] verts = m.vertices;
            int[] tris = m.triangles;
            int numVertsPerSide = (int) Mathf.Sqrt (m.vertexCount);
            for (int j = 0; j < m.vertexCount; j++) {
                Vector3 v = planes[i].transform.TransformPoint (verts[j]);
                obj.Append ("\nv " + v.x + " " + v.y + " " + v.z);
                obj.Append ("\nvt " + U (i, j, numVertsPerSide) + " " + V (i, j, numVertsPerSide));
            }
            for (int k = 0; k < tris.Length; k += 3) {
                int a = (tris[k] + i * numVertsPerMesh + 1);
                int b = (tris[k+1] + i * numVertsPerMesh + 1);
                int c = (tris[k+2] + i * numVertsPerMesh + 1);
                obj.Append ("\nf " + a + "/" + a + " " + b + "/" + b + " " + c + "/" + c); // first vertex is 1 not zero
            }
        }

        string path = Application.dataPath + "/" + name + "_mesh.obj";

        File.WriteAllText (path, obj.ToString());
    }

    public static float U (int planeIdx, int vertNum, int numVertsPerSide) {
        int panel = (planeIdx + 1) % 3 - 1;
        return panel / 3f + ((vertNum % numVertsPerSide) / (float) (numVertsPerSide - 1)) / 3f + 1f / 3f;
    }

    public static float V (int planeIdx, int vertNum, int numVertsPerSide) {
        //return planeIdx / 3 / 2f + (vertNum / numVertsPerSide) / (float) numVertsPerSide;
        return (planeIdx / 3 + (vertNum / numVertsPerSide) / (float) (numVertsPerSide - 1)) / 2f;
    }
}
