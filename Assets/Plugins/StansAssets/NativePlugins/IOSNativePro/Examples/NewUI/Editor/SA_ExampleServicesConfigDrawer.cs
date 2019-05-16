using System;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using SA.Foundation.Editor;
using UnityEditor;
using UnityEngine;

namespace SA.iOS.Examples {

    //[CustomPropertyDrawer(typeof(SA_ExampleServicesConfig))]
    //public class SA_ExampleServicesConfigDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        EditorGUI.BeginProperty(position, GUIContent.none, property);

    //        var subsections = property.FindPropertyRelative("Subsections");

    //        //GameObject test = property.serializedObject.targetObject as GameObject;
    //        //SA_ExampleSceneConfig te = test.GetComponent<SA_ExampleSceneConfig>();

    //        //SA_ExampleServicesConfig test = fieldInfo.GetValue(property.serializedObject.targetObject) as SA_ExampleServicesConfig;

    //        //Debug.Log(test.Name);
    //     //   SA_EditorGUILayout.ReorderablList<SA_ExampleSubsectionConfig>(test.Subsections,
    //     //   (SA_ExampleSubsectionConfig item) =>
    //     //   {
    //     //       //draw item name (Required)
    //     //       return item.Name;
    //     //   },
    //     //   (SA_ExampleSubsectionConfig item) =>
    //     //   {
    //     //       //draw item content (Optional)
    //     //       EditorGUILayout.LabelField("Item Body");
    //     //       //item = EditorGUILayout.TextField(item);
    //     //       EditorGUILayout.LabelField("Item Body");
    //     //   },
    //     //   () => {
    //     //       //draw item add button (Optional)
    //     //       test.Subsections.Add(new SA_ExampleSubsectionConfig());
    //     //   }
    //     //);




    //        EditorGUI.EndProperty();
    //    }
    //}
}

