using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_HelpboxDecoratorAttribute))]
    public class SA_PD_HelpBoxDecorator : DecoratorDrawer
    {
        private float m_propertyHeight = 40f;
        private float m_offsetHeight = 12f;

        SA_PD_HelpboxDecoratorAttribute Attribute {
            get {
                return (SA_PD_HelpboxDecoratorAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position) {
            MessageType type = (MessageType)Attribute.Type;

            position.height = m_propertyHeight;
            
            EditorGUI.HelpBox(position, Attribute.Message, type);
        }

        public override float GetHeight() {
            return ((base.GetHeight() + m_propertyHeight) - m_offsetHeight);
        }
    }
}
