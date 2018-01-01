using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeomProcessor {
    private readonly Vector3 Np = new Vector3(0, 1, 0);
    private readonly float D = 1.5f;

    private Mesh _mesh;
    private int[] _trilinks;
    private Vector3[] _vertices;
    private int[] _triangles;

    public GeomProcessor(Mesh mesh, int[] trilinks) {
        _mesh = mesh;
        _trilinks = trilinks;
        _vertices = mesh.vertices;
        _triangles = mesh.triangles;
    }

    public List<Vector3> GetEdgeIntersectPoints(Vector3 startPoint, int startTriangle) {
        List<Vector3> res = new List<Vector3>();
        res.Add(startPoint);
        float minY = _mesh.bounds.min.y;
        int edgeN;
        Vector3 Anew;
        int n = 0;

        List<int> st = new List<int>();
        do {
            processTriangle(startTriangle, startPoint, out edgeN, out Anew);
            if (res[res.Count-1] == Anew) {
                Debug.Log("");
            }
            res.Add(Anew);
            st.Add(startTriangle);
            if (edgeN == -1) {
                Debug.LogError("DeadEnd");
                break;
            }
            startTriangle = _trilinks[3 * startTriangle + edgeN];
            startPoint = Anew;

            n++;
            if (n>100) {
                break;
            }
        } while (_vertices[_triangles[3*startTriangle]].y > minY && _vertices[_triangles[3 * startTriangle+1]].y > minY && _vertices[_triangles[3 * startTriangle+2]].y > minY);
        return res;
    }

    public void processTriangle(int triN, Vector3 A, out int edgeN, out Vector3 Anew) {
        Vector3 V01 = _vertices[_triangles[triN * 3 + 1]] - _vertices[_triangles[triN * 3 + 0]];
        Vector3 V02 = _vertices[_triangles[triN * 3 + 2]] - _vertices[_triangles[triN * 3 + 0]];
        Vector3 v0 = _vertices[_triangles[triN * 3 + 0]];
        Vector3 v1 = _vertices[_triangles[triN * 3 + 1]];
        Vector3 v2 = _vertices[_triangles[triN * 3 + 2]];

        Vector3 n = Vector3.Cross(V01, V02);

        int k = 1;

        Vector3 N = n.normalized;
        Vector3 X = A + k * N;
        Vector3 P = X - (Vector3.Dot(Np, X) + D) * Np;
        Debug.DrawLine(X, P, new Color(0, 0, 1));

        int edgeIndex = 0;
        Vector3 I0 = edgeIntersect(P, A, X, v1, v0);
        Vector3 I1 = edgeIntersect(P, A, X, v1, v2);
        Vector3 I2 = edgeIntersect(P, A, X, v2, v0);

        Vector3 I = I0;
        if (I1.y < I.y) {
            I = I1;
            edgeIndex = 1;
        }
        if (I2.y < I.y) {
            I = I2;
            edgeIndex = 2;
        }
      /*  if (I == A) {
            Debug.Log("upsss");
        }*/
        edgeN = edgeIndex;
        Anew = I;
    }

    private Vector3 edgeIntersect(Vector3 P, Vector3 A, Vector3 X, Vector3 T1, Vector3 T2) {
        Vector3 res = Vector3.positiveInfinity;
        Vector3 a = A - X;
        Vector3 b = P - X;
        Vector3 NSP = Vector3.Cross(b, a);
        NSP.Normalize();
        Vector3 Nsp = NSP;
        Debug.DrawLine(A, A+Nsp, new Color(0.5f, 0.5f, 0));
        Vector3 V = A - T1;
        Vector3 W = T2 - T1;
        float d = Vector3.Dot(Nsp, V);
        float e = Vector3.Dot(Nsp, W);
        float k = d / e;
        if (k>=0 && k<1 ) {
            res = T1 + k * W;
        }
        return res;
    }

}
