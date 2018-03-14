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

    
    private GeomProcessor _testGeomProcessor_spincone;
    private GeomProcessor _testGeomProcessor_spiralLow;
    private GeomProcessor _testGeomProcessor_spiralMid;
    private GeomProcessor _testGeomProcessor_testIce;
    [SerializeField]
    private Transform cube;
    private List<Transform> cubes = new List<Transform>();

    void Start () {
        Application.targetFrameRate = 60;

/*        string trisAroundVertex = string.Join(",", GeomPreprocessor.CreateTrisAroundVertex(icSpincone.mesh));
        trisAroundVertex = string.Join(",", GeomPreprocessor.CreateTrisAroundVertex(icSpiralLow.mesh));
        trisAroundVertex = string.Join(",", GeomPreprocessor.CreateTrisAroundVertex(icSpiralMid.mesh));
        trisAroundVertex = string.Join(",", GeomPreprocessor.CreateTrisAroundVertex(testIce.mesh));
        Debug.Log(DateTime.Now.ToString());

        _trilinks = GeomPreprocessor.CreateTrilinks(icSpincone.mesh);
        String str = String.Join(",", _trilinks);
        _trilinks = GeomPreprocessor.CreateTrilinks(icSpiralLow.mesh);
        str = String.Join(",", _trilinks);
        _trilinks = GeomPreprocessor.CreateTrilinks(icSpiralMid.mesh);
        str = String.Join(",", _trilinks);

        Debug.Log(DateTime.Now.ToString());*/


        TextAsset txt = Resources.Load("Trilinks/spincone") as TextAsset;
        int[] trilinks_spincone = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/spiralLow") as TextAsset;
        int[] trilinks_spiralLow = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/spiralMid") as TextAsset;
        int[] trilinks_spiralMid = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/testIce") as TextAsset;
        int[] trilinks_testIce = Array.ConvertAll(txt.text.Split(','), int.Parse);

        int[][] tav_spincone = createTav("TrisAroundVertex/spincone");
        int[][] tav_spiralLow = createTav("TrisAroundVertex/spiralLow");
        int[][] tav_spiralMid = createTav("TrisAroundVertex/spiralMid");
        int[][] tav_testIce = createTav("TrisAroundVertex/testIce");

        //TimeSpan ts1 = TimeSpan.FromTicks(DateTime.Now.Ticks);
        //Debug.Log(DateTime.Now.ToString());
        _testGeomProcessor_spincone = new GeomProcessor(icSpincone.mesh, trilinks_spincone, tav_spincone, icSpincone.transform);
        _testGeomProcessor_spiralLow = new GeomProcessor(icSpiralLow.mesh, trilinks_spiralLow, tav_spiralLow, icSpiralLow.transform);
        _testGeomProcessor_spiralMid = new GeomProcessor(icSpiralMid.mesh, trilinks_spiralMid, tav_spiralMid, icSpiralMid.transform);
        _testGeomProcessor_testIce = new GeomProcessor(testIce.mesh, trilinks_testIce, tav_testIce, testIce.transform);
        TimeSpan ts2 = TimeSpan.FromTicks(DateTime.Now.Ticks);
        //Debug.Log((ts2 - ts1).Milliseconds);
        //Debug.Log(DateTime.Now.ToString());

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

    private int[][] createTav(string src) {
        TextAsset txt = Resources.Load(src) as TextAsset;
        string[] temp = txt.text.Split(',');
        int l = temp.Length;
        int[][] res = new int[l][];
        for (int i=0; i<l; i++) {
            res[i] = Array.ConvertAll(temp[i].Split(';'), int.Parse);
        }
        return res;
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
        
  /*      if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            return;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;


        if (meshCollider == icSpincone.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor_spincone.GetEdgeIntersectPoints(icSpincone.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        } else if (meshCollider == icSpiralLow.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor_spiralLow.GetEdgeIntersectPoints(icSpiralLow.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        } else if (meshCollider == icSpiralMid.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor_spiralMid.GetEdgeIntersectPoints(icSpiralMid.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        } else {
            points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        }
    */    
        
        
        //points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        
    /*    MeshCollider meshCollider = testIce.GetComponent<MeshCollider>();
        //points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(new Vector3(-0.7801354f, 0.1426054f, -0.2359176f)), 202);
        points = _testGeomProcessor_testIce.GetEdgeIntersectPoints(testIce.transform.InverseTransformPoint(new Vector3(-1.349147f, 0.7382534f, -0.1626387f)), 322);
        */
        MeshCollider meshCollider = icSpiralLow.GetComponent<MeshCollider>();
        points = _testGeomProcessor_spiralLow.GetEdgeIntersectPoints(icSpiralLow.transform.InverseTransformPoint(new Vector3(2.927554f, 0.9878784f, -0.008455698f)), 9092);
        
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
            if (float.IsInfinity(points[i].x)) {
                Debug.Log("!!!");
            }
            cubes[i].transform.localPosition = points[i];
        }
    }



}
