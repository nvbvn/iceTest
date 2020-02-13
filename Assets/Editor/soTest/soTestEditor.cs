using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(soTestMain))]
public class soTestEditor : Editor
{
    public override VisualElement CreateInspectorGUI() {
//        Debug.LogError("CreateInspectorGUI");
        VisualElement customInspector = new VisualElement();

        Button btn = new Button(btnClickListener);
        btn.text = "Test";
        customInspector.Add(btn);
        return customInspector;
    }

    private void btnClickListener() {
        TestSO so = ScriptableObject.CreateInstance<TestSO>();
        so.n = 666;
        AssetDatabase.CreateAsset(so, "Assets/TestFolder/TestSO.asset");
        AssetDatabase.SaveAssets();
    }
}
