using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIElementsExamples
{
    public class ListViewExampleWindow : EditorWindow
    {
        [MenuItem("Window/ListViewExampleWindow")]
        public static void OpenDemoManual()
        {
            GetWindow<ListViewExampleWindow>().Show();
        }


        private ListView listView;
        private List<string> items;

        public void OnEnable()
        {
            // Create some list of data, here simply numbers in interval [1, 1000]
            const int itemCount = 13;
            items = new List<string>(itemCount);
            for (int i = 1; i <= itemCount; i++)
                items.Add(i.ToString());

            // The "makeItem" function will be called as needed
            // when the ListView needs more items to render
            Func<VisualElement> makeItem = () => new Label();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 16;

            //listView = new ListView(items, itemHeight, makeItem, bindItem);
            listView = new ListView();
            listView.itemsSource = items;
            listView.itemHeight = 16;
            listView.makeItem = makeItem;
            listView.bindItem = bindItem;

          /*  items.Add("qq");
            listView.itemsSource = items;
            listView.Refresh();
            items.Add("qqq");
            listView.itemsSource = items;
            listView.Refresh();
            */
            listView.selectionType = SelectionType.Multiple;

            listView.onItemChosen +=  obj => Debug.Log(obj);//onItemChosenHandler;//
            listView.onSelectionChanged += onItemChosenHandler;//objects => Debug.Log("???"+objects);

            listView.style.flexGrow = 1.0f;

            rootVisualElement.Add(listView);
        }

        /*Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        private VisualElement makeItem() {
            return new Label();
        }*/

        private void onItemChosenHandler(object obj) {
            Debug.LogError("QQWE");
            items.Add("qq");
            listView.itemsSource = items;
            listView.Refresh();
        }
    }
}