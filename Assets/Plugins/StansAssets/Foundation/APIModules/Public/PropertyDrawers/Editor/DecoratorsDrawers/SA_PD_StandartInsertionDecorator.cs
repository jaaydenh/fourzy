using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_StandartInsertionDecoratorAttribute))]
    public class SA_PD_StandartInsertionDecorator : DecoratorDrawer
    {

        private float m_defaultPropertyHeight = 16f;
        private float m_offsetHeight = 6f;

        SA_PD_StandartInsertionDecoratorAttribute Attribute {
            get {
                return (SA_PD_StandartInsertionDecoratorAttribute)attribute;
            }
        }

        public override float GetHeight() {
            return base.GetHeight() + m_defaultPropertyHeight;
        }

        public override void OnGUI(Rect position) {
            position.y += m_offsetHeight;

            bool guiState = GUI.enabled;
            GUI.enabled = false;
            GUI.TextArea(position, "", GUI.skin.horizontalSlider);
            GUI.enabled = guiState;
        }
    }
}