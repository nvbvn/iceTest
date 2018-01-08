using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    [SerializeField]
    private MeshFilter cap1;

    [SerializeField]
    private MeshFilter doubleSphere_tri;
    [SerializeField]
    private MeshFilter doubleSphere;

    [SerializeField]
    private MeshFilter testIce;

    [SerializeField]
    private MeshCollider plane;
    // Use this for initialization

    private float D = 1.5f;
    private Vector3 Np = new Vector3(0, 1, 0);

    private Vector3[] _vertices;
    private int[] _triangles;

    private int[] _trilinks;
    private int[] _trilinks_doubleSphere;
    private int[] _trilinks_doubleSphere_tri;
    private int[] _trilinks_testIce;


    private GeomProcessor _testGeomProcessor;
    private GeomProcessor _testGeomProcessor_doubleSphere;
    private GeomProcessor _testGeomProcessor_doubleSphere_tri;
    private GeomProcessor _testGeomProcessor_testIce;
    [SerializeField]
    private Transform cube;
    private List<Transform> cubes = new List<Transform>();

    void Start () {
        Application.targetFrameRate = 60;
 //      _trilinks = GeomPreprocessor.CreateTrilinks(testIce.mesh);
 //       String str = String.Join(",", _trilinks);
        
        Debug.Log(DateTime.Now.ToString());
        TextAsset txt = Resources.Load("Trilinks/capsule") as TextAsset;
        _trilinks = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/doubleSphere") as TextAsset;
        _trilinks_doubleSphere = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/doubleSphere_tri") as TextAsset;
        _trilinks_doubleSphere_tri = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/testIce") as TextAsset;
        _trilinks_testIce = Array.ConvertAll(txt.text.Split(','), int.Parse);

        _vertices = cap1.mesh.vertices;
        _triangles = cap1.mesh.triangles;

        _testGeomProcessor = new GeomProcessor(cap1.mesh, _trilinks, cap1.transform);
        _testGeomProcessor_doubleSphere = new GeomProcessor(doubleSphere.mesh, _trilinks_doubleSphere, doubleSphere.transform);
        _testGeomProcessor_doubleSphere_tri = new GeomProcessor(doubleSphere_tri.mesh, _trilinks_doubleSphere_tri, doubleSphere_tri.transform);
        _testGeomProcessor_testIce = new GeomProcessor(testIce.mesh, _trilinks_testIce, testIce.transform);
        Debug.Log("???");

  /*      Transform cb;
            cb = Instantiate(cube, new Vector3(), Quaternion.identity);
            cb.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
            cb.SetParent(testIce.transform);
            cb.transform.localPosition = testIce.mesh.vertices[testIce.mesh.triangles[3*120]];

        cb = Instantiate(cube, new Vector3(), Quaternion.identity);
        cb.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
        cb.SetParent(testIce.transform);
        cb.transform.localPosition = testIce.mesh.vertices[testIce.mesh.triangles[3 * 120+1]];

        cb = Instantiate(cube, new Vector3(), Quaternion.identity);
        cb.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
        cb.SetParent(testIce.transform);
        cb.transform.localPosition = testIce.mesh.vertices[testIce.mesh.triangles[3 * 120+2]];*/
    }



	// Update is called once per frame
	void Update () {
        int i;
        for (i=0; i<cubes.Count; i++) {
            cubes[i].GetComponent<Renderer>().enabled = false;
        }

  /*      Vector3 A = new Vector3(6.37312E-08f, -0.7039801f, -0.4557033f);
        int triN = 415;
        Vector3 Anew;
        int edgeN;
        _testGeomProcessor.processTriangle(triN, A, out edgeN, out Anew);


        Vector3 V01 = _vertices[_triangles[triN * 3 + 1]] - _vertices[_triangles[triN * 3 + 0]];
        Vector3 V02 = _vertices[_triangles[triN * 3 + 2]] - _vertices[_triangles[triN * 3 + 0]];
        Vector3 v0 = _vertices[_triangles[triN * 3 + 0]];
        Vector3 v1 = _vertices[_triangles[triN * 3 + 1]];
        Vector3 v2 = _vertices[_triangles[triN * 3 + 2]];

        Vector3 n = Vector3.Cross(V01, V02);
        Debug.DrawRay(A, n);
        return;*/

        List<Vector3> points;

 /*   	RaycastHit hit;
        
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            return;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        if (meshCollider == cap1.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor.GetEdgeIntersectPoints(cap1.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        } else if (meshCollider == doubleSphere.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor_doubleSphere.GetEdgeIntersectPoints(doubleSphere.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        } else if (meshCollider == doubleSphere_tri.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor_doubleSphere_tri.GetEdgeIntersectPoints(doubleSphere_tri.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        } else {
            points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        }*/
        MeshCollider meshCollider = testIce.GetComponent<MeshCollider>();
        points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(new Vector3(-0.7801354f, 0.1426054f, -0.2359176f)), 202);
        
        Transform cb;
        while (points.Count > cubes.Count) {
            cb = Instantiate(cube, new Vector3(), Quaternion.identity);
            cb.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            cubes.Add(cb);
        }
        for (i=0; i<points.Count; i++) {
            cubes[i].GetComponent<Renderer>().enabled = true;
            if (meshCollider == cap1.GetComponent<MeshCollider>()) {
                cubes[i].SetParent(cap1.transform);
            } else if (meshCollider == doubleSphere.GetComponent<MeshCollider>()) {
                cubes[i].SetParent(doubleSphere.transform);
            } else if (meshCollider == doubleSphere_tri.GetComponent<MeshCollider>()) {
                cubes[i].SetParent(doubleSphere_tri.transform);
            } else {
                cubes[i].SetParent(testIce.transform);
            }
            cubes[i].transform.localPosition = points[i];
        }
    }



}
