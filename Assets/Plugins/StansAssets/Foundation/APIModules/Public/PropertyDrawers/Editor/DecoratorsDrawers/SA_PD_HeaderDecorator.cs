using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_HeaderDecoratorAttribute))]
    public class SA_PD_HeaderDecorator : DecoratorDrawer
    {
        private float m_propertyHeight = 20f;
        private float m_offsetHeight = 8f;

        SA_PD_HeaderDecoratorAttribute Attribute {
            get {
                return (SA_PD_HeaderDecoratorAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position) {
            position.height = m_propertyHeight;
            GUI.Box(position, Attribute.Text);
        }

        public override float GetHeight() {
            return base.GetHeight() + m_offsetHeight;
        }
    }
}
