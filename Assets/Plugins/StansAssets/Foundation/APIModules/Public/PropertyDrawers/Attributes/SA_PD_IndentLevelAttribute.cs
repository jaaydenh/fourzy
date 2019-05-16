using System;
using UnityEngine;

namespace SA.Foundation.PropertyDrawers.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SA_PD_IndentLevelAttribute : PropertyAttribute
    {

        private int m_indentLevel;

        public SA_PD_IndentLevelAttribute(int indentLevel) {
            m_indentLevel = indentLevel;
        }

        public int IndentLevel {
            get {
                return m_indentLevel;
            }
        }
    }
}