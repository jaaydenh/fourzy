using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TypeReferences;
using SA.Foundation.Editor;
using Rotorz.ReorderableList;


namespace SA.Foundation.Tests
{

    [CustomEditor(typeof(SA_TestSuiteConfig))]
    public class SA_TestSuiteConfigInspector : UnityEditor.Editor {

        private int GroupIndex = 0;
        private int ListIndex = 0;

        private SerializedObject TestSuiteConfigSerialized = null;

        private void OnEnable() {
            if(target != null) {
                TestSuiteConfigSerialized = new SerializedObject(target);
            }
            
        }

        public override void OnInspectorGUI() {

            if (TestSuiteConfigSerialized == null) { return; }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

           
            SA_EditorGUILayout.ReorderablList(Config.TestGroups, GroupTitle, GroupContent, OnNewGroup, null, OnGorupStartUI);


            EditorGUILayout.Space();
            Config.PauseOnError = SA_EditorGUILayout.ToggleFiled("Error Pause", Config.PauseOnError, SA_StyledToggle.ToggleType.YesNo);
            Config.SkipInteractableTests = SA_EditorGUILayout.ToggleFiled("Skip Interactable", Config.SkipInteractableTests, SA_StyledToggle.ToggleType.YesNo);


            if (EditorGUI.EndChangeCheck()) {
                TestSuiteConfigSerialized.ApplyModifiedProperties();
                TestSuiteConfigSerialized = new SerializedObject(target);
            }


            EditorGUILayout.Space();

        }

        private string GroupTitle(SA_TestGroupConfig group) {
            return group.Name;
        }

        private void OnGorupStartUI(SA_TestGroupConfig group) {
           // GUILayout.Space(5);
            group.Enabled = EditorGUILayout.Toggle(group.Enabled, GUILayout.Width(16));
            GUILayout.Space(10);
        }

        private void GroupContent(SA_TestGroupConfig group) {

            using (new SA_GuiIndentLevel(- 1)) {
                group.Name = EditorGUILayout.TextField("Name:", group.Name);
                group.Texture = (Texture2D) EditorGUILayout.ObjectField("Group Icon:", group.Texture, typeof(Texture2D),  false, GUILayout.Height(16));

                GroupIndex = Config.TestGroups.IndexOf(group);
                ListIndex = 0;
                ReorderableListGUI.ListField(group.Tests, DrawTestListItem, DrawEmpty);
            }

        }

        private void OnNewGroup() {
            var group = new SA_TestGroupConfig();
            group.Name = "New Test Group";

            Config.TestGroups.Add(group);
        }

        private SA_TestConfig DrawTestListItem(Rect pos, SA_TestConfig itemValue) {

            /*
            if(itemValue == null) {
                return itemValue;
            }

            int toggleWidth = 60;

            var toggleRect = new Rect(pos);
            toggleRect.width = toggleWidth;
            itemValue.StopsNextTestsIfFail = EditorGUI.Toggle(toggleRect, "Stop", itemValue.StopsNextTestsIfFail);


            var classRect = new Rect(pos);
            classRect.width -= toggleWidth;
            classRect.x += toggleWidth;
            */

            string path = GetTestReferencePropertyPath(itemValue.TestReference);
            if(!string.IsNullOrEmpty(path)) {
                SerializedProperty p = TestSuiteConfigSerialized.FindProperty(path);
                if(p != null) {
                    EditorGUI.PropertyField(pos, p, new GUIContent(string.Empty));
                }
            }

            ListIndex++;
            return itemValue;
        }

        private string GetTestReferencePropertyPath(ClassTypeReference testReference) {

          
            return string.Format("TestGroups.Array.data[{0}].Tests.Array.data[{1}].TestReference", GroupIndex, ListIndex);
            /*

            TestSuiteConfig config = (TestSuiteConfig)target;
            for (int i = 0; i < config.TestGroups.Count; i++) {
                var groups = config.TestGroups[i];

                for (int j = 0; j < groups.Tests.Count; j++) {
                    var testRef = groups.Tests[j];
                    Debug.Log(testReference.GetHashCode() + " / " + testRef.GetHashCode());
                    if (testRef.Equals(testReference)) {
                       string path = string.Format("TestGroups.Array.data[{0}].Tests.Array.data[{1}].TestReference", i, j);
                       return path;
                    }
                }
            }

            return string.Empty;*/
        }


        private void DrawEmpty() {
            GUILayout.Label("Add a test", EditorStyles.miniLabel);
        }

        private SA_TestSuiteConfig Config {
            get {
                return (SA_TestSuiteConfig)target;
            }
        }


    }
}