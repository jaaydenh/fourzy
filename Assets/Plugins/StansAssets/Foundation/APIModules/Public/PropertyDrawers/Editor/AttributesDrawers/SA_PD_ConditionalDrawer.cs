using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_ConditionalAttribute))]
    public class SA_PD_ConditionalDrawer : PropertyDrawer
    {
        SA_PD_ConditionalAttribute Attribute {
            get {
                return (SA_PD_ConditionalAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            bool enabled = GetConditionalHideAttributeResult(property);

            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!Attribute.HideInInspector || enabled) {
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            bool enabled = GetConditionalHideAttributeResult(property);

            if (!Attribute.HideInInspector || enabled) {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool GetConditionalHideAttributeResult(SerializedProperty property) {
            bool enabled = true;
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, Attribute.ConditionalSourceField);
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue == null) {
                Debug.LogWarning("No matching SourcePropertyValue found: " + Attribute.ConditionalSourceField);

                return enabled;
            }
                
            return sourcePropertyValue.boolValue;
        }
    }
}