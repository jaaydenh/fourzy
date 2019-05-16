using UnityEngine;
using System;

namespace SA.Foundation.PropertyDrawers.Attributes
{

    [AttributeUsage(AttributeTargets.Field)]
    public class SA_PD_ConditionalAttribute : PropertyAttribute
    {
        private string m_conditionalSourceField = "";
        private bool m_hideInInspector = false;

        public SA_PD_ConditionalAttribute(string conditionalSourceField) {
            m_conditionalSourceField = conditionalSourceField;
            m_hideInInspector = false;
        }

        public SA_PD_ConditionalAttribute(string conditionalSourceField, bool hideInInspector) {
            m_conditionalSourceField = conditionalSourceField;
            m_hideInInspector = hideInInspector;
        }

        public string ConditionalSourceField {
            get {
                return m_conditionalSourceField;
            }
        }

        public bool HideInInspector {
            get {
                return m_hideInInspector;
            }
        }
    }
}