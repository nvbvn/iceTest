using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


[CustomEditor(typeof(DataCreation))]
public class DataCreationEditor : Editor
{



    public override VisualElement CreateInspectorGUI() {
        VisualElement customInspector = new VisualElement();

        TextField m_ObjectNameBinding = new TextField("LocalPosition");
        m_ObjectNameBinding.bindingPath = "m_LocalPosition";
        customInspector.Add(m_ObjectNameBinding);

        ObjectField targetMeshBind = new ObjectField("Target Mesh");
        targetMeshBind.objectType =  typeof(Mesh);
        targetMeshBind.bindingPath = "targetSurface";
        customInspector.Add(targetMeshBind);

        /*PropertyField targetMeshBind = new PropertyField(so.FindProperty("targetSurface"), "Target Mesh");
        //m_ObjectNameBinding.bindingPath = "targetSurface";
        customInspector.Add(targetMeshBind);*/

        /*m_ObjectNameBinding = new TextField("Target Mesh");
        m_ObjectNameBinding.bindingPath = "m_Name";
        customInspector.Add(m_ObjectNameBinding);
        */
        SerializedObject so = new SerializedObject(target as DataCreation);
        ///SerializedObject so = new SerializedObject((target as DataCreation).gameObject);
        customInspector.Bind(so);
        //        OnSelectionChange();

      //  Debug.LogError((target as DataCreation).gameObject.name = "QWE");

        Button btn = new Button(createPreDataClickListener);
        btn.text = "Create PreData";
        customInspector.Add(btn);
        return customInspector;
    }

    /*public override void OnInspectorGUI() {
        //serializedObject.Update();
        DrawDefaultInspector();
        //EditorGUILayout.PropertyField(lookAtPoint);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("qq"));
        //EditorGUILayout.HelpBox("This is a help box", MessageType.Error);
        //serializedObject.ApplyModifiedProperties();
        //EditorGUILayout.DropdownButton(GUIContent. "This is a help box", MessageType.Error);
        GUI.Button(new Rect(10, 10, 50, 10), "QQ");
    }*/

    private void createPreDataClickListener() {
        PreDataSO so = ScriptableObject.CreateInstance<PreDataSO>();
        so.trisAroundVertex = GeomPreprocessor.CreateTrisAroundVertex((target as DataCreation).targetSurface);
        AssetDatabase.CreateAsset(so, "Assets/PreData/TestSO.asset");
        AssetDatabase.SaveAssets();
    }
}
