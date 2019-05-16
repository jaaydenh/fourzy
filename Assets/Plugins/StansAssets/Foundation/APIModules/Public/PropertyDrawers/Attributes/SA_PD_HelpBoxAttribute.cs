using System;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SA_PD_HelpBoxAttribute : PropertyAttribute
    {
        
        private SA_PD_MessageType m_type;

        public SA_PD_HelpBoxAttribute(SA_PD_MessageType type) {
            m_type = type;
        }

        public SA_PD_MessageType Type {
            get {
                return m_type;
            }
        }
    }
}