using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Icecream))] 
public class Blobfall : MonoBehaviour
{
    [SerializeField]
    private MeshCollider targetMeshCollider;

    [SerializeField]
    private Transform cube;
    [SerializeField]
    private Transform blobPrefab;
    private List<Transform> cubes = new List<Transform>();

    private Icecream _targetIce;


    private List<Transform> _freeBlobs = new List<Transform>();
    private List<BlobGuide> _guides = new List<BlobGuide>();


    private void Start() {
        _targetIce = GetComponent<Icecream>();
    }

    void Update() {
        int i;
        for (i=0; i<cubes.Count; i++) {
            cubes[i].GetComponent<Renderer>().enabled = false;
        }

        foreach (BlobGuide guide in _guides) {
            guide.Update();
        }


        List<Vector3> points = new List<Vector3>();

   	    RaycastHit hit;
        //Debug.LogError(Input.mousePosition.x+", "+ Input.mousePosition.y+", "+ Input.mousePosition.z);
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            return;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        if (meshCollider == targetMeshCollider) {
            points = _targetIce.GetBlobPath(gameObject.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
        }
        Transform cb;
        while (points.Count > cubes.Count) {
            cb = Instantiate(cube, new Vector3(), Quaternion.identity);
            cb.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            cubes.Add(cb);
        }
        for (i=0; i<points.Count; i++) {
            cubes[i].GetComponent<Renderer>().enabled = true;
            cubes[i].SetParent(gameObject.transform);

            if (float.IsInfinity(points[i].x)) {
                Debug.LogError("!!!"+meshCollider.name+": ("+hit.point.x+"; "+hit.point.y+"; "+hit.point.z+")|"+hit.triangleIndex);
            }
            cubes[i].transform.localPosition = points[i];
        }

        if (Input.GetMouseButtonDown(0)) {
            Transform blob = getBlob();
            blob.SetParent(gameObject.transform);
            BlobGuide blobGuide = new BlobGuide(blob, points);
            _guides.Add(blobGuide);
        }
    }

    private Transform getBlob() {
        Transform res = null;
        if (_freeBlobs.Count > 0) {
            res = _freeBlobs[0];
            _freeBlobs.RemoveAt(0);
        } else {
            res = Instantiate(blobPrefab, new Vector3(), Quaternion.identity);
            res.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
        }
        return res;
    }

}
