using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class TestColorMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("TestColorMaterial");
        Material mat = gameObject.GetComponent<Renderer>().sharedMaterial;
        Color[] cols = new Color[gameObject.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3];
        int i;
        for (i=0; i<cols.Length/2; i++) {
            cols[i] = new Color(1, 1, 0);
        }
        for (; i<cols.Length; i++) {
            cols[i] = new Color(0, 1, 1);
        }
        mat.SetColorArray("_Colors", cols);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
