﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

[CustomEditor(typeof(SpawnAreaCreator))]
public class SpawnAreaEditor : Editor
{
   /* public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawDefaultInspector();
        //EditorGUILayout.PropertyField(lookAtPoint);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("qq"));
        EditorGUILayout.HelpBox("This is a help box", MessageType.Error);
        serializedObject.ApplyModifiedProperties();
    }*/


    private VisualElement _root;
    public override VisualElement CreateInspectorGUI() {
        VisualElement customInspector = new VisualElement();
        _root = customInspector;

        Button btn = new Button();
        btn.text = "Start Redo";
        customInspector.Add(btn);

        return customInspector;
    }


    private static Icecream _icecream;
    private static Transform _transform;
    private static GeomProcessor _geomProcessor;
    private static Mesh _mesh;
    private static MeshFilter _meshFilter;
    private static Renderer _renderer;
    private static MeshCollider _meshCollider;
    private void OnEnable() {
        SpawnAreaCreator spawnArea = target as SpawnAreaCreator;
        _icecream = spawnArea.GetComponent<Icecream>();
        _transform = spawnArea.GetComponent<Transform>();
        _mesh = spawnArea.GetComponent<MeshFilter>().mesh;
        _meshFilter = spawnArea.GetComponent<MeshFilter>();
        _meshCollider = spawnArea.GetComponent<MeshCollider>();
        _renderer = spawnArea.GetComponent<Renderer>();
        _geomProcessor = new GeomProcessor(_mesh, _icecream.trilinks, _icecream.trisAroundVertex, _icecream.gameObject.transform);

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
            Handles.color = Color.red;
            Handles.DrawWireCube(hit.point, new Vector3(0.01f, 0.01f, 0.01f));
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