using SA.Foundation.Editor;
using SA.Foundation.PropertyDrawers.Attributes;
using UnityEditor;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(SA_PD_LabelWithIconAttribute))]
    public class SA_PD_LabelWithIconDrawer : PropertyDrawer
    {

        SA_PD_LabelWithIconAttribute Attribute {
            get {
                return (SA_PD_LabelWithIconAttribute)attribute;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            string iconPath = string.Empty;
            Texture texture;

            if (Attribute.IconPath.Equals(string.Empty)) {
                iconPath = Attribute.InternalIconPath;
                texture = EditorGUIUtility.FindTexture(iconPath);
            }
            else {
                iconPath = Attribute.IconPath;
                texture = SA_EditorAssets.GetTextureAtPath(Attribute.IconPath);
            }

            GUIContent content = new GUIContent(label.text, texture);
            GUI.Label(position, content);
        }
    }
}