using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_PRInsertionDecoratorAttribute))]
    public class SA_PD_PRInsertionDecorator : DecoratorDrawer
    {
        private float m_defaultPropertyHeight = 16f;

        SA_PD_PRInsertionDecoratorAttribute Attribute {
            get {
                return (SA_PD_PRInsertionDecoratorAttribute)attribute;
            }
        }

        public override float GetHeight() {
            return base.GetHeight() + m_defaultPropertyHeight;
        }

        public override void OnGUI(Rect position) {
            GUI.Label(position, "", "PR Insertion");
        }
    }
}
