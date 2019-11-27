using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icecream : MonoBehaviour
{

    [SerializeField]
    public TextAsset trilinks;
    [SerializeField]
    public TextAsset trisAroundVertex;

    private Mesh _mesh;
    private GeomProcessor _geomProcessor;

    void Start() {
        _mesh = GetComponent<MeshFilter>().sharedMesh;
        _geomProcessor = new GeomProcessor(_mesh, trilinks, trisAroundVertex, gameObject.transform);
    }

    public List<Vector3> GetBlobPath(Vector3 startPoint, int startTriangle) { 
        return _geomProcessor.GetEdgeIntersectPoints(startPoint, startTriangle);
    }
}
