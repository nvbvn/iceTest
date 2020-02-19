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


    private SpawnSO _spawnSO;
    private int _currentSpawnZoneI;
    private List<int> _currentSpawnZone;
    private Box _spawnEditBox;
    private Box _spawnEditContentBox;
    private ObjectField _spawnSOfield;
    private Button _spawEditBtn;
    private Button _addSpawZoneBtn;
    private Button _removeSpawZoneBtn;

    private ListView _spawnZoneList;
    private List<string> items;

    private TextField _spawnSetNameTf;


    private static Material s_redoMaterial = null;
    private static Material s_getRedoMaterial() {
        if (s_redoMaterial == null) {
            s_redoMaterial = Resources.Load<Material>("Materials/ForSpawnAreaCreator");
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



        _spawnEditBox = new Box();
        customInspector.Add(_spawnEditBox);
        

        _spawnSOfield = new ObjectField("Spawns source");
        _spawnSOfield.objectType = typeof(SpawnSO);
        _spawnSOfield.RegisterValueChangedCallback(spawnSrcChanged);
        _spawnEditBox.Add(_spawnSOfield);

        _spawnEditContentBox = new Box();
        _spawnEditBox.Add(_spawnEditContentBox);

        _spawEditBtn = new Button(createPreDataClickListener);
        _spawEditBtn.text = START_SPAWN_EDIT;
        _spawnEditContentBox.Add(_spawEditBtn);


        items = new List<string>();
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];
        const int itemHeight = ITEM_HEIGHT;
        _spawnZoneList = new ListView(items, itemHeight, makeItem, bindItem);
        _spawnZoneList.selectionType = SelectionType.Single;

        //_spawnZoneList.onItemChosen +=  obj => Debug.Log(obj);//onItemChosenHandler;//
        _spawnZoneList.onSelectionChanged += onItemChosenHandler;//objects => Debug.Log("???"+objects);
        _spawnEditContentBox.Add(_spawnZoneList);

        Box arBtnBox = new Box();
        _spawnEditContentBox.Add(arBtnBox);
        arBtnBox.style.flexDirection = FlexDirection.Row;
        arBtnBox.style.alignContent = Align.Center;

        _addSpawZoneBtn = new Button(addSpawnZoneClickListener);
        _addSpawZoneBtn.text = "Add Spawn Zone";
        arBtnBox.Add(_addSpawZoneBtn);
        _removeSpawZoneBtn = new Button(removeSpawnZoneClickListener);
        _removeSpawZoneBtn.text = "Remove Spawn Zone";
        arBtnBox.Add(_removeSpawZoneBtn);


        _spawnSetNameTf = new TextField("Spawn Set Name");
        _spawnEditContentBox.Add(_spawnSetNameTf);

        

        checkMeshAvailabilityInTargetSurface(_targetSurfaceBind.value as GameObject);

        return customInspector;
    }

    private void onItemChosenHandler(object obj) {
    }


    private void spawnSrcChanged(ChangeEvent<UnityEngine.Object> e) {
        setSpawnSO(e.newValue as SpawnSO);
    }

    private void setSpawnSO(SpawnSO spawnSO) {
        //Debug.LogError("setSpawnSO: "+spawnSO);
        _spawnSO = spawnSO;
        _spawnSetNameTf.value = _spawnSO==null? string.Empty : _spawnSO.name;
        refreshZoneList(_spawnSO==null? 0 : _spawnSO.spawnTris.Length);
        _spawnEditContentBox.SetEnabled(_spawnSO != null);
    }

    private void addSpawnZoneClickListener() {
        refreshZoneList(_spawnZoneList.itemsSource.Count + 1);
    }

    private void removeSpawnZoneClickListener() {
        refreshZoneList(_spawnZoneList.itemsSource.Count - 1);
    }

    private void refreshZoneList(int n) {
        n = Math.Max(1, n);
        _spawnZoneList.itemsSource.Clear();
        int i;
        for (i=0; i<n; i++) {
            _spawnZoneList.itemsSource.Add((i+1).ToString());
        }
        _spawnZoneList.style.height = i * ITEM_HEIGHT;
        _spawnZoneList.Refresh();
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
        _spawnEditBox.SetEnabled(res);
        _createPreDataBtn.text = res ? CREATE_PREDATA : MESH_UNAVAILABLE;

        _spawnSOfield.value = null;
        setSpawnSO(null);
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
