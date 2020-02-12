using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

[CustomEditor(typeof(DataCreation))]
public class DataCreationEditor : Editor
{
    private static string CREATE_PREDATA = "Create PreData";
    private static string MESH_UNAVAILABLE = "Mesh Unavailable!";

    private ObjectField _targetSurfaceBind;
    private Button _createPreDataBtn;

    public override VisualElement CreateInspectorGUI() {
        VisualElement customInspector = new VisualElement();

        _targetSurfaceBind = new ObjectField("Target Surface");
        _targetSurfaceBind.RegisterValueChangedCallback(targetSurfaceChanged);
        _targetSurfaceBind.objectType =  typeof(GameObject);
        _targetSurfaceBind.bindingPath = "targetSurface";
        customInspector.Add(_targetSurfaceBind);

        SerializedObject so = new SerializedObject(target as DataCreation);
        customInspector.Bind(so);

        _createPreDataBtn = new Button(createPreDataClickListener);
        _createPreDataBtn.text = CREATE_PREDATA;
        customInspector.Add(_createPreDataBtn);

        checkMeshAvailabilityInTargetSurface(_targetSurfaceBind.value as GameObject);

        return customInspector;
    }

    private void targetSurfaceChanged(ChangeEvent<UnityEngine.Object> e) {
        checkMeshAvailabilityInTargetSurface(e.newValue as GameObject);
    }

    private void checkMeshAvailabilityInTargetSurface(GameObject targetSurface) {
        bool res = false;
        if (targetSurface != null) {
            MeshFilter mf = targetSurface.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null) {
                res = true;
            }
        }
        _createPreDataBtn.SetEnabled(res);
        _createPreDataBtn.text = res ? CREATE_PREDATA : MESH_UNAVAILABLE;

        //return res;
    }

    private void targetSurfaceChanged() { 
    
    
    }

    private void createPreDataClickListener() {
        PreDataSO so = ScriptableObject.CreateInstance<PreDataSO>();
        so.trisAroundVertex = GeomPreprocessor.CreateTrisAroundVertex((target as DataCreation).targetSurface.GetComponent<Mesh>());
        AssetDatabase.CreateAsset(so, "Assets/PreData/TestSO.asset");
        AssetDatabase.SaveAssets();
    }

    public void OnDisable() {
        _targetSurfaceBind.UnregisterValueChangedCallback(targetSurfaceChanged);
    }
}
