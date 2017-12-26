using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    [SerializeField]
    private MeshFilter cap1;
    [SerializeField]
    private MeshFilter cap2;
    [SerializeField]
    private MeshFilter cube;
    [SerializeField]
    private MeshFilter cubeP;
    [SerializeField]
    private MeshFilter cubeX;
    [SerializeField]
    private MeshFilter cubeV0;
    [SerializeField]
    private MeshFilter cubeV1;
    [SerializeField]
    private MeshFilter cubeV2;
    [SerializeField]
    private MeshFilter cubeRes;
    [SerializeField]
    private MeshFilter cubeRes2;
    [SerializeField]
    private MeshFilter cubeRes3;
    [SerializeField]
    private MeshFilter cubeRes4;
    [SerializeField]
    private MeshFilter cubeRes5;
    [SerializeField]
    private MeshCollider plane;
    // Use this for initialization

    private float D = 1.5f;
    private Vector3 Np = new Vector3(0, 1, 0);

    private Vector3[] _vertices;
    private int[] _triangles;

    private int[] _trilinks;

    void Start () {
        Debug.Log("");
        cube.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
        cubeP.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
        cubeX.GetComponent<Renderer>().material.color = new Color(0, 0.5f, 0.5f);
        cubeRes.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        cubeRes2.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
        cubeRes3.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
        cubeRes4.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
        cubeRes5.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
        cubeV0.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
        cubeV1.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
        cubeV2.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        //        Vector3 v = default(Vector3);

        Debug.Log(DateTime.Now.ToString());
        _trilinks = createTriangleLinks(cap1.mesh);
        Debug.Log(DateTime.Now.ToLongTimeString());

        _vertices = cap1.mesh.vertices;
        _triangles = cap1.mesh.triangles;

        Debug.DrawRay(cap1.mesh.vertices[cap1.mesh.triangles[0]], cap1.mesh.normals[cap1.mesh.triangles[0]], new Color(1, 0, 0), 1000);
        Debug.DrawRay(cap1.mesh.vertices[cap1.mesh.triangles[1]], cap1.mesh.normals[cap1.mesh.triangles[1]], new Color(1, 0, 0), 1000);
        Debug.DrawRay(cap1.mesh.vertices[cap1.mesh.triangles[2]], cap1.mesh.normals[cap1.mesh.triangles[2]], new Color(1, 0, 0),1000);

        Vector3[] vert = cap1.mesh.vertices;
        int i0, i1 = -1;
        Vector3 v = Vector3.one;
        Vector3 vt;
        for (int i = 1; i<vert.Length; i++) {
            vt = vert[0] - vert[i];
            if (vt.magnitude < v.magnitude) {
                v = vt;
                i0 = i;
            }
        }
        Debug.Log(" ");
    }

    private int[] createTriangleLinks(Mesh srcMesh) {
        int[] res;
        int[] tris = srcMesh.triangles;
        Vector3[] verts = srcMesh.vertices;
        res = new int[tris.Length];
        int l = tris.Length;
        //int v1, v2;
        int i3;
        for (int i=0; 3*i<l; i++) {
            i3 = 3 * i;
            int t1 = findTrisIndex(tris, verts, tris[i3], tris[i3 + 1], i);
            int t2 = findTrisIndex(tris, verts, tris[i3 + 1], tris[i3 + 2], i);
            int t3 = findTrisIndex(tris, verts, tris[i3], tris[i3 + 2], i);
            if (t1 == -1 || t2 == -1 || t3 == -1) {
                Debug.LogError("Triangle without link");
            } else {
                res[i3] = t1;
                res[i3 + 1] = t2;
                res[i3 + 2] = t3;
            }
        }
        return res;
    }

    private int findTrisIndex(int[] tris, Vector3[] verts, int v1, int v2, int excludeTrisIndex) {
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

	// Update is called once per frame
	void Update () {
		RaycastHit hit;
        
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit/*, Mathf.Infinity, 0*/))
            return;
        cube.transform.SetPositionAndRotation(hit.point, cube.transform.rotation);
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;
        
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];
        Vector3 baryCenter = hit.barycentricCoordinate;
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        interpolatedNormal = interpolatedNormal.normalized;
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);
        Debug.DrawRay(hit.point, interpolatedNormal);

        Vector3 V01 = vertices[triangles[hit.triangleIndex * 3 + 1]] - vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 V02 = vertices[triangles[hit.triangleIndex * 3 + 2]] - vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 v0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 v1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 v2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Vector3 n = Vector3.Cross(V01, V02);
        Debug.DrawRay(hit.point, 5*n, new Color(0, 1, 0));

        cubeV1.transform.SetPositionAndRotation(vertices[triangles[hit.triangleIndex * 3 + 0]], cubeRes.transform.rotation);
        cubeV2.transform.SetPositionAndRotation(vertices[triangles[hit.triangleIndex * 3 + 1]], cubeRes.transform.rotation);

        int k = 1;

        Vector3 A = hit.point;
        Vector3 N = n.normalized;
        Vector3 X = A + k * N;
        Vector3 P = X - (Vector3.Dot(Np, X) + D) * Np;
        Debug.DrawLine(X, P, new Color(0, 0, 1));
        cubeP.transform.SetPositionAndRotation(P, cubeP.transform.rotation);
        cubeX.transform.SetPositionAndRotation(X, cubeX.transform.rotation);

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
        if (I.y != float.PositiveInfinity) {
            cubeRes.transform.SetPositionAndRotation(I, cubeRes.transform.rotation);
        }

        // cubeRes.transform.SetPositionAndRotation(I1, cubeRes.transform.rotation);
        int qq;
        Vector3 QQ;
        int tt = _trilinks[hit.triangleIndex * 3 + edgeIndex];
        processTriangle(tt, I, out qq, out QQ);
        if (QQ != Vector3.positiveInfinity) {
            cubeRes2.transform.SetPositionAndRotation(QQ, cubeRes2.transform.rotation);
        } else {
            Debug.LogError("ups");
        }
        
        int qqq;
        Vector3 QQQ;
        int ttt = _trilinks[tt * 3 + qq];
        processTriangle(ttt, QQ, out qqq, out QQQ);
        if (QQQ != Vector3.positiveInfinity)
        {
            cubeRes3.transform.SetPositionAndRotation(QQQ, cubeRes3.transform.rotation);
        }
        int qqqq;
        Vector3 QQQQ;
        int tttt = _trilinks[ttt * 3 + qqq];
        processTriangle(tttt, QQQ, out qqqq, out QQQQ);
        cubeRes4.transform.SetPositionAndRotation(QQQQ, cubeRes4.transform.rotation);
        int qqqqq;
        Vector3 QQQQQ;
        int ttttt = _trilinks[tttt * 3 + qqqq];
        processTriangle(ttttt, QQQQ, out qqqqq, out QQQQQ);
        cubeRes5.transform.SetPositionAndRotation(QQQQQ, cubeRes5.transform.rotation);
    }

    private void processTriangle(int triN, Vector3 A, out int edgeN, out Vector3 Anew) {
        Vector3 V01 = _vertices[_triangles[triN * 3 + 1]] - _vertices[_triangles[triN * 3 + 0]];
        Vector3 V02 = _vertices[_triangles[triN * 3 + 2]] - _vertices[_triangles[triN * 3 + 0]];
        Vector3 v0 = _vertices[_triangles[triN * 3 + 0]];
        Vector3 v1 = _vertices[_triangles[triN * 3 + 1]];
        Vector3 v2 = _vertices[_triangles[triN * 3 + 2]];

        cubeV0.transform.SetPositionAndRotation(v0, cubeV0.transform.rotation);
        cubeV1.transform.SetPositionAndRotation(v1, cubeV1.transform.rotation);
        cubeV2.transform.SetPositionAndRotation(v2, cubeV2.transform.rotation);

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
        if (k>0 && k<1) {
            res = T1 + k * W;
        }

        return res;
    }

}
