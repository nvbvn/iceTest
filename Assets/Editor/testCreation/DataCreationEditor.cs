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
    private static string START_SPAWN_EDIT = "Start Spawan Edit";
    private static string STOP_SPAWN_EDIT = "Stop Spawan Edit";

    private ObjectField _targetSurfaceBind;
    private Button _createPreDataBtn;

    private Box _spawnEditBox;
    private Button _spawEditBtn;
    private Button _addSpawZoneBtn;

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



        _spawnEditBox = new Box();
        customInspector.Add(_spawnEditBox);

        _spawEditBtn = new Button(createPreDataClickListener);
        _spawEditBtn.text = START_SPAWN_EDIT;
        _spawnEditBox.Add(_spawEditBtn);

        _addSpawZoneBtn = new Button(createPreDataClickListener);
        _addSpawZoneBtn.text = "Add Spawn Zone";
        _spawnEditBox.Add(_addSpawZoneBtn);

        Toolbar tb = new Toolbar();
        _spawnEditBox.Add(tb);
        //tb.
        ToolbarToggle t = new ToolbarToggle();
        t.RegisterCallback<ChangeEvent<bool>>(toggleListener);

        t.label = "1";
        t.tabIndex = 0;
        tb.Add(t);
        t = new ToolbarToggle();
        t.label = "2";
        t.tabIndex = 1;
        tb.Add(t);
        t = new ToolbarToggle();
        t.label = "3";
        t.tabIndex = 2;
        tb.Add(t);

        TextField tf = new TextField("Spawn Set Name");
        _spawnEditBox.Add(tf);

        

        checkMeshAvailabilityInTargetSurface(_targetSurfaceBind.value as GameObject);

        return customInspector;
    }
    private void toggleListener(ChangeEvent<bool> e) { 
    
    
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
    }


    private void createPreDataClickListener() {
        PreDataSO so = ScriptableObject.CreateInstance<PreDataSO>();
        MeshFilter meshFilter = (target as DataCreation).targetSurface.GetComponent<MeshFilter>();
        so.trisAroundVertex = GeomPreprocessor.CreateTrisAroundVertex(meshFilter.sharedMesh);
        so.trilinks = GeomPreprocessor.CreateTrilinks(meshFilter.sharedMesh);
        string assetFolderPath = "Resources/PreData";
        prepareFolder(assetFolderPath);
        AssetDatabase.CreateAsset(so, "Assets/"+ assetFolderPath+"/"+ meshFilter.transform.root.name+"_preData.asset");
       // AssetDatabase.SaveAssets();
    }

    private void prepareFolder(string folderPath) {
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

    public void OnDisable() {
        _targetSurfaceBind.UnregisterValueChangedCallback(targetSurfaceChanged);
    }
}
