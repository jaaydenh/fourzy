using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_HelpBoxAttribute))]
    public class SA_PD_HelpBoxDrawer : PropertyDrawer
    {
        private float m_propertyHeight = 40f;
        private float m_offsetHeight = 12f;

        SA_PD_HelpBoxAttribute Attribute {
            get {
                return (SA_PD_HelpBoxAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            MessageType type = (MessageType)Attribute.Type;

            position.height = m_propertyHeight;

            EditorGUI.HelpBox(position, property.stringValue, type);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return ((base.GetPropertyHeight(property, label) + m_propertyHeight) - m_offsetHeight);
        }
    }
}