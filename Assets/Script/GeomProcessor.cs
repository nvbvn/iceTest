using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeomProcessor {
    private readonly Vector3 Np = new Vector3(0, 1, 0);
    private readonly float D = 1.5f;

    private Mesh _mesh;
    private Transform _transform;
    private int[] _trilinks;
    private Vector3[] _vertices;
    private int[] _triangles;

    Vector4[] tangents;
    Vector3[] normals;

    public GeomProcessor(Mesh mesh, int[] trilinks, Transform transform) {
        _mesh = mesh;
        _transform = transform;
        _trilinks = trilinks;
        _vertices = mesh.vertices;
        _triangles = mesh.triangles;
        tangents = mesh.tangents;
        normals = mesh.normals;
    }

    public List<Vector3> GetEdgeIntersectPoints(Vector3 startPoint, int startTriangle) {
        List<Vector3> res = new List<Vector3>();
        res.Add(startPoint);
        float minY = _mesh.bounds.min.y;
        int edgeN;
        Vector3 Anew;
        int n = 0;

        //List<int> st = new List<int>();
        int i;
        int triN;
        int edgeToPreviousTris = -1;
        int previousTris;
        bool temp = false;
        int vN;
        int edgeN_temp;
        Vector3 vNormal;
//        bool b = false;
        do {
            triN = 3 * startTriangle;
            if (startPoint == _vertices[_triangles[triN]] || startPoint == _vertices[_triangles[triN + 1]] || startPoint == _vertices[_triangles[triN + 2]]) {
                List<int> trianglesAroundV = getTrianglesAroundVertex(startPoint, startTriangle, out vNormal);
                List<int> trianglesAroundVLowThan = getLowerTrianglesAroundVertex(trianglesAroundV, startPoint);
                Anew = startPoint;
                edgeN = 0;
                for (i = 0; i < trianglesAroundVLowThan.Count; i++) {
                    processTriangleByVector(trianglesAroundVLowThan[i], startPoint, vNormal, out edgeN, out Anew);
                    if (Anew.y < startPoint.y) {
                        startTriangle = trianglesAroundVLowThan[i];
                        triN = 3 * startTriangle;
                        break;
                    }
                }
                Debug.DrawRay(_transform.TransformPoint( _vertices[_triangles[triN]]), new Vector3(0, 0, -1), new Color(1, 0, 0));
                Debug.DrawRay(_transform.TransformPoint( _vertices[_triangles[triN+1]]), new Vector3(0, 0, -1), new Color(0.5f, 0, 0));
/*                for (int k=0; k<trianglesAroundV.Count; k++) {
                    if (b) {
                        break;
                    }
                    Debug.DrawLine(_transform.TransformPoint(_vertices[_triangles[triN]]), _transform.TransformPoint(_vertices[_triangles[triN]]+ normals[_triangles[triN]]), new Color(1, 0, 0));
                    Debug.DrawLine(_transform.TransformPoint(_vertices[_triangles[triN+1]]), _transform.TransformPoint(_vertices[_triangles[triN+1]] + normals[_triangles[triN+1]]), new Color(0, 1, 0));
                    Debug.DrawLine(_transform.TransformPoint(_vertices[_triangles[triN+2]]), _transform.TransformPoint(_vertices[_triangles[triN+2]] + normals[_triangles[triN+2]]), new Color(0, 0, 1));
                    b = true;
                }
*/
                //processTriangleByNormal(startTriangle, startPoint, out edgeN, out Anew, edgeToPreviousTris);
            } else { 
                processTriangleByNormal(startTriangle, startPoint, out edgeN, out Anew, edgeToPreviousTris);
            
                if (res[res.Count - 1] == Anew) {

                    edgeN_temp = edgeN;
                    //разобраться с ситуацией, когда ребро горизонтально
                    if (edgeN == 0) {
                        vN = getLowestN(_vertices[_triangles[triN + 1]], _vertices[_triangles[triN]]);
                        if (vN == 1) {
                            Anew = _vertices[_triangles[triN + 1]];
                            edgeN = 1;
                            //Debug.DrawLine(_transform.TransformPoint( _vertices[_triangles[triN+1]]), _transform.TransformPoint(_vertices[_triangles[triN + 2]]), new Color(1, 0, 0));
                        } else {
                            Anew = _vertices[_triangles[triN]];
                            edgeN = 2;
                            //Debug.DrawLine(_transform.TransformPoint(_vertices[_triangles[triN]]), _transform.TransformPoint(_vertices[_triangles[triN + 2]]), new Color(0.5f, 0, 0));
                        }
                    } else if (edgeN == 1) {
                        vN = getLowestN(_vertices[_triangles[triN + 2]], _vertices[_triangles[triN + 1]]);
                        if (vN == 1) {
                            Anew = _vertices[_triangles[triN + 2]];
                            edgeN = 2;
                            //Debug.DrawLine(_transform.TransformPoint(_vertices[_triangles[triN]]), _transform.TransformPoint(_vertices[_triangles[triN + 2]]), new Color(0, 1, 0));
                        } else {
                            Anew = _vertices[_triangles[triN + 1]];
                            edgeN = 0;
                            //Debug.DrawLine(_transform.TransformPoint(_vertices[_triangles[triN]]), _transform.TransformPoint(_vertices[_triangles[triN + 1]]), new Color(0, 0.5f, 0));
                        }
                    } else if (edgeN == 2) {
                        vN = getLowestN(_vertices[_triangles[triN + 2]], _vertices[_triangles[triN]]);
                        if (vN == 1) {
                            Anew = _vertices[_triangles[triN + 2]];
                            edgeN = 1;
                        } else {
                            Anew = _vertices[_triangles[triN]];
                            edgeN = 0;
                        }
                    }

                    /*if (edgeN == edgeToPreviousTris) {
                        edgeN = edgeN_temp;
                        Anew = res[res.Count - 1];
                    }*/
              //      temp = true;
                }
            }
            res.Add(Anew);

            previousTris = startTriangle;
            startTriangle = _trilinks[3 * startTriangle + edgeN];

            if (startTriangle == -1) {
                break;
            }

            triN = 3 * startTriangle;
            for (edgeToPreviousTris = 0; edgeToPreviousTris<3; edgeToPreviousTris++) {
                if (_trilinks[triN + edgeToPreviousTris] == previousTris) {
                    break;
                }
            }
            startPoint = Anew;

            n++;
            if (res.Count == 120) {
                break;
            }
            triN = 3 * startTriangle;
            if (res.Count == 11) {
                triN = 3 * startTriangle;
            }
        } while (!MathUtil.ApproximatelyLessThanOrEqual(Anew.y, minY));
        return res;
    }

    private List<int> getTrianglesAroundVertex(Vector3 vertex, int srcTriangle, out Vector3 vNormal) {
        List<int> res = new List<int>();
        int l = _triangles.Length;
        int triN;
        vNormal = new Vector3(0, 0, 0);
        Vector3 n;
        for (int i = 0; (triN = 3 * i) < l; i++) {
            if ((_vertices[_triangles[triN]] == vertex || _vertices[_triangles[triN + 1]] == vertex || _vertices[_triangles[triN + 2]] == vertex) && i != srcTriangle) {
                n = Vector3.Cross(_vertices[_triangles[triN + 1]] - _vertices[_triangles[triN + 0]], _vertices[_triangles[triN + 2]] - _vertices[_triangles[triN + 0]]);
                n.Normalize();
                vNormal += n;
                //if (_vertices[_triangles[triN]].y < vertex.y || _vertices[_triangles[triN + 1]].y < vertex.y || _vertices[_triangles[triN + 2]].y < vertex.y) {
                    res.Add(i);
                //}
            }
        }
        vNormal.Normalize();
        return res;
    }

    private List<int> getLowerTrianglesAroundVertex(List<int> tris, Vector3 vert) {
        List<int> res = new List<int>();
        int triN;
        Debug.DrawRay(_transform.TransformPoint(vert), new Vector3(0, 0, -1), new Color(0.5f, 0.5f, 0.5f));
        for (int i=0; i<tris.Count; i++) {
            triN = tris[i] * 3;
            //Debug.DrawRay(_transform.TransformPoint(_vertices[_triangles[triN]]), new Vector3(0, 0, -1), new Color(0, 1, 0));
            //Debug.DrawRay(_transform.TransformPoint(_vertices[_triangles[triN+1]]), new Vector3(0, 0, -1), new Color(0, 1, 0));
            //Debug.DrawRay(_transform.TransformPoint(_vertices[_triangles[triN+2]]), new Vector3(0, 0, -1), new Color(0, 1, 0));
            if (_vertices[_triangles[triN]].y < vert.y || _vertices[_triangles[triN + 1]].y < vert.y || _vertices[_triangles[triN + 2]].y < vert.y) {
                res.Add(tris[i]);
            }
        }
        return res;
    }

    private int getLowestN(Vector3 V1, Vector3 V0) {
        int res = 1;
        if (V0.y < V1.y) {
            res = 2;
        }
        return res;
    }



    private struct VertexN {
        public Vector3 Vertex;
        public int N;
        public VertexN(Vector3 vertex, int n) { Vertex = vertex; N = n; }
    }
    private VertexN[] _verts = {new VertexN(new Vector3(), -1), new VertexN(new Vector3(), -1), new VertexN(new Vector3(), -1) };
    private int vertsComparer(VertexN a, VertexN b) {
        int res = 1;
        if (Mathf.Approximately(a.Vertex.y, b.Vertex.y)) {
            res = 0;
        } else if (a.Vertex.y < b.Vertex.y) {
            res = -1;
        }
        return res;
    }


    private void processTriangleByVector(int triangleN, Vector3 A, Vector3 vectorFromA, out int edgeN, out Vector3 Anew, int edgeToPreviousTriangle = -1) {
        Vector3 X = A + 3*vectorFromA;
        processTriangle(triangleN, A, X, out edgeN, out Anew, edgeToPreviousTriangle);
    }
    
    private void processTriangleByNormal(int triangleN, Vector3 A, out int edgeN, out Vector3 Anew, int edgeToPreviousTriangle = -1) {
        int triN = triangleN * 3;
        Vector3 V01 = _vertices[_triangles[triN + 1]] - _vertices[_triangles[triN + 0]];
        Vector3 V02 = _vertices[_triangles[triN + 2]] - _vertices[_triangles[triN + 0]];
        Vector3 n = Vector3.Cross(V01, V02);
        int k = 1;
        Vector3 N = n.normalized;
        Vector3 X = A + k * N;

        processTriangle(triangleN, A, X, out edgeN, out Anew, edgeToPreviousTriangle);
    }

    public void processTriangle(int triangleN, Vector3 A, Vector3 X, out int edgeN, out Vector3 Anew, int edgeToPreviousTriangle = -1) {
        int triN = triangleN * 3;
        Vector3 v0 = _vertices[_triangles[triN + 0]];
        Vector3 v1 = _vertices[_triangles[triN + 1]];
        Vector3 v2 = _vertices[_triangles[triN + 2]];


        Vector3 P = X - (Vector3.Dot(Np, X) + D) * Np;
        //Debug.DrawLine(X, P, new Color(0, 0, 1));

        int edgeIndex = 0;
        Vector3 I0 = edgeIntersect(P, A, X, v1, v0);
        Vector3 I1 = edgeIntersect(P, A, X, v1, v2);
        Vector3 I2 = edgeIntersect(P, A, X, v2, v0);

        Vector3 I;

        _verts[0].Vertex = I0; _verts[0].N = 0;
        _verts[1].Vertex = I1; _verts[1].N = 1;
        _verts[2].Vertex = I2; _verts[2].N = 2;
        Array.Sort(_verts, vertsComparer);

        if (Vector3.Equals(_verts[0].Vertex, _verts[1].Vertex)) {
            if (_verts[0].N != edgeToPreviousTriangle) {
                edgeIndex = _verts[0].N;
                I = _verts[0].Vertex;
            } else {
                edgeIndex = _verts[1].N;
                I = _verts[1].Vertex;
            }
        } else {
            edgeIndex = _verts[0].N;
            I = _verts[0].Vertex;
        }
       

 /*       I = I0;
        if (I1.y < I.y) {
            I = I1;
            edgeIndex = 1;
        }
        if (I2.y < I.y) {
            I = I2;
            edgeIndex = 2;
        }*/
      /*  if (I == A) {
            Debug.Log("upsss");
        }*/
        edgeN = edgeIndex;
        Anew = I;
        if (Anew == new Vector3(0.3011334f, 0.3f, -0.9010766f)) {
            Debug.DrawLine(_transform.TransformPoint(v1), _transform.TransformPoint(v0), new Color(0, 0, 1));
            Debug.DrawLine(_transform.TransformPoint(v1), _transform.TransformPoint(v2), new Color(0, 1, 0));
            Debug.DrawLine(_transform.TransformPoint(v2), _transform.TransformPoint(v0), new Color(1, 0, 0));
        }
    }

    private Vector3 edgeIntersect(Vector3 P, Vector3 A, Vector3 X, Vector3 T1, Vector3 T2) {
        Vector3 res = Vector3.positiveInfinity;
        Vector3 a = A - X;
        Vector3 b = P - X;
        Vector3 NSP = Vector3.Cross(b, a);
        NSP.Normalize();
        Vector3 Nsp = NSP;
        //Debug.DrawLine(A, A+Nsp, new Color(0.5f, 0.5f, 0));
        Vector3 V = A - T1;
        Vector3 W = T2 - T1;
        float d = Vector3.Dot(Nsp, V);
        float e = Vector3.Dot(Nsp, W);
        float k = d / e;
        if (k>=0 && k<=1 ) {
            res = T1 + k * W;
        }
        return res;
    }

}
