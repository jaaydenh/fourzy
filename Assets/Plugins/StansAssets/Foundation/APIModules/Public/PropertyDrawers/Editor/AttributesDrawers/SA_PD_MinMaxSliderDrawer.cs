#if !UNITY_5

using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_MinMaxSliderAttribute))]
    public class SA_PD_MinMaxSliderDrawer : PropertyDrawer
    {
        SA_PD_MinMaxSliderAttribute Attribute
        {
            get {
                return (SA_PD_MinMaxSliderAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.backgroundColor = Attribute.Color;

            Vector2 value;
            bool isVector2Int = false;



            switch (property.propertyType) {
#if UNITY_2017_4_OR_NEWER
                case SerializedPropertyType.Vector2Int:
                    isVector2Int = true;
                    value = property.vector2IntValue;
                    break;
#endif
                case SerializedPropertyType.Vector2:
                    value = property.vector2Value;
                    break;
                default:
                    Debug.LogError("Please! Use Vector2 or Vector2Int as a type of the GameObject");
                    return;
            }

            float min = value.x;
            float max = value.y;

            label.text += " " + value.ToString(isVector2Int ? "0" : "0.00");

            label = EditorGUI.BeginProperty(position, label, property);
            {
                EditorGUI.MinMaxSlider(position, label, ref min, ref max, Attribute.MinLimit, Attribute.MaxLimit);


                if (isVector2Int) {
#if UNITY_2017_4_OR_NEWER
                    value.x = Mathf.RoundToInt(min);
                    value.y = Mathf.RoundToInt(max);

					if (Vector2Int.RoundToInt(value) != property.vector2IntValue) {
						property.vector2IntValue = Vector2Int.RoundToInt(value);
					}
#endif

                } else {
                    value.x = min;
                    value.y = max;

                    if (value != property.vector2Value)
                        property.vector2Value = value;
                }
            }
            EditorGUI.EndProperty();

            GUI.backgroundColor = Color.white;
        }
    }
}

#endif