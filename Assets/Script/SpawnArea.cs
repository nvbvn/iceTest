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

    [HideInInspector]
    public Icecream ice;

    private void Reset() {
        targetMesh = GetComponent<MeshFilter>();
        ice = GetComponent<Icecream>();
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
