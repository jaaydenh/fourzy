using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_SimpleSliderAttribute))]
    public class SA_PD_SimpleSliderDrawer : PropertyDrawer
    {

        SA_PD_SimpleSliderAttribute Attribute {
            get {
                return (SA_PD_SimpleSliderAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.backgroundColor = Attribute.Color;

            switch(property.propertyType) {
                case SerializedPropertyType.Float:
                    EditorGUI.Slider(position, property, Attribute.MinLimit, Attribute.MaxLimit, label);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.IntSlider(position, property, (int)Attribute.MinLimit, (int)Attribute.MaxLimit, label);
                    break;
                default:
                    EditorGUI.LabelField(position, label.text, "Use with float or int.");
                    break;
            }
            GUI.backgroundColor = Color.white;
        }
    }
}
