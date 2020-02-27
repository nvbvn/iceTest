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
    private Material _nativeMaterial = null;
    private List<List<int>> _spawns;
    private int _currentEditSpawnZone = -1;

    private static Material s_redoMaterial = null;
    private static Material s_getRedoMaterial() {
        if (s_redoMaterial == null) {
            s_redoMaterial = Resources.Load<Material>("Materials/ForSpawnAreaCreator");
        }
        return s_redoMaterial;
    }


    private void OnValidate()
    {
      //  Debug.LogError("OnValidate");
    }

    private void OnEnable() {
       // Debug.LogError("OnEnable");
    }

    public override VisualElement CreateInspectorGUI() {
        _currentEditSpawnZone = -1;

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
        _spawnZoneList.onSelectionChange += onItemChosenHandler;
        //_spawnZoneList.onSelectionChanged += objects => Debug.Log(objects);
        _spawnBox.Add(_spawnZoneList);

        Box arBtnBox = new Box();
        _spawnBox.Add(arBtnBox);
        arBtnBox.style.flexDirection = FlexDirection.Row;
        arBtnBox.style.alignContent = Align.Center;

        _addSpawZoneBtn = new Button(addSpawnZoneClickListener);
        _addSpawZoneBtn.text = "Add Spawn Zone";
        arBtnBox.Add(_addSpawZoneBtn);
        _removeSpawZoneBtn = new Button(removeSpawnZoneClickListener);
        _removeSpawZoneBtn.text = "Remove Current Spawn Zone";
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
        _currentEditSpawnZone = -1;

        bool res = true;
        MeshFilter mf = null;
        if (_targetObject == null) {
            res = false;
        } else {
            mf = _targetObject?.GetComponent<MeshFilter>();
        }
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
            _spawns = new List<List<int>>();
            for (int i=0; i<_spawnSO.spawnTris.Length; i++) {
                _spawns.Add(_spawnSO.spawnTris[i].Length>0? new List<int>(Array.ConvertAll(_spawnSO.spawnTris[i].Split(','), int.Parse)) : new List<int>());
            }
            prepareEdit();
            _spawnZoneList.selectedIndex = 0;
        }
    }

    private void onItemChosenHandler(IEnumerable<object> items) {
        setSpawnZoneByIndex(_spawnZoneList.selectedIndex);
      //  Debug.LogError("v: "+_spawnZoneList.selectedIndex);
    }

    private void addSpawnZoneClickListener() {
        refreshZoneList(_spawnZoneList.itemsSource.Count + 1);
        _spawns.Add(new List<int>());
    }

    private void removeSpawnZoneClickListener() {
        if (_spawns.Count>1) {
            refreshZoneList(_spawnZoneList.itemsSource.Count - 1);
            _spawns.RemoveAt(_currentEditSpawnZone);
          //  Debug.LogError("-: "+_spawnZoneList.selectedIndex);
            setSpawnZoneByIndex(Math.Max(_spawns.Count-1, _spawnZoneList.selectedIndex), false);
         //   _spawnZoneList.selectedIndex = _currentEditSpawnZone;
        }
      //  
    }

    private void refreshZoneList(int n) {
        _spawnZoneList.itemsSource.Clear();
        int i;
        for (i=0; i<n; i++) {
            _spawnZoneList.itemsSource.Add((i+1).ToString());
        }
        _spawnZoneList.style.height = i * ITEM_HEIGHT;
        _spawnZoneList.Refresh();
    }

    private void setSpawnZoneByIndex(int index, bool isSaveCurrentSelection = true) {
//        Debug.LogError("setSpawnZoneByIndex");
        if (isSaveCurrentSelection && _currentEditSpawnZone != -1) {
            saveCurrentSelection();
        }
        _currentEditSpawnZone = index;
        int l = _mesh.triangles.Length / 3;
        _selectedTriangles = new bool[l];
        _colors = new Color[l];
        int i;
        for (i=0; i<l; i++) {
            _colors[i] = new Color(1, 1, 1, 1);
        }
       // if (_currentEditSpawnZone != -1) {
            l = _spawns[index].Count;
            for (i = 0; i < l; i++) {
                setTriangleColor(_spawns[index][i], 0.5f);
                _selectedTriangles[_spawns[index][i]] = true;
            }
        //}
        _matForEdit.SetColorArray("_Colors", _colors);
    }

    private void setTriangleColor(int triangleI, float color) { 
        int n = triangleI % 4;
        switch (n) {
            case 0:
                _colors[triangleI / 4].r = color;
                break;
            case 1:
                _colors[triangleI / 4].g = color;
                break;
            case 2:
                _colors[triangleI / 4].b = color;
                break;
            case 3:
                _colors[triangleI / 4].a = color;
                break;
        }
    }

    private void saveCurrentSelection() {
        _spawns[_currentEditSpawnZone].Clear();
        int l = _selectedTriangles.Length;
        for (int i=0; i<l; i++) {
            if (_selectedTriangles[i]) {
                _spawns[_currentEditSpawnZone].Add(i); ;
            }
        }
    }






    private Transform _transform;
    
    
//    private MeshFilter _meshFilter;
  //  private Renderer _renderer;

    private Mesh _mesh;
    private static bool[] _selectedTriangles;
    private Color[] _colors;
    private Material _matForEdit;
    private GeomProcessor _geomProcessor;
    private MeshCollider _meshCollider;
    private void prepareEdit() {

        _meshCollider = _targetObject.GetComponent<MeshCollider>();
        _mesh = _targetObject.GetComponent<MeshFilter>().sharedMesh;
        _geomProcessor = new GeomProcessor(_mesh, _preData.trilinks, _preData.GetTav(), _targetObject.transform);
        _transform = _targetObject.GetComponent<Transform>();
        Renderer renderer = _targetObject.GetComponent<Renderer>();
        _nativeMaterial = renderer.sharedMaterial;
        renderer.sharedMaterial = _matForEdit = s_getRedoMaterial();
    }

    private void clearSelection() { 
    
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
            setTriangleColor(triangleIndex, 0.25f);
            _matForEdit.SetColorArray("_Colors", _colors);
            _selectedTriangles[triangleIndex] = true;
        }
    }

    private void fillAsNonselected(int triangleIndex) {
        if (_selectedTriangles[triangleIndex]) {
            setTriangleColor(triangleIndex, 1);
            _matForEdit.SetColorArray("_Colors", _colors);
            _selectedTriangles[triangleIndex] = false;
        }
    }


    private void OnDisable() {
        if (_isSpwnEditAvailable) {
            _targetObject.GetComponent<Renderer>().sharedMaterial = _nativeMaterial;
        }
    }

}
