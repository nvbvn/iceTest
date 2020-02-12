using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TankScript))]
public class TankEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var visualTree = Resources.Load("binding/tank_inspector_uxml") as VisualTreeAsset;
        var uxmlVE = visualTree.CloneTree();
        uxmlVE.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/binding/tank_inspector_styles.uss"));
        return uxmlVE;
    }
}