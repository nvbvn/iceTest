using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtil : MonoBehaviour
{
    public static void PrepareFolder(string folderPath) {
        string[] folderPathByStep = folderPath.Split('/');
        string increasingPath = string.Empty;
        string parentPath = "Assets";
        for (int i=0; i<folderPathByStep.Length; i++) {
            increasingPath += "/"+folderPathByStep[i];
            if (!AssetDatabase.IsValidFolder("Assets"+increasingPath)) {
                AssetDatabase.CreateFolder(parentPath, folderPathByStep[i]);
            }
            parentPath += "/"+folderPathByStep[i];
        }
    }
}
