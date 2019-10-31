using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField]
    private TextAsset triLinks;
    [SerializeField]
    private TextAsset trisAroundVertex;

    [SerializeField]
    private List<int> spawnTriangles;

    [HideInInspector]
    public  MeshFilter targetMesh;

    private void Reset() {
        targetMesh = GetComponent<MeshFilter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
