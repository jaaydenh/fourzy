using System;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SA_PD_HelpboxDecoratorAttribute : PropertyAttribute
    {

        private string m_message;
        private SA_PD_MessageType m_type;

        public SA_PD_HelpboxDecoratorAttribute(SA_PD_MessageType type, string message = "") {
            m_message = message;
            m_type = type;
        }

        public string Message {
            get {
                return m_message;
            }
        }

        public SA_PD_MessageType Type {
            get {
                return m_type;
            }
        }
    }
}
