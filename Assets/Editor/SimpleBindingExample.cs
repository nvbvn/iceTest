using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UIElementsExamples
{
   public class SimpleBindingExample : EditorWindow
   {
       TextField m_ObjectNameBinding;
      
       [MenuItem("Window/UIElementsExamples/Simple Binding Example")]
       public static void ShowDefaultWindow()
       {
           var wnd = GetWindow<SimpleBindingExample>();
           wnd.titleContent = new GUIContent("Simple Binding");
       }
    
       public void OnEnable()
       {
           var root = this.rootVisualElement;

            Toolbar t = new Toolbar();
            root.Add(t);

            ToolbarToggle tg = new ToolbarToggle() { text = "tg1" };
            t.Add(tg);
            tg = new ToolbarToggle() { text = "tg2" };
            t.Add(tg);
            tg.RegisterValueChangedCallback(toggleChanged);
            tg.RegisterCallback<ChangeEvent<bool>>(toggleListener);

            m_ObjectNameBinding = new TextField("Width");
            m_ObjectNameBinding.bindingPath = "m_SizeDelta.x";
            root.Add(m_ObjectNameBinding);
            m_ObjectNameBinding = new TextField("Object Name Binding");
           m_ObjectNameBinding.bindingPath = "m_Name";
           root.Add(m_ObjectNameBinding);
            OnSelectionChange();
       }

        private void toggleListener(ChangeEvent<bool> e)
        {
            Debug.LogError("?");
        }

        private void toggleChanged(ChangeEvent<bool> e)
        {
            Debug.LogError("???");
        }

        public void OnSelectionChange()
       {
            object o = Selection.activeObject;
            //UnityEngine.UI.Button selectedObject = Selection.activeObject as UnityEngine.UI.Button;
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
           {
               // Create serialization object
               SerializedObject so = new SerializedObject(selectedObject);
               // Bind it to the root of the hierarchy. It will find the right object to bind to...
               rootVisualElement.Bind(so);
    
               // ... or alternatively you can also bind it to the TextField itself.
               // m_ObjectNameBinding.Bind(so);
           }
           else
           {
               // Unbind the object from the actual visual element
               rootVisualElement.Unbind();
              
               // m_ObjectNameBinding.Unbind();
              
               // Clear the TextField after the binding is removed
               m_ObjectNameBinding.value = "";
           }
       }
   }
}