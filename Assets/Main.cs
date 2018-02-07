using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    [SerializeField]
    private MeshFilter icSpiralLow;

    [SerializeField]
    private MeshFilter icSpiralMid;
    [SerializeField]
    private MeshFilter icSpincone;

    [SerializeField]
    private MeshFilter testIce;

    [SerializeField]
    private MeshCollider plane;
    // Use this for initialization

    private Vector3[] _vertices;
    private int[] _triangles;

    private int[] _trilinks_spincone;
    private int[] _trilinks_spiralLow;
    private int[] _trilinks_spiralMid;
    private int[] _trilinks_testIce;


    private GeomProcessor _testGeomProcessor_spincone;
    private GeomProcessor _testGeomProcessor_spiralLow;
    private GeomProcessor _testGeomProcessor_spiralMid;
    private GeomProcessor _testGeomProcessor_testIce;
    [SerializeField]
    private Transform cube;
    private List<Transform> cubes = new List<Transform>();

    void Start () {
        Application.targetFrameRate = 60;

 /*       Debug.Log(DateTime.Now.ToString());

        _trilinks = GeomPreprocessor.CreateTrilinks(icSpincone.mesh);
        String str = String.Join(",", _trilinks);
        _trilinks = GeomPreprocessor.CreateTrilinks(icSpiralLow.mesh);
        str = String.Join(",", _trilinks);
        _trilinks = GeomPreprocessor.CreateTrilinks(icSpiralMid.mesh);
        str = String.Join(",", _trilinks);

        Debug.Log(DateTime.Now.ToString());*/

        TextAsset txt = Resources.Load("Trilinks/spincone") as TextAsset;
        _trilinks_spincone = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/spiralLow") as TextAsset;
        _trilinks_spiralLow = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/spiralMid") as TextAsset;
        _trilinks_spiralMid = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/testIce") as TextAsset;
        _trilinks_testIce = Array.ConvertAll(txt.text.Split(','), int.Parse);

        _testGeomProcessor_spincone = new GeomProcessor(icSpincone.mesh, _trilinks_spincone, icSpincone.transform);
        _testGeomProcessor_spiralLow = new GeomProcessor(icSpiralLow.mesh, _trilinks_spiralLow, icSpiralLow.transform);
        _testGeomProcessor_spiralMid = new GeomProcessor(icSpiralMid.mesh, _trilinks_spiralMid, icSpiralMid.transform);
        _testGeomProcessor_testIce = new GeomProcessor(testIce.mesh, _trilinks_testIce, testIce.transform);

  /*      Transform cb;
            cb = Instantiate(cube, new Vector3(), Quaternion.identity);
            cb.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
            cb.SetParent(testIce.transform);
            cb.transform.localPosition = testIce.mesh.vertices[testIce.mesh.triangles[3*83]];

        cb = Instantiate(cube, new Vector3(), Quaternion.identity);
        cb.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        cb.SetParent(testIce.transform);
        cb.transform.localPosition = testIce.mesh.vertices[testIce.mesh.triangles[3 * 83+1]];

        cb = Instantiate(cube, new Vector3(), Quaternion.identity);
        cb.GetComponent<Renderer>().material.color = new Color(0.9f, 0.9f, 0.9f);
        cb.SetParent(testIce.transform);
        cb.transform.localPosition = testIce.mesh.vertices[testIce.mesh.triangles[3 * 83+2]];
        */
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

   	RaycastHit hit;
        
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            return;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;


        /*        if (meshCollider == cap1.GetComponent<MeshCollider>()) {
                    points = _testGeomProcessor.GetEdgeIntersectPoints(cap1.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
                } else if (meshCollider == doubleSphere.GetComponent<MeshCollider>()) {
                    points = _testGeomProcessor_doubleSphere.GetEdgeIntersectPoints(doubleSphere.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
                } else if (meshCollider == doubleSphere_tri.GetComponent<MeshCollider>()) {
                    points = _testGeomProcessor_doubleSphere_tri.GetEdgeIntersectPoints(doubleSphere_tri.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
                } else {
                    points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
                }
        */

        //if (meshCollider == testIce.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        //}
        /*MeshCollider meshCollider = testIce.GetComponent<MeshCollider>();
        points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(new Vector3(-0.7801354f, 0.1426054f, -0.2359176f)), 202);
        */
        Transform cb;
        while (points.Count > cubes.Count) {
            cb = Instantiate(cube, new Vector3(), Quaternion.identity);
            cb.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            cubes.Add(cb);
        }
        for (i=0; i<points.Count; i++) {
            cubes[i].GetComponent<Renderer>().enabled = true;
            if (meshCollider == icSpincone.GetComponent<MeshCollider>()) {
                cubes[i].SetParent(icSpincone.transform);
            } else if (meshCollider == icSpiralLow.GetComponent<MeshCollider>()) {
                cubes[i].SetParent(icSpiralLow.transform);
            } else if (meshCollider == icSpiralMid.GetComponent<MeshCollider>()) {
                cubes[i].SetParent(icSpiralMid.transform);
            } else {
                cubes[i].SetParent(testIce.transform);
            }
            cubes[i].transform.localPosition = points[i];
        }
    }



}
