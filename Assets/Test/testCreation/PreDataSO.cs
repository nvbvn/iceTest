using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PreDataSO", menuName = "ice/SO/PreDataSO")]
public class PreDataSO : ScriptableObject {
    public string[] trisAroundVertex;
    public int[] trilinks;

    public int[][] GetTav() {
        int l = trisAroundVertex.Length;
        int[][] res = new int[l][];
        for (int i=0; i<l; i++) {
            res[i] = Array.ConvertAll(trisAroundVertex[i].Split(';'), int.Parse);
        }
        return res;
    }
    
    public int[][] createTav(string src) {
        int l = trisAroundVertex.Length;
        int[][] res = new int[l][];
        for (int i=0; i<l; i++) {
            res[i] = Array.ConvertAll(trisAroundVertex[i].Split(';'), int.Parse);
        }
        return res;
    }
}
