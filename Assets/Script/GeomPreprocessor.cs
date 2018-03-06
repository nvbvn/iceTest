using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeomPreprocessor {

    public static int[] CreateTrilinks(Mesh srcMesh) {
        int[] res;
        int[] tris = srcMesh.triangles;
        Vector3[] verts = srcMesh.vertices;
        res = new int[tris.Length];
        int l = tris.Length;
        int i3;
        for (int i=0; 3*i<l; i++) {
            i3 = 3 * i;
            int t1 = findTrisIndex(tris, verts, tris[i3], tris[i3 + 1], i);
            int t2 = findTrisIndex(tris, verts, tris[i3 + 1], tris[i3 + 2], i);
            int t3 = findTrisIndex(tris, verts, tris[i3], tris[i3 + 2], i);
            if (t1 == -1 || t2 == -1 || t3 == -1) {
  //              Debug.LogError("Triangle without link");
            }
            res[i3] = t1;
            res[i3 + 1] = t2;
            res[i3 + 2] = t3;
        }
        return res;
    }

    private static int findTrisIndex(int[] tris, Vector3[] verts, int v1, int v2, int excludeTrisIndex) {
        int res = -1;
        int l = tris.Length;
        int i3;
        for (int i=0; 3*i<l; i++) {
            i3 = 3 * i;
            /*if ((tris[i3] == v1 || tris[i3 + 1] == v1 || tris[i3 + 2] == v1) && (tris[i3] == v2 || tris[i3 + 1] == v2 || tris[i3 + 2] == v2) && i3 != excludeTrisIndex) {
                res = i;
                break;
            }*/
            if ((verts[tris[i3]] == verts[v1] || verts[tris[i3 + 1]] == verts[v1] || verts[tris[i3 + 2]] == verts[v1]) && (verts[tris[i3]] == verts[v2] || verts[tris[i3 + 1]] == verts[v2] || verts[tris[i3 + 2]] == verts[v2]) && i != excludeTrisIndex)
            {
                res = i;
                break;
            }
        }
        return res;
    }

    public static string[] CreateTrisAroundVertex(Mesh srcMesh) {
        Vector3[] verts = srcMesh.vertices;
        int[] tris = srcMesh.triangles;
        int l = verts.Length;
        int tl = tris.Length;
        string[] res = new string[l];
        Vector3 v;
        int triN, j;
        List<int> temp = new List<int>();
        for (int i=0; i<l; i++) {
            v = verts[i];
            for (j=0; (triN = 3*j)<tl; j++) {
                if (verts[tris[triN]] == v || verts[tris[triN+1]] == v || verts[tris[triN+2]] == v) {
                    temp.Add(j);
                }
            }
            res[i] = string.Join(";", temp.ToArray());
            temp.Clear();
        }
        return res;
    }

}
