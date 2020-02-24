using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(SpawnCreation))]
public class SpawnCreationEditor : Editor
{
    private const int ITEM_HEIGHT = 16;

    private ObjectField _targetSurfaceBind;
    private ObjectField _preDataBind;
    private ObjectField _spawnSObind;

    private Box _spawnBox;
    private Button _addSpawZoneBtn;
    private Button _removeSpawZoneBtn;
    private ListView _spawnZoneList;
    private TextField _spawnSetNameTf;
    private Button _saveSpawSO;

    private GameObject _targetObject;
    private PreDataSO _preData;
    private SpawnSO _spawnSO;

    private bool _isSpwnEditAvailable = false;
    private Vector3[] _nativeVertices = null;
    private int[] _nativeTriangles = null;

    private static Material s_redoMaterial = null;
    private static Material s_getRedoMaterial() {
        if (s_redoMaterial == null) {
            s_redoMaterial = Resources.Load<Material>("Materials/ForSpawnAreaCreator");
        }
        return s_redoMaterial;
    }


    private void OnValidate()
    {
        Debug.LogError("OnValidate");
    }

    private void OnEnable() {
        Debug.LogError("OnEnable");
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement customInspector = new VisualElement();

        _targetSurfaceBind = new ObjectField("Target Surface");
        _targetSurfaceBind.RegisterValueChangedCallback(targetSurfaceChanged);
        _targetSurfaceBind.objectType = typeof(GameObject);
        _targetSurfaceBind.bindingPath = "targetSurface";
        customInspector.Add(_targetSurfaceBind);

        _preDataBind = new ObjectField("PreData");
        _preDataBind.RegisterValueChangedCallback(preDataChanged);
        _preDataBind.objectType = typeof(PreDataSO);
        _preDataBind.bindingPath = "preData";
        customInspector.Add(_preDataBind);

        _spawnSObind = new ObjectField("Spawn Data");
        _spawnSObind.RegisterValueChangedCallback(spawnDataChanged);
        _spawnSObind.objectType = typeof(SpawnSO);
        _spawnSObind.bindingPath = "spawnData";
        customInspector.Add(_spawnSObind);



        _spawnBox = new Box();
        customInspector.Add(_spawnBox);

        List<string> items = new List<string>();
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];
        const int itemHeight = ITEM_HEIGHT;
        _spawnZoneList = new ListView(items, itemHeight, makeItem, bindItem);
        _spawnZoneList.selectionType = SelectionType.Single;
        _spawnZoneList.onSelectionChanged += onItemChosenHandler;
        _spawnBox.Add(_spawnZoneList);

        Box arBtnBox = new Box();
        _spawnBox.Add(arBtnBox);
        arBtnBox.style.flexDirection = FlexDirection.Row;
        arBtnBox.style.alignContent = Align.Center;

        _addSpawZoneBtn = new Button(addSpawnZoneClickListener);
        _addSpawZoneBtn.text = "Add Spawn Zone";
        arBtnBox.Add(_addSpawZoneBtn);
        _removeSpawZoneBtn = new Button(removeSpawnZoneClickListener);
        _removeSpawZoneBtn.text = "Remove Spawn Zone";
        arBtnBox.Add(_removeSpawZoneBtn);

        _spawnSetNameTf = new TextField("Spawn Set Name");
        _spawnBox.Add(_spawnSetNameTf);
        Button saveBtn = new Button(removeSpawnZoneClickListener);
        saveBtn.text = "Save Spawns";
        _spawnBox.Add(saveBtn);

        SpawnCreation sc = target as SpawnCreation;
        _targetObject = sc.targetSurface;
        _preData = sc.preData;
        _spawnSO = sc.spawnData;
        checkSRCdata();

        return customInspector;
    }


    private void targetSurfaceChanged(ChangeEvent<UnityEngine.Object> e) {
        _targetObject = e.newValue as GameObject;
        _preDataBind.SetValueWithoutNotify(_preData = null);
        _spawnSObind.value = null;
        checkSRCdata();
    }

    private void preDataChanged(ChangeEvent<UnityEngine.Object> e) {
        _preData = e.newValue as PreDataSO;
        checkSRCdata();
    }

    private void spawnDataChanged(ChangeEvent<UnityEngine.Object> e) {
        //тут надо бы проверить на предмет сохранения текущих наработок
        _spawnSO = e.newValue as SpawnSO;
        checkSRCdata();
    }

    private void checkSRCdata() {
        //_targetObject = _targetSurfaceBind.value as GameObject;
        //_preData = _preDataBind.value as PreDataSO;
        //_spawnSO = _spawnSObind.value as SpawnSO;

        bool res = true;
        MeshFilter mf = _targetObject?.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) {
            res = false;
        }
        if (res && (_preData == null || mf.sharedMesh.triangles.Length != _preData.trilinks.Length || mf.sharedMesh.vertexCount != _preData.trisAroundVertex.Length)) {
            res = false;
        }
        if (res && _spawnSO == null) {
            res = false;
        }
        prepareSpawnBox(res);
    }

    private void prepareSpawnBox(bool isAVailable) {
        _isSpwnEditAvailable = isAVailable;
        refreshZoneList(isAVailable ? _spawnSO.spawnTris.Length : 1);
        _spawnSetNameTf.value = isAVailable ? _spawnSO.name : string.Empty;
        _spawnBox.SetEnabled(isAVailable);
        if (isAVailable) {
            _spawnZoneList.selectedIndex = 0;
            prepareEdit();
        }
    }

    private void onItemChosenHandler(object obj) {
        Debug.LogError("onItemChosenHandler");
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






    private Transform _transform;
    
    
//    private MeshFilter _meshFilter;
  //  private Renderer _renderer;

    private Mesh _mesh;
    private static bool[] _selectedTriangles;
    private GeomProcessor _geomProcessor;
    private MeshCollider _meshCollider;
    private void prepareEdit() {

        _meshCollider = _targetObject.GetComponent<MeshCollider>();
        _mesh = _targetObject.GetComponent<MeshFilter>().sharedMesh;
        _geomProcessor = new GeomProcessor(_mesh, _preData.trilinks, _preData.GetTav(), _targetObject.transform);
        _transform = _targetObject.GetComponent<Transform>();
        Renderer r = _targetObject.GetComponent<Renderer>();

        _selectedTriangles = new bool[_mesh.triangles.Length/3];
        //breakUpTriangles(_mesh);
        Vector2[] uv = new Vector2[_mesh.vertices.Length];
        int l = uv.Length;
        for (int i=0; i<l; i++) {
            uv[i] = new Vector2(0.9f, 0.9f);
        }
        _targetObject.GetComponent<Renderer>().material = s_getRedoMaterial();
          _mesh.uv = uv;
    }

    private void breakUpTriangles(Mesh mesh) {
        int l = mesh.triangles.Length;
        Vector3[] newVerts = new Vector3[l];
        _nativeVertices = mesh.vertices;
        _nativeTriangles = mesh.triangles;
        int[] newTris = mesh.triangles;
        for (int i=0; i<l; i++) {
            newVerts[i] = _nativeVertices[newTris[i]];
            newTris[i] = i;
        }
        mesh.vertices = newVerts;
        mesh.triangles = newTris;
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

    private void fillingTriangle(int triangleIndex) { 
//        if (_isRedoOn) {
            if (Event.current.shift) {
                fillAsSelected(triangleIndex);
            } else if (Event.current.control) {
                fillAsNonselected(triangleIndex);
            }
            
  //      }
    }

    private void fillAsSelected(int triangleIndex) {
        if (!_selectedTriangles[triangleIndex]) {
            //  Debug.LogError(triangleIndex);
            Vector2[] uv = _mesh.uv;
            uv[_mesh.triangles[3 * triangleIndex]] = uv[_mesh.triangles[3 * triangleIndex + 1]] = uv[_mesh.triangles[3 * triangleIndex + 2]] = new Vector2(0.749f, 0.749f);
            _mesh.uv = uv;
            _selectedTriangles[triangleIndex] = true;
        }
    }

    private void fillAsNonselected(int triangleIndex) {
        if (_selectedTriangles[triangleIndex]) {
            //    Debug.LogError("-"+triangleIndex);
            Vector2[] uv = _mesh.uv;
            uv[_mesh.triangles[3 * triangleIndex]] = uv[_mesh.triangles[3 * triangleIndex + 1]] = uv[_mesh.triangles[3 * triangleIndex + 2]] = new Vector2(0.99f, 0.99f);
            _mesh.uv = uv;
            _selectedTriangles[triangleIndex] = false;
        }
    }


    private void OnDisable() {
        if (_isSpwnEditAvailable) {
            _targetObject.GetComponent<MeshFilter>().sharedMesh.triangles = _nativeTriangles;
            _targetObject.GetComponent<MeshFilter>().sharedMesh.vertices = _nativeVertices;
        }
    }

}
