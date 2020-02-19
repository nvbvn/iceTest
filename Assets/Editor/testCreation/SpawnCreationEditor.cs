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
        //_preData = e.newValue as PreDataSO;
        checkSRCdata();
    }

    private void spawnDataChanged(ChangeEvent<UnityEngine.Object> e) {
        //тут надо бы проверить на предмет сохранения текущих наработок
        //_spawnSO = e.newValue as SpawnSO;
        checkSRCdata();
    }

    private void checkSRCdata() {
        _targetObject = _targetSurfaceBind.value as GameObject;
        _preData = _preDataBind.value as PreDataSO;
        _spawnSO = _spawnSObind.value as SpawnSO;

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
        refreshZoneList(isAVailable ? _spawnSO.spawnTris.Length : 1);
        _spawnSetNameTf.value = isAVailable ? _spawnSO.name : string.Empty;
        _spawnBox.SetEnabled(isAVailable);
    }

    private void onItemChosenHandler(object obj) {
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

}
