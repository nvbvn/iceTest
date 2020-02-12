using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(DataCreation))]
public class DataCreationEditor : Editor
{
    public override VisualElement CreateInspectorGUI() {
//        Debug.LogError("CreateInspectorGUI");
        VisualElement customInspector = new VisualElement();

        Button btn = new Button(createPreDataClickListener);
        btn.text = "Create PreData";
        customInspector.Add(btn);
        return customInspector;
    }

    private void createPreDataClickListener() { 
    
    }
}
