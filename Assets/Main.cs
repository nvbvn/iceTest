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


    private GeomProcessor _testGeomProcessor;
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

        Debug.Log(DateTime.Now.ToString());
        _trilinks = GeomPreprocessor.CreateTrilinks(cap1.mesh);
      //  String str = String.Join(",", _trilinks);
        Debug.Log(DateTime.Now.ToLongTimeString()+","+_trilinks.ToString());

        _vertices = cap1.mesh.vertices;
        _triangles = cap1.mesh.triangles;

        _testGeomProcessor = new GeomProcessor(cap1.mesh, _trilinks);
        Debug.Log("???");
    }



	// Update is called once per frame
	void Update () {
        int i;
        for (i=0; i<cubes.Count; i++) {
            cubes[i].GetComponent<Renderer>().enabled = false;
        }

		RaycastHit hit;
        
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit/*, Mathf.Infinity, 0*/))
            return;
        //cube.transform.SetPositionAndRotation(hit.point, cube.transform.rotation);
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        List<Vector3> points = _testGeomProcessor.GetEdgeIntersectPoints(hit.point, hit.triangleIndex);

        Transform cb;
        while (points.Count > cubes.Count) {
            cb = Instantiate(cube, new Vector3(), Quaternion.identity);
            cb.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            cubes.Add(cb);
        }
        for (i=0; i<points.Count; i++) {
            cubes[i].GetComponent<Renderer>().enabled = true;
            cubes[i].transform.SetPositionAndRotation(points[i], Quaternion.identity);
        }
    }



}
