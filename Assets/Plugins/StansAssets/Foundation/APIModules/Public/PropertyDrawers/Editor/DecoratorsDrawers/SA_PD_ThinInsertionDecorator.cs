using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_ThingInsertionDecoratorAttribute))]
    public class SA_PD_ThinInsertionDecorator : DecoratorDrawer
    {
        private float m_offsetHeight = 5f;

        SA_PD_ThingInsertionDecoratorAttribute Attribute {
            get {
                return (SA_PD_ThingInsertionDecoratorAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position) {
            position.y += m_offsetHeight;
            GUI.Label(position, "", "sv_iconselector_sep");
        }
    }
}