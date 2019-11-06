using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

[CustomEditor(typeof(SpawnArea))]
public class SpawnAreaEditor : Editor
{
    public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawDefaultInspector();
        //EditorGUILayout.PropertyField(lookAtPoint);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("qq"));
        EditorGUILayout.HelpBox("This is a help box", MessageType.Error);
        serializedObject.ApplyModifiedProperties();
    }


    private VisualElement _root;
    private bool b = false;
    public override VisualElement CreateInspectorGUI() {
        VisualElement customInspector = new VisualElement();
        _root = customInspector;

        Button btn = new Button();
        btn.text = "Start Redo";
        customInspector.Add(btn);

        return customInspector;
    }
    

    private void OnSceneGUI() {
        SpawnArea spawnArea = target as SpawnArea;
        RaycastHit hit;
        // Debug.LogError(Event.current.mousePosition.x+", "+ Event.current.mousePosition.y);
        Vector2 rayPoint = new Vector2(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y);
        if (!Physics.Raycast(SceneView.currentDrawingSceneView.camera.ScreenPointToRay(rayPoint), out hit))
        {
            return;
            Debug.LogError("?");
        }
        if (hit.triangleIndex == 140 && !b) {
            b = true;
            Button btn = new Button();
            btn.text = "QQQ";
            _root.Add(btn);
        }
        //Debug.LogError(hit.triangleIndex);
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Transform targetObject;
        GeomProcessor targetProcessor;
        if (meshCollider == spawnArea.targetMesh.GetComponent<MeshCollider>())
        {
            targetObject = spawnArea.targetMesh.transform;
            Handles.color = Color.red;
            Handles.DrawWireCube(hit.point, new Vector3(0.01f, 0.01f, 0.01f));
            spawnArea.ice.GetBlobPath(spawnArea.gameObject.transform.InverseTransformPoint(hit.point), hit.triangleIndex);
            //Handles.PositionHandle(hit.point, Quaternion.identity);
            //Gizmos.DrawSphere(hit.point, 0.05f);
 //           Debug.LogError(hit.point.x + ", " + hit.point.y + ", " + hit.point.z);
        }
    }

/*    [DrawGizmo(GizmoType.Selected)]
    static void DrawGizmosSelected(SpawnArea spawnArea, GizmoType gizmoType) {
        Gizmos.color = Color.red;
        RaycastHit hit;
        Vector2 rayPoint = new Vector2(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight-Event.current.mousePosition.y);
        if (!Physics.Raycast(SceneView.currentDrawingSceneView.camera.ScreenPointToRay(rayPoint), out hit))
        {
            return;
            Debug.LogError("?");
        }

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Transform targetObject;
        GeomProcessor targetProcessor;
        if (meshCollider == spawnArea.targetMesh.GetComponent<MeshCollider>()) {
            targetObject = spawnArea.targetMesh.transform;
            Gizmos.DrawSphere(hit.point, 0.05f);
            Debug.LogError(hit.point.x+", "+hit.point.y+", "+hit.point.z);
        }
    }*/

}
