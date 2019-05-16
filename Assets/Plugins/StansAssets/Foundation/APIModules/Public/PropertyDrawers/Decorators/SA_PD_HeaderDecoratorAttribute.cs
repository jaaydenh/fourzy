using UnityEngine;

namespace SA.Foundation.PropertyDrawers.Attributes
{

    public class SA_PD_HeaderDecoratorAttribute : PropertyAttribute
    {

        private string m_text;

        public SA_PD_HeaderDecoratorAttribute(string text) {
            m_text = text;
        }

        public string Text {
            get {
                return m_text;
            }
        }
    }
}