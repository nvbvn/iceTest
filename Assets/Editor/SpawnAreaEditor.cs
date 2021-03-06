﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

[CustomEditor(typeof(SpawnAreaCreator))]
public class SpawnAreaEditor : Editor
{
    private const string EDIT_BTN_NAME_OFF = "Start Redo";
    private const string EDIT_BTN_NAME_ON = "Stop Redo";

    private static bool _isRedoOn = false;
    private static Mesh _lastProcessedMesh = null;
    private static Material _nativeMaterial;
    private static Vector2[] _nativeUV;

    private static bool[] _selectedTriangles;

    private static Material s_redoMaterial = null;
    private static Material s_getRedoMaterial() {
        if (s_redoMaterial == null) {
            s_redoMaterial = Resources.Load<Material>("Materials/ForSpawnAreaCreator");
        }
        return s_redoMaterial;
    }

   /* public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawDefaultInspector();
        //EditorGUILayout.PropertyField(lookAtPoint);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("qq"));
        EditorGUILayout.HelpBox("This is a help box", MessageType.Error);
        serializedObject.ApplyModifiedProperties();
    }*/


    private VisualElement _root;
    private Button _editBtn;

    public override VisualElement CreateInspectorGUI() {
        Debug.LogError("CreateInspectorGUI");
        VisualElement customInspector = new VisualElement();
        _root = customInspector;

        _editBtn = new Button(clickListener);
        _editBtn.text = EDIT_BTN_NAME_OFF;
        customInspector.Add(_editBtn);
        /*   Foldout fo = new Foldout();
           customInspector.Add(fo);
           Label notice = new Label("SHIFT: add triangle\nCTRL: remove triangle");
           fo.Add(notice);*/
        //  TextField tf = new TextField()

        updateEditBtn();
        return customInspector;
    }
    
    private void updateEditBtn() { 
    if (_isRedoOn) {
            _editBtn.text = EDIT_BTN_NAME_ON;
        } else {
            _editBtn.text = EDIT_BTN_NAME_OFF;
        }
    }

    private void clickListener() {
        _isRedoOn = !_isRedoOn;
        Debug.LogError(_isRedoOn);
        updateEditBtn();
        if (_isRedoOn) {
            setRedoState();
        } else {
            setNativeState();
        }
    }

    

    private void setRedoState() {
        _lastProcessedMesh = _mesh;
        _nativeMaterial = _renderer.sharedMaterial;
        _nativeUV = _mesh.uv;
        _selectedTriangles = new bool[_mesh.triangles.Length/3];
        _uv = new Vector2[_mesh.vertices.Length];
        int l = _uv.Length;
        for (int i=0; i<l; i++) {
            _uv[i] = new Vector2(0.99f, 0.99f);
        }
        if (_icecream.spawnTriangles != null) {
            foreach (int n in _icecream.spawnTriangles) {
                fillAsSelected(n);
            }
        }
        _renderer.material = s_getRedoMaterial();
        _mesh.uv = _uv;
        Debug.LogError("+++++++++++++++++++++");
    }

    private void setNativeState() {
        List<int> selTris = new List<int>();
        for (int i=0; i<_selectedTriangles.Length; i++) {
            if (_selectedTriangles[i]) {
                selTris.Add(i);
            }
        }
        _icecream.spawnTriangles = selTris.Count > 0 ? selTris.ToArray() : null;
        _renderer.material = _nativeMaterial;
        _mesh.uv = _nativeUV;

        _nativeMaterial = null;
        _nativeUV = null;
        _lastProcessedMesh = null;

        serializedObject.ApplyModifiedProperties();
        Debug.LogError("========================");
    }

    private Vector2[] _uv;

    private Icecream _icecream;
    private Transform _transform;
    private GeomProcessor _geomProcessor;
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private Renderer _renderer;
    private MeshCollider _meshCollider;

    private void OnEnable() {
        Debug.LogError("OnEnable");

        SpawnAreaCreator spawnArea = target as SpawnAreaCreator;
        _icecream = spawnArea.GetComponent<Icecream>();
        _transform = spawnArea.GetComponent<Transform>();
        _mesh = spawnArea.GetComponent<MeshFilter>().sharedMesh;
        _meshFilter = spawnArea.GetComponent<MeshFilter>();
        _meshCollider = spawnArea.GetComponent<MeshCollider>();
        _renderer = spawnArea.GetComponent<Renderer>();
        _geomProcessor = new GeomProcessor(_mesh, _icecream.trilinks, _icecream.trisAroundVertex, _icecream.gameObject.transform);
    }

    private void OnDisable() {
        Debug.LogError("OnDisable");
    }

    private void Awake() {
        Debug.LogError("Awake "+_isRedoOn);
    }

    private void OnSceneGUI() {
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
            Handles.DrawWireCube(hit.point, new Vector3(0.05f, 0.05f, 0.05f));
            List<Vector3> points = _geomProcessor.GetEdgeIntersectPoints(_transform.InverseTransformPoint(hit.point), hit.triangleIndex);
            // points[0].
            int l = points.Count;
            Vector3[] vp = new Vector3[l];
            for (int i=0; i<l; i++) {
                vp[i] = _transform.TransformPoint(points[i]);
            }
            Handles.DrawAAPolyLine(vp);
        }
    }

    private void fillingTriangle(int triangleIndex) { 
        if (_isRedoOn) {
            if (Event.current.shift) {
                fillAsSelected(triangleIndex);
            } else if (Event.current.control) {
                fillAsNonselected(triangleIndex);
            }
            
        }
    }

    private void fillAsSelected(int triangleIndex) {
        if (!_selectedTriangles[triangleIndex]) {
            //  Debug.LogError(triangleIndex);
            _uv = _mesh.uv;
            _uv[_mesh.triangles[3 * triangleIndex]] = _uv[_mesh.triangles[3 * triangleIndex + 1]] = _uv[_mesh.triangles[3 * triangleIndex + 2]] = new Vector2(0.749f, 0.749f);
            _mesh.uv = _uv;
            _selectedTriangles[triangleIndex] = true;
        }
    }

    private void fillAsNonselected(int triangleIndex) {
        if (_selectedTriangles[triangleIndex]) {
            //    Debug.LogError("-"+triangleIndex);
            _uv = _mesh.uv;
            _uv[_mesh.triangles[3 * triangleIndex]] = _uv[_mesh.triangles[3 * triangleIndex + 1]] = _uv[_mesh.triangles[3 * triangleIndex + 2]] = new Vector2(0.99f, 0.99f);
            _mesh.uv = _uv;
            _selectedTriangles[triangleIndex] = false;
        }
    }

/*    [DrawGizmo(GizmoType.Selected)]
    static void DrawGizmosSelected(SpawnAreaCreator spawnArea, GizmoType gizmoType) {
        Gizmos.color = Color.red;
        RaycastHit hit;
        Vector2 rayPoint = new Vector2(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y);
        if (!Physics.Raycast(SceneView.currentDrawingSceneView.camera.ScreenPointToRay(rayPoint), out hit)) {
            return;
        }
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        if (meshCollider == _meshCollider) {
            Gizmos.DrawSphere(hit.point, 0.025f);
            List<Vector3> points = _geomProcessor.GetEdgeIntersectPoints(_transform.InverseTransformPoint(hit.point), hit.triangleIndex);
            // points[0].
            Vector3 t;
            int l = points.Count;
            for (int i=1; i<l; i++) {
                Gizmos.DrawCube(t = _transform.TransformPoint(points[i]), new Vector3(0.02f, 0.02f, 0.02f));
                Gizmos.DrawLine(_transform.TransformPoint(points[i-1]), t);
            }
            
        }
    }*/

}
