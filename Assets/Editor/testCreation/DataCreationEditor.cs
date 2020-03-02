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
    private static string START_SPAWN_EDIT = "Start Spawn Edit";
    private static string STOP_SPAWN_EDIT = "Stop Spawn Edit";

    private const int ITEM_HEIGHT = 16;

    private ObjectField _targetSurfaceBind;
    private Button _createPreDataBtn;




    private static Material s_redoMaterial = null;
    private static Material s_getRedoMaterial() {
        if (s_redoMaterial == null) {
            s_redoMaterial = Resources.Load<Material>("Materials/VertexColorMaterial");
        }
        return s_redoMaterial;
    }

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
        //setSpawnSO(null);
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
        EditorUtil.PrepareFolder(assetFolderPath);
        AssetDatabase.CreateAsset(so, "Assets/"+ assetFolderPath+"/"+ meshFilter.transform.root.name+"_preData.asset");
       // AssetDatabase.SaveAssets();
    }


    /*private void OnSceneGUI() {
        if ((target as DataCreation).targetSurface != null && ) {
            RaycastHit hit;
            Vector2 rayPoint = new Vector2(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y);
            if (!Physics.Raycast(SceneView.currentDrawingSceneView.camera.ScreenPointToRay(rayPoint), out hit)) {
                return;
            }
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;

            if (meshCollider == _meshCollider) {
                fillingTriangle(hit.triangleIndex);
                //_mesh.triangles[hit.triangleIndex]
                Handles.color = Color.red;
                Handles.DrawWireCube(hit.point, new Vector3(0.01f, 0.01f, 0.01f));
                List<Vector3> points = _geomProcessor.GetEdgeIntersectPoints(_transform.InverseTransformPoint(hit.point), hit.triangleIndex);
                // points[0].
                int l = points.Count;
                Vector3[] vp = new Vector3[l];
                for (int i = 0; i < l; i++) {
                    vp[i] = _transform.TransformPoint(points[i]);
                }
                Handles.DrawAAPolyLine(vp);
            }
        }
    }*/

    public void OnDisable() {
        _targetSurfaceBind.UnregisterValueChangedCallback(targetSurfaceChanged);
    }
}
