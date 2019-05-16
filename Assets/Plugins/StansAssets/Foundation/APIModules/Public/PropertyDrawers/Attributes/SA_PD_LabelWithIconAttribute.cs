using UnityEngine;
using System;

namespace SA.Foundation.PropertyDrawers.Attributes
{

    [AttributeUsage(AttributeTargets.Field)]
    public class SA_PD_LabelWithIconAttribute : PropertyAttribute
    {

        private string m_iconPath = String.Empty;
        private string m_internalIconPath = String.Empty;

        private SA_PD_EditorIcons.IconType m_iconType;

        public SA_PD_LabelWithIconAttribute(string iconPath) {
            m_iconPath = iconPath;
        }

        public SA_PD_LabelWithIconAttribute(SA_PD_EditorIcons.IconType icon) {
            m_internalIconPath = SA_PD_EditorIcons.GetInternalStringPathOfEnumValue(icon);
        }

        public string IconPath {
            get {
                return m_iconPath;
            }
        }

        public string InternalIconPath {
            get {
                return m_internalIconPath;
            }
        }
    }
}
