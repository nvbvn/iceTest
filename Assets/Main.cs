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
    private MeshCollider plane;
    // Use this for initialization

    private float D = 1.5f;
    private Vector3 Np = new Vector3(0, 1, 0);

    private Vector3[] _vertices;
    private int[] _triangles;

    private int[] _trilinks;
    private int[] _trilinks_doubleSphere;
    private int[] _trilinks_doubleSphere_tri;


    private GeomProcessor _testGeomProcessor;
    private GeomProcessor _testGeomProcessor_doubleSphere;
    private GeomProcessor _testGeomProcessor_doubleSphere_tri;
    [SerializeField]
    private Transform cube;
    private List<Transform> cubes = new List<Transform>();

    void Start () {
        Application.targetFrameRate = 60;
        /*       cube.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
               cubeP.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
               cubeX.GetComponent<Renderer>().material.color = new Color(0, 0.5f, 0.5f);
               cubeRes.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
               cubeRes2.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
               cubeRes3.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
               cubeRes4.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
               cubeRes5.GetComponent<Renderer>().material.color = new Color(0.75f, 0, 0);
               cubeV0.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
               cubeV1.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
               cubeV2.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);*/
        //        Vector3 v = default(Vector3);

/*        _trilinks = GeomPreprocessor.CreateTrilinks(doubleSphere.mesh);
        String str = String.Join(",", _trilinks);

        int[] tri = GeomPreprocessor.CreateTrilinks(doubleSphere_tri.mesh);
        String str_tri = String.Join(",", tri);
        */
        Debug.Log(DateTime.Now.ToString());
        //  _trilinks = GeomPreprocessor.CreateTrilinks(cap1.mesh);
        //  String str = String.Join(",", _trilinks);
        TextAsset txt = Resources.Load("Trilinks/capsule") as TextAsset;
        _trilinks = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/doubleSphere") as TextAsset;
        _trilinks_doubleSphere = Array.ConvertAll(txt.text.Split(','), int.Parse);
        txt = Resources.Load("Trilinks/doubleSphere_tri") as TextAsset;
        _trilinks_doubleSphere_tri = Array.ConvertAll(txt.text.Split(','), int.Parse);
        Debug.Log(DateTime.Now.ToLongTimeString()+","+_trilinks.ToString());

        _vertices = cap1.mesh.vertices;
        _triangles = cap1.mesh.triangles;

        _testGeomProcessor = new GeomProcessor(cap1.mesh, _trilinks);
        _testGeomProcessor_doubleSphere = new GeomProcessor(doubleSphere.mesh, _trilinks_doubleSphere);
        _testGeomProcessor_doubleSphere_tri = new GeomProcessor(doubleSphere_tri.mesh, _trilinks_doubleSphere_tri);
        Debug.Log("???");
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

		RaycastHit hit;
        
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit/*, Mathf.Infinity, 0*/))
            return;
        //cube.transform.SetPositionAndRotation(hit.point, cube.transform.rotation);
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        List<Vector3> points;
        if (meshCollider == cap1.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor.GetEdgeIntersectPoints(hit.point, hit.triangleIndex);
        } else if (meshCollider == doubleSphere.GetComponent<MeshCollider>()) {
            points = _testGeomProcessor_doubleSphere.GetEdgeIntersectPoints(doubleSphere.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        } else {
            points = _testGeomProcessor_doubleSphere_tri.GetEdgeIntersectPoints(doubleSphere_tri.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        }

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
            } else {
                cubes[i].SetParent(doubleSphere_tri.transform);
            }
            cubes[i].transform.localPosition = points[i];
        }
    }



}
