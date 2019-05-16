using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_IndentLevelAttribute))]
    public class SA_PD_IndentLevelDrawer : PropertyDrawer
    {

        SA_PD_IndentLevelAttribute Attribute {
            get {
                return (SA_PD_IndentLevelAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.indentLevel = Attribute.IndentLevel;
            position = EditorGUI.IndentedRect(position);
            GUI.Label(position, property.stringValue + " L = " + EditorGUI.indentLevel);
        }
    }
}