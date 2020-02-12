using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;

namespace UIElementsExamples
{
    public class SimpleBindingExampleUXML : EditorWindow
    {
        [MenuItem("Window/UIElementsExamples/Simple Binding Example UXML")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<SimpleBindingExampleUXML>();
            wnd.titleContent = new GUIContent("Simple Binding UXML");
        }

        TankScript m_Tank;
        public void OnEnable()
        {
            m_Tank = GameObject.FindObjectOfType<TankScript>();
            if (m_Tank == null)
                return;

            var inspector = new InspectorElement(m_Tank);
            rootVisualElement.Add(inspector);
        }
    }
}