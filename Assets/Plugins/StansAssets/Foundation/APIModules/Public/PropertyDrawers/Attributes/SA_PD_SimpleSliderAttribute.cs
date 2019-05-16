using System;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SA_PD_SimpleSliderAttribute : PropertyAttribute
    {

        private float m_minLimit;
        private float m_maxLimit;
        private Color m_color;

        public SA_PD_SimpleSliderAttribute(float minLimit, float maxLimit, float r, float g, float b) {
            m_minLimit = minLimit;
            m_maxLimit = maxLimit;
            m_color = new Color(r, g, b);
        }

        public float MinLimit {
            get {
                return m_minLimit;
            }
        }
        public float MaxLimit {
            get {
                return m_maxLimit;
            }
        }

        public Color Color {
            get {
                return m_color;
            }
        }
    }
}
